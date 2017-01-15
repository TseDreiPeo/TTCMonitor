using System;

namespace ComMonitor {
    public class ReceiveExec : Package {
        public byte Pid { get; internal set; }

        public ReceiveExec(DateTime creationTime) : base(creationTime) {
        }
        protected override int GetExpectedLength() {
            return 49;
        }

        public override void FillData(byte[] currentPackageBytes) {
            base.FillData(currentPackageBytes);
            Pid = currentPackageBytes[2];
            // ...
        }

        public override string ToString() {
            return $"Receive-EXEC [{Pid.ToString("x2")}] - CRC: {crc.ToString("x2")}";
        }
    }
}