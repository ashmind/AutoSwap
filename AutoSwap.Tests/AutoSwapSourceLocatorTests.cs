using System;
using System.Collections.Generic;
using System.Linq;
using AutoSwap.Tests.Helpers;
using Xunit;

namespace AutoSwap.Tests {
    public class AutoSwapSourceLocatorTests {
        [Fact]
        public void LocateSource_ReturnsCorrectPath() {
            var sourcePath = TempPathHelper.GetTempSourcePath();
            var originalType = CompilationHelper.CompileAndLoadType(sourcePath, "public class X { public string M() { return \"S\"; } }");
            
            var foundPath = new AutoSwapSourceLocator().FindSourcePath(originalType);

            Assert.Equal(sourcePath, foundPath, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
