using Push4711.Shared;

namespace Push4711.Sender
{
    interface IPushNotificationService
    {
        void BroadcastRemoteDataNotification<TDataNotification>(TDataNotification dataNotificationObject) where TDataNotification : INotificationIdentifier;

        void SendRemoteDataNotification<TDataNotification>(TDataNotification dataNotificationObject, string receiverConnectionId) where TDataNotification : INotificationIdentifier;
    }
}
