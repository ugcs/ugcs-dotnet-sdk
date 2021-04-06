using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using UgCS.SDK.Examples.Common;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Commands
{
    internal sealed class ArmCommand : VehicleCommandBase
    {
        public override string Code => "arm";


        public ArmCommand(UcsFacade ucs)
            :base(ucs)
        {
        }

        protected override void ExecuteVehicleCommand(UcsFacade ucs, Vehicle v, object parameter)
        {
            ucs.Arm(v);
        }
    }
}
