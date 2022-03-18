using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Commands.CommandWithErrorHandling;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Commands
{
    public static class CommandsUtils
    {
        public static ICommand AddErrorHandler(this ICommand cmd,  CommandErrorHandler handler)
        {
            return new CommandWithErrorHandling(cmd, handler);
        }
    }
}
