using SmartCalendar.Domain.Exceptions;

namespace SmartCalendar.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public string PasswordHash { get; private set; } = null!;

    // Required by EF Core
    private User() { }

    public User(string email, string name, UserRole role, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required");

        Id = Guid.NewGuid();
        Email = email;
        Name = name;
        Role = role;
        PasswordHash = passwordHash;
    }
}
