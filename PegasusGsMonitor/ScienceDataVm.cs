using MMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void ParseForScienceData(Uri dataPage)
        {
            Resulttext += "Clicked " + Environment.NewLine;
        }

        internal void ClearResults()
        {
            Resulttext = String.Empty;
        }
    }
}
