using System;
using System.Collections.Generic;
using System.Linq;
using AutoSwap.Tests.Helpers;
using Xunit;

namespace AutoSwap.Tests {
    public class AutoSwapSourceLocatorTests {
        [Fact]
        public void LocateSource_ReturnsCorrectPath() {
            var helper = new AutoSwapTestHelper("public class X { public string M() { return \"S\"; } }");
            var originalType = helper.CompileAndLoadType();

            var foundPath = new AutoSwapSourceLocator().FindSourcePath(originalType);

            Assert.Equal(helper.SouceFile.FullName, foundPath, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
