using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGCS.Example.Enums
{
    public enum GPSFixStatus
    {
        [Description("N/A")]
        NA = 0,

        [Description("No fix")]
        NO_FIX = 1,

        [Description("2D fix")]
        FIX_2D = 2,

        [Description("3D fix")]
        FIX_3D = 3,

        [Description("DGPS fix")]
        FIX_DGPS = 4,

        [Description("RTK fix")]
        FIX_RTK = 5
    }    
}
