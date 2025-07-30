namespace ChatSystem.Core.Interface.Chats
{
    internal interface IChatMessageService
    {
        public Task SendPublicMessageAsync(string playerName, string message);
        public Task SendTeamMessageAsync(string playerName, string message);
        public Task SendSystemMessageAsync(string playerName, string message);
    }
}
