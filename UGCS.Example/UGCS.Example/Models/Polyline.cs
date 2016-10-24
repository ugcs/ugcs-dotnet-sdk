using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGCS.Example.Models
{
    public class Polyline : Caliburn.Micro.PropertyChangedBase
    {
        public String Name { get; set; }
        private LocationCollection _locations;
        public LocationCollection Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }
    }
}
