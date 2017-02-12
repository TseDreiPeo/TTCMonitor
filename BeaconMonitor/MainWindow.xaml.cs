using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using ComMonitor;

namespace BeaconMonitor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private StacieCom MyStacie;


        public MainWindow() {
            InitializeComponent();
            this.comSelector.ItemsSource = new List<string>(SerialPort.GetPortNames());
            this.comSelector.SelectionChanged += ComSelector_SelectionChanged;
        }

        private void ComSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (MyStacie != null) {
                MyStacie.OnPackageReceived -= MyMessageFactory_OnPackageReceived;
            }
            MyStacie = new StacieCom(new SerialPort(e.AddedItems[0].ToString(), 19200));
            MyStacie.OnPackageReceived += MyMessageFactory_OnPackageReceived;
        }

        private void MyMessageFactory_OnPackageReceived(object source, ProtocolPackageReceivedEventArgs e) {
            Application.Current?.Dispatcher?.Invoke(new Action(() => {
                this.logText.Text = e.ReceivedPackage.ToString();
            }));
        }

        private void sendDelta_Click(object sender, RoutedEventArgs e) {
            try {
                Int32 deltaSec = Int32.Parse(this.setDays.Text) * 24 * 60 * 60
                               + Int32.Parse(this.setHours.Text) * 60 * 60
                               + Int32.Parse(this.setMinute.Text) * 60
                               + Int32.Parse(this.setSeconds.Text);

                this.logText.Text = $"Send {deltaSec} delta.";
                MyStacie.SendAdusrtRTC(deltaSec);
            } catch (Exception ex) {
                this.logText.Text = ex.Message;
            }
        }
    }
}
