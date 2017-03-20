using MMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public async void ParseForScienceData(Uri dataPage)
        {
            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(dataPage))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                // ... Display the result.
                if (result != null &&
                    result.Length > 0)
                {
                    //ParseStringAsDom(result);
                    ParseString(result);
                }
            }
        }

      
        internal void ClearResults()
        {
            Resulttext = String.Empty;
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
