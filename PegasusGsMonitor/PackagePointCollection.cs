using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace PegasusGsMonitor {

    public class PackagePointCollection : RingArray<MyPoint> {
        private const int TOTAL_POINTS = 3000;

        public PackagePointCollection()
            : base(TOTAL_POINTS) // here i set how much values to show 
        {
        }
    }
}
