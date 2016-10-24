using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IConnect
    {
        AuthorizeHciResponse AuthorizeHciResponse { get; set; }
        LoginResponse LoginResponce { get; set; }
        MessageExecutor Executor { get;  }
        Task ConnectUgcs(String login, String password, System.Action onConnect);
    }
}
