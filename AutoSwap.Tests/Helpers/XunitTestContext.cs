using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace AutoSwap.Tests.Helpers {
    public static class XunitTestContext {
        public static string CurrentTestName {
            get { return GetCurrentTestName(); }
        }

        private static string GetCurrentTestName() {
            var trace = new StackTrace();
            var testFrame = trace.GetFrames()
                                 .Select(f => f.GetMethod())
                                 .Single(m => m.GetCustomAttributes(false).Any(a => a is FactAttribute));

            return testFrame.Name;
        }
    }
}
