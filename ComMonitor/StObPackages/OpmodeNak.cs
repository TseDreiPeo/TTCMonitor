using System;

namespace ComMonitor {
    internal class OpmodeNak : Package {
        public OpmodeNak(DateTime creationTime) : base(creationTime) {
        }

        protected override int GetExpectedLength() {
            return 2;
        }

        public override string ToString() {
            return $"OPMODE-NAK";
        }

    }
}