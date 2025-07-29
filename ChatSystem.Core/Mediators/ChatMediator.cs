using ChatSystem.Core.Data;
using ChatSystem.Core.Interface.Chats;

namespace ChatSystem.Core.Mediators
{
    internal class ChatMediator : IDisposable
    {
        private IChatMessageStream _chatService;
        private IDisposable _disposable;
        private IPublicChat _publicChat;
        private ITeamChat _teamChat;
        private ISystemChat _systemChat;

        public ChatMediator(
            IChatMessageStream chatService,
            IPublicChat publicChat,
            ITeamChat teamChat,
            ISystemChat systemChat)
        {
            _chatService = chatService;
            _publicChat = publicChat;
            _teamChat = teamChat;
            _systemChat = systemChat;
            _disposable = _chatService.Message.Subscribe(RouteMessage);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        private void RouteMessage(ChatMessage message)
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
