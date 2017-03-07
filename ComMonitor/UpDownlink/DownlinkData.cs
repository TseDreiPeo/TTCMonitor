using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {
    public class DownlinkData : LinkData {

        public static DownlinkData CreateDownlinkdata(List<TransmitExec> packages)  {
            DownlinkData retData = null;
            Byte pid = packages[0].Pid;
            if (!packages.All(p=>p.Pid == pid)) {
                throw new Exception("Mixed pid in packages can not form a data link! ");
            }
            switch (pid) {
                case LinkData.C_DLPID_OBC1:
                    retData = new ObcBeacon1(packages);
                    break;
                case LinkData.C_DLPID_OBC2:
                    retData = new ObcBeacon2(packages);
                    break;
                default:
                    break;
            }

            return retData;
        }
    }
}
