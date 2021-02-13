using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Push4711.Shared;

namespace Push4711.Sender
{
    public class PushNotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<PushNotificationSender> _logger;

        public PushNotificationSender(IHubContext<NotificationHub> hubContext, ILogger<PushNotificationSender> logger)
        {
            this._hubContext = hubContext;
            this._logger = logger;
        }

        public async void BroadcastRemoteDataNotification<TDataNotification>(TDataNotification dataNotificationObject) where TDataNotification : INotificationIdentifier
        {
            if (dataNotificationObject == null)
                return;

            try
            {
                await this._hubContext.Clients.All.SendAsync($"OnDataNotification_{typeof(TDataNotification)}", dataNotificationObject);
            }
            catch
            {
                this._logger.LogWarning($"Unable to broadcast data notification {dataNotificationObject}");
            }
        }

        public async void SendRemoteDataNotification<TDataNotification>(TDataNotification dataNotificationObject, string receiverConnectionId) where TDataNotification : INotificationIdentifier
        {
            if (dataNotificationObject == null)
                return;

            var client = this._hubContext.Clients.Client(receiverConnectionId);
            if (client != null)
            {
                try
                {
                    await client.SendAsync($"OnDataNotification_{typeof(TDataNotification)}", dataNotificationObject);
                }
                catch
                {
                    this._logger.LogWarning($"Unable to deliver data notification {dataNotificationObject} to {receiverConnectionId}");
                }
            }
        }
    }
}
