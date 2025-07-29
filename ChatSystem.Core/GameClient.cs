using ChatSystem.Core.Interface.Chats;
using ChatSystem.Core.Interface.Notifications;

namespace ChatSystem.Core
{
    internal class GameClient
    {
        private IChatMessageService _chatMessageService;
        private INotificationService _notificationService;

        public GameClient(
            IChatMessageService chatMessageService,
            INotificationService notificationService)
        {
            _chatMessageService = chatMessageService;
            _notificationService = notificationService;
        }

        public string Name;

        public void SendPublicMessage(string message)
        {
            _chatMessageService.SendPublicMessage(Name, message);
        }

        public void SendTeamMessage(string message)
        {
            _chatMessageService.SendTeamMessage(Name, message);
        }

        public void SendSystemMessage(string message)
        {
            _chatMessageService.SendTeamMessage(Name, message);
        }
    }
}
