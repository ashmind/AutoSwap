using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Registration;
using Module = Autofac.Module;

namespace AutoSwap.Autofac {
    public class AutoSwapModule : Module {
        private readonly IList<Type> typesToMonitor = new List<Type>();

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);
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

            typesToMonitor.Add(reflectionActivator.LimitType);
            ((ComponentRegistration)registration).Activator = new AutoSwapActivator(reflectionActivator);
        }
    }
}