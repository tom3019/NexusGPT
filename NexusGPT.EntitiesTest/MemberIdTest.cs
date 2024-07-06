using FluentAssertions;
using NexusGPT.Entities;

namespace NexusGPT.EntitiesTest;

public class MemberIdTest
{
    [Fact]
    public void MemberId_WhenValueIsEmpty_ThrowsArgumentNullException()
    {
        // Arrange
        var value = Guid.Empty;

        // Act
        Action act = () => new MemberId(value);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The value cannot be empty. (Parameter 'value')");
    }
    
    [Fact]
    public void MemberId_WhenValueIsNotEmpty_ReturnsMemberId()
    {
        // Arrange
        var value = Guid.NewGuid();

        // Act
        var memberId = new MemberId(value);

        // Assert
        memberId.Value.Should().Be(value);
    }
}