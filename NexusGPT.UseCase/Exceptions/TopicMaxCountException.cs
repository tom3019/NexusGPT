namespace NexusGPT.UseCase.Exceptions;

public class TopicMaxCountException : Exception
{
    public TopicMaxCountException(string message) : base(message)
    {
    }
}