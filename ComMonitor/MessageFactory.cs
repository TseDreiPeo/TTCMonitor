using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {

    public class ProtocolPackageReceivedEventArgs : EventArgs {
        public Package ReceivedPackage { get; internal set; }

        public ProtocolPackageReceivedEventArgs(Package package) {
            ReceivedPackage = package;
        }
    }

    public class MessageFactory {
        private const int C_MAX_PACKAGE_SIZE = 50;

        private SerialPort Port;
        private bool OtoS;
        
        private byte[] CurrentPackageBytes = new byte[C_MAX_PACKAGE_SIZE];
        private int CurrentOffset = 0;
        private DateTime PackageStartTime;
        private Package CurrentPackage;

        // Received Package Event        
        public delegate void ProtocolPackageReceivedHandler(object source, ProtocolPackageReceivedEventArgs e);
        public event ProtocolPackageReceivedHandler OnPackageReceived;


        public MessageFactory(SerialPort sp, bool dirOtoS = true) {
            Port = sp;
            Port.DataReceived += new SerialDataReceivedEventHandler(PortDataReceived);
            Port.Open();
            OtoS = dirOtoS;
        }

        void PortDataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort sp = sender as SerialPort;
            int len = sp.Read(CurrentPackageBytes, CurrentOffset, C_MAX_PACKAGE_SIZE - CurrentOffset);
            if(len > 0) {
                AnalyseNewBytes(len);
            } 
        }

        void AnalyseNewBytes(int len) {
            if(CurrentOffset == 0) {
                // remember the Start time with first byte received!
                PackageStartTime = DateTime.UtcNow;
            }
            if((len >= 2) || (CurrentOffset >= 1)) {
                // 2 bytes are enough to decide on Package cmd/type.
                if(CurrentOffset <= 1) {
                    // Only the first 2 bytes of a new Package create that Package.
                    if (CurrentPackageBytes[0] == 0x1e) {
                        byte b2 = CurrentPackageBytes[1];
                    }
                    CurrentPackage = Package.CreateStacieObcPackage(PackageStartTime, CurrentPackageBytes[0], CurrentPackageBytes[1], !OtoS);
                }
                if(CurrentPackage.ExpectedLength > len + CurrentOffset) {
                    // Not all bytes here yet. We just wait for next receive event.    
                    CurrentOffset += len;
                } else {
                    // Thats nice, we have enough data to finalize currentPackage. Lets create and signal that one.
                    CurrentPackage.FillData(CurrentPackageBytes);   // If there is too much it doesn't matter. The packages don't care about hangover in buffer.
                    if(CurrentPackage.ExpectedLength > len + CurrentOffset) {
                        // Some Packages have variable lenght :-( ! Only the 3rd byte decides on real lenght
                        // so now we could find out here thet still something is missing.
                        CurrentOffset += len;
                        return;         // This is ugly but I have no idea how to do otherwise....
                    }
                    OnPackageReceived?.Invoke(this, new ProtocolPackageReceivedEventArgs(CurrentPackage));

                    if(CurrentPackage.ExpectedLength == len + CurrentOffset) {
                        // The buffer length was exactly like expected. We clear all and prepare for next one.
                        CurrentOffset = 0;
                        CurrentPackage = null;
                    } else {
                        // We have had to much bytes left after the package creation. Restart Analyses with 'hangoverbytes' ...
                        // Console.WriteLine($"Hangover: {CurrentPackage.Time}: {CurrentPackage.ToString()}");
                        int hangoverLen = len + CurrentOffset - CurrentPackage.ExpectedLength;
                        byte[] hangover = CurrentPackageBytes.Skip(CurrentPackage.ExpectedLength).Take(hangoverLen).ToArray();

                        CurrentOffset = 0;
                        Array.Copy(hangover, CurrentPackageBytes, hangoverLen);
                        len = hangoverLen;
                        
                        // Recursion as if we received from here on.
                        AnalyseNewBytes(len);
                    }
                }
            } else {
                // We got a single byte! What to do with that ? Wait if there is more to come...
                CurrentOffset += len;
            }
        }
    }
}
