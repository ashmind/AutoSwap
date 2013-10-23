using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace AutoSwap.CastleProxy {
    public class AutoSwapCastleProxyFactory : IAutoSwapProxyFactory {
        private readonly ProxyGenerator proxyGenerator;
        private readonly AutoSwapTypeResolver typeResolver;
        private readonly Func<AutoSwapInterceptorArguments, IInterceptor> interceptorFactory;

        public AutoSwapCastleProxyFactory(ProxyGenerator proxyGenerator, AutoSwapTypeResolver typeResolver, Func<AutoSwapInterceptorArguments, IInterceptor> interceptorFactory) {
            this.proxyGenerator = proxyGenerator;
            this.typeResolver = typeResolver;
            this.interceptorFactory = interceptorFactory;
        }

        public object CreateProxy(IReadOnlyCollection<Type> serviceTypes, Type originalType, Func<Type, object> activator) {
            Argument.NotNull("serviceTypes", serviceTypes);
            Argument.NotNull("originalType", originalType);
            Argument.NotNull("activator", activator);

            var currentType = this.typeResolver.Resolve(originalType);
            var currentInstance = activator(currentType);
            return this.proxyGenerator.CreateInterfaceProxyWithTargetInterface(
                serviceTypes.First(),
                serviceTypes.Skip(1).ToArray(),
                currentInstance,
                interceptorFactory(new AutoSwapInterceptorArguments(originalType, activator))
            );
        }
    }
}
