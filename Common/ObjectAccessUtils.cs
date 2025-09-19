using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Com.Ugcs.Ucs.Proto;
using Google.Protobuf;

namespace UgCS.SDK.Examples.Common
{
    public static class ObjectAccessUtils
    {
        public delegate void ObjectModificationEventHandler(ObjectModificationEvent e);


        public static Route GetRoute(this UcsFacade ucs, int id)
        {
            var req = new GetObjectRequest
            {
                ClientId = ucs.ClientId,
                ObjectType = typeof(Route).Name,
                ObjectId = id
            };
            var res = ucs.Execute<GetObjectRequest, GetObjectResponse>(req);

                return res.Object.Route;
        }

        public static List<T> List<T>(this UcsFacade ucs, bool withDependencies = false)
            where T: class, IMessage
        {
            var req = new GetObjectListRequest
            {
                ClientId = ucs.ClientId,
                ObjectType = typeof(T).Name,
                RefreshDependencies = withDependencies,
            };
            var res = ucs.Execute<GetObjectListRequest, GetObjectListResponse>(req);

            if (typeof(T) == typeof(Route))
                return res.Objects.Select(x => x.Route as T).ToList();
            else if (typeof(T) == typeof(Vehicle))
                return res.Objects.Select(x => x.Vehicle as T).ToList();
            else if (typeof (T) == typeof(VehicleProfile))
                return res.Objects.Select(x => x.VehicleProfile as T).ToList();


            throw new ArgumentException();
        }

        public static void SubscribeToObjectModification(this UcsFacade ucs, String typeName,
           ObjectModificationEventHandler handler, out SubscriptionToken token)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (String.IsNullOrEmpty(typeName))
                throw new ArgumentNullException(nameof(typeName));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            ucs.Subscribe(
                new EventSubscriptionWrapper
                {
                    ObjectModificationSubscription = new ObjectModificationSubscription
                    {
                        ObjectType = typeName
                    }
                },
                (e) => handler(e.Event.ObjectModificationEvent),
                out token);
        }
    }
}
