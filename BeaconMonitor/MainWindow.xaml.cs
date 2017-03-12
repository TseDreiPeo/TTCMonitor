using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using System.Windows.Threading;

namespace BeaconMonitor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private StacieCom MyStacie;
        private StreamWriter MyLogFile;
        private AdcsUplinkVm AdcsVm;
        private DispatcherTimer dispatcherTimer;


        public MainWindow() {
            InitializeComponent();
            this.comSelector.ItemsSource = new List<string>(SerialPort.GetPortNames());
            this.comSelector.SelectionChanged += ComSelector_SelectionChanged;

            AdcsVm = new AdcsUplinkVm();
            this.adcsTab.DataContext = AdcsVm;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
         
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (this.AdcsVm.BoardTime != null)
            {
                this.AdcsVm.BoardTime = this.AdcsVm.BoardTime.Value.AddSeconds(1);

            }
        }


        private void ComSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (MyStacie != null) {
                MyStacie.OnPackageReceived -= MyMessageFactory_OnPackageReceived;
            }
            MyStacie = new StacieCom(new SerialPort(e.AddedItems[0].ToString(), 19200));
            MyStacie.OnPackageReceived += MyMessageFactory_OnPackageReceived;
        }


        List<TransmitExec> packatizedDownlink = new List<TransmitExec>();
        private void MyMessageFactory_OnPackageReceived(object source, ProtocolPackageReceivedEventArgs e) {
            Application.Current?.Dispatcher?.Invoke(new Action(() => {
                this.logText.Text = e.ReceivedPackage.ToString();

                TransmitExec te = e.ReceivedPackage as TransmitExec;
                if (te != null) {
                    MyLogFile?.WriteLine($"Packge received: {te.ReadableRepresentation}");
                    MyStacie.SendTransmitAck();
                    packatizedDownlink.Add(te);

                    if((te.PckNr == 0xFF) || (te.Pid == 0x53) || (te.Pid == 0x56)) {
                        DownlinkData dd = DownlinkData.CreateDownlinkdata(packatizedDownlink);
                        packatizedDownlink.Clear();
                        OnDownlinkFinished(dd);
                    }
                    
                }
            }));
            

        }

        private void OnDownlinkFinished(DownlinkData dd) {
            ObcBeacon2 ob2 = dd as ObcBeacon2;
            if (ob2 != null) {
                this.AdcsVm.BoardTime = ob2.BoardTime;
            }
        }

        private void sendDelta_Click(object sender, RoutedEventArgs e) {
            try {
                Int32 deltaSec = AdcsVm.RtcDelta;
                this.logText.Text = $"Send {deltaSec} delta.";
                MyStacie.SendAdusrtRTC(deltaSec);
                MyLogFile?.WriteLine(DateTime.UtcNow + ":" + this.logText.Text);
            } catch (Exception ex) {
                this.logText.Text = ex.Message;
            }
        }

        private void selectFile_Click(object sender, RoutedEventArgs e) {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = this.logFile.Text;
            if(saveFileDialog.ShowDialog() == true) {
                this.logFile.Text = saveFileDialog.FileName;
            }
                
        }

        private void writeLog_Checked(object sender, RoutedEventArgs e) {
            try {
                MyLogFile = File.AppendText(this.logFile.Text);
            } catch (Exception ex) {
                this.logText.Text = ex.Message;
            }
        }

        private void writeLog_Unchecked(object sender, RoutedEventArgs e) {
            MyLogFile?.Close();
            MyLogFile = null;
        }


        private void Calculate_DeltaTime(object sender, RoutedEventArgs e) {
            TimeSpan? diff = (this.AdcsVm.DesiredDate.AddHours(AdcsVm.DesiredHour).AddMinutes(AdcsVm.DesiredMin) - this.AdcsVm.BoardTime);
            if (diff != null) {
                this.AdcsVm.RtcDeltaSeconds = (short)diff.Value.Seconds;
                this.AdcsVm.RtcDeltaMinutes = (short)diff.Value.Minutes;
                this.AdcsVm.RtcDeltaHours = (short)diff.Value.Hours;
                this.AdcsVm.RtcDeltaDays = (short)diff.Value.TotalDays;
            }

        }

    }
}
