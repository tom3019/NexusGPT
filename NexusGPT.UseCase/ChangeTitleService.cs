using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

public class ChangeTitleService : IChangeTitleService
{
    private readonly IMessageChannelOutPort _messageChannel;
    private readonly IDomainEventBus _domainEventBus;

    public ChangeTitleService(IMessageChannelOutPort messageChannel,
        IDomainEventBus domainEventBus)
    {
        _messageChannel = messageChannel;
        _domainEventBus = domainEventBus;
    }

    /// <summary>
    /// 變更聊天室標題
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="memberId"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public async Task<bool> HandleAsync(Guid channelId, Guid memberId, string title)
    {
        var messageChannel = await _messageChannel.GetAsync(channelId, memberId);
        if (messageChannel.IsNull())
        {
            throw new MessageChannelNotFoundException("找不到訊息頻道");
        }
        
        messageChannel.ChangeTitle(title);

        var success = await _messageChannel.UpdateAsync(messageChannel);
        if (success)
        {
            await _domainEventBus.DispatchDomainEventsAsync(messageChannel);
            return true;
        }

        return false;
    }
}