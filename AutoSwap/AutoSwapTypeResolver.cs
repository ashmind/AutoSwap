using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoSwap {
    public class AutoSwapTypeResolver {
        private readonly AutoSwapMonitor monitor;
        private readonly AutoSwapRecompiler recompiler;

        public AutoSwapTypeResolver(AutoSwapMonitor monitor, AutoSwapRecompiler recompiler) {
            this.monitor = monitor;
            this.recompiler = recompiler;
        }

        public Type Resolve(Type originalType) {
            var data = this.monitor.GetMonitoringData(originalType);
            if (!data.NeedsUpdate)
                return data.LastType;

            var newType = this.recompiler.Recompile(data.SourceFile, data.OriginalType);
            data.UpdateType(newType);
            return newType;
        }
    }
}
