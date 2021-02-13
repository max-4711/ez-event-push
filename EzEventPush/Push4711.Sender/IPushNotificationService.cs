using Push4711.Shared;

namespace Push4711.Sender
{
    public interface IPushNotificationService
    {
        public void BroadcastNotification(object payload);
        public void BroadcastNotification(RawJsonNotification payload);
        public void BroadcastNotification(string type, string jsonSerializedPayload);

        public void SendNotification(object payload, string receiverConnectionId);
        public void SendNotification(RawJsonNotification payload, string receiverConnectionId);
        public void SendNotification(string type, string jsonSerializedPayload, string receiverConnectionId);
    }
}
