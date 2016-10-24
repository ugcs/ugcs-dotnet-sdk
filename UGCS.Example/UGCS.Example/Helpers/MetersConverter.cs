using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UGCS.Example.Properties;

namespace UGCS.Example.Helpers
{
    public class StringMetersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "N/A";
            }
            Double val = (Double)value;
            if (Double.IsNaN(val))
            {
                val = 0;
            }
            return Math.Round(val, 1) + " m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("StringMetersConverter, ConvertBack not implemented");
        }
    }

    public class MetersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "N/A";
            }
            float val = (float)value;
            if (float.IsNaN(val))
            {
                val = 0;
            }
            return Math.Round(val, 1) + " m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("MetersConverter, ConvertBack not implemented");
        }
    }

    public class MetersSecConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "N/A";
            }
            float val = (float)value;
            if (float.IsNaN(val))
            {
                val = 0;
            }
            return Math.Round(val, 1) + " m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("MetersSecConverter, ConvertBack not implemented");
        }
    }
}
