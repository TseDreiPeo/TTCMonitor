using System;

namespace ComMonitor {
    public class ReceiveAck : Package {
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