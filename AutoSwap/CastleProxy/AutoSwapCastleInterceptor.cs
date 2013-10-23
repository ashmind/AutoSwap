using System;
using Castle.DynamicProxy;

namespace AutoSwap.CastleProxy {
    public class AutoSwapCastleInterceptor : IInterceptor {
        private readonly Type originalType;
        private readonly AutoSwapTypeResolver typeResolver;
        private readonly Func<Type, object> activator;
        private Type currentType;

        public AutoSwapCastleInterceptor(AutoSwapInterceptorArguments arguments, AutoSwapTypeResolver typeResolver) {
            this.originalType = arguments.OriginalType;
            this.activator = arguments.Activator;
            this.currentType = arguments.OriginalType;
            this.typeResolver = typeResolver;
        }

        public void Intercept(IInvocation invocation) {
            var latestType = this.typeResolver.Resolve(this.originalType);
            if (latestType != this.currentType)
                ChangeType(invocation, latestType);

            invocation.Proceed();
        }

        private void ChangeType(IInvocation invocation, Type latestType) {
            var instance = this.activator(latestType);
            var change = (IChangeProxyTarget)invocation;
            change.ChangeProxyTarget(instance);
            change.ChangeInvocationTarget(instance);
            this.currentType = latestType;
        }
    }
}