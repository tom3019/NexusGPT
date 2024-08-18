using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase.Port.In;

public interface IImportTopicService
{
    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="TopicMaxCountException"></exception>
    Task<ShareTopicResultModel> HandleAsync(ImportTopicInput input);
}