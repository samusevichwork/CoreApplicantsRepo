using ChatSystem.Core.Data;
using ChatSystem.Core.Interface.Notifications;

namespace ChatSystem.Core.Service
{
    internal class NotificationService : INotificationService, IDisposable
    {
        private readonly IChatNetwork _network;
        private readonly INotificationMediator _notificationMediator;
        private readonly Builders.NotificationBuilder _notificationBuilder;
        private IDisposable _disposable;

        public NotificationService(IChatNetwork network, INotificationMediator notificationMediator, Builders.NotificationBuilder notificationBuilder)
        {
            _network = network;
            _notificationBuilder = notificationBuilder;
            _notificationMediator = notificationMediator;
            _disposable = _network.OnEventReceived.Subscribe(HandleNotification);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private void HandleNotification((EventType, object) rawNotification)
        {
            var notification = ParseNotification(rawNotification);
            _notificationMediator.RouteNotification(notification);

            Notification ParseNotification((EventType, object) notification)
            {
                return _notificationBuilder.SetType(notification.Item1).SetMessage((string)notification.Item2).Build();
            }
        }
    }
}