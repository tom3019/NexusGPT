using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

/// <inheritdoc />
public class CreateTopicService : ICreateTopicService
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly TimeProvider _timeProvider;
    private readonly IDomainEventBus _domainEventBus;

    public CreateTopicService(ITopicOutPort topicOutPort,
        TimeProvider timeProvider,
        IDomainEventBus domainEventBus)
    {
        _topicOutPort = topicOutPort;
        _timeProvider = timeProvider;
        _domainEventBus = domainEventBus;
    }

    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public async Task<Guid> HandleAsync(Guid memberId, string title)
    {
        var messageChannels = await _topicOutPort.GetListAsync(memberId);
        if (messageChannels.Count() >= 5)
        {
            throw new TopicMaxCountException("超過最大聊天室數量");
        }

        var topicId = await _topicOutPort.GenerateIdAsync();
        var topic = new Topic(topicId, memberId, title, _timeProvider);

        var success = await _topicOutPort.SaveAsync(topic);
        if (success)
        {
            await _domainEventBus.DispatchDomainEventsAsync(topic);
            return topicId;
        }
        
        return Guid.Empty;
    }
}