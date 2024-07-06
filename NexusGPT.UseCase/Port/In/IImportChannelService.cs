using NexusGPT.UseCase.Exceptions;

namespace NexusGPT.UseCase.Port.In;

public interface IImportChannelService
{
    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="id"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    /// <exception cref="MessageChannelNotFoundException"></exception>
    Task<Guid> HandlerAsync(Guid id, Guid memberId);
}