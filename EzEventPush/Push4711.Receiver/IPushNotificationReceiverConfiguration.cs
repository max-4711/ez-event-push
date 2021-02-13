namespace Push4711.Receiver
{
    public interface IPushNotificationReceiverConfiguration
    {
        bool IsConfigured { get; }
        string NotificationHubUrl { get; }
        string? DefaultTypeSearchAssembly { get; }
    }
}
