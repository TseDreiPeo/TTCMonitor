using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {
    public class GetTelemetryAnswer : Package {
        protected int varLength = 4; // Thats the minimum length. Final length differs for each recordId.
        protected int RecordId;

        public GetTelemetryAnswer(DateTime creationTime) : base(creationTime) {
        }

        public override void FillData(byte[] currentPackageBytes) {
           RecordId = currentPackageBytes[2];
            switch(RecordId) {
                case 1:
                case 2:
                case 4:
                    varLength = 5;
                    break;
                case 6:
                    varLength = 6;
                    break;
                default:
                    varLength = 6;
                    break;
            }
            base.FillData(currentPackageBytes);
        }

        protected override int GetExpectedLength() {
            return varLength;
        }
    }
}
