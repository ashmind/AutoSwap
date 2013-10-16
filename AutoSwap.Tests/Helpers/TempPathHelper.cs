using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AutoSwap.Tests.Helpers {
    public static class TempPathHelper {
        public static string GetTempPath() {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tempPath = Path.Combine(location, "Temp");
            Directory.CreateDirectory(tempPath);
            return tempPath;
        }

        public static string GetTempSourcePath() {
            return Path.Combine(GetTempPath(), XunitTestContext.CurrentTestName + ".cs");
        }
    }
}
