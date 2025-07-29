namespace ChatSystem.Core.Interface.Chats
{
    internal interface IChatMessageService
    {
        public void SendPublicMessage(string playerName, string message);
        public void SendTeamMessage(string playerName, string message);
        public void SendSystemMessage(string playerName, string message);
    }
}
