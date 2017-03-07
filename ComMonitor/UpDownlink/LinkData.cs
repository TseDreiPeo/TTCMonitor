using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {
    public class LinkData {
        public const Byte C_DLPID_OBC1 = 0x53;
        public const Byte C_DLPID_OBC2 = 0x56;


        protected Byte PID { get; set; }
        
    }
}
