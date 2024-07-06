namespace NexusGPT.UseCase.Port.Out;

public class ShareTopicResultModel
{
    /// <summary>
    /// 主題ID
    /// </summary>
    public Guid TopicId { get; set; }
    
    /// <summary>
    /// 標題
    /// </summary>
    public string Title { get; set; }
}