using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using UGCS.Example.Enums;

namespace UGCS.Example.Helpers
{
    public class GPSFixUtility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return GPSFixStatus.NO_FIX;
            return (StringToEnum<GPSFixStatus>(value.ToString())).GetDescription(); 
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString())) 
                return GPSFixStatus.NO_FIX;
            return StringToEnum<GPSFixStatus>(value.ToString());
        }

        public static T StringToEnum<T>(string name)
        {
            T str;
            try
            {
                str = (T)Enum.Parse(typeof(T), name);
            }
            catch
            {
                str = default(T);
            }
            return str;
        }
 
    }
}
