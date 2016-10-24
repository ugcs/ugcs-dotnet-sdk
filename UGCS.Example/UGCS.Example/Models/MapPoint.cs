using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGCS.Example.Models
{
    public class MapPoint : Caliburn.Micro.PropertyChangedBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        private Location location;
        public Location Location
        {
            get { return location; }
            set
            {
                location = value;
                NotifyOfPropertyChange(() => Location);
            }
        }
    }
}
