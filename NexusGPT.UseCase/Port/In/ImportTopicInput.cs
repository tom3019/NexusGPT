namespace NexusGPT.UseCase.Port.In;

public class ImportTopicInput
{
    public Guid MemberId { get; set; }
    public string Title { get; set; }
    public IEnumerable<ImportMessageParameter> Messages { get; set; }
}