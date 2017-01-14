using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DynamicDataDisplay.Markers.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using ComMonitor;

namespace TtcObcMonitor {

    public class MyPoint {
        public MyPoint(DateTime now, int v, Package p) {
            this.Time = now;
            this.Channel = v;
            Package = p;
        }

        public int Channel { get; set; }
        public DateTime Time { get; set; }
        public Package Package { get; set; }        
        

        public string GetFloatoverText() {
            return Package.ToString();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        //Point[] Points = new Point[100];
        //PointArrayDataSource Data;

        private int channel = 2;

        public PackagePointCollection myPointCollection;
        //public EnumerableDataSource Data;

        public MainWindow() {
            myPointCollection = new PackagePointCollection();

            InitializeComponent();
            this.DataContext = this;

            EnumerableDataSource<MyPoint> ipds = new EnumerableDataSource<MyPoint>(myPointCollection);
            ipds.SetXYMapping((o) => {
                return new Point(this.XAxis.ConvertToDouble(o.Time), (o.Channel));
            });
            ipds.AddMapping(CircleElementPointMarker.ToolTipTextProperty, mp => { return mp.GetFloatoverText(); });

            ElementMarkerPointsGraph mpg = new ElementMarkerPointsGraph(ipds);
            mpg.Marker = new CircleElementPointMarker() { Brush = Brushes.Blue, Size= 10.3, Fill = Brushes.Blue, Pen = new Pen(Brushes.Black, 2), ToolTipText="TTT" };
            mpg.AddToPlotter(ThePlotter);

            //            ThePlotter.Children.Add(line);

            //ThePlotter.AddLineChart(Data)
            //    .WithStroke(Brushes.Red)
            //    .WithStrokeThickness(2)
            //    .WithDescription("x vs y");

            CircleElementPointMarker pm = new CircleElementPointMarker() { Size = 10.0, Fill = Brushes.Blue, Brush = Brushes.Black };




            ThePlotter.FitToView();

        }

        private void AddPoint_Click(object sender, RoutedEventArgs e) {
            myPointCollection.Add(new MyPoint(DateTime.Now, channel, new TransmitExec(DateTime.Now)));
        }


        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            if(PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }


    public class PackagePointCollection : RingArray<MyPoint> {
        private const int TOTAL_POINTS = 300;

        public PackagePointCollection()
            : base(TOTAL_POINTS) // here i set how much values to show 
        {
        }
    }

}
