using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;
using NSubstitute;

namespace NexusGPT.UseCaseTest;

public class ChangeTitleServiceTest
{
    private readonly IDomainEventBus _domainEventBus;
    private readonly ITopicOutPort _topicOutPort;
    private readonly TimeProvider _timeProvider;

    public ChangeTitleServiceTest()
    {
        _topicOutPort = Substitute.For<ITopicOutPort>();
        _domainEventBus = Substitute.For<IDomainEventBus>();
        _timeProvider = new FakeTimeProvider();
    }

    private IChangeTitleService GetSystemUnderTest()
    {
        return new ChangeTitleService(_topicOutPort, _domainEventBus);
    }

    [Fact]
    public async Task HandleAsync_WhenTopicNotFound_ShouldThrowTopicNotFoundException()
    {
        // Arrange
        var topicId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var title = "New Title";
        _topicOutPort.GetAsync(topicId, memberId).Returns(Topic.Null);

        // Act
        var sut = GetSystemUnderTest();
        var act = async () => await sut.HandleAsync(topicId, memberId, title);

        // Assert
        await act.Should().ThrowAsync<TopicNotFoundException>();
    }

    [Fact]
    public async Task HandleAsync_WhenTopicFound_ShouldChangeTitle()
    {
        // Arrange
        var topicId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var newTitle = "New Title";
        var oldTitle = "old Title";
        var topic = new Topic(topicId, memberId, oldTitle, _timeProvider);
        _topicOutPort.GetAsync(topicId, memberId).Returns(topic);
        _topicOutPort.UpdateAsync(topic).Returns(true);

        // Act
        var service = GetSystemUnderTest();
        var actual = await service.HandleAsync(topicId, memberId, newTitle);

        // Assert
        topic.Title.Should().Be(newTitle);
        _domainEventBus.Received(1).DispatchDomainEventsAsync(topic);
        actual.Should().BeTrue();
    }
    
    [Fact]
    public async Task HandleAsync_WhenUpdateFailed_ShouldReturnFalse()
    {
        // Arrange
        var topicId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var newTitle = "New Title";
        var oldTitle = "old Title";
        var topic = new Topic(topicId, memberId, oldTitle, _timeProvider);
        _topicOutPort.GetAsync(topicId, memberId).Returns(topic);
        _topicOutPort.UpdateAsync(topic).Returns(false);

        // Act
        var service = GetSystemUnderTest();
        var actual = await service.HandleAsync(topicId, memberId, newTitle);

        // Assert
        _domainEventBus.DidNotReceive().DispatchDomainEventsAsync(topic);
        actual.Should().BeFalse();
    }
}