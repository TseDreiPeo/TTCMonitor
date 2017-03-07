using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {
    public class ObcBeacon2 : DownlinkData {
        private List<TransmitExec> packages;

        public DateTime? BoardTime { get; set; } = null;
        
        public ObcBeacon2(List<TransmitExec> packages) {
            this.packages = packages;
            if (packages.Count != 1) {
                throw new Exception("OBC Beacon must consist of a single Trasmit Package!");
            }
            byte[] beaconBytes = packages[0].GetRawData();

            int year = (beaconBytes[9]) & 0x1f;
            int month = (beaconBytes[9] >> 5) | ((beaconBytes[10] & 0x01) << 3);


            int day = (beaconBytes[10] & 0x3E) >> 1;

            int sec = ((beaconBytes[10]) >> 6) | ((beaconBytes[11] & 0x0F) << 2);
            int min = ((beaconBytes[11] & 0xF0) >> 4) | ((beaconBytes[12] & 0x03) << 4);
            int hour = (beaconBytes[12] & 0x7C) >> 2;

            bool fix = (beaconBytes[12] & 0x80) != 0;

            if (!fix) {
                BoardTime = new DateTime(year + 2000, month, day, hour, min, sec);  
            } 

        }


        private string CreateObcBeacon2String(byte[] currentPackageBytes) {

            //BitArray x = new BitArray(currentPackageBytes);


            List<String> l = new List<string>();


            string retVal = String.Empty;

            string callSign = String.Empty;
            for(int i = 0; i < 6; i++) {
                callSign += (char)currentPackageBytes[i + 3];
            }
            retVal += $"Call : {callSign}" + Environment.NewLine;
            int year = (currentPackageBytes[9]) & 0x1f;
            int month = (currentPackageBytes[9] >> 5) | ((currentPackageBytes[10] & 0x01) << 3);


            int day = (currentPackageBytes[10] & 0x3E) >> 1;

            int sec = ((currentPackageBytes[10]) >> 6) | ((currentPackageBytes[11] & 0x0F) << 2);
            int min = ((currentPackageBytes[11] & 0xF0) >> 4) | ((currentPackageBytes[12] & 0x03) << 4);
            int hour = (currentPackageBytes[12] & 0x7C) >> 2;

            bool fix = (currentPackageBytes[12] & 0x80) != 0;

            retVal += $"Date : {day.ToString("D2")}.{month.ToString("D2")}.20{year.ToString("D2")}" + Environment.NewLine;
            retVal += $"Time : {hour.ToString("D2")}:{min.ToString("D2")}:{sec.ToString("D2")}   Fix: {fix}" + Environment.NewLine;

            int nrOfSatelites = currentPackageBytes[13] & 0x0f;

            int latMinFract = currentPackageBytes[13] & 0xf0 >> 4 | ((int)currentPackageBytes[14]) << 4 |
                               ((int)currentPackageBytes[15] & 0x01) << 12;
            int latMin = currentPackageBytes[15] & 0xfE >> 1;
            int lat = currentPackageBytes[16] & 0x7F;
            if((currentPackageBytes[16] & 0x10) > 0) {
                lat = -lat;
            }
            int lon = currentPackageBytes[17] | ((int)currentPackageBytes[18]) << 8 |
                      ((int)currentPackageBytes[18]) << 16 | ((int)(currentPackageBytes[19] & 0x1F)) << 24;
            int alt = currentPackageBytes[19] & 0xE0 >> 5 | ((int)currentPackageBytes[20]) << 3 |
                      ((int)currentPackageBytes[21]) << 11 | ((int)currentPackageBytes[22] & 0x01) << 19;

            retVal += $"Sats : {nrOfSatelites}" + Environment.NewLine;
            retVal += $"Lat  : {lat} {latMin}.{latMinFract}" + Environment.NewLine;
            retVal += $"Lon  : {lon}" + Environment.NewLine;
            retVal += $"Alt  : {alt}" + Environment.NewLine;


            return retVal;
        }
    }
}
