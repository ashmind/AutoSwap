using System;
using System.IO;

namespace AutoSwap {
    public class AutoSwapMonitorData {
        public AutoSwapMonitorData(Type originalType, FileInfo sourceFile) {
            this.OriginalType = originalType;
            this.SourceFile = sourceFile;
            UpdateType(originalType);
        }

        public Type OriginalType { get; private set; }
        public FileInfo SourceFile { get; private set; }
        public DateTimeOffset LastUpdated { get; private set; }
        public Type LastType { get; private set; }

        public bool NeedsUpdate {
            get {
                this.SourceFile.Refresh();
                return this.SourceFile.LastWriteTimeUtc > this.LastUpdated.UtcDateTime;
            }
        }

        public void UpdateType(Type newType) {
            this.LastType = newType;
            this.LastUpdated = DateTimeOffset.Now;
        }
    }
}