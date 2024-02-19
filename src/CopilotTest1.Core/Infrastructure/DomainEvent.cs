using MediatR;
using Sketch.EventSourcing;

namespace CopilotTest1.Core.Infrastructure
{
    public abstract class DomainEvent : Event, INotification
    {
    }
}
