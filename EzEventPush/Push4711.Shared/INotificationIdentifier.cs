using System;

namespace Push4711.Shared
{
    public interface INotificationIdentifier
    {
        Guid Identifier { get; set; }
    }
}
