namespace NexusGPT.UseCase.Port.In;

/// <summary>
/// 建立訊息頻道服務
/// </summary>
public interface ICreateMessageChannelService
{
    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    Task<Guid> HandlerAsync(Guid memberId,string title);
}