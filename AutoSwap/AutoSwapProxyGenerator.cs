using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace AutoSwap {
    //public class AutoSwapProxyGenerator {
    //    private readonly Func<Type, AutoSwapInterceptor> interceptorFactory;
    //    private readonly ProxyGenerator proxyGenerator;

    //    public AutoSwapProxyGenerator(Func<Type, AutoSwapInterceptor> interceptorFactory, ProxyGenerator proxyGenerator) {
    //        this.interceptorFactory = interceptorFactory;
    //        this.proxyGenerator = proxyGenerator;
    //    }

    //    public object GenerateProxy(Type serviceType, Type targetType) {
    //        return proxyGenerator.CreateInterfaceProxyWithoutTarget(serviceType, this.interceptorFactory(targetType));
    //    }
    //}
}
