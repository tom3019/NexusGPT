using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;
using NSubstitute;

namespace NexusGPT.UseCaseTest;

public class CreateMessageChannelServiceTest
{
    private readonly IMessageChannelOutPort _messageChannelOutPort;
    private readonly TimeProvider _timeProvider;
    private readonly IDomainEventBus _domainEventBus;
    

    public CreateMessageChannelServiceTest()
    {
        _messageChannelOutPort  = Substitute.For<IMessageChannelOutPort>();
        _timeProvider = new FakeTimeProvider();
        _domainEventBus = Substitute.For<IDomainEventBus>();
    }

    public ICreateMessageChannelService GetSystemUnderTest()
    {
        return new CreateMessageChannelService(_messageChannelOutPort,_timeProvider,_domainEventBus);
    }
    
    [Fact]
    public async Task HandlerAsyncTest_輸入MemberId_建立訊息頻道成功_傳回True與發送事件()
    {
        var memberId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        _messageChannelOutPort.GenerateIdAsync().Returns(channelId);
        _messageChannelOutPort.SaveAsync(Arg.Any<MessageChannel>()).Returns(true);

        var title = "title";
        
        var sut = GetSystemUnderTest();
        var actual = await sut.HandlerAsync(memberId,title);
        
        actual.Should().Be(channelId);
         _domainEventBus.Received(1).DispatchDomainEventsAsync(Arg.Any<MessageChannel>());
    }

    [Fact]
    public async Task HandlerAsyncTest_輸入MemberId_建立訊息頻道失敗_傳回False()
    {
        var memberId = Guid.NewGuid();
        _messageChannelOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _messageChannelOutPort.SaveAsync(Arg.Any<MessageChannel>()).Returns(false);
        var title = "title";
        
        var sut = GetSystemUnderTest();
        var actual = await sut.HandlerAsync(memberId,title);
        
        actual.Should().Be(Guid.Empty);
    }
}