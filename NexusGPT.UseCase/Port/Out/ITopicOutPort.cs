using NexusGPT.Entities;

namespace NexusGPT.UseCase.Port.Out;

public interface ITopicOutPort
{
    /// <summary>
    /// 產生Id
    /// </summary>
    /// <returns></returns>
    Task<Guid> GenerateIdAsync();

    /// <summary>
    /// 儲存
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    Task<bool> SaveAsync(Topic topic);

    /// <summary>
    /// 取得MessageChannel
    /// </summary>
    /// <param name="topicId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<Topic> GetAsync(Guid topicId, Guid memberId);

    /// <summary>
    /// 更新MessageChannel
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(Topic topic);

    /// <summary>
    /// 刪除頻道
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Topic topic);

    /// <summary>
    /// 取得清單
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<IEnumerable<TopicDataModel>> GetListAsync(Guid memberId);

    /// <summary>
    /// 取得MessageChannel
    /// </summary>
    /// <param name="topicId"></param>
    /// <returns></returns>
    Task<Topic> GetAsync(Guid topicId);

    /// <summary>
    /// 搜尋聊天室
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    Task<IEnumerable<SearchTopicDataModel>> SearchMessageChannelAsync
        (Guid memberId, string keyword);
}