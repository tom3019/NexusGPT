using NexusGPT.Entities;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase.Port.In;

public interface IMessageChannelQueryService
{
    /// <summary>
    /// 取得聊天室列表
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<IEnumerable<MessageChannelDataModel>> GetListAsync(Guid memberId);

    /// <summary>
    /// 取得聊天室詳細資訊
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<MessageChannel> GetDetailAsync(Guid channelId, Guid memberId);

    /// <summary>
    /// 搜尋聊天室
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    Task<IEnumerable<SearchMessageChannelDataModel>> SearchTopicAsync(Guid memberId, string keyword);
}