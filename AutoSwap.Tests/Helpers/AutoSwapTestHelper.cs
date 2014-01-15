using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using AutoSwap.Tests.TestInterfaces;

namespace AutoSwap.Tests.Helpers {
    public class AutoSwapTestHelper {
        private readonly string sourcePath;

        public AutoSwapTestHelper(string source) {
            this.sourcePath = GetTempSourcePath();
            File.WriteAllText(this.sourcePath, source);
        }

        public static AutoSwapTestHelper ForClassWithVersion(int version) {
            return new AutoSwapTestHelper(CreateSourceForClassWithVersion(version));
        }

        public Type CompileAndLoadType(string typeName = null) {
            var dllPath = Path.ChangeExtension(this.sourcePath, "dll");
            var cscPath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "csc.exe");
            var cscArguments = string.Format(
                "/debug /target:library /out:\"{0}\" /reference:\"{1}\" \"{2}\"",
                dllPath, Assembly.GetExecutingAssembly().Location, this.sourcePath
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

        public FileInfo SouceFile {
            get { return new FileInfo(this.sourcePath); }
        }

        public void WriteNewSource(string newSource) {
            File.WriteAllText(this.sourcePath, newSource);
        }

        public void WriteNewVersion(int version) {
            WriteNewSource(CreateSourceForClassWithVersion(version));
        }

        private static string CreateSourceForClassWithVersion(int version) {
            return "public class X : " + typeof(IVersioned).FullName + " { public int Version { get { return " + version + "; } } }";
        }

        private static string GetTempSourcePath() {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tempPath = Path.Combine(location, "Temp");
            Directory.CreateDirectory(tempPath);

            return Path.Combine(tempPath, XunitTestContext.CurrentTestName + ".cs");
        }
    }
}
