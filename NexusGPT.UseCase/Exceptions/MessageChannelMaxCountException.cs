namespace NexusGPT.UseCase.Exceptions;

public class MessageChannelMaxCountException : Exception
{
    public MessageChannelMaxCountException(string message) : base(message)
    {
    }
}