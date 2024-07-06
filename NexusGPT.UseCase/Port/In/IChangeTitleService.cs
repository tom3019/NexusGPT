namespace NexusGPT.UseCase.Port.In;

public interface IChangeTitleService
{
    /// <summary>
    /// 變更聊天室標題
    /// </summary>
    /// <param name="topicId"></param>
    /// <param name="memberId"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    Task<bool> HandleAsync(Guid topicId, Guid memberId, string title);
}