using NexusGPT.UseCase.Exceptions;

namespace NexusGPT.UseCase.Port.In;

public interface IDeleteTopicService
{
    /// <summary>
    /// 刪除訊息頻道
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    /// <exception cref="TopicNotFoundException"></exception>
    Task<bool> HandleAsync(Guid channelId, Guid memberId);
}