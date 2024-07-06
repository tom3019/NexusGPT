using FluentAssertions;
using NexusGPT.Entities;

namespace NexusGPT.EntitiesTest;

public class MessageIdTest
{
    [Fact]
    public void MessageId_WhenValueIsEmpty_ThrowsArgumentNullException()
    {
        // Arrange
        var value = Guid.Empty;

        // Act
        Action act = () => new MessageId(value);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The value cannot be empty. (Parameter 'value')");
    }
    
    [Fact]
    public void MessageId_WhenValueIsNotEmpty_ReturnsMemberId()
    {
        // Arrange
        var value = Guid.NewGuid();

        // Act
        var memberId = new MessageId(value);

        // Assert
        memberId.Value.Should().Be(value);
    }
}