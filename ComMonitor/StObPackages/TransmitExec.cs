using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ComMonitor {
    public  class TransmitExec : Package {
        public TransmitExec(DateTime creationTime) : base(creationTime) {
        }

        public byte Pid { get; internal set; }
        public byte CntLen { get; internal set; }
        public byte PckNr { get; internal set; }

        public string ReadableRepresentation { get; set; }
    
        protected override int GetExpectedLength() {
            return 49;
        }

        public byte[] GetRawData() {
            return this.RawData;
        }

        public override void FillData( byte[] currentPackageBytes) {
            base.FillData( currentPackageBytes);
            Pid = currentPackageBytes[2];
            CntLen = currentPackageBytes[3];
            PckNr = currentPackageBytes[4];
            // ...
            if (Pid == 0x53) {
                ReadableRepresentation = CreateObcBeacon1String(currentPackageBytes);
            } else if (Pid == 0x56) {
                ReadableRepresentation = CreateObcBeacon2String(currentPackageBytes);
            } else {
                ReadableRepresentation = "Received: ";
                foreach (byte b in currentPackageBytes) {
                    ReadableRepresentation += b.ToString("X2") + " ";
                }
                ReadableRepresentation += System.Environment.NewLine;
            }

        }


 
        private string CreateObcBeacon2String(byte[]currentPackageBytes) {
            return ObcBeacon2.CreateContetntString(currentPackageBytes);
            BitArray x = new BitArray(currentPackageBytes);
           
            
            List<String> l = new List<string>();
           

            string retVal = String.Empty;

            string callSign = String.Empty;
            for (int i=0; i < 6; i++) {
                callSign += (char)currentPackageBytes[i + 3];
            }
            retVal += $"Call : {callSign}" + Environment.NewLine;
            int year =  (currentPackageBytes[9]) & 0x1f;
            int month = (currentPackageBytes[9] >> 5) | ((currentPackageBytes[10] & 0x01) << 3);
          
            
            int day =   (currentPackageBytes[10] & 0x3E) >> 1;

            int sec =  ((currentPackageBytes[10]) >> 6) | ((currentPackageBytes[11] & 0x0F) << 2);
            int min  = ((currentPackageBytes[11] & 0xF0) >> 4) | ((currentPackageBytes[12] & 0x03) << 4);
            int hour = (currentPackageBytes[12] & 0x7C) >> 2;

            bool fix = (currentPackageBytes[12] & 0x80)  !=  0;

            retVal += $"Date : {day.ToString("D2")}.{month.ToString("D2")}.20{year.ToString("D2")}" + Environment.NewLine;
            retVal += $"Time : {hour.ToString("D2")}:{min.ToString("D2")}:{sec.ToString("D2")}   Fix: {fix}" + Environment.NewLine;

            int nrOfSatelites = currentPackageBytes[13] & 0x0f;

            int latMinFract = ((currentPackageBytes[13] & 0xf0) >> 4) | 
                              (((int)currentPackageBytes[14]) << 4) |
                              (((int)currentPackageBytes[15] & 0x01) << 12);
            int latMin = (currentPackageBytes[15] & 0xfE) >> 1;
            int lat = currentPackageBytes[16] & 0x7F;
            if ((currentPackageBytes[16] & 0x80) > 0) {
                lat = -lat;
            }

            int lonMinFract = currentPackageBytes[17] | (((int)currentPackageBytes[18] & 0x1F) << 8);
            int lonMin = ((currentPackageBytes[18] & 0xE0) >> 5) |
                          (currentPackageBytes[19] & 0x0F) << 3;
            int lon = (currentPackageBytes[19] & 0xF0) >> 4 |
                      (currentPackageBytes[20] & 0x0F) << 4;
            if ((currentPackageBytes[20] & 0x10) > 0)
            {
                lon = -lon;
            }

            int alt = (currentPackageBytes[20] & 0xE0) >> 5 | ((int)currentPackageBytes[21]) << 3 |
                      ((int)currentPackageBytes[22]) << 11 | ((int)currentPackageBytes[23] & 0x01) << 19;

            retVal += $"Sats: {nrOfSatelites}" + Environment.NewLine;
            retVal += $"Lat : {lat} deg {latMin}.{latMinFract} min" + Environment.NewLine;
            retVal += $"Lon : {lon} deg {lonMin}.{lonMinFract} min" + Environment.NewLine;
            retVal += $"Alt : {alt} m" + Environment.NewLine;


            return retVal;
        }

        private string CreateObcBeacon1String(byte[] currentPackageBytes) {
            return "not yet coded ...";
        }

        public override string ToString() {
            return $"TRANSMIT-EXEC [{Pid.ToString("x2")}] - CRC: {crc.ToString("x2")}" + Environment.NewLine + ReadableRepresentation;
        }
    }
}