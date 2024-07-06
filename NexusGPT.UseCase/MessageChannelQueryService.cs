using NexusGPT.Entities;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

public class MessageChannelQueryService : IMessageChannelQueryService
{
    private readonly IMessageChannelOutPort _messageChannelOutPort;

    public MessageChannelQueryService(IMessageChannelOutPort messageChannelOutPort)
    {
        _messageChannelOutPort = messageChannelOutPort;
    }

    /// <summary>
    /// 取得聊天室列表
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<MessageChannelDataModel>> GetListAsync(Guid memberId)
    {
        var messageChannels = await _messageChannelOutPort.GetListAsync(memberId);
        return messageChannels;
    }
    
    /// <summary>
    /// 取得聊天室詳細資訊
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public async Task<MessageChannel> GetDetailAsync(Guid channelId,Guid memberId)
    {
        var messageChannel = await _messageChannelOutPort.GetAsync(channelId, memberId);
        if (messageChannel.IsNull())
        {
            throw new MessageChannelNotFoundException("找不到訊息頻道");
        }
        
        return messageChannel;
    }

    /// <summary>
    /// 搜尋聊天室
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SearchMessageChannelDataModel>> SearchTopicAsync(Guid memberId, string keyword)
    {
        var searchMessageChannelDataModels = await _messageChannelOutPort.SearchMessageChannelAsync(memberId, keyword);
        return searchMessageChannelDataModels;
    }

}