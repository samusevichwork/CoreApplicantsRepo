using ChatSystem.Core;
using ChatSystem.Core.Builders;
using ChatSystem.Core.Interface.Chats;
using ChatSystem.Core.Interface.Notifications;
using ChatSystem.Core.Mediators;
using ChatSystem.Core.Service;
using ChatSystem.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ChatSystem.Tests
{
    internal class ChatSystemTests : IDisposable
    {
        private Mock<IPublicChat> _publicChat;
        private Mock<ITeamChat> _teamChat;
        private Mock<ISystemChat> _systemChat;
        private Mock<IKillNotificationHandler> _killNotificationHandler;
        private Mock<IMatchStartNotificationHandler> _matchStartNotificationHandler;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void SetUp()
        {
            ConfigurateMocks();
            ConfigurateServices();
        }

        private void ConfigurateServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IChatNetwork, MockChatNetwork>()
                .AddSingleton(_publicChat.Object)
                .AddSingleton(_teamChat.Object)
                .AddSingleton(_systemChat.Object)
                .AddSingleton(_killNotificationHandler.Object)
                .AddSingleton(_matchStartNotificationHandler.Object)
                .AddScoped<GameClient>()
                .AddScoped<IChatMessageService, ChatService>()
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<IChatMediator, ChatMediator>()
                .AddScoped<INotificationMediator, NotificationMediator>()
                .AddTransient<Core.Builders.NotificationBuilder>()
                .AddTransient<MessageBuilder>();
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigurateMocks()
        {
            _publicChat = new Mock<IPublicChat>();
            _publicChat.Setup(n => n.Handle(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((message, sender) => { Console.WriteLine($"PablicMessage: {sender}:{message}"); });
            _teamChat = new Mock<ITeamChat>();
            _teamChat.Setup(n => n.Handle(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((message, sender) => { Console.WriteLine($"TeamMessage: {sender}:{message}"); });
            _systemChat = new Mock<ISystemChat>();
            _systemChat.Setup(n => n.Handle(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((message, sender) => { Console.WriteLine($"SystemMessage: {sender}:{message}"); });
            _killNotificationHandler = new Mock<IKillNotificationHandler>();
            _killNotificationHandler.Setup(n => n.Handle(It.IsAny<string>())).Callback<string>((message) => { Console.WriteLine($"KillNotification:{message}"); });
            _matchStartNotificationHandler = new Mock<IMatchStartNotificationHandler>();
            _matchStartNotificationHandler.Setup(n => n.Handle(It.IsAny<string>())).Callback<string>((message) => { Console.WriteLine($"MatchStartNotification:{message}"); });
        }

        [TearDown]
        public void Dispose()
        {
            (_serviceProvider as IDisposable)?.Dispose();
        }

        [Test]
        public async Task SendMessages_SendingMessagesWithoutDisconect_ShouldDeliverToCorrectMessage()
        {
            var scope1 = _serviceProvider.CreateScope();
            var gameClient1 = scope1.ServiceProvider.GetRequiredService<GameClient>();
            gameClient1.Name = "Player1";
            var scope2 = _serviceProvider.CreateScope();
            var gameClient2 = scope2.ServiceProvider.GetRequiredService<GameClient>();
            gameClient2.Name = "Player2";
            gameClient1.SendPublicMessage("Hello world");
            gameClient2.SendTeamMessage("Hello team");
            gameClient2.SendSystemMessage("Error");

            await Task.Delay(5000);

            _publicChat.Verify(n => n.Handle(" Hello world", "Player1"), Times.Exactly(2));
            _teamChat.Verify(n => n.Handle(" Hello team", "Player2"), Times.Exactly(2));
            _systemChat.Verify(n => n.Handle(" Error", "Player2"), Times.Exactly(2));
            scope2.Dispose();
            scope1.Dispose();
        }

        [Test]
        public async Task SendMessages_SendingMessagesWithDisconect_ShouldDeliverToCorrectMessage()
        {
            var chatNetwork = _serviceProvider.GetRequiredService<IChatNetwork>();
            var scope1 = _serviceProvider.CreateScope();
            var gameClient1 = scope1.ServiceProvider.GetRequiredService<GameClient>();
            gameClient1.Name = "Player1";
            var scope2 = _serviceProvider.CreateScope();
            var gameClient2 = scope2.ServiceProvider.GetRequiredService<GameClient>();
            gameClient2.Name = "Player2";
            gameClient1.SendPublicMessage("Hello world");
            chatNetwork.SimulateDisconnect();
            await Task.Delay(1000);
            gameClient1.SendTeamMessage("Hello");
            gameClient2.SendTeamMessage("Hello team");
            await Task.Delay(1000);
            chatNetwork.SimulateReconnect();

            await Task.Delay(5000);

            _publicChat.Verify(n => n.Handle(" Hello world", "Player1"), Times.Exactly(2));
            _teamChat.Verify(n => n.Handle(" Hello team", "Player2"), Times.Exactly(2));
            scope2.Dispose();
            scope1.Dispose();
        }
    }
}
