using NexusGPT.Entities;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase.Port.In;

public interface ITopicQueryService
{
    /// <summary>
    /// 取得聊天室列表
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<IEnumerable<TopicDataModel>> GetListAsync(Guid memberId);

    /// <summary>
    /// 取得聊天室詳細資訊
    /// </summary>
    /// <param name="topicId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<Topic> GetDetailAsync(Guid topicId, Guid memberId);

    /// <summary>
    /// 搜尋聊天室
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    Task<IEnumerable<SearchTopicDataModel>> SearchTopicAsync(Guid memberId, string keyword);
}