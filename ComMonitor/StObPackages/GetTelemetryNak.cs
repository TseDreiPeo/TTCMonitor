using System;

namespace ComMonitor {
    public class GetTelemetryNak : GetTelemetryAnswer {
        public GetTelemetryNak(DateTime creationTime) : base(creationTime) {
        }

        public override string ToString() {
            return $"GETTELEMETRY-NAK [{RecordId.ToString("x2")}]";
        }
    }
}