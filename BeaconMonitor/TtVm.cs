using MMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComMonitor;

namespace BeaconMonitor
{
    public class TtVm :ObservableObject
    {
        private Func<StacieCom> StacieService = null;
        public TtVm(Func<StacieCom> getStacieService)
        {
            this.StacieService = getStacieService;
        }


        private bool _Enabled = false;
        public bool Enabled
        {
            get { return _Enabled; }
            set { ChangeValue(value); }
        }

        private int _Mode = 0x00;
        public int Mode
        {
            get { return _Mode; }
            set { ChangeValue(value); }
        }

        private int _Temp1 = 0x55;
        public int Temp1
        {
            get { return _Temp1; }
            set { ChangeValue(value); }
        }

        private int _Temp2 = 0x66;
        public int Temp2
        {
            get { return _Temp2; }
            set { ChangeValue(value); }
        }

        private int _RssiAC = 0x2233;
        public int RssiAC
        {
            get { return _RssiAC; }
            set { ChangeValue(value); }
        }

        private int _Version = 0xCD;
        public int Version
        {
            get { return _Version; }
            set { ChangeValue(value); }
        }
        
        public void SendTelemetryAck(int recordId)
        {
            if (Enabled) {
                StacieCom sc = this.StacieService();
                int value;
                switch (recordId)
                {
                    case GetTelemetryExec.C_TTRECID_TEMP1:
                        value = this.Temp1;
                        break;
                    case GetTelemetryExec.C_TTRECID_TEMP2:
                        value = this.Temp2;
                        break;
                    case GetTelemetryExec.C_TTRECID_MODE:
                        value = this.Mode;
                        break;
                    case GetTelemetryExec.C_TTRECID_RSSI:
                        value = this.RssiAC;
                        break;
                    case GetTelemetryExec.C_TTRECID_VERSION:
                        value = this.Version;
                        break;
                    default:
                        value = 0x88;
                        break;
                }
                sc?.SendTelemetryAck((byte)recordId, value);
            }
        }
    }
}
