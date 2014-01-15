using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoSwap.Tests.Helpers;
using AutoSwap.Tests.TestInterfaces;
using Xunit;

namespace AutoSwap.Tests {
    public class AutoSwapRecompilerTests {
        [Fact]
        public void Recompile_ReturnsCorrectType() {
            var helper = AutoSwapTestHelper.ForClassWithVersion(1);
            var originalType = helper.CompileAndLoadType();

            helper.WriteNewVersion(2);

            var newType = new AutoSwapRecompiler().Recompile(helper.SouceFile, originalType);
            var instance = (IVersioned)Activator.CreateInstance(newType);

            Assert.Equal(2, instance.Version);
        }

        [Fact]
        public void Recompile_Works_IfTypesFromOriginalAssemblyAreReferenced() {
            var helper = new AutoSwapTestHelper(@"
                public class Y {}
                public class X { public Y M() { return new Y(); } }
            ");

            var originalType = helper.CompileAndLoadType("X");

            var newType = new AutoSwapRecompiler().Recompile(helper.SouceFile, originalType);

            Assert.NotNull(newType);
        }
    }
}
