using System;

namespace Push4711.Receiver
{
    public class DataNotificationSubscription : IDisposable
    {
        private bool isCanceled = false;
        private readonly IPushNotificationHandler _notificationHandler;

        public Guid SubscriptionIdentifier { get; }

        public DataNotificationSubscription(Guid subscriptionIdentifier, IPushNotificationHandler notificationHandler)
        {
            this._notificationHandler = notificationHandler;
            this.SubscriptionIdentifier = subscriptionIdentifier;
        }

        public void Cancel()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.isCanceled)
                return;

            this._notificationHandler.CancelSubscription(this);

            this.isCanceled = true;
        }
    }
}
