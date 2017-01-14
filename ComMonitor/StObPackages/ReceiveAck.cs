using System;

namespace ComMonitor {
    internal class ReceiveAck : Package {
        public ReceiveAck(DateTime creationTime) : base(creationTime) {
        }

        protected override int GetExpectedLength() {
            return 2;
        }

        public override string ToString() {
            return $"RECEIVE-ACK";
        }
    }
}