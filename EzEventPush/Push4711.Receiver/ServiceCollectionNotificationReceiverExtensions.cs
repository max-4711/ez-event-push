using Microsoft.Extensions.DependencyInjection;
using System;

namespace Push4711.Receiver
{
    public static class ServiceCollectionNotificationReceiverExtensions
    {
        public static IServiceCollection AddPushNotificationService(IServiceCollection services, IPushNotificationReceiverConfiguration pushNotificationReceiverConfiguration)
        {

            services.AddSingleton<INotificationReceiver, SignalRNotificationReceiver>();
            services.AddSingleton<IPushNotificationHandler, PushNotificationHandler>();
            services.AddSingleton(pushNotificationReceiverConfiguration);

            return services;
        }

        public static IServiceCollection AddPushNotificationService(IServiceCollection services, Func<IServiceProvider, IPushNotificationReceiverConfiguration> pushNotificationReceiverConfiguration)
        {

            services.AddSingleton<INotificationReceiver, SignalRNotificationReceiver>();
            services.AddSingleton<IPushNotificationHandler, PushNotificationHandler>();
            services.AddSingleton<IPushNotificationReceiverConfiguration>(pushNotificationReceiverConfiguration);

            return services;
        }

        public static IServiceCollection AddPushNotificationService(IServiceCollection services, string notificationHubUrl, string? defaultTypeSearchAssembly)
        {

            services.AddSingleton<INotificationReceiver, SignalRNotificationReceiver>();
            services.AddSingleton<IPushNotificationHandler, PushNotificationHandler>();
            services.AddSingleton(new PushNotificationReceiverConfiguration(notificationHubUrl, defaultTypeSearchAssembly));

            return services;
        }
    }
}
