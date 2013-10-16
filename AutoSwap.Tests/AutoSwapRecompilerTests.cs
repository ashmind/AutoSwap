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
            var sourcePath = TempPathHelper.GetTempSourcePath();

            Func<int, string> makeSourceForVersion = version => "public class X : " + typeof(IVersioned).FullName + " { public int Version { get { return " + version + "; } } }";
            var originalType = CompilationHelper.CompileAndLoadType(sourcePath, makeSourceForVersion(1));
            File.WriteAllText(sourcePath, makeSourceForVersion(2));

            var newType = new AutoSwapRecompiler().Recompile(new FileInfo(sourcePath), originalType);
            var instance = (IVersioned)Activator.CreateInstance(newType);

            Assert.Equal(2, instance.Version);
        }

        [Fact]
        public void Recompile_Works_IfTypesFromOriginalAssemblyAreReferenced() {
            var sourcePath = TempPathHelper.GetTempSourcePath();
            var originalType = CompilationHelper.CompileAndLoadType(
                sourcePath,
                @"public class Y {}
                  public class X { public Y M() { return new Y(); } }",
                typeName: "X"
            );

            var newType = new AutoSwapRecompiler().Recompile(new FileInfo(sourcePath), originalType);

            Assert.NotNull(newType);
        }
    }
}
