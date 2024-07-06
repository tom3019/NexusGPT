namespace NexusGPT.UseCase.Exceptions;

public class ImageCreateException : Exception
{
    public ImageCreateException(string message) : base(message)
    {
    }
}