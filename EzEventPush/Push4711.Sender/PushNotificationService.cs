using Push4711.Shared;
using System.Text.Json;

namespace Push4711.Sender
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly INotificationSender _pushNotificationSender;

        public PushNotificationService(INotificationSender pushNotificationSender)
        {
            this._pushNotificationSender = pushNotificationSender;
        }

        public void BroadcastNotification(object payload)
        {
            var dataNotification = new RawJsonNotification
            {
                EmbeddedType = payload.GetType().ToString(),
                JsonPayload = JsonSerializer.Serialize(payload)
            };
            this.BroadcastNotification(dataNotification);
        }

        public void BroadcastNotification(RawJsonNotification payload)
        {
            this._pushNotificationSender.BroadcastRemoteDataNotification(payload);
        }

        public void BroadcastNotification(string type, string jsonSerializedPayload)
        {
            var dataNotification = new RawJsonNotification
            {
                EmbeddedType = type,
                JsonPayload = jsonSerializedPayload
            };
            this.BroadcastNotification(dataNotification);
        }

        public void SendNotification(object payload, string receiverConnectionId)
        {
            var dataNotification = new RawJsonNotification
            {
                EmbeddedType = payload.GetType().ToString(),
                JsonPayload = JsonSerializer.Serialize(payload)
            };
            this.SendNotification(dataNotification, receiverConnectionId);
        }

        public void SendNotification(RawJsonNotification payload, string receiverConnectionId)
        {
            this._pushNotificationSender.SendRemoteDataNotification(payload, receiverConnectionId);
        }

        public void SendNotification(string type, string jsonSerializedPayload, string receiverConnectionId)
        {
            var dataNotification = new RawJsonNotification
            {
                EmbeddedType = type,
                JsonPayload = jsonSerializedPayload
            };
            this.SendNotification(dataNotification, receiverConnectionId);
        }
    }
}
