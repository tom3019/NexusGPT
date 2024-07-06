using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

public class ChangeTitleService : IChangeTitleService
{
    private readonly ITopicOutPort _topic;
    private readonly IDomainEventBus _domainEventBus;

    public ChangeTitleService(ITopicOutPort topic,
        IDomainEventBus domainEventBus)
    {
        _topic = topic;
        _domainEventBus = domainEventBus;
    }

    /// <summary>
    /// 變更聊天室標題
    /// </summary>
    /// <param name="topicId"></param>
    /// <param name="memberId"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public async Task<bool> HandleAsync(Guid topicId, Guid memberId, string title)
    {
        var messageChannel = await _topic.GetAsync(topicId, memberId);
        if (messageChannel.IsNull())
        {
            throw new MessageChannelNotFoundException("找不到訊息頻道");
        }
        
        messageChannel.ChangeTitle(title);

        var success = await _topic.UpdateAsync(messageChannel);
        if (success)
        {
            await _domainEventBus.DispatchDomainEventsAsync(messageChannel);
            return true;
        }

        return false;
    }
}