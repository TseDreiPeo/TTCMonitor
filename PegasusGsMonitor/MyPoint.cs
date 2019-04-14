using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace PegasusGsMonitor {

    public class MyPoint {
        public MyPoint(DateTime now, int v, BeaconData p, string key, Brush color) {
            Time = now;
            Channel = v;
            Package = p;
            ValueKey = key;
            Color = color;
        }

        public int Channel { get; set; }
        public DateTime Time { get; set; }
        public BeaconData Package { get; set; }
        public string ValueKey { get; set; }

        private Brush _Color;
        public Brush Color { get { return Package.Values.ContainsKey(ValueKey) ? _Color : Brushes.Red; } internal set { _Color = value; } }

        
        public Brush PenColour { get { return GetPenColor(); } }

        private Brush GetPenColor() {
            Brush retCol = Brushes.Black;
            string pid = String.Empty;
            Package.Values.TryGetValue("PID", out pid);

            if (pid == "53") { 
                retCol = Brushes.Blue;
            } else if(pid == "56") { 
                retCol = Brushes.Green;
            } else {
                retCol = Brushes.Gray;
            }
            return retCol;
        }

        public string GetFloatoverText() {
            return Package.ReceivedTime + " ID: " + Package.ReceivedId;
        }

        public string GetDeatailedText() {
            return Package.ToString();
        }

    }

}
