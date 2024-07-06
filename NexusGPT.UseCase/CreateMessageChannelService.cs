using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

/// <inheritdoc />
public class CreateMessageChannelService : ICreateMessageChannelService
{
    private readonly IMessageChannelOutPort _messageChannelOutPort;
    private readonly TimeProvider _timeProvider;
    private readonly IDomainEventBus _domainEventBus;

    public CreateMessageChannelService(IMessageChannelOutPort messageChannelOutPort,
        TimeProvider timeProvider,
        IDomainEventBus domainEventBus)
    {
        _messageChannelOutPort = messageChannelOutPort;
        _timeProvider = timeProvider;
        _domainEventBus = domainEventBus;
    }

    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public async Task<Guid> HandlerAsync(Guid memberId, string title)
    {
        var messageChannels = await _messageChannelOutPort.GetListAsync(memberId);
        if (messageChannels.Count() >= 5)
        {
            throw new MessageChannelMaxCountException("超過最大聊天室數量");
        }

        var channelId = await _messageChannelOutPort.GenerateIdAsync();
        var messageChannel = new MessageChannel(channelId, memberId, title, _timeProvider);

        var success = await _messageChannelOutPort.SaveAsync(messageChannel);
        if (success)
        {
            await _domainEventBus.DispatchDomainEventsAsync(messageChannel);
            return channelId;
        }
        
        return Guid.Empty;
    }
}