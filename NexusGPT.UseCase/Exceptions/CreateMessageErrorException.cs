namespace NexusGPT.UseCase.Exceptions;

public class CreateMessageErrorException : Exception
{
    /// <summary>
    /// 建立訊息錯誤
    /// </summary>
    /// <param name="message"></param>
    public CreateMessageErrorException(string message):base(message)
    {
    }
}