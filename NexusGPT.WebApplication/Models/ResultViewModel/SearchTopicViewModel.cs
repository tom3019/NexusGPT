namespace NexusGPT.WebApplication.Models.ResultViewModel;

/// <summary>
/// SearchTopicViewModel
/// </summary>
public class SearchTopicViewModel
{

    /// <summary>
    /// 聊天室Id
    /// </summary>
    /// <value>
    /// The topic identifier.
    /// </value>
    public Guid TopicId { get; set; }

    /// <summary>
    /// MessageId
    /// </summary>
    /// <value>
    /// The message identifier.
    /// </value>
    public IEnumerable<Guid> MessageId { get; set; }
}
