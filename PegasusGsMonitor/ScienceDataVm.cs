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
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                        if (beaconLines.Count>0) {
                            this.Resulttext = "";
                            beaconLines.ForEach(l => this.Resulttext += l + Environment.NewLine);
                        } else {
                            this.Resulttext = "Data: " + result;
                        }
                    }));
                }
            }
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


        private void ParseStringAsDom(string page)
        {
            using (System.Windows.Forms.WebBrowser w = new System.Windows.Forms.WebBrowser())
            {
                w.Navigate(String.Empty);
                System.Windows.Forms.HtmlDocument doc = w.Document;
                doc.Write(page);

                var root = doc.Body.Children[0];
                var all = root.Children[4];

                Resulttext += $"Get finished with {doc.All.Count} Elements." + Environment.NewLine;
            }

        }

        private void ParseString(string result)
        {
            string[] lines = result.Split(new[] {"<br>"}, StringSplitOptions.RemoveEmptyEntries );

            Resulttext = $"Parsing {lines.Count()} lines:" + Environment.NewLine;
            for (int i = lines.Count()-1; i>0; i--)
            {
                Resulttext += StripFormating(lines[i])+Environment.NewLine;
            }

            
        }

        private string StripFormating(string line)
        {
            return Regex.Replace(line, "<.*?>", String.Empty);
        }
    }
}
