using System;

namespace ComMonitor {
    internal class TransmitNak : Package {
        public TransmitNak(DateTime creationTime) : base(creationTime) {
        }
        protected override int GetExpectedLength() {
            return 2;
        }
        public override string ToString() {
            return $"TRANSMIT-NAK";
        }
    }
}