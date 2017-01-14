using System;

namespace ComMonitor {
    public  class TransmitExec : Package {
        public TransmitExec(DateTime creationTime) : base(creationTime) {
        }

        public byte Pid { get; internal set; }
    
        protected override int GetExpectedLength() {
            return 49;
        }

        public override void FillData( byte[] currentPackageBytes) {
            base.FillData( currentPackageBytes);
            Pid = currentPackageBytes[2];
            // ...
        }

        public override string ToString() {
            return $"TRANSMIT-EXEC [{Pid.ToString("x2")}] - CRC: {crc.ToString("x2")}";
        }
    }
}