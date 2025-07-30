using ChatSystem.Core.Data;
using ChatSystem.Core.Interface.Chats;

namespace ChatSystem.Core.Mediators
{
    internal class ChatMediator : IChatMediator
    {
        private IPublicChat _publicChat;
        private ITeamChat _teamChat;
        private ISystemChat _systemChat;

        public ChatMediator(
            IPublicChat publicChat,
            ITeamChat teamChat,
            ISystemChat systemChat)
        {
            _publicChat = publicChat;
            _teamChat = teamChat;
            _systemChat = systemChat;
        }

        public void RouteMessage(ChatMessage message)
        {
            switch (message.Type)
            {
                case MessageType.Public:
                    _publicChat.Handle(message.Message, message.PlayerName);
                    break;
                case MessageType.Team:
                    _teamChat.Handle(message.Message, message.PlayerName);
                    break;
                case MessageType.System:
                    _systemChat.Handle(message.Message, message.PlayerName);
                    break;
                default:
                    break;
            }
        }
    }
}
