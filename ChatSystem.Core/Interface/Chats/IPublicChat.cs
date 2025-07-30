namespace ChatSystem.Core.Interface.Chats
{
    public interface IPublicChat
    {
        public void Handle(string message, string sender);
    }
}
