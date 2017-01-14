using System;

namespace ComMonitor {
    internal class OpmodeAck : Package {
        public OpmodeAck(DateTime creationTime) : base(creationTime) {
        }

        protected override int GetExpectedLength() {
            return 2;
        }

        public override string ToString() {
            return $"OPMODE-ACK";
        }
    }
}