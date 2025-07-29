using ChatSystem.Core.Data;
using ChatSystem.Core.Interface;
using ChatSystem.Core.Interface.Notifications;

namespace ChatSystem.Core.Mediators
{
    internal class NotificationMediator : IDisposable
    {
        private INotificationService _notificationService;
        private IDisposable _disposable;
        private IMatchStartNotificationHandler _matchStartNotificationHandler;
        private IKillNotificationHandler _killNotificationHandler;

        public NotificationMediator(
            INotificationService notificationService,
            IMatchStartNotificationHandler matchStartNotificationHandler,
            IKillNotificationHandler killNotificationHandler)
        {
            _notificationService = notificationService;
            _matchStartNotificationHandler = matchStartNotificationHandler;
            _killNotificationHandler = killNotificationHandler;
            _disposable = _notificationService.Notification.Subscribe(RouteNotification);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        private void RouteNotification(Notification notification)
        {
            switch (notification.Type)
            {
                case EventType.MatchStart:
                    _matchStartNotificationHandler.Handle(notification.Message);
                    break;
                case EventType.KillNotification:
                    _killNotificationHandler.Handle(notification.Message);
                    break;
                default:
                    break;
            }
        }
    }
}
