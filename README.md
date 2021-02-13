# Push4711
Need a really easy way to push typed events via SignalR? This one might be interesting for you...

# Usage / Setup
It's easy to setup and use this library. Let's walk through the process step by step while observing an example.

## Sender
Preparation: Install NuGet package `Push4711.Sender` to your project. 

Add SignalR and Push4711 to your IoC container by adding them to the service collection at startup:

    using Microsoft.Extensions.DependencyInjection;
    using Push4711.Sender;

    //...

    public void ConfigureServices(IServiceCollection services)
    {
        //... - You probably are adding some other stuff here

        services.AddSignalR();
        services.AddPushNotificationService();
    }

You are good to go! You can now obtain an IPushNotificationService instance from you IoC container and start sending push notifications - either broadcasting them or sending them to only a specific receiver (specified by the SignalR connection id):

    IPushNotificationService pushNotificationService; //Obtain this from the IoC container

    pushNotificationService.BroadcastNotification(new YourDataClassOrDomainEvent 
    { 
        SomeRandomProperty = 4711, 
        SomeRandomOtherProperty = "4712"
    });

To enable make this work, your transmitted data class -in this case YourDataClassOrDomainEvent- needs to be serializable and implement the INotificationIdentifier interface. It could for exemple use this code:

    using System;

    public class YourDataClassOrDomainEvent
    {
        public int SomeRandomProperty { get;set; }

        public string SomeRandomOtherProperty { get;set; }
    }

## Receiver
Preparation: Install NuGet pacakge `Push4711.Receiver` to your project.

The receiver needs a little bit more configuration effort. To be functional, the receiver needs a configuration; an object implementing the IPushNotificationReceiverConfiguration interface. You can either implement something yourself (in case you need some kind of dynamic configuration) or just use the included PushNotificationReceiverConfiguration class. You don't even need to instantiate it yourself, as you just can just set the whole thing up by this setup method (which adds everything needed to the IoC container):

    using Push4711.Receiver;

    //...

    public void ConfigureServices(IServiceCollection services)
    {
        //... - You probably are adding some other stuff here

        services.Add("https://my-random-hostname:4711/notificationHub/", "MyProject.CommonCode");
    }

This example code will expect your server to receive push data from at `https://my-random-hostname:4711/notificationHub/` and will search for the type definitions of received events by default in `MyProject.CommonCode`. Considering the sample sender setup above, your class YourDataClassOrDomainEvent should be within a project named `MyProject.CommonCode`, whose assembly needs to be accessible by the receiver. You can bypass this behaviour - see chapter "Advanced" for more information on this topic.

You are now ready to consume the received events! Just get a `IPushNotificationHandler` instance from the IoC container and call the `Subscribe` method. Provide the type of the data you're expecting to receive -in our previous example `YourDataClassOrDomainEvent`- and a callback with it as parameter, which will be executed upon data arrival. That's it. 

When subscribing, you will get `DataNotificationSubscription` object in return as kind of token for your subscription. The subscription can be cancelled by calling the `CancelSubscription` and passing this `DataNotificationSubscription` object. As an alternative, you can also cause the subscription to be cancelled by simply disposing the obtained `DataNotificationSubscription` object.

If you are for example using it to update a page within a Blazor project, your code could possibly look like this:

    @page "/sample"
    @using Push4711.Receiver
    @inject IPushNotificationHandler NotificationHandler
    @implements IDisposable

    <h1>SamplePage</h1>

    <p>This is a really basic sample page. SomeRandomProperty: @myDataObject.SomeRandomProperty / SomeRandomOtherProperty: @myDataObject.SomeRandomOtherProperty</p>

    @code {
        private YourDataClassOrDomainEvent myDataObject;
        private DataNotificationSubscription dataPushUpdateSubscription;

        protected override async Task OnInitializedAsync()
        {
            this.dataPushUpdateSubscription = this.NotificationHandler.Subscribe<YourDataClassOrDomainEvent>(this.HandlePushUpdateData);
        }

        private void HandlePushUpdateData(YourDataClassOrDomainEvent pushDataUpdate)
        {
            this.myDataObject = pushDataUpdate;
            base.StateHasChanged();
        }

        public void Dispose()
        {
            this.dataPushUpdateSubscription?.Dispose();
        }
    }

Of course you can also use more "traditional" code to consume your incoming push notification data:

    using System;
    using Push4711.Receiver;

    public class BusinessLogicPushDataConsumer : IDisposable
    {
        private readonly DataNotificationSubscription _pushUpdateSubscription;

        public BusinessLogicPushDataConsumer(IPushNotificationHandler notificationHandler)
        {
            this._pushUpdateSubscription = notificationHandler.Subscribe<YourDataClassOrDomainEvent>(this.HandlePushUpdate);
        }

        private void HandlePushUpdate(YourDataClassOrDomainEvent pushUpdateData)
        {
            //TODO: Do something useful with your received data!
        }

        public void Dispose()
        {
            this._pushUpdateSubscription?.Dispose();
        }
    }


# Advanced usage
Coming soon...

## Specifying full type identifier string / Manually creating the push events
Coming soon...

## Using topics
Coming soon...

## Manually cancelling subscriptions (instead of disposing)
Coming soon...

# Technical remarks
Coming soon...

# Contribution
Feedback, issues and/or merge requests are always welcome! :)