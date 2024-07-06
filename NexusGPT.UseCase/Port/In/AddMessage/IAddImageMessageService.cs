namespace NexusGPT.UseCase.Port.In.AddMessage;

/// <summary>
/// 新增圖片訊息輸入
/// </summary>
public interface IAddImageMessageService
{
    /// <summary>
    /// 處理新增圖片訊息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<string> HandlerAsync(AddImageMessageInput input);
}