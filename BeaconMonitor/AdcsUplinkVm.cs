using ComMonitor;
using MMVVM;
using System;
//using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BeaconMonitor {
    public class AdcsUplinkVm : ObservableObject {

        private Func<StacieCom> StacieService = null;
        public AdcsUplinkVm(Func<StacieCom> getStacieService)
        {
            this.StacieService = getStacieService;
        }

        #region Data Mappings
        // ADCS Mapping members
        private Double _AdcsTime = 1234.5678;
        public Double AdcsTime
        {
            get { return _AdcsTime; }
            set { ChangeValue(value); }
        }
        private Single _AdcsAxis = 1.2F;
        public Single AdcsAxis
        {
            get { return _AdcsAxis; }
            set { ChangeValue(value); }
        }
        private Single _AdcsEccentricity = 2.3F;
        public Single AdcsEccentricity
        {
            get { return _AdcsEccentricity; }
            set { ChangeValue(value); }
        }
        private Single _AdcsPeriapsis = 3.4F;
        public Single AdcsPeriapsis
        {
            get { return _AdcsPeriapsis; }
            set { ChangeValue(value); }
        }
        private Single _AdcsLongOfAsc = 4.5F;
        public Single AdcsLongOfAsc
        {
            get { return _AdcsLongOfAsc; }
            set { ChangeValue(value); }
        }
        private Single _AdcsInclination = 5.6F;
        public Single AdcsInclination
        {
            get { return _AdcsInclination; }
            set { ChangeValue(value); }
        }
        private Single _AdcsMean = 6.7F;
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

        private DateTime? _LastSynced = null;
        public DateTime? LastSynced
        {
            get { return _LastSynced; }
            set { if (ChangeValue(value))
                {
                    InovokePropertyChanged(() => LastSyncValue);
                }; }
        }
        public Double LastSyncValue {
            get {
                TimeSpan delta = UTC - LastSynced.Value;
                return (delta.TotalSeconds * 2.0);
            }
        }

        public DateTime UTC
        {
            get { return DateTime.UtcNow; }
        }

        private DateTime _DesiredDate =  new DateTime(2015,1,1,0,0,0);
        public DateTime DesiredDate
        {
            get { return _DesiredDate; }
            set { ChangeValue(value); }
        }

        private int _DesiredHour = 0;
        public int DesiredHour
        {
            get { return _DesiredHour; }
            set { ChangeValue(value); }
        }

        private int _DesiredMin = 0;
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

        internal string SyncToUTC()
        {
            String retVal = String.Empty;
            TimeSpan? diff = (UTC - BoardTime);
            if (diff != null)
            {
                Int32 deltaSec = (Int32)diff.Value.TotalSeconds - 1;
                StacieService().SendAdusrtRTC(deltaSec);
                LastSynced = null;
                retVal = $"Sent {deltaSec} delta.";
            }
            return retVal;
        }

        internal String SendDelta()
        {
            String retVal = String.Empty;
            Int32 deltaSec = RtcDelta;
            StacieService().SendOrbitAndAdusrtRTC(this.AdcsAxis, this.AdcsEccentricity, this.AdcsInclination,
                this.AdcsLongOfAsc, this.AdcsMean, this.AdcsPeriapsis, this.AdcsTime, deltaSec);
            LastSynced = null;
            retVal = $"Sent: {deltaSec} delta.";
            return retVal;
        }

        public void Refresh(int deltaSeconds = 1)
        {
            InovokePropertyChanged(() => UTC);
            InovokePropertyChanged(() => LastSyncValue);

            if (this.LastSynced != null)
            {
                TimeSpan delta = UTC - LastSynced.Value;
                if (BoardTime != null)
                {
                    BoardTime = BoardTime.Value.AddSeconds(deltaSeconds);
                }
            }
        }

        public void SyncBoardTime(DateTime? boardTime)
        {
            this.LastSynced = DateTime.UtcNow;
            this.BoardTime = boardTime;
        }

        public void Calculate_DeltaTime()
        {
            TimeSpan? diff = (DesiredDate.AddHours(DesiredHour).AddMinutes(DesiredMin) - BoardTime);
            if (diff != null)
            {
                RtcDeltaSeconds = (short)diff.Value.Seconds;
                RtcDeltaMinutes = (short)diff.Value.Minutes;
                RtcDeltaHours = (short)diff.Value.Hours;
                RtcDeltaDays = (short)diff.Value.TotalDays;
            }
        }
    }
}
