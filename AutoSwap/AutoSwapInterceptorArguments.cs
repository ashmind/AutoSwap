using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoSwap {
    public class AutoSwapInterceptorArguments {
        public Type OriginalType { get; private set; }
        public Func<Type, object> Activator { get; private set; }

        public AutoSwapInterceptorArguments(Type originalType, Func<Type, object> activator) {
            this.OriginalType = Argument.NotNull("originalType", originalType);
            this.Activator = Argument.NotNull("activator", activator);
        }
    }
}
