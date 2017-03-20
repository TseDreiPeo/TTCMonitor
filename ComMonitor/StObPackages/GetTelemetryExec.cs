using System;

namespace ComMonitor {
    public class GetTelemetryExec : Package {
        public const byte C_TTRECID_TEMP1 = 0x01;
        public const byte C_TTRECID_TEMP2 = 0x02;
        public const byte C_TTRECID_MODE = 0x04;
        public const byte C_TTRECID_VERSION = 0x05;
        public const byte C_TTRECID_RSSI = 0x06;
        
        public int RecordId { get; set; }

        public GetTelemetryExec(DateTime creationTime) : base(creationTime) {
        }

        public override void FillData(byte[] currentPackageBytes) {
            base.FillData(currentPackageBytes);
            RecordId = currentPackageBytes[2];
        }

        protected override int GetExpectedLength() {
            return 4;
        }

        public override string ToString() {
            return $"GETTELEMETRY-EXEC [{RecordId.ToString("x2")}]";
        }
    }
}