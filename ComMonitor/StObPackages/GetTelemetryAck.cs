using System;

namespace ComMonitor {
    public class GetTelemetryAck : GetTelemetryAnswer {
        public GetTelemetryAck(DateTime creationTime) : base(creationTime) {
        }

        public override string ToString() {
            return $"GETTELEMETRY-ACK [{RecordId.ToString("x2")}]";
        }
    }
}