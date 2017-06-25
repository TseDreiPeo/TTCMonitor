using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PegasusGsMonitor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private UriVm UriVm = null;
        private ScienceDataVm ScienceVm = null;
        private HttpClient client = null;

        public MainWindow() {
            InitializeComponent();
            UriVm = new UriVm();
            this.DataContext = UriVm;

            ScienceVm = new ScienceDataVm();
            this.ScienceDataTab.DataContext = ScienceVm;
            this.UPB.Password = "";
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string uri = UriVm.UriBase + "includes/process_login.php";
            string un = UriVm.UserName;
            string p = hex_sha512(this.UPB.Password);

            if (client == null) {
                client = new HttpClient();
            }
            //using(HttpClient client = new HttpClient()) {

            var request = new HttpRequestMessage(HttpMethod.Post, uri);
                
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("email", un));
            formData.Add(new KeyValuePair<string, string>("password", ""));
            formData.Add(new KeyValuePair<string, string>("p", p));
            request.Content = new FormUrlEncodedContent(formData);

            using(HttpResponseMessage response = await client.SendAsync(request))
            using(HttpContent content = response.Content) {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                string logintxt = String.Empty;

                // ... Display the result.
                if(result != null &&
                    result.Length > 0) {
                    
                    logintxt = GetLoginTextFromResult(result);
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.loginResult.Text = "Complete: " + result;
                    }));
                }

                if(response.IsSuccessStatusCode) {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                        this.UriVm.LoggedInUser = logintxt.Replace("\n","")
                                                          .Replace("\r","")
                                                          .Replace("<b>","")
                                                          .Replace("\t", "")
                                                          .Replace("</b>", "")
                                                          .Replace("<br />", "").Trim();
                        this.SpLogin.Visibility = Visibility.Collapsed;
                        this.SpLogout.Visibility = Visibility.Visible;
                    }));
                }


            }

            //this.wb.Navigate(uri, null, null, hdr);
            //this.wb.Navigate(UriVm.Uri);
        }

        private string GetLoginTextFromResult(string result) {
            var pos = result.IndexOf("You are currently logged in");
            var start = result.LastIndexOf("<b>", pos);
            var end = result.IndexOf("</b>", pos);

            return result.Substring(start, (end - start));
        }

        private void ScienceData_Click(object sender, RoutedEventArgs e)
        {
            ScienceVm.ParseForScienceData(client, UriVm.Uri);
        }

        private void ClrData_Click(object sender, RoutedEventArgs e)
        {
            ScienceVm.ClearResults();
        }

        private string hex_sha512(string v) {

            //function sha512_vm_test()
            //{
            //    return hex_sha512("abc").toLowerCase() ==
            //      "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a" +
            //      "2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f";
            //}

            var data = Encoding.UTF8.GetBytes(v);
            byte[] hash;
            using(SHA512 shaM = new SHA512Managed()) {
                hash = shaM.ComputeHash(data);
            }
            StringBuilder hex = new StringBuilder(hash.Length * 2);
            foreach(byte b in hash)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (client != null) {
                client.Dispose();
            }
        }

        private async void GetData_Click(object sender, RoutedEventArgs e) {
            var request = new HttpRequestMessage(HttpMethod.Post, UriVm.Uri);

            using(HttpResponseMessage response = await client.SendAsync(request))
            using(HttpContent content = response.Content) {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();
                // ... Display the result.
                if(result != null &&
                    result.Length > 0) {
                    //ParseStringAsDom(result);
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                        this.loginResult.Text = "Data: " + result;
                    }));
                }
            }

        }

        private void Logout_Click(object sender, RoutedEventArgs e) {
            this.SpLogin.Visibility = Visibility.Visible;
            this.SpLogout.Visibility = Visibility.Collapsed;
        }
    }
}
