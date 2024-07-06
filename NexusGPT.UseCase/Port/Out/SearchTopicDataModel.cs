namespace NexusGPT.UseCase.Port.Out;

public class SearchTopicDataModel
{
    public Guid Id { get; set; }
    public IEnumerable<Guid> MessageIds { get; set; }
}