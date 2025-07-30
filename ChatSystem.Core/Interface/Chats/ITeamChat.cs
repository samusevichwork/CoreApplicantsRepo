namespace ChatSystem.Core.Interface.Chats
{
    public interface ITeamChat
    {
        public void Handle(string message, string sender);
    }
}
