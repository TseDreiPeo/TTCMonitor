using System;

namespace ComMonitor {
    public class TransmitAck : Package {
        public TransmitAck(DateTime creationTime) : base(creationTime) {
        }

        protected override int GetExpectedLength() {
            return 2;
        }
        public override string ToString() {
            return $"TRANSMIT-ACK";
        }
    }
}