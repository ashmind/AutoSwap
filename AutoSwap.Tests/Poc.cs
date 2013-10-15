using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AutoSwap.Tests.TestInterfaces;
using Xunit;

namespace AutoSwap.Tests {
    public class Poc {
        [Fact]
        public void LocateSource_ReturnsCorrectPath() {
            var originalType = CompileAndLoadType("public class X { public string M() { return \"S\"; } }");
            var path = new AutoSwapSourceLocator().FindSourcePath(originalType);

            Assert.Equal(GetSourcePath(), path, StringComparer.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void Recompile_ReturnsCorrectType() {
            Func<int, string> makeSourceForVersion = version => "public class X : " + typeof(IVersioned).FullName + " { public int Version { get { return " + version + "; } } }";
            var originalType = CompileAndLoadType(makeSourceForVersion(1));
            WriteSource(makeSourceForVersion(2));

            var newType = new AutoSwapRecompiler().Recompile(new FileInfo(GetSourcePath()), originalType);
            var instance = (IVersioned)Activator.CreateInstance(newType);

            Assert.Equal(2, instance.Version);
        }

        [Fact]
        public void Recompile_Works_IfTypesFromOriginalAssemblyAreReferenced() {
            var originalType = CompileAndLoadType("public class Y {} public class X { public Y M() { return new Y(); } }", "X");
            var newType = new AutoSwapRecompiler().Recompile(new FileInfo(GetSourcePath()), originalType);

            Assert.NotNull(newType);
        }

        [Fact]
        public void Resolving_ReturnsNewType_IfSourceHasChanged() {
            Func<int, string> makeSourceForVersion = version => "public class X : " + typeof(IVersioned).FullName + " { public int Version { get { return " + version + "; } } }";
            var originalType = CompileAndLoadType(makeSourceForVersion(1));
            var monitor = new AutoSwapMonitor(new AutoSwapSourceLocator());
            monitor.StartMonitoring(originalType);

            var resolver = new AutoSwapTypeResolver(monitor, new AutoSwapRecompiler());
            WriteSource(makeSourceForVersion(2));
            var resultType = resolver.Resolve(originalType);
            var instance = (IVersioned)Activator.CreateInstance(resultType);

            Assert.Equal(2, instance.Version);
        }

        private string GetTempPath() {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tempPath = Path.Combine(location, "Temp");
            Directory.CreateDirectory(tempPath);
            return tempPath;
        }

        private string GetSourcePath([CallerMemberName] string name = null) {
            return Path.Combine(this.GetTempPath(), name + ".cs");
        }

        private void WriteSource(string source, [CallerMemberName] string name = null) {
            File.WriteAllText(GetSourcePath(name), source);
        }

        private Type CompileAndLoadType(string source, string typeName = null, [CallerMemberName] string name = null) {
            WriteSource(source, name);

            var tempPath = GetTempPath();
            var dllPath = Path.Combine(tempPath, name + ".dll");
            var cscPath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "csc.exe");
            var cscArguments = string.Format(
                "/debug /target:library /out:\"{0}\" /reference:\"{1}\" \"{2}\"",
                dllPath, Assembly.GetExecutingAssembly().Location, GetSourcePath(name)
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
