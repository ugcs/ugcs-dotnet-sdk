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
        [Description("No fix")]
        NO_FIX = 0,

        [Description("2D fix")]
        FIX_2D = 1,

        [Description("3D fix")]
        FIX_3D = 2,

        [Description("DGPS fix")]
        FIX_DGPS = 3,

        [Description("RTK fix")]
        FIX_RTK = 4,

        [Description("RTK float")]
        FLOAT_RTK = 5
    }
}
