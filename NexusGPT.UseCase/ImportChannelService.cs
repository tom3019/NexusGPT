using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

/// <summary>
/// 匯入頻道服務
/// </summary>
public class ImportChannelService : IImportChannelService
{
    private readonly IMessageChannelOutPort _messageChannelOutPort;
    private readonly TimeProvider _timeProvider;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IDomainEventBus _domainEventBus;

    public ImportChannelService(IMessageChannelOutPort messageChannelOutPort,
        TimeProvider timeProvider,
        IMessageOutPort messageOutPort,
        IDomainEventBus domainEventBus)
    {
        _messageChannelOutPort = messageChannelOutPort;
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
    /// <exception cref="MessageChannelNotFoundException"></exception>
    public async Task<Guid> HandlerAsync(Guid id, Guid memberId)
    {
        var messageChannel = await _messageChannelOutPort.GetAsync(id);
        if (messageChannel.IsNull())
        {
            throw new MessageChannelNotFoundException("找不到訊息頻道");
        }

        var newId = await _messageChannelOutPort.GenerateIdAsync();
        var newMessageChannel = new MessageChannel(newId, memberId, messageChannel.Title,
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

        var success = await _messageChannelOutPort.SaveAsync(newMessageChannel);
        if (success)
        {
            await _domainEventBus.DispatchDomainEventsAsync(messageChannel);
            return newId;
        }

        return Guid.Empty;
    }
}