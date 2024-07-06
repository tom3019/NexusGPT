using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In.AddMessage;
using NexusGPT.UseCase.Port.Out;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;

namespace NexusGPT.UseCase;

public class AddMessageAsStreamService : IAddMessageAsStreamService
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IOpenAIService _openAiService;
    private readonly IDomainEventBus _domainEventBus;
    private readonly TimeProvider _timeProvider;
    
    public AddMessageAsStreamService(ITopicOutPort topicOutPort,
        IMessageOutPort messageOutPort,
        IOpenAIService openAiService, 
        IDomainEventBus domainEventBus, 
        TimeProvider timeProvider)
    {
        _topicOutPort = topicOutPort;
        _messageOutPort = messageOutPort;
        _openAiService = openAiService;
        _domainEventBus = domainEventBus;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<string> HandlerAsync(AddMessageInput input)
    {
        var messageChannel = await _topicOutPort.GetAsync(input.TopicId, input.MemberId);
        if (messageChannel.IsNull())
        {
            throw new TopicNotFoundException("找不到訊息頻道");
        }
        
        var openAiMessages = new List<ChatMessage>
        {
            ChatMessage.FromSystem(input.SystemMessage),
        };

        foreach (var messageChannelMessage in messageChannel.Messages)
        {
            openAiMessages.Add(ChatMessage.FromUser(messageChannelMessage.Question));
            openAiMessages.Add(ChatMessage.FromAssistant(messageChannelMessage.Answer));
        }
        
        openAiMessages.Add(ChatMessage.FromUser(input.Question));

        var completionResult =
             _openAiService.ChatCompletion.CreateCompletionAsStream(
                new ChatCompletionCreateRequest
                {
                    Messages = openAiMessages,
                    Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo,
                    N = 1,
                    TopP = 0.1f,
                    StreamOptions = new StreamOptions
                    {
                        IncludeUsage = true
                    }
                });
        
        var answer = string.Empty;
        var promptTokens = 0;
        var completionTokens = 0;

        await foreach (var chatCompletionCreateResponse in completionResult)
        {
            promptTokens += chatCompletionCreateResponse?.Usage?.PromptTokens ?? 0;
            completionTokens += chatCompletionCreateResponse?.Usage?.CompletionTokens ?? 0;
            var message = chatCompletionCreateResponse?.Choices?.FirstOrDefault()?.Message.Content!;
            answer += message;
            yield return message;
        }
        
        var messageId = await _messageOutPort.GenerateIdAsync();
        messageChannel.AddMessage(messageId,
            input.Question,
            answer,
            promptTokens,
            completionTokens,
            _timeProvider);

        var success = await _topicOutPort.UpdateAsync(messageChannel);
        if (!success)
        {
            throw new CreateMessageErrorException("Create message failed.");
        }
        
        await _domainEventBus.DispatchDomainEventsAsync(messageChannel);
    }
}