using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UGCS.Example.Models
{
    public class ArcSector : Caliburn.Micro.PropertyChangedBase
    {
        private double x1, x2, y1, y2;
        private int _startAngle, _endAngle;
        public void SetAngle(int startAngle, int endAngle, double radius)
        {
            _startAngle = startAngle;
            _endAngle = endAngle;
            var FirstAngle = (Math.PI / 180) * (startAngle - 90);
            var SecondAngle = (Math.PI / 180) * (endAngle - 90);

            x1 = (radius) * Math.Cos(FirstAngle);
            y1 = (radius) * Math.Sin(FirstAngle);

            x2 = (radius) * Math.Cos(SecondAngle);
            y2 = (radius) * Math.Sin(SecondAngle);
            NotifyOfPropertyChange(() => LineSegmentPoint);
            NotifyOfPropertyChange(() => ArcSegmentPoint);
        }
        public Point LineSegmentPoint
        {
            get
            {                
                return new Point(x1, y1);
            }
        }
        public Point ArcSegmentPoint
        {
            get
            {
                return new Point(x2, y2);
            }
        }

        public Boolean Visible
        {
            get
            {
                if (_startAngle != _endAngle)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
