using NexusGPT.SeedWork;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In.AddMessage;
using NexusGPT.UseCase.Port.Out;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace NexusGPT.UseCase;

/// <inheritdoc />
public class AddImageMessageService : IAddImageMessageService
{
    private readonly IOpenAIService _openAiService;
    private readonly ITopicOutPort _topicOutPort;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IImageStorageOutPort _imageStorageOutPort;
    private readonly IDomainEventBus _domainEventBus;
    private readonly TimeProvider _timeProvider;

    public AddImageMessageService(IOpenAIService openAiService,
        ITopicOutPort topicOutPort, 
        IMessageOutPort messageOutPort, 
        IImageStorageOutPort imageStorageOutPort, 
        IDomainEventBus domainEventBus, 
        TimeProvider timeProvider)
    {
        _openAiService = openAiService;
        _topicOutPort = topicOutPort;
        _messageOutPort = messageOutPort;
        _imageStorageOutPort = imageStorageOutPort;
        _domainEventBus = domainEventBus;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 處理新增圖片訊息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<string> HandlerAsync(AddImageMessageInput input)
    {
        var messageChannel = await _topicOutPort.GetAsync(input.TopicId, input.MemberId);
        if (messageChannel.IsNull())
        {
            throw new MessageChannelNotFoundException("找不到訊息頻道");
        }

        var imageCreateResponse = await _openAiService.Image.CreateImage(
            new ImageCreateRequest
            {
                N = 1,
                Size = StaticValues.ImageStatics.Size.Size1024,
                ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Base64,
                User = null,
                Model = Models.Dall_e_3,
                Prompt = input.Message,
                Quality = StaticValues.ImageStatics.Quality.Standard,
            });

        if (!imageCreateResponse.Successful)
        {
            throw new ImageCreateException("生成圖片失敗");
        }

        var imageBase64 = imageCreateResponse.Results.First().B64;
        var imageUrl = await _imageStorageOutPort.SaveObjectAsync(imageBase64);
        
        var messageId = await _messageOutPort.GenerateIdAsync();
        
        var answerMd = @$"![{input.Message.Replace("/draw","")}]({imageUrl})";
        
        messageChannel.AddMessage(
            messageId,
            input.Message,
            answerMd,
            0,
            0,
            _timeProvider);
        
        var success = await _topicOutPort.UpdateAsync(messageChannel);
        if (!success)
        {
            throw new CreateMessageErrorException("Create message failed.");
        }
        
        await _domainEventBus.DispatchDomainEventsAsync(messageChannel);
        return answerMd;
    }
}