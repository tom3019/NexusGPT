using NexusGPT.Entities;

namespace NexusGPT.UseCase.Port.Out;

public interface IMessageChannelOutPort
{
    /// <summary>
    /// 產生Id
    /// </summary>
    /// <returns></returns>
    Task<Guid> GenerateIdAsync();

    /// <summary>
    /// 儲存
    /// </summary>
    /// <param name="messageChannel"></param>
    /// <returns></returns>
    Task<bool> SaveAsync(MessageChannel messageChannel);

    /// <summary>
    /// 取得MessageChannel
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<MessageChannel> GetAsync(Guid channelId, Guid memberId);

    /// <summary>
    /// 更新MessageChannel
    /// </summary>
    /// <param name="messageChannel"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(MessageChannel messageChannel);

    /// <summary>
    /// 刪除頻道
    /// </summary>
    /// <param name="messageChannel"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(MessageChannel messageChannel);

    /// <summary>
    /// 取得清單
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<IEnumerable<MessageChannelDataModel>> GetListAsync(Guid memberId);

    /// <summary>
    /// 取得MessageChannel
    /// </summary>
    /// <param name="channelId"></param>
    /// <returns></returns>
    Task<MessageChannel> GetAsync(Guid channelId);

    /// <summary>
    /// 搜尋聊天室
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    Task<IEnumerable<SearchMessageChannelDataModel>> SearchMessageChannelAsync
        (Guid memberId, string keyword);
}