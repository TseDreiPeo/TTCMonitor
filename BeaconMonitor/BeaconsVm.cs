using ComMonitor;
using MMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconMonitor
{

    public class BeaconsVm :ObservableObject
    {
        private class ReceivedBeacon
        {
            public DateTime Received { get; set; }
            public DownlinkData Beacon { get; set; }
        }

        private List<ReceivedBeacon> Beacons = new List<ReceivedBeacon>();

        private ObcBeacon1 SelectedBeacon1;
        private ObcBeacon2 SelectedBeacon2;

        public void Add(ObcBeacon1 beacon1)
        {
            Add((DownlinkData)beacon1);
        }

        public void Add(ObcBeacon2 beacon2)
        {
            Add((DownlinkData)beacon2);
        }

        private void Add(DownlinkData beacon)
        {
            DateTime received = DateTime.UtcNow;
            Beacons.Add(new ReceivedBeacon() { Beacon = beacon, Received = received });

            ObcBeacon1 b1 = beacon as ObcBeacon1;
            if (b1 != null)
            {
                SelectedBeacon1Received = received;
                SelectedBeacon1 = b1;
                //B1CallSign = beacon.ToString();
            }

            ObcBeacon2 b2 = beacon as ObcBeacon2;
            if (b2 != null)
            {
                SelectedBeacon2Received = received;
                SelectedBeacon2 = b2;
                InovokePropertyChanged(() => B2CallSign);
                InovokePropertyChanged(() => Longitude);
                InovokePropertyChanged(() => Lattitude);
                InovokePropertyChanged(() => Altitude);
                InovokePropertyChanged(() => GpsFix);
                InovokePropertyChanged(() => FlashCheckH);
                InovokePropertyChanged(() => FlashCheckL);
                InovokePropertyChanged(() => AdcsStatus);
                InovokePropertyChanged(() => AdcsAngleDeviation);
                InovokePropertyChanged(() => TimeLabel);
                InovokePropertyChanged(() => BoardFixTime);



            }
        }

        private DateTime? _SelectedBeacon1Received;
        public DateTime? SelectedBeacon1Received
        {
            get { return _SelectedBeacon1Received; }
            set { ChangeValue(value); }
        }

        private DateTime? _SelectedBeacon2Received;
        public DateTime? SelectedBeacon2Received
        {
            get { return _SelectedBeacon2Received; }
            set { ChangeValue(value); }
        }

        
        public string B1CallSign
        {
            get { return SelectedBeacon1.ToString(); }
            set { ChangeValue(value); }     // TODO: make ObservableObject to Wrapper View Model
        }

        public string B2CallSign
        {
            get { return SelectedBeacon2?.CallSign; }
            set { ChangeValue(value); }
        }

        public String Longitude
        {
            get {
                String retVal = SelectedBeacon2?.MyLocation?.Longitude.ToString();
                if (!String.IsNullOrEmpty(retVal))
                {
                    retVal += $" ({SelectedBeacon2?.LonDeg} deg {SelectedBeacon2?.LonMin} min)";
                }
                return retVal;
            }
            set { ChangeValue(value); }
        }

        public String Lattitude
        {
            get {
                String retVal = SelectedBeacon2?.MyLocation?.Latitude.ToString();
                if (!String.IsNullOrEmpty(retVal)) {
                    retVal += $" ({SelectedBeacon2?.LatDeg} deg {SelectedBeacon2?.LatMin} min)";
                }
                return retVal;
            }
            set { ChangeValue(value); }
        }

        public Double? Altitude
        {
            get { return SelectedBeacon2?.MyLocation?.Altitude;  }
            set { ChangeValue(value); }
        }

        public bool GpsFix
        {
            get { return SelectedBeacon2?.Fix??false; }
            set { ChangeValue(value); }
        }

        public DateTime? BoardFixTime
        {
            get { return SelectedBeacon2?.BoardTime; }
            set { ChangeValue(value); }
        }

        public string TimeLabel
        {
            get { return this.GpsFix ? "Fix Time:" : "Board Time:"; }
        }

        public bool FlashCheckL
        {
            get { return SelectedBeacon2?.Flash1_check ?? false; }
            set { ChangeValue(value); }
        }
        public bool FlashCheckH
        {
            get { return SelectedBeacon2?.Flash2_check ?? false; }
            set { ChangeValue(value); }
        }

        public byte? AdcsStatus
        {
            get { return SelectedBeacon2?.AdcsStatus; }
            set { ChangeValue(value); }
        }

        public byte? AdcsAngleDeviation
        {
            get { return SelectedBeacon2?.AdcsAngleDeviation; }
            set { ChangeValue(value); }
        }

    }
}
