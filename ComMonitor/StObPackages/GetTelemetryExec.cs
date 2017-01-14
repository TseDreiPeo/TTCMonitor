using System;

namespace ComMonitor {
    public class GetTelemetryExec : Package {
        private int RecordId;

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