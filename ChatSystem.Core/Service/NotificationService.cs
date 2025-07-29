using ChatSystem.Core.Data;
using ChatSystem.Core.Interface.Notifications;
using System.Reactive.Subjects;

namespace ChatSystem.Core.Service
{
    internal class NotificationService : INotificationService, INotificationStream, IDisposable
    {
        private IChatNetwork _network;
        private Subject<Notification> _notificationStream;
        private Builders.NotificationBuilder _notificationBuilder;
        private IDisposable _disposable;

        public NotificationService(IChatNetwork network, Builders.NotificationBuilder notificationBuilder)
        {
            _network = network;
            _notificationBuilder = notificationBuilder;
            _notificationStream = new();
            _disposable = _network.OnEventReceived.Subscribe(HandleNotification);
        }

        public IObservable<Notification> Notification => _notificationStream;

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        private void HandleNotification((EventType, object) rawNotification)
        {
            var notification = ParseNotification(rawNotification);
            _notificationStream.OnNext(notification);

            Notification ParseNotification((EventType, object) notification)
            {
                return _notificationBuilder.SetType(notification.Item1).SetMessage((string)notification.Item2).Build();
            }
        }
    }
}