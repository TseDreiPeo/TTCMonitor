using System;

namespace ComMonitor {
    public class ReceiveNak : Package {
        public ReceiveNak(DateTime creationTime) : base(creationTime) {
        }

        protected override int GetExpectedLength() {
            return 2;
        }

        public override string ToString() {
            return $"RECEIVE-ACK";
        }

    }
}