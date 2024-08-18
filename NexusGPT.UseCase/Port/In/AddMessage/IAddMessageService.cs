namespace NexusGPT.UseCase.Port.In.AddMessage;

public interface IAddMessageService
{
    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<string> HandleAsync(AddMessageInput input);
}