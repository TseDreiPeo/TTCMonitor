using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ComMonitor;

namespace TtcObcMonitor {

    public class MyPoint {
        public MyPoint(DateTime now, int v, Package p, Brush color) {
            Time = now;
            Channel = v;
            Package = p;
            Color = color;
        }

        public int Channel { get; set; }
        public DateTime Time { get; set; }
        public Package Package { get; set; }

        private Brush _Color;
        public Brush Color { get { return Package.crcOk ? _Color : Brushes.Red; } internal set { _Color = value; } }

        
        public Brush PenColour { get { return GetPenColor(); } }

        private Brush GetPenColor() {
            Brush retCol = Brushes.Black;
            var t = Package?.GetType();
            if((t == typeof(TransmitExec))
                 || (t == typeof(TransmitAck))
                 || (t == typeof(TransmitNak))) {
                retCol = Brushes.Blue;
            } else if((t == typeof(ReceiveExec))
                    || (t == typeof(ReceiveAck))
                    || (t == typeof(ReceiveNak))) {
                retCol = Brushes.Green;
            } else if((t == typeof(OpmodeExecIn))
                    || (t == typeof(OpmodeExecOut))
                    || (t == typeof(OpmodeNak))
                    || (t == typeof(OpmodeAck))) {
                retCol = Brushes.DarkKhaki;
            } else if((t == typeof(GetTelemetryExec))
                    || (t == typeof(GetTelemetryAck))
                    || (t == typeof(GetTelemetryNak))) {
                retCol = Brushes.Gray;
            }

            return retCol;
        }

        public string GetFloatoverText() {
            return Package.GetDebugText();
        }
    }


}
