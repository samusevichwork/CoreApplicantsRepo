using ChatSystem.Core.Data;

namespace ChatSystem.Core.Builders
{
    public class NotificationBuilder
    {
        private EventType _type;
        private string _message;

        public NotificationBuilder SetType(EventType type)
        {
            _type = type;
            return this;
        }

        public NotificationBuilder SetMessage(string message)
        {
            _message = message;
            return this;
        }

        public Notification Build()
        {
            return new Notification() { Type = _type, Message = _message };
        }
    }
}