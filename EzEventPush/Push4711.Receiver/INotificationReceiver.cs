using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Push4711.Receiver
{
    public interface INotificationReceiver
    {
        event EventHandler<HubConnectionStateChangedEventArgs> ConnectionStateChanged;

        HubConnectionState ConnectionState { get; }

        string? ConnectionId { get; }

        Task InitializeAsync();

        Task ReInitializeAsync();
    }
}
