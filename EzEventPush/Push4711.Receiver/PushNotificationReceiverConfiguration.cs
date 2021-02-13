namespace Push4711.Receiver
{
    public class PushNotificationReceiverConfiguration : IPushNotificationReceiverConfiguration
    {
        public PushNotificationReceiverConfiguration(string notificationHubUrl)
        {
            this.NotificationHubUrl = notificationHubUrl;
        }

        public PushNotificationReceiverConfiguration(string notificationHubUrl, string? defaultTypeSearchAssembly) : this(notificationHubUrl)
        {
            this.DefaultTypeSearchAssembly = defaultTypeSearchAssembly;
        }

        public bool IsConfigured => !string.IsNullOrEmpty(this.NotificationHubUrl);

        public string NotificationHubUrl { get; }

        public string? DefaultTypeSearchAssembly { get; }
    }
}
