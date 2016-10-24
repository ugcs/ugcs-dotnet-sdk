using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    class ServiceActionTelemetry
    {
        public ServiceTelemetryDTO ServiceTelemetryDTO { get; set; }
        public System.Action<ServiceTelemetryDTO, int> Callback { get; set; }
    }
}
