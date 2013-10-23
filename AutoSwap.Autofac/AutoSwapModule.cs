using System;
using System.Collections.Generic;
using System.Linq;
using AutoSwap.CastleProxy;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Registration;
using Castle.DynamicProxy;

namespace AutoSwap.Autofac {
    public class AutoSwapModule : Module {
        private readonly IList<Type> typesToMonitor = new List<Type>();

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.RegisterType<ProxyGenerator>()
                   .AsSelf();

            builder.RegisterType<AutoSwapCastleProxyFactory>()
                   .As<IAutoSwapProxyFactory>()
                   .SingleInstance();

            builder.RegisterType<AutoSwapCastleInterceptor>()
                   .As<IInterceptor>()
                   .InstancePerDependency();

            builder.RegisterType<AutoSwapSourceLocator>().SingleInstance();
            builder.RegisterType<AutoSwapMonitor>().SingleInstance();
            builder.RegisterType<AutoSwapRecompiler>().SingleInstance();
            builder.RegisterType<AutoSwapTypeResolver>().SingleInstance();
            builder.RegisterType<AutoSwapStartable>()
                   .As<IStartable>()
                   .WithParameter(TypedParameter.From(typesToMonitor))
                   .SingleInstance();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration) {
            base.AttachToComponentRegistration(componentRegistry, registration);
            var reflectionActivator = ((ComponentRegistration)registration).Activator as ReflectionActivator;
            if (reflectionActivator == null)
                return;

            var targetType = reflectionActivator.LimitType;
            if (targetType.Assembly == typeof(AutoSwapMonitor).Assembly || targetType.Assembly == typeof(AutoSwapStartable).Assembly)
                return;

            if (registration.Services.Select(s => s as TypedService).Any(s => s == null || !s.ServiceType.IsInterface))
                return;

            typesToMonitor.Add(reflectionActivator.LimitType);
            ((ComponentRegistration)registration).Activator = new AutoSwapActivator(registration, reflectionActivator);
        }
    }
}