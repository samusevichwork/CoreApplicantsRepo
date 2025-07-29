using ChatSystem.Core.Data;

namespace ChatSystem.Core.Interface.Notifications
{
    public interface INotificationStream
    {
        public IObservable<Notification> Notification { get; }
    }
}
