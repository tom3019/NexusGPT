using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.In.AddMessage;
using NexusGPT.UseCase.Port.Out;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;

namespace NexusGPT.UseCase;

public class AddMessageService : IAddMessageService
{
    private readonly ITopicOutPort _topicOutPort;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IOpenAIService _openAiService;
    private readonly IDomainEventBus _domainEventBus;
    private readonly TimeProvider _timeProvider;

    public AddMessageService(ITopicOutPort topicOutPort,
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
    public async Task<string> HandlerAsync(AddMessageInput input)
    {
        var topic = await _topicOutPort.GetAsync(input.TopicId, input.MemberId);
        if (topic.IsNull())
        {
            throw new TopicNotFoundException("找不到訊息頻道");
        }
        
        var openAiMessages = new List<ChatMessage>
        {
            ChatMessage.FromSystem(input.SystemMessage),
        };

        foreach (var messageChannelMessage in topic.Messages)
        {
            openAiMessages.Add(ChatMessage.FromUser(messageChannelMessage.Question));
            openAiMessages.Add(ChatMessage.FromAssistant(messageChannelMessage.Answer));
        }
        
        openAiMessages.Add(ChatMessage.FromUser(input.Question));

        var completionResult =
            await _openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest
                {
                    Messages = openAiMessages,
                    Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo,
                    N = 1,
                    TopP = 0.1f,
                });

        var messageId = await _messageOutPort.GenerateIdAsync();
        topic.AddMessage(messageId,
            input.Question,
            completionResult.Choices.First().Message.Content!,
            completionResult.Usage.PromptTokens,
            completionResult.Usage.CompletionTokens ?? 0,
            _timeProvider);

        var success = await _topicOutPort.UpdateAsync(topic);
        if (!success)
        {
            throw new CreateMessageErrorException("Create message failed.");
        }
        
        await _domainEventBus.DispatchDomainEventsAsync(topic);
        
        return completionResult.Choices.First().Message.Content!;
    }
}