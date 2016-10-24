using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserSettings
{
    [Serializable]
    public enum Theme
    {
        Light,
        Dark,
        Blue
    }
    [Serializable]
    public class LocalSettings
    {
        public Dictionary<Int64, int> VechileToRoute { get; set; }

        public LocalSettings()
        {
            VechileToRoute = new Dictionary<Int64, int>();
        }

    }
}
