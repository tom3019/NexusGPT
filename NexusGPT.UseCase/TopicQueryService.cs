using NexusGPT.Entities;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase;

public class TopicQueryService : ITopicQueryService
{
    private readonly ITopicOutPort _topicOutPort;

    public TopicQueryService(ITopicOutPort topicOutPort)
    {
        _topicOutPort = topicOutPort;
    }

    /// <summary>
    /// 取得聊天室列表
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TopicDataModel>> GetListAsync(Guid memberId)
    {
        var messageChannels = await _topicOutPort.GetListAsync(memberId);
        return messageChannels;
    }
    
    /// <summary>
    /// 取得聊天室詳細資訊
    /// </summary>
    /// <param name="topicId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public async Task<Topic> GetDetailAsync(Guid topicId,Guid memberId)
    {
        var messageChannel = await _topicOutPort.GetAsync(topicId, memberId);
        if (messageChannel.IsNull())
        {
            throw new TopicNotFoundException("找不到訊息頻道");
        }
        
        return messageChannel;
    }

    /// <summary>
    /// 搜尋聊天室
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SearchTopicDataModel>> SearchTopicAsync(Guid memberId, string keyword)
    {
        var searchMessageChannelDataModels = await _topicOutPort.SearchMessageChannelAsync(memberId, keyword);
        return searchMessageChannelDataModels;
    }

}