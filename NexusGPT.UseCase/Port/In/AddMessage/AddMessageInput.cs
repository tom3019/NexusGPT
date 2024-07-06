namespace NexusGPT.UseCase.Port.In.AddMessage;

public class AddMessageInput
{
    public Guid TopicId { get; set; }
    public Guid MemberId { get; set; }
    public string Question { get; set; }

    public string SystemMessage { get; set; }
}