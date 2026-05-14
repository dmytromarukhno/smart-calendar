using SmartCalendar.Domain.Exceptions;

namespace SmartCalendar.Domain.Entities;

public sealed class Scene
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private readonly List<Command> _commands = new();
    public IReadOnlyCollection<Command> Commands => _commands;

    // Required by EF Core
    private Scene() { }

    public Scene(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Scene name is required");

        Id = Guid.NewGuid();
        Name = name;
    }

    public void AddCommand(Command command) =>
        _commands.Add(command);
}
