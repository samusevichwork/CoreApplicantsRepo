using ChatSystem.Core.Data;
using ChatSystem.Core.Interface.Notifications;

namespace ChatSystem.Core.Mediators
{
    internal class NotificationMediator : INotificationMediator
    {
        private IMatchStartNotificationHandler _matchStartNotificationHandler;
        private IKillNotificationHandler _killNotificationHandler;

        public NotificationMediator(
            IMatchStartNotificationHandler matchStartNotificationHandler,
            IKillNotificationHandler killNotificationHandler)
        {
            _matchStartNotificationHandler = matchStartNotificationHandler;
            _killNotificationHandler = killNotificationHandler;
        }

        public void RouteNotification(Notification notification)
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
