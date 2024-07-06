using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

/// <summary>
/// 匯入頻道服務
/// </summary>
public class ShareTopicService : IShareTopicService
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly TimeProvider _timeProvider;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IDomainEventBus _domainEventBus;

    public ShareTopicService(ITopicOutPort topicOutPort,
        TimeProvider timeProvider,
        IMessageOutPort messageOutPort,
        IDomainEventBus domainEventBus)
    {
        _topicOutPort = topicOutPort;
        _timeProvider = timeProvider;
        _messageOutPort = messageOutPort;
        _domainEventBus = domainEventBus;
    }

    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="id"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    /// <exception cref="TopicNotFoundException"></exception>
    public async Task<ShareTopicResultModel> HandlerAsync(Guid id, Guid memberId)
    {
        var messageChannel = await _topicOutPort.GetAsync(id);
        if (messageChannel.IsNull())
        {
            throw new TopicNotFoundException("找不到訊息頻道");
        }

        var newId = await _topicOutPort.GenerateIdAsync();
        var newMessageChannel = new Topic(newId, memberId, messageChannel.Title,
            _timeProvider);

        foreach (var messageChannelMessage in messageChannel.Messages.OrderBy(x=>x.CreateTime))
        {
            var messageId = await _messageOutPort.GenerateIdAsync();
            newMessageChannel.AddMessage(messageId,
                messageChannelMessage.Question,
                messageChannelMessage.Answer,
                messageChannelMessage.QuestionTokenCount,
                 messageChannel.TotalAnswerTokenCount,
                _timeProvider);
        }

        var success = await _topicOutPort.SaveAsync(newMessageChannel);
        if (success)
        {
            await _domainEventBus.DispatchDomainEventsAsync(messageChannel);
            return new ShareTopicResultModel
            {
                TopicId = newId,
                Title = newMessageChannel.Title
            };
        }

        return new ShareTopicResultModel
        {
            TopicId = Guid.Empty,
            Title = newMessageChannel.Title
        };
    }
}