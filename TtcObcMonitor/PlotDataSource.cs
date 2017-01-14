using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicDataDisplay.Markers.DataSources;

namespace TtcObcMonitor {
    class PlotDataSource : INotifyPropertyChanged {

        private List<MyPoint> Points = new List<MyPoint>();
        public EnumerableDataSource Data;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e) {
            PropertyChanged?.Invoke(this, e);
        }

        protected void OnPropertyChanged(string propertyName) {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

    }
}
