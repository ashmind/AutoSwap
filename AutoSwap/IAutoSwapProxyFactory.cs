using System;
using System.Collections.Generic;
using AutoSwap.CastleProxy;

namespace AutoSwap {
    public interface IAutoSwapProxyFactory {
        object CreateProxy(IReadOnlyCollection<Type> serviceTypes, Type originalType, Func<Type, object> activator);
    }
}