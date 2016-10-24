using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SdkServices
{
    public class ServerObjectListener
    {
        protected NotificationListener _notificationListener;
        protected List<SubscriptionToken> tokens = new List<SubscriptionToken>();
        protected NotificationHandler _getObjectNotificationHandler<T>(ObjectChangeSubscriptionCallback<T> callback) where T : class, IIdentifiable
        {
            string invariantName = InvariantNames.GetInvariantName<T>();
            return notification =>
            {
                ObjectModificationEvent @event = notification.Event.ObjectModificationEvent;

                callback(@event.ModificationType, @event.ObjectId,
                    @event.ModificationType == ModificationType.MT_DELETE ?
                        null : (T)@event.Object.Get(invariantName));
            };
        }

        public delegate void ObjectChangeSubscriptionCallback<T>(ModificationType modification, int id, T obj) where T : IIdentifiable;
        public void UnsubscribeAll()
        {
            tokens.ForEach(x =>
            {
                bool removed;
                _notificationListener.RemoveSubscription(x, out removed);
            });
            tokens = new List<SubscriptionToken>();
        }

    }
}
