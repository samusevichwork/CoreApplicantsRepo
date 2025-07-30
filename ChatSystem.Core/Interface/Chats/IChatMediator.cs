using ChatSystem.Core.Data;

namespace ChatSystem.Core.Interface.Chats
{
    public interface IChatMediator
    {
        public void RouteMessage(ChatMessage message);
    }
}
