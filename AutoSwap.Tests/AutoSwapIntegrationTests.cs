using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoSwap.Tests.Helpers;
using AutoSwap.Tests.TestInterfaces;
using Xunit;

namespace AutoSwap.Tests {
    public class AutoSwapIntegrationTests {
        [Fact]
        public void Resolving_ReturnsNewType_IfSourceHasChanged() {
            var sourcePath = TempPathHelper.GetTempSourcePath();
            Func<int, string> makeSourceForVersion = version => "public class X : " + typeof(IVersioned).FullName + " { public int Version { get { return " + version + "; } } }";
            var originalType = CompilationHelper.CompileAndLoadType(sourcePath, makeSourceForVersion(1));
            var monitor = new AutoSwapMonitor(new AutoSwapSourceLocator());
            monitor.StartMonitoring(originalType);

            var resolver = new AutoSwapTypeResolver(monitor, new AutoSwapRecompiler());
            File.WriteAllText(sourcePath, makeSourceForVersion(2));
            var resultType = resolver.Resolve(originalType);
            var instance = (IVersioned)Activator.CreateInstance(resultType);

            Assert.Equal(2, instance.Version);
        }
    }
}
