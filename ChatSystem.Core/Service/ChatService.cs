using ChatSystem.Core.Builders;
using ChatSystem.Core.Data;
using ChatSystem.Core.Interface.Chats;
using System.Reactive.Linq;

namespace ChatSystem.Core.Service
{
    internal class ChatService : IChatMessageService, IDisposable
    {
        private readonly IChatNetwork _network;
        private readonly IChatMediator _chatMediator;
        private readonly MessageBuilder _messageBuilder;
        private readonly IDisposable _disposable;
        private readonly Queue<ChatMessage> _messagesQueue;
        private readonly object _lock;
        private CancellationTokenSource _cancellationTokenSource;

        public ChatService(IChatNetwork network, IChatMediator chatMediator, MessageBuilder messageBuilder)
        {
            _network = network;
            _messageBuilder = messageBuilder;
            _chatMediator = chatMediator;
            _disposable = _network.OnMessageReceived.Subscribe(HandleMessage);
            _messagesQueue = new Queue<ChatMessage>();
            _lock = new object();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _cancellationTokenSource?.Cancel();
        }

        public async Task SendPublicMessageAsync(string playerName, string message)
        {
            try
            {
                await _network.SendMessageAsync(message, $"{(int)MessageType.Public}:{playerName}");
            }
            catch (Exception ex)
            {
                var systemMessage = _messageBuilder.SetPlayerName(playerName).SetType(MessageType.System).SetMessage(ex.Message).Build();
                _chatMediator.RouteMessage(systemMessage);
                await AddMessageToQueueAsync(playerName, MessageType.Public, message);
            }
        }

        public async Task SendTeamMessageAsync(string playerName, string message)
        {
            try
            {
                await _network.SendMessageAsync(message, $"{(int)MessageType.Team}:{playerName}");
            }
            catch (Exception ex)
            {
                var systemMessage = _messageBuilder.SetPlayerName(playerName).SetType(MessageType.System).SetMessage(ex.Message).Build();
                _chatMediator.RouteMessage(systemMessage);
                await AddMessageToQueueAsync(playerName, MessageType.Team, message);
            }
        }

        public async Task SendSystemMessageAsync(string playerName, string message)
        {
            try
            {
                await _network.SendMessageAsync(message, $"{(int)MessageType.System}:{playerName}");
            }
            catch (Exception ex)
            {
                var systemMessage = _messageBuilder.SetPlayerName(playerName).SetType(MessageType.System).SetMessage(ex.Message).Build();
                _chatMediator.RouteMessage(systemMessage);
                await AddMessageToQueueAsync(playerName, MessageType.System, message);
            }
        }

        private async Task AddMessageToQueueAsync(string playerName, MessageType messageType, string message)
        {
            lock (_lock)
            {
                _messagesQueue.Enqueue(_messageBuilder.SetPlayerName(playerName).SetType(messageType).SetMessage(message).Build());
                if (_messagesQueue.Count == 1)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    RetrySendMessagesAsync(_cancellationTokenSource.Token).Start();
                }
            }
        }

        private void HandleMessage(string rawMessage)
        {
            var message = ParseMessage(rawMessage);
            _chatMediator.RouteMessage(message);

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

        private async Task RetrySendMessagesAsync(CancellationToken token)
        {
            var count = 1;
            do
            {
                var message = _messagesQueue.Peek();
                try
                {
                    if (token.IsCancellationRequested) return;
                    await _network.SendMessageAsync(message.Message, $"{(int)message.Type}:{message.PlayerName}");
                    if (token.IsCancellationRequested) return;
                    lock (_lock)
                    {
                        _messagesQueue.Dequeue();
                        count = _messagesQueue.Count;
                    }
                }
                catch (Exception ex)
                {
                    var systemMessage = _messageBuilder.SetPlayerName(message.PlayerName).SetType(MessageType.System).SetMessage(ex.Message).Build();
                    _chatMediator.RouteMessage(systemMessage);
                    await Task.Delay(2000, token);
                }
            }
            while (count > 0);
        }
    }
}
