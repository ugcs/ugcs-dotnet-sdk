using System;
using System.Collections.Generic;
using System.Text;

namespace UgCS.SDK.Examples.Common
{
    public static class GisUtils
    {
        public static double ToDegrees(this double radians)
        {
            return radians * 180 / Math.PI;
        }
    }
}
