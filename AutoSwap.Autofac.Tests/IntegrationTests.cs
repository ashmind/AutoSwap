using System;
using System.Collections.Generic;
using System.Linq;
using AutoSwap.Tests.Helpers;
using AutoSwap.Tests.TestInterfaces;
using Autofac;
using Xunit;

namespace AutoSwap.Autofac.Tests {
    public class IntegrationTests {
        private readonly ContainerBuilder builder;

        public IntegrationTests() {
            this.builder = new ContainerBuilder();
            this.builder.RegisterModule<AutoSwapModule>();
        }

        [Fact]
        public void Class_RegisteredAsInterface_SingleInstance_IsSwapped() {
            var helper = AutoSwapTestHelper.ForClassWithVersion(1);
            this.builder.RegisterType(helper.CompileAndLoadType())
                        .As<IVersioned>()
                        .SingleInstance();
            var container = builder.Build();
            var versioned = container.Resolve<IVersioned>();

            helper.WriteNewVersion(2);
            Assert.Equal(2, versioned.Version);
        }

        [Fact]
        public void Class_RegisteredAsSelf_Transient_IsSwapped() {
            var helper = AutoSwapTestHelper.ForClassWithVersion(1);
            var type = helper.CompileAndLoadType();
            this.builder.RegisterType(type)
                        .AsSelf()
                        .InstancePerDependency();

            var container = builder.Build();
            var versioned = (IVersioned)container.Resolve(type);

            helper.WriteNewVersion(2);
            Assert.Equal(2, versioned.Version);
        }
    }
}
