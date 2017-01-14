using System;

namespace ComMonitor {

    // This is the command when send from STACIE -> OBC
    public class OpmodeExecIn : Package {
        int     Opmode;

        public OpmodeExecIn(DateTime creationTime) : base(creationTime) {
        }

        protected override int GetExpectedLength() {
            return 4;
        }

        public override void FillData(byte[] currentPackageBytes) {
            base.FillData(currentPackageBytes);
            Opmode = currentPackageBytes[2];    
        }

        public override string ToString() {
            return $"OPMODE-EXEC Stc->Obc [{Opmode.ToString("x2")}] - CRC: {crc.ToString("x2")}";
        }
    }

    // This is the command when send from OBC -> STACIE
    public class OpmodeExecOut : Package {
        int Opmode;
        UInt32 Timeout;


        public OpmodeExecOut(DateTime creationTime) : base(creationTime) {
        }

        protected override int GetExpectedLength() {
            return 8;
        }
        public override void FillData(byte[] currentPackageBytes) {
            base.FillData(currentPackageBytes);
            Opmode = currentPackageBytes[2];

            Timeout = BitConverter.ToUInt32(currentPackageBytes, 3);
        }

        public override string ToString() {
            return $"OPMODE-EXEC Obc->Stc [{Opmode.ToString("x2")}] Tmo: {Timeout.ToString("x8")} - CRC: {crc.ToString("x2")}";
        }
    }
}