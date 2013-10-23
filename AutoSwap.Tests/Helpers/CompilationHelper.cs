using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AutoSwap.Tests.Helpers {
    public static class CompilationHelper {
        public static Type CompileAndLoadType(string sourcePath, string source, string typeName = null) {
            File.WriteAllText(sourcePath, source);
            var dllPath = Path.ChangeExtension(sourcePath, "dll");
            var cscPath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "csc.exe");
            var cscArguments = string.Format(
                "/debug /target:library /out:\"{0}\" /reference:\"{1}\" \"{2}\"",
                dllPath, Assembly.GetExecutingAssembly().Location, sourcePath
            );
            var csc = Process.Start(new ProcessStartInfo(cscPath, cscArguments) {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            });
            csc.WaitForExit();

            if (csc.ExitCode != 0)
                throw new Exception("CSC failed with code " + csc.ExitCode + ": " + csc.StandardOutput.ReadToEnd());

            var assembly = Assembly.LoadFrom(dllPath);
            return assembly.GetExportedTypes().Single(t => typeName == null || t.Name == typeName);
        }
    }
}
