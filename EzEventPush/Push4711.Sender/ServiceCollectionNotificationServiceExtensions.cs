using Microsoft.Extensions.DependencyInjection;

namespace Push4711.Sender
{
    public static class ServiceCollectionNotificationServiceExtensions
    {
        public static IServiceCollection AddPushNotificationService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<INotificationSender, PushNotificationSender>();
            serviceCollection.AddSingleton<IPushNotificationService, PushNotificationService>();
            return serviceCollection;
        }
    }
}
