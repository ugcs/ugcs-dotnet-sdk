using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class ServiceHelpers
    {
        public static readonly DateTime PosixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CreationTime()
        {
            DateTime now = DateTime.Now;
            DateTime utcTime = now.ToUniversalTime();
            TimeSpan span = utcTime - PosixEpoch;
            return (long)span.TotalMilliseconds;
        }
    }
}
