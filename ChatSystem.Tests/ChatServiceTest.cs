using ChatSystem.Core;
using ChatSystem.Core.Data;
using ChatSystem.Core.Interface.Chats;
using ChatSystem.Core.Service;
using Moq;
using System.Reactive.Subjects;

namespace ChatSystem.Tests
{
    internal class ChatServiceTest
    {
        private Mock<IChatNetwork> _mockNetwork;
        private Mock<IChatMediator> _mockChatMediator;

        [SetUp]
        public void SetUp()
        {
            _mockNetwork = new Mock<IChatNetwork>();
            _mockNetwork.Setup(n => n.OnMessageReceived).Returns(new Subject<string>());
            _mockNetwork.Setup(n => n.OnEventReceived).Returns(new Subject<(EventType, object)>());
            _mockChatMediator = new Mock<IChatMediator>();
        }

        [Test]
        public async Task SendPublicMessageAsync_SendsCorrectFormat()
        {
            // Arrange
            string? receivedMessage = null;
            var chatService = new ChatService(_mockNetwork.Object, _mockChatMediator.Object, new Core.Builders.MessageBuilder());
            _mockNetwork.Setup(n => n.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns(Task.CompletedTask)
                       .Callback<string, string>((msg, sender) =>
                       {
                           receivedMessage = $"{sender}: {msg}";
                       });
            // Act
            await chatService.SendPublicMessageAsync("Player1", "Hello");
            // Assert
            _mockNetwork.Verify(n => n.SendMessageAsync("Hello", "1:Player1"), Times.Once());
            Assert.That(receivedMessage, Is.EqualTo("1:Player1: Hello"));
        }

        [Test]
        public async Task SendTeamMessageAsync_SendsCorrectFormat()
        {
            // Arrange
            string? receivedMessage = null;
            var chatService = new ChatService(_mockNetwork.Object, _mockChatMediator.Object, new Core.Builders.MessageBuilder());
            _mockNetwork.Setup(n => n.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns(Task.CompletedTask)
                       .Callback<string, string>((msg, sender) =>
                       {
                           receivedMessage = $"{sender}: {msg}";
                       });
            // Act
            await chatService.SendTeamMessageAsync("Player2", "Hello team");
            // Assert
            _mockNetwork.Verify(n => n.SendMessageAsync("Hello team", "2:Player2"), Times.Once());
            Assert.That(receivedMessage, Is.EqualTo("2:Player2: Hello team"));
        }

        [Test]
        public async Task SendSystemMessageAsync_SendsCorrectFormat()
        {
            // Arrange
            string? receivedMessage = null;
            var chatService = new ChatService(_mockNetwork.Object, _mockChatMediator.Object, new Core.Builders.MessageBuilder());
            _mockNetwork.Setup(n => n.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns(Task.CompletedTask)
                       .Callback<string, string>((msg, sender) =>
                       {
                           receivedMessage = $"{sender}: {msg}";
                       });
            // Act
            await chatService.SendSystemMessageAsync("Player3", "Error 404");
            // Assert
            _mockNetwork.Verify(n => n.SendMessageAsync("Error 404", "3:Player3"), Times.Once());
            Assert.That(receivedMessage, Is.EqualTo("3:Player3: Error 404"));
        }

        [Test]
        public void HandleMessage_ParsesRawPublicMessage_Correctly()
        {
            // Arrange
            ChatMessage receivedMessage = new ChatMessage();
            _mockChatMediator.Setup(m => m.RouteMessage(It.IsAny<ChatMessage>())).Callback<ChatMessage>(msg => receivedMessage = msg);
            var messageSubject = new Subject<string>();
            _mockNetwork.Setup(n => n.OnMessageReceived).Returns(messageSubject);
            var chatService = new ChatService(_mockNetwork.Object, _mockChatMediator.Object, new Core.Builders.MessageBuilder());
            // Act
            messageSubject.OnNext($"1:Player1: Hello");
            // Assert
            Assert.That(receivedMessage.Type, Is.EqualTo(MessageType.Public));
            Assert.That(receivedMessage.Message, Is.EqualTo(" Hello"));
            Assert.That(receivedMessage.PlayerName, Is.EqualTo("Player1"));
        }

        [Test]
        public void HandleMessage_ParsesRawTeamMessage_Correctly()
        {
            // Arrange
            ChatMessage receivedMessage = new ChatMessage();
            _mockChatMediator.Setup(m => m.RouteMessage(It.IsAny<ChatMessage>())).Callback<ChatMessage>(msg => receivedMessage = msg);
            var messageSubject = new Subject<string>();
            _mockNetwork.Setup(n => n.OnMessageReceived).Returns(messageSubject);
            var chatService = new ChatService(_mockNetwork.Object, _mockChatMediator.Object, new Core.Builders.MessageBuilder());
            // Act
            messageSubject.OnNext($"2:Player2: Hello team");
            // Assert
            Assert.That(receivedMessage.Type, Is.EqualTo(MessageType.Team));
            Assert.That(receivedMessage.Message, Is.EqualTo(" Hello team"));
            Assert.That(receivedMessage.PlayerName, Is.EqualTo("Player2"));
        }

        [Test]
        public void HandleMessage_ParsesRawSystemMessage_Correctly()
        {
            // Arrange
            ChatMessage receivedMessage = new ChatMessage();
            _mockChatMediator.Setup(m => m.RouteMessage(It.IsAny<ChatMessage>())).Callback<ChatMessage>(msg => receivedMessage = msg);
            var messageSubject = new Subject<string>();
            _mockNetwork.Setup(n => n.OnMessageReceived).Returns(messageSubject);
            var chatService = new ChatService(_mockNetwork.Object, _mockChatMediator.Object, new Core.Builders.MessageBuilder());
            // Act
            messageSubject.OnNext($"3:Player3: Error 404");
            // Assert
            Assert.That(receivedMessage.Type, Is.EqualTo(MessageType.System));
            Assert.That(receivedMessage.Message, Is.EqualTo(" Error 404"));
            Assert.That(receivedMessage.PlayerName, Is.EqualTo("Player3"));
        }
    }
}
