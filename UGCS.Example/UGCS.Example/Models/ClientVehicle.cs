using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol.Encoding;

namespace UGCS.Example.Models
{
    public class ClientVehicle : Caliburn.Micro.PropertyChangedBase
    {
        public ClientVehicle()
        {
            Telemetry = new ClientTelemetry();
            Telemetry.PropertyChanged += new PropertyChangedEventHandler(_telemetryChanged);
        }
        private void _telemetryChanged(object sender, PropertyChangedEventArgs e)
        {
            //evnt on change telemetry
        }
        public Vehicle Vehicle { get; set; }
        public String Name { get; set; }     
        public ClientTelemetry Telemetry { get; set; }
    }
}
