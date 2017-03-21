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

namespace BeaconMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StacieCom MyStacie;
        private StreamWriter MyLogFile;
        private AdcsUplinkVm AdcsVm;
        private GpsVm GpsVm;
        private TtVm TelemetryVm;
        private DispatcherTimer dispatcherTimer;


        public MainWindow()
        {
            InitializeComponent();
            this.comSelector1.ItemsSource = new List<string>(SerialPort.GetPortNames());
            this.comSelector1.SelectionChanged += ComSelector_SelectionChanged;

            this.comSelector2.ItemsSource = new List<string>(SerialPort.GetPortNames());
            this.comSelector2.SelectionChanged += ComSelector2_SelectionChanged;


            AdcsVm = new AdcsUplinkVm(GetStacieService);
            this.adcsTab.DataContext = AdcsVm;

            GpsVm = new GpsVm(terminalTxt);
            this.gpsTab.DataContext = GpsVm;

            TelemetryVm = new TtVm(GetStacieService);
            this.TTGrid.DataContext = TelemetryVm;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }

        StacieCom GetStacieService()
        {
            return MyStacie;
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.AdcsVm.Refresh();
        }


        private void ComSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyStacie != null)
            {
                MyStacie.OnPackageReceived -= MyMessageFactory_OnPackageReceived;
                MyStacie.Diconnect();
            }
            try
            {
                MyStacie = new StacieCom(new SerialPort(e.AddedItems[0].ToString(), 19200));
                MyStacie.OnPackageReceived += MyMessageFactory_OnPackageReceived;
                logText1.Clear();
            } catch (Exception ex)
            {
                logText1.Text = ex.Message;
            }
        }


        List<TransmitExec> packatizedDownlink = new List<TransmitExec>();
        private void MyMessageFactory_OnPackageReceived(object source, ProtocolPackageReceivedEventArgs e)
        {
            Application.Current?.Dispatcher?.Invoke(new Action(() =>
            {
                bool logPackage = true;
                TransmitExec te = e.ReceivedPackage as TransmitExec;
                if (te != null)
                {
                    MyLogFile?.WriteLine($"Packge received: {te.ReadableRepresentation}");
                    MyStacie.SendTransmitAck();
                    packatizedDownlink.Add(te);

                    if ((te.PckNr == 0xFF) || (te.Pid == 0x53) || (te.Pid == 0x56))
                    {
                        DownlinkData dd = DownlinkData.CreateDownlinkdata(packatizedDownlink);
                        packatizedDownlink.Clear();
                        OnDownlinkFinished(dd);
                    }

                }

                GetTelemetryExec tte = e.ReceivedPackage as GetTelemetryExec;
                if (tte != null)
                {
                    if (!this.ShowTT.IsChecked ?? true)
                    {
                        logPackage = false;
                    }
                    TelemetryVm.SendTelemetryAck(tte.RecordId);
                }

                if (logPackage) {
                    this.logText1.Text += Environment.NewLine + e.ReceivedPackage.ToString();
                }
            }));
        }


        private void OnDownlinkFinished(DownlinkData dd)
        {
            ObcBeacon2 ob2 = dd as ObcBeacon2;
            if (ob2 != null)
            {
                AdcsVm.SyncBoardTime(ob2.BoardTime);

            }
        }

        private void sendDelta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.logText1.Text = AdcsVm.SendDelta();
            }
            catch (Exception ex)
            {
                this.logText1.Text = ex.Message;
            }
        }

        private void SyncToUTC(object sender, RoutedEventArgs e)
        {
            try
            {
                this.logText1.Text = AdcsVm.SyncToUTC();
            }
            catch (Exception ex)
            {
                this.logText1.Text = ex.Message;
            }
           
        }

        private void selectFile_Click(object sender, RoutedEventArgs e)
        {

            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.FileName = this.logFile.Text;
            //if (saveFileDialog.ShowDialog() == true)
            //{
            //    this.logFile.Text = saveFileDialog.FileName;
            //}

        }

        private void writeLog_Checked(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    MyLogFile = File.AppendText(this.logFile.Text);
            //}
            //catch (Exception ex)
            //{
            //    this.logText1.Text = ex.Message;
            //}
        }

        private void writeLog_Unchecked(object sender, RoutedEventArgs e)
        {
            MyLogFile?.Close();
            MyLogFile = null;
        }


        private void Calculate_DeltaTime(object sender, RoutedEventArgs e)
        {
            AdcsVm.Calculate_DeltaTime();
        }

       

        private void ClearCom1_Click(object sender, RoutedEventArgs e)
        {
            logText1.Clear();
        }



        private void ComSelector2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GpsVm.ComSelection(sender, e);
        }


        private void ClearCom2_Click(object sender, RoutedEventArgs e)
        {
            terminalTxt.Document = new FlowDocument();

        }

        private void selectGpsFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.FileName = this.gpsFile.Text;
            if (saveFileDialog.ShowDialog() == true)
            {
                this.GpsVm.GpsSimPath = saveFileDialog.FileName;
            }
        }

        private void startStop_Click(object sender, RoutedEventArgs e)
        {
            if (this.startStop.Content.ToString() == "Start")
            {
                this.startStop.Content = "Stop";
                this.GpsVm.StartGpsSim();
            } else
            {
                this.startStop.Content = "Start";
                this.GpsVm.StopGpsSim();
            }
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.GpsVm.SendLine(this.cmdLine.Text);
                this.cmdLine.Text = String.Empty;
            }
        }

        private void Cmd1_Click(object sender, RoutedEventArgs e)
        {
            this.GpsVm.SendLine("$C,10001,0,0*");
        }

        private void Cmd2_Click(object sender, RoutedEventArgs e)
        {
            this.GpsVm.SendLine("$C,10002,0,0*");
        }

        private void Cmd3_Click(object sender, RoutedEventArgs e)
        {
            this.GpsVm.SendLine("$C,10003,0,0*");
        }

        private void Cmd4_Click(object sender, RoutedEventArgs e)
        {
            this.GpsVm.SendLine("$C,10004,0,0*");
        }

        private void Cmd0_Click(object sender, RoutedEventArgs e)
        {
            this.GpsVm.SendLine("$C,666,60,0*");
        }

        private void Cmd5_Click(object sender, RoutedEventArgs e)
        {
            this.GpsVm.SendLine($"$C,51,{this.gpsTimeMin.Text},0*");
        }
    }
}

