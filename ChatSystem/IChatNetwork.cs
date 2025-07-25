using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ChatSystem.Core
{
    public interface IChatNetwork
    {
        Task SendMessageAsync(string message, string sender);
        Task RaiseEventAsync(EventType eventType, object data);
        ISubject<string> OnMessageReceived { get; }
        ISubject<(EventType, object)> OnEventReceived { get; }
        void SimulateDisconnect();
        void SimulateReconnect();
    }

    public enum EventType
    {
        MatchStart,
        KillNotification
    }
}
