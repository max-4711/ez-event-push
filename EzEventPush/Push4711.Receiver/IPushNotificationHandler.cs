using Push4711.Shared;
using System;

namespace Push4711.Receiver
{
    public interface IPushNotificationHandler
    {
        void OnDataNotification(RawJsonNotification rawJsonNotification);

        //TODO: Weitere Nachrichtentypen ergänzen?

        DataNotificationSubscription Subscribe<TNotification>(Action<TNotification> notificationCallback);

        DataNotificationSubscription Subscribe<TNotification>(Action<TNotification> notificationCallback, string topicFilter);

        void CancelSubscription(DataNotificationSubscription subscription);
    }
}
