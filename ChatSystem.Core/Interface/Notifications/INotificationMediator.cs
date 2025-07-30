using ChatSystem.Core.Data;

namespace ChatSystem.Core.Interface.Notifications
{
    public interface INotificationMediator
    {
        public void RouteNotification(Notification notification);
    }
}
