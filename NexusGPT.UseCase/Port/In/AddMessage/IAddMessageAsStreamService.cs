namespace NexusGPT.UseCase.Port.In.AddMessage;

public interface IAddMessageAsStreamService
{
    /// <summary>
    /// 處理程序
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    IAsyncEnumerable<string> HandleAsync(AddMessageInput input);
}