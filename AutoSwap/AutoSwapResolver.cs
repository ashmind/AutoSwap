using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoSwap {
    public class AutoSwapResolver {
        private readonly Type originalType;
        private readonly AutoSwapMonitor monitor;
        private readonly AutoSwapBuilder builder;

        public AutoSwapResolver(Type originalType, AutoSwapMonitor monitor, AutoSwapBuilder builder) {
            this.originalType = originalType;
            this.monitor = monitor;
            this.builder = builder;
            monitor.StartMonitoring(originalType);
        }

        public Type Resolve() {
            var data = this.monitor.GetMonitoringData(this.originalType);
            if (!data.NeedsUpdate)
                return data.LastType;

            var newType = this.builder.Rebuild(data.SourceFile, data.LastType);
            data.UpdateType(newType);
            return newType;
        }
    }
}
