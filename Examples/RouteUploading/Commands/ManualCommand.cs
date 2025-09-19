using Com.Ugcs.Ucs.Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using UgCS.SDK.Examples.Common;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Commands
{
    internal sealed class ManualCommand : VehicleCommandBase
    {
        public override string Code => "manual";


        public ManualCommand(UcsFacade ucs)
            :base(ucs)
        {
        }

        protected override void ExecuteVehicleCommand(UcsFacade ucs, Vehicle v, object parameter)
        {
            ucs.Manual(v);
        }
    }
}
