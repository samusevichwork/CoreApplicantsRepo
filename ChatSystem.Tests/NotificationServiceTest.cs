using ChatSystem.Core;
using ChatSystem.Core.Data;
using ChatSystem.Core.Interface.Notifications;
using ChatSystem.Core.Service;
using Moq;
using System.Reactive.Subjects;

namespace ChatSystem.Tests
{
    internal class NotificationServiceTest
    {
        private Mock<IChatNetwork> _mockNetwork;
        private Mock<INotificationMediator> _mockNotificationMediator;

        [SetUp]
        public void SetUp()
        {
            _mockNetwork = new Mock<IChatNetwork>();
            _mockNetwork.Setup(n => n.OnMessageReceived).Returns(new Subject<string>());
            _mockNetwork.Setup(n => n.OnEventReceived).Returns(new Subject<(EventType, object)>());
            _mockNotificationMediator = new Mock<INotificationMediator>();
        }

        [Test]
        public void HandleMessage_ParsesRawMatchStartEvent_Correctly()
        {
            // Arrange
            Notification receivedNotification = new Notification();
            _mockNotificationMediator
                .Setup(m => m.RouteNotification(It.IsAny<Notification>()))
                .Callback<Notification>(notification => receivedNotification = notification);
            var eventSubject = new Subject<(EventType, object)>();
            _mockNetwork.Setup(n => n.OnEventReceived).Returns(eventSubject);
            var notificationService = new NotificationService(_mockNetwork.Object, _mockNotificationMediator.Object, new Core.Builders.NotificationBuilder());
            // Act
            eventSubject.OnNext((EventType.MatchStart, "Start"));
            // Assert
            Assert.That(receivedNotification.Type, Is.EqualTo(EventType.MatchStart));
            Assert.That(receivedNotification.Message, Is.EqualTo("Start"));
        }

        [Test]
        public void HandleMessage_ParsesRawTeamMessage_Correctly()
        {
            // Arrange
            Notification receivedNotification = new Notification();
            _mockNotificationMediator
                .Setup(m => m.RouteNotification(It.IsAny<Notification>()))
                .Callback<Notification>(notification => receivedNotification = notification);
            var eventSubject = new Subject<(EventType, object)>();
            _mockNetwork.Setup(n => n.OnEventReceived).Returns(eventSubject);
            var notificationService = new NotificationService(_mockNetwork.Object, _mockNotificationMediator.Object, new Core.Builders.NotificationBuilder());
            // Act
            eventSubject.OnNext((EventType.KillNotification, "Player1 kill Player2"));
            // Assert
            Assert.That(receivedNotification.Type, Is.EqualTo(EventType.KillNotification));
            Assert.That(receivedNotification.Message, Is.EqualTo("Player1 kill Player2"));
        }
    }
}
