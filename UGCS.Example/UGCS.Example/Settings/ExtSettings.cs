using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UGCS.Example.Properties
{
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {
        public void IsValidConfiguration()
        {
            foreach (SettingsProperty currentProperty in Settings.Default.Properties)
            {
                if (Settings.Default[currentProperty.Name] == null)
                {
                    throw new Exception("Invalid " + currentProperty.Name + " value");
                }
            }
        }
    }
}
