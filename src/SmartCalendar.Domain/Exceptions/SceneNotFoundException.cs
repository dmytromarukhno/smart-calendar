namespace SmartCalendar.Domain.Exceptions;

public sealed class SceneNotFoundException : DomainException
{
    public Guid SceneId { get; }

    public SceneNotFoundException(Guid id)
        : base($"Scene {id} not found") => SceneId = id;
}
