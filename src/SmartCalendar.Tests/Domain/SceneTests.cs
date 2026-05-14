using FluentAssertions;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Exceptions;

namespace SmartCalendar.Tests.Domain;

public sealed class SceneTests
{
    [Fact]
    public void Constructor_WithValidName_CreatesScene()
    {
        var scene = new Scene("Cinema Mode");

        scene.Name.Should().Be("Cinema Mode");
        scene.Id.Should().NotBeEmpty();
        scene.Commands.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithEmptyName_ThrowsDomainException()
    {
        var act = () => new Scene(string.Empty);
        act.Should().Throw<DomainException>();
    }
}
