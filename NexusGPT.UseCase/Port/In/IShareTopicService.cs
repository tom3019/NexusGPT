using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase.Port.In;

public interface IShareTopicService
{
    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="id"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    /// <exception cref="TopicNotFoundException"></exception>
    Task<ShareTopicResultModel> HandleAsync(Guid id, Guid memberId);
}