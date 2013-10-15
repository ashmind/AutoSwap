using System;
using System.Collections.Generic;
using Autofac;

namespace AutoSwap.Autofac {
    public class AutoSwapStartable : IStartable {
        private readonly AutoSwapMonitor monitor;
        private readonly IList<Type> typesToMonitor;

        public AutoSwapStartable(AutoSwapMonitor monitor, IList<Type> typesToMonitor) {
            this.monitor = monitor;
            this.typesToMonitor = typesToMonitor;
        }

        public void Start() {
            foreach (var type in this.typesToMonitor) {
                this.monitor.StartMonitoring(type);
            }
        }
    }
}