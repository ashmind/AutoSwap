using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;

namespace AutoSwap.Autofac {
    public class AutoSwapActivator : IInstanceActivator {
        private readonly Type originalType;
        private ReflectionActivator reflectionActivator;

        public AutoSwapActivator(ReflectionActivator reflectionActivator) {
            this.reflectionActivator = reflectionActivator;
            this.originalType = this.reflectionActivator.LimitType;
        }

        public object ActivateInstance(IComponentContext context, IEnumerable<Parameter> parameters) {
            var typeResolver = context.Resolve<AutoSwapTypeResolver>();
            var currentType = typeResolver.Resolve(this.originalType);
            if (currentType != this.LimitType) {
                this.reflectionActivator = new ReflectionActivator(
                    currentType,
                    this.reflectionActivator.ConstructorFinder,
                    this.reflectionActivator.ConstructorSelector,
                    new Parameter[0], new Parameter[0]
                );
            }

            return this.reflectionActivator.ActivateInstance(context, parameters);
        }

        public void Dispose() {
            this.reflectionActivator.Dispose();
        }

        public Type LimitType {
            get { return this.reflectionActivator.LimitType; }
        }
    }
}