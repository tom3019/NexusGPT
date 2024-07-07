using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

public class ChangeTitleService : IChangeTitleService
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly IDomainEventBus _domainEventBus;

    public ChangeTitleService(ITopicOutPort topicOutPort,
        IDomainEventBus domainEventBus)
    {
        _topicOutPort = topicOutPort;
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
        var topic = await _topicOutPort.GetAsync(topicId, memberId);
        if (topic.IsNull())
        {
            throw new TopicNotFoundException("找不到訊息頻道");
        }
        
        topic.ChangeTitle(title);

        var success = await _topicOutPort.UpdateAsync(topic);
        if (success)
        {
            await _domainEventBus.DispatchDomainEventsAsync(topic);
            return true;
        }

        return false;
    }
}