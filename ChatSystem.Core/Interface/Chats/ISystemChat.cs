namespace ChatSystem.Core.Interface.Chats
{
    public interface ISystemChat
    {
        public void Handle(string message, string sender);
    }
}
