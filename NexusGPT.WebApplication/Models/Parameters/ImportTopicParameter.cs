using NexusGPT.WebApplication.Models.ResultViewModel;

namespace NexusGPT.WebApplication.Models.Parameters;

/// <summary>
/// ImportTopicParameter
/// </summary>
public class ImportTopicParameter
{
    /// <summary>
    /// 聊天室名稱
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; }


    /// <summary>
    /// 對話訊息
    /// </summary>
    /// <value>
    /// The messages.
    /// </value>
    public IEnumerable<TopicMessageParameter> Messages { get; set; }
    
}