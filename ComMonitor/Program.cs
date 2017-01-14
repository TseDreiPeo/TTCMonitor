using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {

    class Program {
        static void Main(string[] args) {

            string[] names = SerialPort.GetPortNames();
            Console.WriteLine("Serial ports:");
            foreach(string name in names)
                Console.WriteLine(name);
            Console.Write("Choose one:");
            SerialPort p = new SerialPort(Console.ReadLine(), 19200);
            MessageFactory mf = new MessageFactory(p);
            mf.OnPackageReceived += Mf_OnPackageReceived; 

            string line;
            do {
                line = Console.ReadLine();
            } while(line != "quit");
            p.Close();
        }

        private static void Mf_OnPackageReceived(object source, ProtocolPackageReceivedEventArgs e) {
            Console.WriteLine($"{e.ReceivedPackage.Time}-{e.ReceivedPackage.ToString()}");
        }
    }
}
