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
using System.IO.Ports;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;

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
        public Brush Color { get; set; }

        

        public string GetFloatoverText() {
            return Package.GetDebugText();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        //Point[] Points = new Point[100];
        //PointArrayDataSource Data;

        private MessageFactory MessageFactoryIn;
        private MessageFactory MessageFactoryOut;

        public PackagePointCollection myPointCollection;
        CursorCoordinateGraph mouseTrack;

        public MainWindow() {
            myPointCollection = new PackagePointCollection();

            InitializeComponent();
            this.DataContext = this;

            EnumerableDataSource<MyPoint> ipds = new EnumerableDataSource<MyPoint>(myPointCollection);
            ipds.SetXYMapping((o) => {
                return new Point(this.XAxis.ConvertToDouble(new TimeSpan(0, o.Time.Hour, o.Time.Minute, o.Time.Second, o.Time.Millisecond )), (o.Channel));
            });
            ipds.AddMapping(CircleElementPointMarker.ToolTipTextProperty, mp => { return mp.GetFloatoverText(); });
            ipds.AddMapping(CircleElementPointMarker.FillProperty, mp => { return mp.Color; });
            ipds.AddMapping(CircleElementPointMarker.PenProperty, mp => { return new Pen(mp.Color, 2); });


            ElementMarkerPointsGraph mpg = new ElementMarkerPointsGraph(ipds);
            mpg.Marker = new CircleElementPointMarker() { Brush = Brushes.Blue, Size= 10.3, Fill = Brushes.Blue, Pen = new Pen(Brushes.Black, 2), ToolTipText="TTT" };
            mpg.AddToPlotter(ThePlotter);

            mouseTrack = new CursorCoordinateGraph();
            ThePlotter.Children.Add(mouseTrack);

            // this will make the most left axis to display ticks as percents
            XAxis.LabelProvider = new TimeSpanLabelProvider();
            XAxis.LabelProvider.LabelStringFormat = "{0}";
            XAxis.LabelProvider.SetCustomFormatter(info => (info.Tick).ToString(@"hh\:mm\:ss\.fff"));
      
            ThePlotter.FitToView();
            ThePlotter.MouseDown += ThePlotter_MouseDown;


            SerialPort p1 = new SerialPort("COM17", 19200);
            MessageFactoryOut = new MessageFactory(p1);
            MessageFactoryOut.OnPackageReceived += SerialPort_OnPackageReceived_Out;

            SerialPort p2 = new SerialPort("COM18", 19200);
            MessageFactoryIn = new MessageFactory(p2, false);
            MessageFactoryIn.OnPackageReceived += SerialPort_OnPackageReceived_In;

        }

        private void ThePlotter_MouseDown(object sender, MouseButtonEventArgs e) {
            Point mousePos = mouseTrack.Position;
            
            var transform = ThePlotter.Viewport.Transform;
            Point mousePosInData = mousePos.ScreenToData(transform);
            double xValue = mousePosInData.X;
            TimeSpan xPos = this.XAxis.ConvertFromDouble(xValue);

            MyPoint theClickedPoint = myPointCollection.Where(mp => (mp.Time.Hour == xPos.Hours) &&
                                                                  (mp.Time.Minute == xPos.Minutes) &&
                                                                  (mp.Time.Second == xPos.Seconds)).FirstOrDefault();

            if (theClickedPoint != null) {
                MessageBox.Show(theClickedPoint.GetFloatoverText());
            }
        }


        private void SerialPort_OnPackageReceived_Out(object source, ProtocolPackageReceivedEventArgs e) {
            Package rp = e.ReceivedPackage;
            Application.Current?.Dispatcher?.Invoke(new Action(() => {
                myPointCollection.Add(new MyPoint(rp.Time, 1, rp, Brushes.Blue));
            }));
        }

        private void SerialPort_OnPackageReceived_In(object source, ProtocolPackageReceivedEventArgs e) {
            Package rp = e.ReceivedPackage;
            Application.Current?.Dispatcher?.Invoke(new Action(() =>
            {
                myPointCollection.Add(new MyPoint(rp.Time, 1, rp, Brushes.LightBlue));
            }));
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e) {
            myPointCollection.Clear();
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
        private const int TOTAL_POINTS = 3000;

        public PackagePointCollection()
            : base(TOTAL_POINTS) // here i set how much values to show 
        {
        }
    }

}
