using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PegasusGsMonitor {
    public  class BeaconData {
        private int ReceivedId = -1;
        private DateTime ReceivedTime = DateTime.MinValue;
        //private string Data;
        private byte[] RawData;
        Dictionary<string, string> Values = null;

        public BeaconData(int id, DateTime time, string data, Dictionary<string, string> keyValuePairs) {
            ReceivedId = id;
            ReceivedTime = time;
            RawData = ParseData(data);
            Values = keyValuePairs;
        }

        private Byte[] ParseData(string data) {
            
            int pid = -1;
            if (int.TryParse(data.Substring(0,2), NumberStyles.HexNumber, null, out pid)) {
                if (data[2] == ' ') {
                    // Hex format with blanks
                    var hex = data.ToLower();
                    return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 3 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
                } else {
                    // HexFormat without blanks
                    var hex = data.Substring(0, 128).ToLower();
                    return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
                }
            } else {
                return new Byte[] { (byte)0 };
            }
        }

        public override string ToString() {
            string retVal = $"Id:{ReceivedId} Time: {ReceivedTime} Data: '{BitConverter.ToString(RawData)}'";

            if (Values != null) {
                foreach(string key in Values.Keys) {
                    retVal += Environment.NewLine + key + ": " + Values[key];
                }
            }

            return retVal;
        }

    }
}