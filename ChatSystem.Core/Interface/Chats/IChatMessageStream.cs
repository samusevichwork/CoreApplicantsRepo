using ChatSystem.Core.Data;

namespace ChatSystem.Core.Interface.Chats
{
    internal interface IChatMessageStream
    {
        public IObservable<ChatMessage> Message { get; }
    }
}
