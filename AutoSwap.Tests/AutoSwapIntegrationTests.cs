using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoSwap.CastleProxy;
using AutoSwap.Tests.Helpers;
using AutoSwap.Tests.TestInterfaces;
using Castle.DynamicProxy;
using Xunit;

namespace AutoSwap.Tests {
    public class AutoSwapIntegrationTests {
        [Fact]
        public void Resolving_ReturnsNewType_IfSourceHasChanged() {
            var sourcePath = TempPathHelper.GetTempSourcePath();
            var originalType = CompilationHelper.CompileAndLoadType(sourcePath, CreateSourceForClassWithVersion(1));
            var monitor = new AutoSwapMonitor(new AutoSwapSourceLocator());
            monitor.StartMonitoring(originalType);

            var resolver = new AutoSwapTypeResolver(monitor, new AutoSwapRecompiler());
            File.WriteAllText(sourcePath, CreateSourceForClassWithVersion(2));
            var resultType = resolver.Resolve(originalType);
            var instance = (IVersioned)Activator.CreateInstance(resultType);

            Assert.Equal(2, instance.Version);
        }

        [Fact]
        public void Proxy_CallsNewType_IfSourceHasChanged() {
            var sourcePath = TempPathHelper.GetTempSourcePath();
            var originalType = CompilationHelper.CompileAndLoadType(sourcePath, CreateSourceForClassWithVersion(1));
            var monitor = new AutoSwapMonitor(new AutoSwapSourceLocator());
            monitor.StartMonitoring(originalType);

            var typeResolver = new AutoSwapTypeResolver(monitor, new AutoSwapRecompiler());
            var proxyFactory = new AutoSwapCastleProxyFactory(
                new ProxyGenerator(),
                typeResolver,
                a => new AutoSwapCastleInterceptor(a, typeResolver)
            );
            var proxy = (IVersioned)proxyFactory.CreateProxy(new[] { typeof(IVersioned) }, originalType, Activator.CreateInstance);
            File.WriteAllText(sourcePath, CreateSourceForClassWithVersion(2));

            Assert.Equal(2, proxy.Version);
        }

        private string CreateSourceForClassWithVersion(int version) {
            return "public class X : " + typeof(IVersioned).FullName + " { public int Version { get { return " + version + "; } } }";
        }
    }
}
