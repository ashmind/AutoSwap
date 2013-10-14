using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSwap {
    public class AutoSwapMonitor {
        private readonly AutoSwapSourceLocator locator;
        private readonly IDictionary<Type, AutoSwapMonitorData> monitoring = new Dictionary<Type, AutoSwapMonitorData>();

        public AutoSwapMonitor(AutoSwapSourceLocator locator) {
            this.locator = locator;
        }

        public void StartMonitoring(Type type) {
            var path = this.locator.FindSourcePath(type);
            var data = new AutoSwapMonitorData(type, new FileInfo(path));
            monitoring.Add(type, data);
        }

        public AutoSwapMonitorData GetMonitoringData(Type type) {
            return monitoring[type];
        }
    }
}
