namespace ChatSystem.Core.Interface.Chats
{
    internal interface IPublicChat
    {
        public void Handle(string message, string sender);
    }
}
