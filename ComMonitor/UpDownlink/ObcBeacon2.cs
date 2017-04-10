using System;
using System.Collections;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {
    public class ObcBeacon2 : DownlinkData {
        private List<TransmitExec> packages;

        public ObcBeacon2 DeepBeacon
        {
            get { return this; }
            set { }
        }

        public string CallSign { get;  set; }
        public bool Fix { get; private set; } = false;
        public DateTime? BoardTime { get; private set; } = null;

        public int? NrOfSatelites { get; private set; } = null;
        public GeoCoordinate MyLocation { get; private set; } = null;
        public int? LatDeg { get; private set; }
        public Double? LatMin { get; private set; }
        public int? LonDeg { get; private set; }
        public Double? LonMin { get; private set; }
        public bool Flash1_check { get; private set; }
        public bool Flash2_check { get; private set; }
        public byte AdcsStatus { get; private set; }
        public byte AdcsAngleDeviation { get; private set; }
        public BitArray ObcBitlist { get; private set; }
        public byte ErrorCode { get; private set; }
        public byte ErrorCodeBr { get; private set; }
        public long ObcResets { get; private set; }
        public byte SpTemp1 { get; private set; }
        public byte SpTemp2 { get; private set; }
        public byte SpTemp3 { get; private set; }
        public byte SpTemp4 { get; private set; }
        public BitArray ScriptSlotBits { get; private set; }

        public ObcBeacon2(List<TransmitExec> packages) {
            this.packages = packages;
            if (packages.Count != 1) {
                throw new Exception("OBC Beacon must consist of a single Trasmit Package!");
            }
            byte[] beaconBytes = packages[0].GetRawData();
            ParseBeaconBytes(beaconBytes);
        }

        private void ParseBeaconBytes(byte[] beaconBytes)
        {
            CallSign = Encoding.UTF8.GetString(beaconBytes).Substring(3, 6);

            int year = (beaconBytes[9]) & 0x1f;
            int month = (beaconBytes[9] >> 5) | ((beaconBytes[10] & 0x01) << 3);
            int day = (beaconBytes[10] & 0x3E) >> 1;

            int sec = ((beaconBytes[10]) >> 6) | ((beaconBytes[11] & 0x0F) << 2);
            int min = ((beaconBytes[11] & 0xF0) >> 4) | ((beaconBytes[12] & 0x03) << 4);
            int hour = (beaconBytes[12] & 0x7C) >> 2;

            BoardTime = new DateTime(year + 2000, month, day, hour, min, sec);
            Fix = (beaconBytes[12] & 0x80) != 0;
            if (Fix)
            {
                NrOfSatelites = beaconBytes[13] & 0x0f;

                int latMinFract = ((beaconBytes[13] & 0xf0) >> 4) |
                                  (((int)beaconBytes[14]) << 4) |
                                  (((int)beaconBytes[15] & 0x01) << 12);
                int latMin = (beaconBytes[15] & 0xfE) >> 1;
                LatMin = (Double)latMin + 0.0001 * (Double)latMinFract;
                LatDeg = beaconBytes[16] & 0x7F;
                if ((beaconBytes[16] & 0x80) > 0)
                {
                    LatDeg = -LatDeg;
                }

                int lonMinFract = beaconBytes[17] | (((int)beaconBytes[18] & 0x1F) << 8);
                int lonMin = ((beaconBytes[18] & 0xE0) >> 5) |
                              (beaconBytes[19] & 0x0F) << 3;
                LonMin = (Double)lonMin + 0.0001 * (Double)lonMinFract;
                LonDeg = (beaconBytes[19] & 0xF0) >> 4 |
                          (beaconBytes[20] & 0x0F) << 4;
                if ((beaconBytes[20] & 0x10) > 0)
                {
                    LonDeg = -LonDeg;
                }

                int alt = (beaconBytes[20] & 0xE0) >> 5 | ((int)beaconBytes[21]) << 3 |
                          ((int)beaconBytes[22]) << 11 | ((int)beaconBytes[23] & 0x01) << 19;

                try
                {
                    MyLocation = new GeoCoordinate((Double)(LatDeg ?? 0) + ((Double)latMin / 60.0) + (0.0001 * (Double)latMinFract / 60.0),
                                                   (Double)(LonDeg ?? 0) + ((Double)lonMin / 60.0) + (0.0001 * (Double)lonMinFract / 60.0),
                                                   alt);
                } catch (Exception )
                {

                }
            }

            Flash1_check = (beaconBytes[23] & 0x02) > 0;
            Flash2_check = (beaconBytes[23] & 0x04) > 0;

            AdcsStatus = beaconBytes[24];
            AdcsAngleDeviation = beaconBytes[25];

            // Obc bitlisten 26-35 ...
            ObcBitlist = new BitArray(beaconBytes.Skip(26).Take(10).ToArray());
            
            ErrorCode = beaconBytes[36];
            ErrorCodeBr = beaconBytes[37];
            ObcResets = (Int64)beaconBytes[38] |
                        (Int64)beaconBytes[39] << 8 |
                        (Int64)beaconBytes[40] << 16 |
                        (Int64)beaconBytes[41] << 24;
            SpTemp1 = beaconBytes[42];
            SpTemp2 = beaconBytes[43];
            SpTemp3 = beaconBytes[44];
            SpTemp4 = beaconBytes[45];

            ScriptSlotBits = new BitArray(beaconBytes.Skip(46).Take(2).ToArray());

        }

        public static string CreateContetntString(byte[] currentPackageBytes)
        {
            //BitArray x = new BitArray(currentPackageBytes);
            //List<String> l = new List<string>();
            string retVal = String.Empty;

            //conv.ParseBeaconBytes(currentPackageBytes);

            string callSign = String.Empty;
            for (int i = 0; i < 6; i++)
            {
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

            int latMinFract = ((currentPackageBytes[13] & 0xf0) >> 4) |
                              (((int)currentPackageBytes[14]) << 4) |
                              (((int)currentPackageBytes[15] & 0x01) << 12);
            int latMin = (currentPackageBytes[15] & 0xfE) >> 1;
            int lat = currentPackageBytes[16] & 0x7F;
            if ((currentPackageBytes[16] & 0x80) > 0)
            {
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


        //private string CreateObcBeacon2String(byte[] currentPackageBytes) {

        //    //BitArray x = new BitArray(currentPackageBytes);


        //    List<String> l = new List<string>();


        //    string retVal = String.Empty;

        //    string callSign = String.Empty;
        //    for(int i = 0; i < 6; i++) {
        //        callSign += (char)currentPackageBytes[i + 3];
        //    }
        //    retVal += $"Call : {callSign}" + Environment.NewLine;
        //    int year = (currentPackageBytes[9]) & 0x1f;
        //    int month = (currentPackageBytes[9] >> 5) | ((currentPackageBytes[10] & 0x01) << 3);


        //    int day = (currentPackageBytes[10] & 0x3E) >> 1;

        //    int sec = ((currentPackageBytes[10]) >> 6) | ((currentPackageBytes[11] & 0x0F) << 2);
        //    int min = ((currentPackageBytes[11] & 0xF0) >> 4) | ((currentPackageBytes[12] & 0x03) << 4);
        //    int hour = (currentPackageBytes[12] & 0x7C) >> 2;

        //    bool fix = (currentPackageBytes[12] & 0x80) != 0;

        //    retVal += $"Date : {day.ToString("D2")}.{month.ToString("D2")}.20{year.ToString("D2")}" + Environment.NewLine;
        //    retVal += $"Time : {hour.ToString("D2")}:{min.ToString("D2")}:{sec.ToString("D2")}   Fix: {fix}" + Environment.NewLine;

        //    int nrOfSatelites = currentPackageBytes[13] & 0x0f;

        //    int latMinFract = currentPackageBytes[13] & 0xf0 >> 4 | ((int)currentPackageBytes[14]) << 4 |
        //                       ((int)currentPackageBytes[15] & 0x01) << 12;
        //    int latMin = currentPackageBytes[15] & 0xfE >> 1;
        //    int lat = currentPackageBytes[16] & 0x7F;
        //    if((currentPackageBytes[16] & 0x80) > 0) {
        //        lat = -lat;
        //    }
        //    int lon = currentPackageBytes[17] | ((int)currentPackageBytes[18]) << 8 |
        //              ((int)currentPackageBytes[18]) << 16 | ((int)(currentPackageBytes[19] & 0x1F)) << 24;
        //    int alt = currentPackageBytes[19] & 0xE0 >> 5 | ((int)currentPackageBytes[20]) << 3 |
        //              ((int)currentPackageBytes[21]) << 11 | ((int)currentPackageBytes[22] & 0x01) << 19;

        //    retVal += $"Sats : {nrOfSatelites}" + Environment.NewLine;
        //    retVal += $"Lat  : {lat} {latMin}.{latMinFract}" + Environment.NewLine;
        //    retVal += $"Lon  : {lon}" + Environment.NewLine;
        //    retVal += $"Alt  : {alt}" + Environment.NewLine;


        //    return retVal;
        //}
    }
}
