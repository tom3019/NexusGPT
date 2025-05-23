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

public class CreateTopicServiceTest
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly TimeProvider _timeProvider;
    private readonly IDomainEventBus _domainEventBus;


    public CreateTopicServiceTest()
    {
        _topicOutPort = Substitute.For<ITopicOutPort>();
        _timeProvider = new FakeTimeProvider();
        _domainEventBus = Substitute.For<IDomainEventBus>();
    }

    public ICreateTopicService GetSystemUnderTest()
    {
        return new CreateTopicService(_topicOutPort, _timeProvider, _domainEventBus);
    }

    [Fact]
    public async Task HandleAsyncTest_輸入MemberId_建立訊息頻道成功_傳回True與發送事件()
    {
        var memberId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        _topicOutPort.GenerateIdAsync().Returns(channelId);
        _topicOutPort.SaveAsync(Arg.Any<Topic>()).Returns(true);

        var title = "title";

        var sut = GetSystemUnderTest();
        var actual = await sut.HandleAsync(memberId, title);

        actual.Should().Be(channelId);
        _domainEventBus.Received(1).DispatchDomainEventsAsync(Arg.Any<Topic>());
    }

    [Fact]
    public async Task HandleAsyncTest_輸入MemberId_建立訊息頻道失敗_傳回False()
    {
        var memberId = Guid.NewGuid();
        _topicOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _topicOutPort.SaveAsync(Arg.Any<Topic>()).Returns(false);
        var title = "title";

        var sut = GetSystemUnderTest();
        var actual = await sut.HandleAsync(memberId, title);

        actual.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task HandleAsyncTest_建立頻道超過用戶最大值_拋出TopicMaxCountException()
    {
        var memberId = Guid.NewGuid();
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
        var title = "title";

        var sut = GetSystemUnderTest();
        var actual = () => sut.HandleAsync(memberId, title);

        await actual.Should().ThrowAsync<TopicMaxCountException>();
    }
}