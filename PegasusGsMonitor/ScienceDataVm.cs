using MMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace PegasusGsMonitor
{
    public class ScienceDataVm :ObservableObject
    {
        
        private string _Resulttext = String.Empty;
        public string Resulttext
        {
            get { return _Resulttext; }
            set { ChangeValue(value); }
        }

        internal void ClearResults() {
            Resulttext = String.Empty;
        }

        public async void ParseForScienceData(HttpClient client, Uri dataPage)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, dataPage);

            using(HttpResponseMessage response = await client.SendAsync(request))
            using(HttpContent content = response.Content) {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();
                // ... Display the result.
                if(result != null &&
                    result.Length > 0) {
                    List<string>beaconLines = GetBeaconLinesFromDom(result);
                    List<BeaconData> beacons = new List<BeaconData>();

                    //beaconLines.ForEach(async l => beacons.Add(await GetBeaconDataForWebsiteLine(client, l, dataPage)));

                    foreach(string line in beaconLines) {
                        BeaconData bd = await GetBeaconDataForWebsiteLine(client, line, dataPage);
                        beacons.Add(bd);
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                        if (beacons.Count>0) {
                            this.Resulttext = "";
                            beacons.ForEach(l => this.Resulttext += l + Environment.NewLine);
                        } else {
                            this.Resulttext = "Data: " + result;
                        }
                    }));
                }
            }
        }

        private async Task<BeaconData> GetBeaconDataForWebsiteLine(HttpClient client, string webLine, Uri dataPage) {
            int idStart = webLine.IndexOf("id=") + 3;
            string idStr = webLine.Substring(idStart, (webLine.IndexOf("\"", idStart) - idStart));
            string timeStr  = webLine.Substring(webLine.IndexOf(">R") + 3, 19);

            int id = -1;
            Int32.TryParse(idStr, out id);

            var ub = new UriBuilder(dataPage);
            ub.Query = $"id={id}";
            Uri singleDataUri = ub.Uri;

            string data = string.Empty;
            Dictionary<string, string> keyValuePairs = null;

            var request = new HttpRequestMessage(HttpMethod.Post, singleDataUri);
            using(HttpResponseMessage response = await client.SendAsync(request))
            using(HttpContent content = response.Content) {
                string result = await content.ReadAsStringAsync();
                if(result != null &&
                    result.Length > 0) {

                    int dataStart = result.IndexOf(idStr + " </span><br>");
                    if(dataStart > 0) {
                        dataStart += idStr.Length + 13;
                        int len = 191;
                        if (result.Length < dataStart + len) {
                            len = result.Length - dataStart;
                        }
                        data = result.Substring(dataStart, len);
                    }

                    dataStart = result.IndexOf("<TABLE>");
                    int dataEnd = result.IndexOf("</TABLE>");
                    if(dataStart > 0 && dataEnd > 0) {
                        string tabledata = result.Substring(dataStart + 8, (dataEnd - dataStart - 7));
                        if(tabledata.Length > 1) {
                            keyValuePairs = ExtractValues(tabledata);
                        }
                    }

                }
            }

            DateTime time = DateTime.MinValue;
            DateTime.TryParse(timeStr, out time);
            return new BeaconData(id, time, data, keyValuePairs);
        }

        private Dictionary<string, string> ExtractValues(string tabledata) {
            char splitChar = (char)14;
            Dictionary<string, string> retVals = new Dictionary<string, string>();
            tabledata = tabledata.Replace("<TR>", "").Replace("</TR>", new String(new[] { splitChar }));
            List<string> rows = new List<string>(tabledata.Split(splitChar));

            foreach(string item in rows) {
                if(item.Length > 4) {
                    int cnt = 0;
                    string row = item.Replace("<TD>", "").Replace("</TD>", new string(new[] { splitChar }));
                    List<string> cols = new List<string>(row.Split(splitChar));
                    if(cols.Count >= 3) {
                        if (retVals.ContainsKey(cols[1])) {
                            cols[1] += $"_{cnt++}";
                        }
                        retVals.Add(cols[1], cols[2]);
                    } else {
                        int x = 55;
                    }
                }
            }
            return retVals;
        }


        private List<string> GetBeaconLinesFromDom(string result) {
            List<string> results = new List<string>();
            int pos = 0;
            while ((pos = result.IndexOf("beacon.php?id=", pos + 1)) > 0){
                int start = result.LastIndexOf("<span class=norm >R", pos);
                int end   = result.IndexOf("<br>", pos);

                if((end > start) && start > 0) {
                    results.Add(result.Substring(start, (end-start)));
                } else {
                    results.Add(result.Substring(pos, 30) + "..." );
                }
            }
            return results;
        }


        //private void ParseStringAsDom(string page)
        //{
        //    using (System.Windows.Forms.WebBrowser w = new System.Windows.Forms.WebBrowser())
        //    {
        //        w.Navigate(String.Empty);
        //        System.Windows.Forms.HtmlDocument doc = w.Document;
        //        doc.Write(page);

        //        var root = doc.Body.Children[0];
        //        var all = root.Children[4];

        //        Resulttext += $"Get finished with {doc.All.Count} Elements." + Environment.NewLine;
        //    }

        //}

        //private void ParseString(string result)
        //{
        //    string[] lines = result.Split(new[] {"<br>"}, StringSplitOptions.RemoveEmptyEntries );

        //    Resulttext = $"Parsing {lines.Count()} lines:" + Environment.NewLine;
        //    for (int i = lines.Count()-1; i>0; i--)
        //    {
        //        Resulttext += StripFormating(lines[i])+Environment.NewLine;
        //    }

            
        //}

        private string StripFormating(string line)
        {
            return Regex.Replace(line, "<.*?>", String.Empty);
        }
    }
}
