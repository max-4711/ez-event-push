using Microsoft.AspNetCore.SignalR.Client;

namespace Push4711.Receiver
{
    public class HubConnectionStateChangedEventArgs
    {
        public HubConnectionState ConnectionState { get; set; }
    }
}
