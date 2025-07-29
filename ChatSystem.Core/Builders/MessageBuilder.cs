using ChatSystem.Core.Data;

namespace ChatSystem.Core.Builders
{
    public class MessageBuilder
    {
        private string _playerName = string.Empty;
        private MessageType _type;
        private string _message = string.Empty;

        public MessageBuilder SetPlayerName(string playerName)
        {
            _playerName = playerName;
            return this;
        }
        public MessageBuilder SetType(MessageType type)
        {
            _type = type;
            return this;
        }

        public MessageBuilder SetMessage(string message)
        {
            _message = message;
            return this;
        }

        public ChatMessage Build()
        {
            return new ChatMessage() { PlayerName = _playerName, Type = _type, Message = _message };
        }
    }
}
