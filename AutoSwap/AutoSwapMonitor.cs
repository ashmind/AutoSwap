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

        public bool StartMonitoring(Type type) {
            var path = this.locator.FindSourcePath(type);
            if (path == null)
                return false;

            var data = new AutoSwapMonitorData(type, new FileInfo(path));
            monitoring.Add(type, data);
            return true;
        }

        public bool IsMonitoring(Type type) {
            return monitoring.ContainsKey(type);
        }

        public AutoSwapMonitorData GetMonitoringData(Type type) {
            AutoSwapMonitorData data;
            var found = monitoring.TryGetValue(type, out data);
            if (!found)
                throw new NotSupportedException("Type '" + type.AssemblyQualifiedName + "' is not currently being monitored.");

            return data;
        }
    }
}
