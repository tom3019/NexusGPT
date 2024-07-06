namespace NexusGPT.WebApplication.Models.ViewModels;

public class ShareTopicViewModel
{
    /// <summary>
    /// 聊天室ID
    /// </summary>
    public Guid TopicId { get; set; }
    
    /// <summary>
    /// 聊天室名稱
    /// </summary>
    public string Title { get; set; }
}