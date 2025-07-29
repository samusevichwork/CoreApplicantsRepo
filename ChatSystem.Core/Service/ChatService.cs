using ChatSystem.Core.Builders;
using ChatSystem.Core.Data;
using ChatSystem.Core.Interface.Chats;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ChatSystem.Core.Service
{
    internal class ChatService : IChatMessageStream, IChatMessageService, IDisposable
    {
        private IChatNetwork _network;
        private Subject<ChatMessage> _messageStream;
        private MessageBuilder _messageBuilder;
        private IDisposable _disposable;

        public ChatService(IChatNetwork network, MessageBuilder messageBuilder)
        {
            _network = network;
            _messageBuilder = messageBuilder;
            _messageStream = new();
            _disposable = _network.OnMessageReceived.Subscribe(HandleMessage);
        }

        public IObservable<ChatMessage> Message => _messageStream;

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        public void SendPublicMessage(string playerName, string message)
        {
            _network.SendMessageAsync(message, $"{(int)MessageType.Public}:{playerName}");
        }

        public void SendTeamMessage(string playerName, string message)
        {
            _network.SendMessageAsync(message, $"{(int)MessageType.Team}:{playerName}");
        }

        public void SendSystemMessage(string playerName, string message)
        {
            _network.SendMessageAsync(message, $"{(int)MessageType.System}:{playerName}");
        }

        private void HandleMessage(string rawMessage)
        {
            var message = ParseMessage(rawMessage);
            _messageStream.OnNext(message);

            ChatMessage ParseMessage(string rawMessage)
            {
                var info = rawMessage.Split(':');
                MessageType type = MessageType.None;
                if (int.TryParse(info[0], out int result))
                {
                    type = (MessageType)result;
                }
                return _messageBuilder.SetPlayerName(info[1]).SetType(type).SetMessage(info[2]).Build();
            }
        }
    }
}
