using NexusGPT.UseCase.Exceptions;

namespace NexusGPT.UseCase.Port.In;

public interface IDeleteMessageChannelService
{
    /// <summary>
    /// 刪除訊息頻道
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    /// <exception cref="MessageChannelNotFoundException"></exception>
    Task<bool> HandleAsync(Guid channelId, Guid memberId);
}