using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {
    public class ObcBeacon1 : DownlinkData {
        private List<TransmitExec> packages;

        public ObcBeacon1(List<TransmitExec> packages) {
            this.packages = packages;
        }
    }
}
