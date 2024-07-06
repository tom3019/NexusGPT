namespace NexusGPT.UseCase.Port.In.AddMessage;

public class AddImageMessageInput
{
    public string Message { get; set; }
    public Guid MemberId { get; set; }
    public Guid TopicId { get; set; }
}