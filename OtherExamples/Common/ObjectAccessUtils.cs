using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UGCS.Sdk.Protocol.Encoding;
using ProtoBuf;

namespace UgCS.SDK.Examples.Common
{
    public static class ObjectAccessUtils
    {
        public static T Get<T>(this UcsFacade ucs, int id)
            where T:Route
        {
            var req = new GetObjectRequest
            {
                ClientId = ucs.ClientId,
                ObjectType = typeof(T).Name,
                ObjectId = id
            };
            var res = ucs.Execute<GetObjectResponse>(req);

            if (typeof(T) == typeof(Route))
                return (T)res.Object.Route;

            throw new ArgumentException();
        }

        public static List<T> List<T>(this UcsFacade ucs)
            where T: class, IExtensible
        {
            var req = new GetObjectListRequest
            {
                ClientId = ucs.ClientId,
                ObjectType = typeof(T).Name
            };
            var res = ucs.Execute<GetObjectListResponse>(req);

            if (typeof(T) == typeof(Route))
                return res.Objects.Select(x => x.Route as T).ToList();
            else if (typeof(T) == typeof(Vehicle))
                return res.Objects.Select(x => x.Vehicle as T).ToList();

            throw new ArgumentException();
        }
    }
}
