using Push4711.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Push4711.Receiver
{
    public class PushNotificationHandler : IPushNotificationHandler
    {
        private class StoredSubscription
        {
            public Guid SubscriptionIdentifier { get; }

            public object SubscriptionAction { get; }

            public string? TopicFilter { get; }

            public StoredSubscription(Guid subscriptionIdentifier, object subscriptionAction)
            {
                this.SubscriptionIdentifier = subscriptionIdentifier;
                this.SubscriptionAction = subscriptionAction;
            }

            public StoredSubscription(Guid subscriptionIdentifier, object subscriptionAction, string? topicFilter) : this(subscriptionIdentifier, subscriptionAction)
            {
                this.TopicFilter = topicFilter;
            }
        }

        private readonly IDictionary<Type, List<StoredSubscription>> NotificationCallbackRegister = new Dictionary<Type, List<StoredSubscription>>();
        private readonly IPushNotificationReceiverConfiguration _config;

        public PushNotificationHandler(IPushNotificationReceiverConfiguration config)
        {
            this._config = config;
        }

        public void OnDataNotification(RawJsonNotification rawJsonNotification)
        {
            if (rawJsonNotification.EmbeddedType == null || string.IsNullOrEmpty(rawJsonNotification.JsonPayload))
                return;

            string typeString;
            if (rawJsonNotification.EmbeddedType.Contains(',') || string.IsNullOrEmpty(this._config.DefaultTypeSearchAssembly))
            {
                typeString = rawJsonNotification.EmbeddedType;
            }
            else
            {
                typeString = $"{rawJsonNotification.EmbeddedType}, {this._config.DefaultTypeSearchAssembly}";
            }

            var embeddedType = Type.GetType(typeString, false);
            if (embeddedType == null)
            {
                Console.WriteLine($"Unable to load embedded type from string '{typeString}'. Could not process data notification.");
                return;
            }

            dynamic? payloadObject = JsonSerializer.Deserialize(rawJsonNotification.JsonPayload, embeddedType);

            if (payloadObject == null)
                return;

            if (this.NotificationCallbackRegister.TryGetValue(embeddedType, out var registeredSubscriptions))
            {
                foreach (var subscription in registeredSubscriptions)
                {
                    if (string.IsNullOrEmpty(subscription.TopicFilter) || subscription.TopicFilter == rawJsonNotification.TopicIdentifier)
                    {
                        var callbackObject = subscription.SubscriptionAction;

                        var actionType = typeof(Action<>);
                        Type[] actionTypeArgs = { embeddedType };
                        var genericActionType = actionType.MakeGenericType(actionTypeArgs);
                        var actualCallbackObjectType = callbackObject.GetType();
                        if (actualCallbackObjectType == genericActionType)
                        {
                            dynamic callback = callbackObject;
                            try
                            {
                                callback.Invoke(payloadObject);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"There was an exception while executing the registered callback of the data notification: {ex}/{ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Registered callback for notification is not of expected type {genericActionType} and thus cannot be executed (actual type: {actualCallbackObjectType}).");
                        }
                    }
                    //else
                    //{
                    //    Console.WriteLine($"Subscription {subscription.SubscriptionIdentifier} has configured filter {subscription.TopicFilter} which does not match the topic of the notification {rawJsonNotification.TopicIdentifier}");
                    //}
                }
            }
            //else
            //{
            //    Console.WriteLine($"No active subscriptions found for payload type '{typeString}'");
            //}
        }

        public DataNotificationSubscription Subscribe<TNotification>(Action<TNotification> notificationCallback)
        {
            return this.Subscribe(notificationCallback, null);
        }

        public void CancelSubscription(DataNotificationSubscription subscription)
        {
            foreach (var kvp in this.NotificationCallbackRegister)
            {
                var storedSubscription = kvp.Value.FirstOrDefault(x => x.SubscriptionIdentifier == subscription.SubscriptionIdentifier);
                if (storedSubscription != null)
                {
                    kvp.Value.Remove(storedSubscription);
                    return;
                }
            }
        }

        public DataNotificationSubscription Subscribe<TNotification>(Action<TNotification> notificationCallback, string? topicFilter)
        {
            var subscriptionIdentifier = Guid.NewGuid();
            var externalHandle = new DataNotificationSubscription(subscriptionIdentifier, this);
            var internalHandle = new StoredSubscription(subscriptionIdentifier, notificationCallback, topicFilter);

            var type = typeof(TNotification);
            if (this.NotificationCallbackRegister.TryGetValue(type, out var registeredCallbacks))
            {
                registeredCallbacks.Add(internalHandle);
            }
            else
            {
                this.NotificationCallbackRegister[type] = new List<StoredSubscription> { internalHandle };
            }

            return externalHandle;
        }
    }
}
