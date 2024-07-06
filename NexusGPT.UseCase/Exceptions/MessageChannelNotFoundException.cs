namespace NexusGPT.UseCase.Exceptions;

public class MessageChannelNotFoundException : Exception
{
    public MessageChannelNotFoundException(string message):base(message)
    {
    }
}