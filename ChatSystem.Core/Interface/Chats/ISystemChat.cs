namespace ChatSystem.Core.Interface.Chats
{
    internal interface ISystemChat
    {
        public void Handle(string message, string sender);
    }
}
