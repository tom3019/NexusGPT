using Microsoft.AspNetCore.SignalR;
using NexusGPT.UseCase.Port.In.AddMessage;
using NexusGPT.WebApplication.Models.Parameters;
using NexusGPT.WebApplication.Models.ResultViewModel;
using SignalRSwaggerGen.Attributes;

namespace NexusGPT.WebApplication.Hubs;

/// <summary>
/// 對話
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.SignalR.Hub" />
[SignalRHub("MessageHub")]
public class MessageHub : Hub
{
    private readonly IAddMessageAsStreamService _addMessageAsStreamService;
    private readonly IAddImageMessageService _addImageMessageService;

    public MessageHub(IAddMessageAsStreamService addMessageAsStreamService,
        IAddImageMessageService addImageMessageService)
    {
        _addMessageAsStreamService = addMessageAsStreamService;
        _addImageMessageService = addImageMessageService;
    }

    /// <summary>
    /// 與AI對話
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public async Task SendMessage(SendMessageParameter parameter)
    {
        var memberId = new Guid("E4727ED6-52E8-4C4C-AF92-2ED42ECF1D59");
        var createTime = DateTime.Now;
        await foreach (var messageStream in _addMessageAsStreamService.HandlerAsync(
                           new AddMessageInput
                           {
                               TopicId = parameter.TopicId,
                               MemberId = memberId,
                               Question = parameter.Message,
                               SystemMessage = "你是一個智慧AI"
                           }))
        {
            await Clients.Caller.SendAsync("SendMessageResult",
                new ResultViewModel<MessageViewModel>
                {
                    StatuesCode = 200,
                    StatusMessage = "OK",
                    Data = new MessageViewModel
                    {
                        MessageId = parameter.MessageId,
                        Answer = messageStream,
                        CreateTime = createTime,
                    }
                });
        }
    }

    /// <summary>
    /// 文字轉圖片
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public async Task SendImageMessage(ImageMessageParameter parameter)
    {
        var memberId = new Guid("E4727ED6-52E8-4C4C-AF92-2ED42ECF1D59");
        var imageUrl = await _addImageMessageService.HandlerAsync(
            new AddImageMessageInput
            {
                Message = parameter.Message,
                MemberId = memberId,
                TopicId = parameter.TopicId
            });
        
        await Clients.Caller.SendAsync("SendImageMessageResult",
            new ResultViewModel<MessageViewModel>
            {
                StatuesCode = 200,
                StatusMessage = "OK",
                Data = new MessageViewModel
                {
                    MessageId = parameter.MessageId,
                    Answer = imageUrl,
                    CreateTime = DateTime.Now,
                }
            });
    }
}