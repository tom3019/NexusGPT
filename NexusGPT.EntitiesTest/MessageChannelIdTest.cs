using FluentAssertions;
using NexusGPT.Entities;

namespace NexusGPT.EntitiesTest;

public class MessageChannelIdTest
{
    [Fact]
    public void MessageChannelId_WhenValueIsEmpty_ThrowsArgumentNullException()
    {
        // Arrange
        var value = Guid.Empty;

        // Act
        Action act = () => new MessageChannelId(value);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The value cannot be empty. (Parameter 'value')");
    }

    [Fact]
    public void MessageChannelId_WhenValueIsNotEmpty_ReturnsMemberId()
    {
        // Arrange
        var value = Guid.NewGuid();

        // Act
        var memberId = new MessageChannelId(value);

        // Assert
        memberId.Value.Should().Be(value);
    }
}