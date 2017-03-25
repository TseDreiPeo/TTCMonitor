using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {

    class Program {
        static void Main(string[] args) {
            String line;
            String Long = "0000.0";
            String Lat = "0000.0";

            using (var fileStream = File.OpenRead(@"C:\qtemp\qb50test\gps3.txt"))
            {

                using (StreamWriter writetext = new StreamWriter(@"C:\qtemp\qb50test\gps4.txt"))
                {

                    using (var streamReader = new StreamReader(fileStream))
                    {
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            string newline = line; //FormatTime(AdjustChecksum(line));
                            if (newline.StartsWith("$GPGGA,"))
                            {
                                newline = newline.Insert(13, ".000");
                                //Long = newline.Substring(14,6);
                                //Lat = newline.Substring(25, 6);
                            } else
                            {
                                //newline = newline.Remove(20,6);
                                //newline = newline.Insert(20, Long);
                                //newline = newline.Remove(30, 6);
                                //newline = newline.Insert(30, Lat);
                            }

                            writetext.WriteLine(newline);
                        }
                    }
                }
            }
      



            //string[] names = SerialPort.GetPortNames();
            //Console.WriteLine("Serial ports:");
            //foreach(string name in names)
            //    Console.WriteLine(name);
            //Console.Write("Choose one:");
            //SerialPort p = new SerialPort(Console.ReadLine(), 19200);
            //MessageFactory mf = new MessageFactory(p);
            //mf.OnPackageReceived += Mf_OnPackageReceived; 

//            string line;
            do {
                line = Console.ReadLine();
            } while(line != "quit");
            //p.Close();
        }

        private static void Mf_OnPackageReceived(object source, ProtocolPackageReceivedEventArgs e) {
            if(e.ReceivedPackage is TransmitExec) {
                Console.WriteLine($"{e.ReceivedPackage.Time}-{e.ReceivedPackage.ToString()}");
            }
        }


        private static string AdjustChecksum(string line)
        {
            string content = line.Substring(1, line.IndexOf("*") - 1);
            int checksum = 0;
            for (int i = 0; i < content.Length; i++)
            {
                checksum ^= Convert.ToByte(content[i]);
            }
            return "$" + content + "*" + checksum.ToString("X2");
        }

        private static string FormatTime(string line)
        {
            string retVal = line;
            int desPos = 0;
            if (line.StartsWith("$GPGGA,"))
            {
                desPos = 13;
            } else if (line.StartsWith("$GPRMC,"))
            {
                desPos = 17;
            } else
            {
                return retVal;
            }

            int ix = line.IndexOf(',', 7);
            if (ix < desPos)
            {
                string zeros = new String('0', desPos - ix);
                retVal = line.Insert(7, zeros);
            }

            return retVal;
        }

    }
}
