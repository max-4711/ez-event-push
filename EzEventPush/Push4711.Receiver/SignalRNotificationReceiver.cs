using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Push4711.Receiver
{
    public interface IPushNotificationReceiverConfiguration
    {
        bool IsConfigured { get; }
        string NotificationHubUrl { get; }
        string? DefaultTypeSearchAssembly { get; }
    }

    public class PushNotificationReceiverConfiguration : IPushNotificationReceiverConfiguration
    {
        public PushNotificationReceiverConfiguration(string notificationHubUrl)
        {
            this.NotificationHubUrl = notificationHubUrl;
        }

        public PushNotificationReceiverConfiguration(string notificationHubUrl, string defaultTypeSearchAssembly) : this(notificationHubUrl)
        {
            this.DefaultTypeSearchAssembly = defaultTypeSearchAssembly;
        }

        public bool IsConfigured => !string.IsNullOrEmpty(this.NotificationHubUrl);

        public string NotificationHubUrl { get; }

        public string? DefaultTypeSearchAssembly { get; }
    }
        

    public class SignalRNotificationReceiver : INotificationReceiver
    {
        private HubConnection? _hubConnection;
        private IPushNotificationReceiverConfiguration _config;
        private IPushNotificationHandler _notificationHandler;

        private bool isInitialized = false;

        public SignalRNotificationReceiver(IPushNotificationReceiverConfiguration config, IPushNotificationHandler notificationHandler)
        {
            this._config = config;
            this._notificationHandler = notificationHandler;
        }

        public HubConnectionState ConnectionState => this._hubConnection?.State ?? HubConnectionState.Disconnected;

        public string? ConnectionId => this._hubConnection?.ConnectionId;

        public event EventHandler<HubConnectionStateChangedEventArgs>? ConnectionStateChanged;

        public async Task InitializeAsync()
        {
            if (this.isInitialized)
                return;

            if (!this._config.IsConfigured)
                return;

            this.isInitialized = true;

            this._hubConnection = new HubConnectionBuilder().WithUrl($"{this._config.NotificationHubUrl}").WithAutomaticReconnect().Build();
            this._hubConnection.Closed += _hubConnection_Closed;
            this._hubConnection.Reconnected += _hubConnection_Reconnected;
            this._hubConnection.Reconnecting += _hubConnection_Reconnecting;

            this._hubConnection.MapClientMethods(this._notificationHandler);

            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to setup SignalRNotificationReceiver: {e.Message}");
                this.ConnectionStateChanged?.Invoke(this, new HubConnectionStateChangedEventArgs { ConnectionState = HubConnectionState.Disconnected });
            }
        }

        private Task _hubConnection_Reconnecting(Exception arg)
        {
            this.ConnectionStateChanged?.Invoke(this, new HubConnectionStateChangedEventArgs { ConnectionState = HubConnectionState.Reconnecting });
            return Task.CompletedTask;
        }

        private Task _hubConnection_Reconnected(string arg)
        {
            this.ConnectionStateChanged?.Invoke(this, new HubConnectionStateChangedEventArgs { ConnectionState = HubConnectionState.Connected });
            return Task.CompletedTask;
        }

        private Task _hubConnection_Closed(Exception arg)
        {
            this.ConnectionStateChanged?.Invoke(this, new HubConnectionStateChangedEventArgs { ConnectionState = HubConnectionState.Disconnected });
            return Task.CompletedTask;
        }

        public async Task ReInitializeAsync()
        {
            this.isInitialized = false;

            await this.InitializeAsync();
        }
    }
}
