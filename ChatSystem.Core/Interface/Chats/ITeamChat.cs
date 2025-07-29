namespace ChatSystem.Core.Interface.Chats
{
    internal interface ITeamChat
    {
        public void Handle(string message, string sender);
    }
}
