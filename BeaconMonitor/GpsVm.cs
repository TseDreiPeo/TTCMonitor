using System;
using System.Windows.Controls;
using MMVVM;
using System.IO.Ports;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Threading;

namespace BeaconMonitor
{
    public class GpsVm : ObservableObject
    {
        private SerialPort Port = null;

        private RichTextBox terminalTxt;

        private string _GpsSimPath = @"c:\qtemp\qb50test\gps.txt";
        public string GpsSimPath
        {
            get { return _GpsSimPath; }
            set { ChangeValue(value); }
        }


        public GpsVm(RichTextBox terminalTxt)
        {
            this.terminalTxt = terminalTxt;
        }


        public void ComSelection(object sender, SelectionChangedEventArgs e)
        {
            if (Port != null)
            {
                Port.Close();
                Port.Dispose();
            }
            try
            {
                terminalTxt.Document = new FlowDocument();
                Port = new SerialPort(e.AddedItems[0].ToString(), 9600);
                Port.DataReceived += Port_DataReceived;
                Port.Open();
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message, Colors.Red);
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = sender as SerialPort;
            String[] lines = (sp.ReadLine()).Split('\r');
            Application.Current?.Dispatcher?.Invoke(new Action(() =>
            {
                foreach (var l in lines)
                {
                    WriteLine(l + Environment.NewLine);
                }
            }));
        }



        internal void SendLine(String line)
        {
            Port.WriteLine(line);
            // local echo:
            this.WriteLine(line + Environment.NewLine, Colors.Blue);
        }


        private void WriteLine(string line, Color? color = null)
        {
            TextRange rangeOfText1 = new TextRange(terminalTxt.Document.ContentEnd, terminalTxt.Document.ContentEnd);
            rangeOfText1.Text = line;
            rangeOfText1.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color ?? Colors.Black));
        }


        private StreamReader sr = null;
        private Timer t = null;
        bool stopGpsSim = false;
        internal void StartGpsSim()
        {
            sr = File.OpenText(GpsSimPath);
            stopGpsSim = false;
            t = new Timer(TimerCallback, null, 0, 1000);
        }

        private void TimerCallback(Object o)
        {
            if (stopGpsSim)
            {
                sr.Close();
                sr.Dispose();
                sr = null;
                t.Dispose();
                t = null;
            }

            var l1 = sr?.ReadLine();
            var l2 = sr?.ReadLine();

            if (l2 == null || l1 == null)
            {
                stopGpsSim = true;
            }

            Application.Current?.Dispatcher?.Invoke(new Action(() =>
            {
                if (l1 != null)
                {
                    SendLine(l1);
                }
                if (l2 != null)
                {
                    SendLine(l2);
                }
            }));
        }

        internal void StopGpsSim()
        {
            stopGpsSim = true;
        }
    }
}