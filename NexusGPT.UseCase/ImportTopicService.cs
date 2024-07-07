using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

/// <summary>
/// 匯入聊天室服務
/// </summary>
public class ImportTopicService : IImportTopicService
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly TimeProvider _timeProvider;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IDomainEventBus _domainEventBus;

    public ImportTopicService(ITopicOutPort topicOutPort, 
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
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="TopicMaxCountException"></exception>
    public async Task<ShareTopicResultModel> HandlerAsync(ImportTopicInput input)
    {
        var topics = await _topicOutPort.GetListAsync(input.MemberId);
        if (topics.Count() >= 5)
        {
            throw new TopicMaxCountException("超過最大聊天室數量");
        }

        var topicId = await _topicOutPort.GenerateIdAsync();
        var topic = new Topic(topicId, input.MemberId, input.Title, _timeProvider);
        
        foreach (var importMessageParameter in input.Messages)
        {
            var messageId = await _messageOutPort.GenerateIdAsync();
            topic.AddMessage(messageId,
                importMessageParameter.Question,
                importMessageParameter.Answer,
                importMessageParameter.QuestionTokenCount, 
                importMessageParameter.AnswerTokenCount,
                _timeProvider);
        }
        
        var success = await _topicOutPort.SaveAsync(topic);
        if (success)
        {
            await _domainEventBus.DispatchDomainEventsAsync(topic);
            return new ShareTopicResultModel
            {
                TopicId = topicId,
                Title = topic.Title
            };
        }
        
        return new ShareTopicResultModel
        {
            TopicId = Guid.Empty,
            Title = topic.Title
        };
        
    }
}