using FluentAssertions;
using NexusGPT.Entities;

namespace NexusGPT.EntitiesTest;

public class TopicIdTest
{
    [Fact]
    public void MessageChannelId_WhenValueIsEmpty_ThrowsArgumentNullException()
    {
        // Arrange
        var value = Guid.Empty;

        // Act
        Action act = () => new TopicId(value);

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
        var memberId = new TopicId(value);

        // Assert
        memberId.Value.Should().Be(value);
    }
}