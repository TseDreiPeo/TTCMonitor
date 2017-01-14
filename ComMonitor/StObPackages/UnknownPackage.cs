using System;

namespace ComMonitor {
    public class UnknownPackage : Package {
        public UnknownPackage(DateTime creationTime) : base(creationTime) {
        }

        public override void FillData( byte[] currentPackageBytes) {
            base.FillData( currentPackageBytes);
         }

        protected override int GetExpectedLength() {
            return 2;       // TODO: Hmm was machen wir damit !? Wir sind out of sync so kurz wie möglich ist also ok.....
        }

        public override string ToString() {
            return $"UNKNOWN ";
        }
    }
}