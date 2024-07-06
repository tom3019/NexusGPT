namespace NexusGPT.UseCase.Port.Out;

public class SearchMessageChannelDataModel
{
    public Guid Id { get; set; }
    public IEnumerable<Guid> MessageIds { get; set; }
}