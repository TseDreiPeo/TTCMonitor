using System;
//using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconMonitor {
    public class AdcsUplinkVm :ObservableObject {

        #region Data Mappings
        // ADCS Mapping members
        private Double _AdcsTime = 0;
        public Double AdcsTime
        {
            get { return _AdcsTime; }
            set { ChangeValue(value); }
        }
        private Single _AdcsAxis = 0;
        public Single AdcsAxis
        {
            get { return _AdcsAxis; }
            set { ChangeValue(value); }
        }
        private Single _AdcsEcc = 0;
        public Single AdcsEccentricity
        {
            get { return _AdcsEcc; }
            set { ChangeValue(value); }
        }
        private Single _AdcsPeriapsis = 0;
        public Single AdcsPeriapsis
        {
            get { return _AdcsPeriapsis; }
            set { ChangeValue(value); }
        }
        private Single _AdcsLongOfAsc = 0;
        public Single AdcsLongOfAsc
        {
            get { return _AdcsLongOfAsc; }
            set { ChangeValue(value); }
        }
        private Single _AdcsInclination = 0;
        public Single AdcsInclination
        {
            get { return _AdcsInclination; }
            set { ChangeValue(value); }
        }
        private Single _AdcsMean = 0;
        public Single AdcsMean
        {
            get { return _AdcsMean; }
            set { ChangeValue(value); }
        }

        // RTC Mapping Members
        private Int16 _RtcDeltaDays = 0;
        public Int16 RtcDeltaDays
        {
            get { return _RtcDeltaDays; }
            set
            {
                if (ChangeValue(value))
                {
                    this.InovokePropertyChanged(() => RtcDelta);
                }
            }
        }

        private Int16 _RtcDeltaHours = 0;
        public Int16 RtcDeltaHours
        {
            get { return _RtcDeltaHours; }
            set
            {
                if (ChangeValue(value))
                {
                    this.InovokePropertyChanged(() => RtcDelta);
                }
            }
        }

        private Int16 _RtcDeltaMinutes = 0;
        public Int16 RtcDeltaMinutes
        {
            get { return _RtcDeltaMinutes; }
            set
            {
                if (ChangeValue(value))
                {
                    this.InovokePropertyChanged(() => RtcDelta);
                }
            }
        }

        private Int16 _RtcDeltaSeconds = 0;
        public Int16 RtcDeltaSeconds
        {
            get { return _RtcDeltaSeconds; }
            set
            {
                if (ChangeValue(value))
                {
                    this.InovokePropertyChanged(() => RtcDelta);
                }
            }
        }

        private DateTime? _BoardTime = null;
        public DateTime? BoardTime
        {
            get { return _BoardTime; }
            set { ChangeValue(value); }
        }



        private DateTime _DesiredDate = DateTime.Today;
        public DateTime DesiredDate
        {
            get { return _DesiredDate; }
            set { ChangeValue(value); }
        }

        private int _DesiredHour = DateTime.Now.Hour;
        public int DesiredHour
        {
            get { return _DesiredHour; }
            set { ChangeValue(value); }
        }

        private int _DesiredMin = DateTime.Now.Minute;
        public int DesiredMin
        {
            get { return _DesiredMin; }
            set { ChangeValue(value); }
        }

        public Int32 RtcDelta
        {
            get
            {
                return (Int32)RtcDeltaDays * 24 * 60 * 60
                     + (Int32)RtcDeltaHours * 60 * 60
                     + (Int32)RtcDeltaMinutes * 60
                     + (Int32)RtcDeltaSeconds;
            }
        }

        #endregion

    }
}
