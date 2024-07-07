using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NexusGPT.Entities;
using NexusGPT.UseCase;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;
using NSubstitute;

namespace NexusGPT.UseCaseTest;

public class TopicQueryServiceTest
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly TimeProvider _timeProvider;
    
    public TopicQueryServiceTest()
    {
        _topicOutPort = Substitute.For<ITopicOutPort>();
        _timeProvider = new FakeTimeProvider();
    }
    
    private ITopicQueryService GetSystemUnderTest()
    {
        return new TopicQueryService(_topicOutPort);
    }
    
    [Fact]
    public async Task GetListAsync_WhenCalled_ReturnTopics()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var topicDataModels = new List<TopicDataModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
            }
        };
        _topicOutPort.GetListAsync(memberId).Returns(topicDataModels);
        var sut = GetSystemUnderTest();
        
        // Act
        var actual = await sut.GetListAsync(memberId);
        
        // Assert
        actual.Should().BeEquivalentTo(topicDataModels);
    }
    
    [Fact]
    public async Task GetDetailAsync_WhenTopicIsNull_ThrowTopicNotFoundException()
    {
        // Arrange
        var topicId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        _topicOutPort.GetAsync(topicId, memberId).Returns(Topic.Null);
        var sut = GetSystemUnderTest();
        
        // Act
        Func<Task> act = async () => await sut.GetDetailAsync(topicId, memberId);
        
        // Assert
        await act.Should().ThrowAsync<TopicNotFoundException>();
    }
    
    [Fact]
    public async Task GetDetailAsync_WhenTopicIsNotNull_ReturnTopic()
    {
        // Arrange
        var topicId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var topic = new Topic(topicId, memberId, "title", _timeProvider);
        _topicOutPort.GetAsync(topicId, memberId).Returns(topic);
        var sut = GetSystemUnderTest();
        
        // Act
        var actual = await sut.GetDetailAsync(topicId, memberId);
        
        // Assert
        actual.Should().BeEquivalentTo(topic);
    }
    
    [Fact]
    public async Task SearchTopicAsync_WhenCalled_ReturnTopicDataModels()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var keyword = "keyword";
        var topicDataModels = new List<SearchTopicDataModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
            }
        };
        _topicOutPort.SearchMessageChannelAsync(memberId, keyword).Returns(topicDataModels);
        var sut = GetSystemUnderTest();
        
        // Act
        var actual = await sut.SearchTopicAsync(memberId, keyword);
        
        // Assert
        actual.Should().BeEquivalentTo(topicDataModels);
    }
}