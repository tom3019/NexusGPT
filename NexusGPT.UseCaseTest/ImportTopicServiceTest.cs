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

public class ImportTopicServiceTest
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly TimeProvider _timeProvider;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IDomainEventBus _domainEventBus;

    public ImportTopicServiceTest()
    {
        _topicOutPort = Substitute.For<ITopicOutPort>();
        _timeProvider = new FakeTimeProvider();
        _messageOutPort = Substitute.For<IMessageOutPort>();
        _domainEventBus = Substitute.For<IDomainEventBus>();
    }

    private IImportTopicService GetSystemUnderTest()
    {
        return new ImportTopicService(_topicOutPort, _timeProvider, _messageOutPort, _domainEventBus);
    }

    [Fact]
    public async Task HandleAsync_WhenMessageChannelsCountIsGreaterThan5_ThrowTopicMaxCountException()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var input = new ImportTopicInput
        {
            MemberId = memberId,
            Title = "Title",
            Messages = new List<ImportMessageParameter>
            {
                new ImportMessageParameter
                {
                    Question = "Question",
                    Answer = "Answer",
                    QuestionTokenCount = 1,
                    AnswerTokenCount = 1
                }
            }
        };
        _topicOutPort.GetListAsync(memberId).Returns(
            new List<TopicDataModel>
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
                },
                new()
                {
                    Id = Guid.NewGuid(),
                },
                new()
                {
                    Id = Guid.NewGuid(),
                }
            });
        
        // Act
        var sut = GetSystemUnderTest();
        Func<Task> act = async () => await sut.HandleAsync(input);
        
        // Assert
        await act.Should().ThrowAsync<TopicMaxCountException>();
    }
    
    [Fact]
    public async Task HandleAsync_WhenSaveTopicSuccess_ReturnShareTopicResultModel()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var input = new ImportTopicInput
        {
            MemberId = memberId,
            Title = "Title",
            Messages = new List<ImportMessageParameter>
            {
                new ImportMessageParameter
                {
                    Question = "Question",
                    Answer = "Answer",
                    QuestionTokenCount = 1,
                    AnswerTokenCount = 1
                }
            }
        };
        _topicOutPort.GetListAsync(memberId).Returns(new List<TopicDataModel>());
        _topicOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _messageOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _topicOutPort.SaveAsync(Arg.Any<Topic>()).Returns(true);
        
        // Act
        var sut = GetSystemUnderTest();
        var result = await sut.HandleAsync(input);
        
        // Assert
        result.Should().NotBeNull();
        result.TopicId.Should().NotBe(Guid.Empty);
        result.Title.Should().Be(input.Title);
    }
    
    [Fact]
    public async Task HandleAsync_WhenSaveTopicFailed_ReturnShareTopicResultModel()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var input = new ImportTopicInput
        {
            MemberId = memberId,
            Title = "Title",
            Messages = new List<ImportMessageParameter>
            {
                new ImportMessageParameter
                {
                    Question = "Question",
                    Answer = "Answer",
                    QuestionTokenCount = 1,
                    AnswerTokenCount = 1
                }
            }
        };
        _topicOutPort.GetListAsync(memberId).Returns(new List<TopicDataModel>());
        _topicOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _messageOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _topicOutPort.SaveAsync(Arg.Any<Topic>()).Returns(false);
        
        // Act
        var sut = GetSystemUnderTest();
        var result = await sut.HandleAsync(input);
        
        // Assert
        result.Should().NotBeNull();
        result.TopicId.Should().Be(Guid.Empty);
        result.Title.Should().Be(input.Title);
    }
}