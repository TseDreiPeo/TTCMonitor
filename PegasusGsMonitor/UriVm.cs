using MMVVM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PegasusGsMonitor
{
    public class UriVm :ObservableObject
    {
        private string _UriBase = "https://spacedatacenter.at/pegasus/";
        public string UriBase
        {
            get { return _UriBase; }
            set { ChangeValue(value); }
        }


        private string _UserName = "reinhard.schnitzer@fhwn.ac.at";
        public string UserName {
            get { return _UserName; }
            set { ChangeValue(value); }
        }

        
        private string _LoggedInUser = string.Empty;
        public string LoggedInUser {
            get { return _LoggedInUser; }
            set { ChangeValue(value); }
        }


        private string _AppBase = "beacon.php";
        public string AppBase
        {
            get { return _AppBase; }
            set { ChangeValue(value); }
        }

        private int _parId = -1;
        public int parId
        {
            get { return _parId; }
            set { ChangeValue(value); }
        }

        private DateTime _parFrom = new DateTime(2017,6,23,0,0,0);
        public DateTime parFrom
        {
            get { return _parFrom; }
            set { ChangeValue(value); }
        }


        private DateTime _parTo = new DateTime(2017, 6, 23, 23, 59, 59);
        public DateTime parTo
        {
            get { return _parTo; }
            set { ChangeValue(value); }
        }

        public Uri Uri { get { return ConstructUri(); } }

        private Uri ConstructUri()
        {
            Uri retVal = null;

            NameValueCollection uriParams = HttpUtility.ParseQueryString(string.Empty);
            uriParams.Add("id",this.parId.ToString());
            uriParams.Add("from", this.parFrom.ToString("yyyy-MM-dd+HH:mm"));
            uriParams.Add("to", this.parTo.ToString("yyyy-MM-dd+HH:mm"));

            retVal = new Uri(new Uri(new Uri(this.UriBase), this.AppBase), "?" + uriParams.ToString());

            return retVal;
        }
    }
}
