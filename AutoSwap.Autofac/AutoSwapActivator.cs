using System;
using System.Collections.Generic;
using System.Linq;
using AutoSwap.CastleProxy;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;

namespace AutoSwap.Autofac {
    public class AutoSwapActivator : IInstanceActivator {
        private readonly IComponentRegistration registration;
        private readonly ReflectionActivator originalActivator;

        public AutoSwapActivator(IComponentRegistration registration, ReflectionActivator originalActivator) {
            this.registration = registration;
            this.originalActivator = originalActivator;
        }

        public object ActivateInstance(IComponentContext context, IEnumerable<Parameter> parameters) {
            var monitor = context.Resolve<AutoSwapMonitor>();
            if (!monitor.IsMonitoring(this.originalActivator.LimitType))
                return this.originalActivator.ActivateInstance(context, parameters);

            var proxyFactory = context.Resolve<IAutoSwapProxyFactory>();
            var serviceTypes = this.registration.Services.OfType<TypedService>().Select(s => s.ServiceType).ToArray();

            return proxyFactory.CreateProxy(
                serviceTypes,
                this.originalActivator.LimitType,
                type => ActivateInstanceInProxy(type, context, parameters)
            );
        }

        private object ActivateInstanceInProxy(Type targetType, IComponentContext context, IEnumerable<Parameter> parameters) {
            using (var reflectionActivator = CreateReflectionActivator(targetType)) {
                return reflectionActivator.ActivateInstance(context, parameters);
            }
        }

        private ReflectionActivator CreateReflectionActivator(Type targetType) {
            return new ReflectionActivator(
                targetType,
                this.originalActivator.ConstructorFinder,
                this.originalActivator.ConstructorSelector,
                Enumerable.Empty<Parameter>(),
                Enumerable.Empty<Parameter>()
            );
        }

        public void Dispose() {
            this.originalActivator.Dispose();
        }

        public Type LimitType {
            get { return this.originalActivator.LimitType; }
        }
    }
}