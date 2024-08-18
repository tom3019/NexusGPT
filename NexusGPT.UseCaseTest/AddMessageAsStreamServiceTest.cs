using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In.AddMessage;
using NexusGPT.UseCase.Port.Out;
using NSubstitute;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.ObjectModels.SharedModels;

namespace NexusGPT.UseCaseTest;

public class AddMessageAsStreamServiceTest
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IOpenAIService _openAiService;
    private readonly IDomainEventBus _domainEventBus;
    private readonly FakeTimeProvider _timeProvider;

    public AddMessageAsStreamServiceTest()
    {
        _topicOutPort = Substitute.For<ITopicOutPort>();
        _messageOutPort = Substitute.For<IMessageOutPort>();
        _openAiService = Substitute.For<IOpenAIService>();
        _domainEventBus = Substitute.For<IDomainEventBus>();
        _timeProvider = new FakeTimeProvider();
    }

    private IAddMessageAsStreamService SystemUnderTest()
    {
        return new AddMessageAsStreamService(_topicOutPort, 
            _messageOutPort,
            _openAiService, 
            _domainEventBus,
            _timeProvider);
    }

    private static async IAsyncEnumerable<ChatCompletionCreateResponse> GetTestData(string answer)
    {
        yield return new ChatCompletionCreateResponse
        {
            Choices = new List<ChatChoiceResponse>
            {
                new()
                {
                    Message = new ChatMessage
                    {
                        Content = answer,
                    },
                },
            },
            Usage = new UsageResponse
            {
                PromptTokens = 100,
                CompletionTokens = 100,
            },
        };

        await Task.CompletedTask;
    }

    [Fact]
    public async Task HandleAsyncTest_輸入問題_回傳答案並發佈事件()
    {
        var channelId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var title = "title";

        var messageChannel = new Topic(channelId, memberId, title, _timeProvider);
        messageChannel.AddMessage(Guid.NewGuid(), "第一個問題",
            "第一個問題的答案", 200, 200
            ,_timeProvider);

        var question = "第二個問題";
        var systemMessage = "test";
        var resultMessage = "第二個問題的答案";

        _topicOutPort.GetAsync(channelId, memberId).Returns(messageChannel);
        _messageOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _topicOutPort.UpdateAsync(messageChannel).Returns(true);

        _openAiService.ChatCompletion.CreateCompletionAsStream(Arg.Any<ChatCompletionCreateRequest>()).Returns(
            GetTestData(resultMessage));

        var sut = SystemUnderTest();
        var actual = sut.HandleAsync(
            new AddMessageInput
            {
                TopicId = channelId,
                MemberId = memberId,
                Question = question,
                SystemMessage = systemMessage
            });

        await foreach (var se in actual)
        {
            se.Should().Be(resultMessage);
        }

        _domainEventBus.Received(1).DispatchDomainEventsAsync(Arg.Any<Topic>());
    }


    [Fact]
    public async Task HandleAsyncTest_輸入問題_更新資料庫失敗_拋出CreateMessageErrorException()
    {
        var channelId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var title = "title";

        var messageChannel = new Topic(channelId, memberId, title, _timeProvider);
        messageChannel.AddMessage(Guid.NewGuid(), "第一個問題",
            "第一個問題的答案", 200, 200,
            _timeProvider);

        var question = "第二個問題";
        var systemMessage = "test";
        var resultMessage = "第二個問題的答案";

        _topicOutPort.GetAsync(channelId, memberId).Returns(messageChannel);
        _messageOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _topicOutPort.UpdateAsync(messageChannel).Returns(false);

        _openAiService.ChatCompletion.CreateCompletionAsStream(Arg.Any<ChatCompletionCreateRequest>()).Returns(
            GetTestData(resultMessage));

        var sut = SystemUnderTest();
        var actual =  sut.HandleAsync(
            new AddMessageInput
            {
                TopicId = channelId,
                MemberId = memberId,
                Question = question,
                SystemMessage = systemMessage
            });

        var func = async () =>
        {
            await foreach (var s in actual)
            {
            }
        };

        await func.Should().ThrowAsync<CreateMessageErrorException>();
    }
    
    [Fact]
    public async Task HandleAsyncTest_找不到訊息頻道_拋出TopicNotFoundException()
    {
        var channelId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var title = "title";

        _topicOutPort.GetAsync(channelId, memberId).Returns(Topic.Null);

        var question = "第二個問題";
        var systemMessage = "test";
        var resultMessage = "第二個問題的答案";

        var sut = SystemUnderTest();
        var actual =  sut.HandleAsync(
            new AddMessageInput
            {
                TopicId = channelId,
                MemberId = memberId,
                Question = question,
                SystemMessage = systemMessage
            });

        var func = async () =>
        {
            await foreach (var s in actual)
            {
            }
        };

        await func.Should().ThrowAsync<TopicNotFoundException>();
    }
}