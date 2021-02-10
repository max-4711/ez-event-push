using System;

namespace Push4711.Shared
{
    public class RawJsonNotification : INotificationIdentifier
    {
        public Guid Identifier { get; set; } = Guid.NewGuid();

        public string? EmbeddedType { get; set; }

        public string? JsonPayload { get; set; }

        public string? TopicIdentifier { get; set; }
    }
}
