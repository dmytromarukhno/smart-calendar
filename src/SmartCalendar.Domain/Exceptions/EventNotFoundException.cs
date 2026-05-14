namespace SmartCalendar.Domain.Exceptions;

public sealed class EventNotFoundException : DomainException
{
    public Guid EventId { get; }

    public EventNotFoundException(Guid id)
        : base($"Event {id} not found") => EventId = id;
}
