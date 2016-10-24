using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class HttpRemoteException : Exception
    {
        private readonly HttpStatusCode _code;

        private readonly object _response;

        public HttpStatusCode HttpCode
        {
            get
            {
                return _code;
            }
        }

        public object Response
        {
            get
            {
                return _response;
            }
        }

        public override string Message
        {
            get
            {
                return string.Format("An HTTP error has occurred, code {0} ({1}).", (int)_code, _code);
            }
        }

        public HttpRemoteException(HttpStatusCode code, object response)
        {
            _code = code;
            _response = response;
        }

        public HttpRemoteException(HttpStatusCode code, Exception cause)
            : base("Remote exception could not be deserialized.", cause)
        {
            _code = code;
        }
    }
}
