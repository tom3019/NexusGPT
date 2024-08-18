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

public class ShareTopicServiceTest
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly TimeProvider _timeProvider;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IDomainEventBus _domainEventBus;
    
    public ShareTopicServiceTest()
    {
        _topicOutPort = Substitute.For<ITopicOutPort>();
        _timeProvider = new FakeTimeProvider();
        _messageOutPort = Substitute.For<IMessageOutPort>();
        _domainEventBus = Substitute.For<IDomainEventBus>();
    }
    
    private IShareTopicService GetSystemUnderTest()
    {
        return new ShareTopicService(_topicOutPort, _timeProvider, _messageOutPort, _domainEventBus);
    }
    
    [Fact]
    public async Task HandleAsync_WhenMessageChannelIsNull_ThrowTopicNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        _topicOutPort.GetAsync(id).Returns(Topic.Null);
        var sut = GetSystemUnderTest();
        
        // Act
        var act = async () => await sut.HandleAsync(id, memberId);
        
        // Assert
        await act.Should().ThrowAsync<TopicNotFoundException>();
    }
    
    [Fact]
    public async Task HandleAsync_WhenMessageChannelIsNotNull_ShouldReturnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var topic = new Topic(id, memberId, "Title", _timeProvider);
        var messageId = Guid.NewGuid();
        topic.AddMessage(messageId, "Question", "Answer", 1, 1, _timeProvider);
        _topicOutPort.GetAsync(id).Returns(topic);
        _topicOutPort.GenerateIdAsync().Returns(id);
        _messageOutPort.GenerateIdAsync().Returns(messageId);
        _topicOutPort.SaveAsync(Arg.Any<Topic>()).Returns(true);
        var sut = GetSystemUnderTest();
        
        // Act
        var actual = await sut.HandleAsync(id, memberId);
        
        // Assert
        actual.Should().NotBeNull();
        actual.TopicId.Should().Be(id);
        _domainEventBus.Received(1).DispatchDomainEventsAsync(Arg.Any<Topic>());
    }
    
    [Fact]
    public async Task HandleAsync_WhenSaveFailed_ShouldReturnTopicIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var topic = new Topic(id, memberId, "Title", _timeProvider);
        var messageId = Guid.NewGuid();
        topic.AddMessage(messageId, "Question", "Answer", 1, 1, _timeProvider);
        _topicOutPort.GetAsync(id).Returns(topic);
        _topicOutPort.GenerateIdAsync().Returns(id);
        _messageOutPort.GenerateIdAsync().Returns(messageId);
        _topicOutPort.SaveAsync(Arg.Any<Topic>()).Returns(false);
        var sut = GetSystemUnderTest();
        
        // Act
        var actual = await sut.HandleAsync(id, memberId);
        
        // Assert
        actual.TopicId.Should().Be(Guid.Empty);
    }
}