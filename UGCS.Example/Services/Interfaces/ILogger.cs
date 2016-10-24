using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ILogger
    {
        void LogException(Exception exception);
        void LogError(string message);
        void LogWarningMessage(string message);
        void LogInfoMessage(string message);
    }
}
