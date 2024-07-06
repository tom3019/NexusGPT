namespace NexusGPT.WebApplication.Models.ViewModels;

public class TopicDetailViewModel
{

    /// <summary>
    /// 聊天室Id
    /// </summary>
    /// <value>
    /// The topic identifier.
    /// </value>
    public Guid TopicId { get; set; }

    /// <summary>
    /// 聊天室名稱
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title  { get; set; }


    /// <summary>
    /// 對話訊息
    /// </summary>
    /// <value>
    /// The messages.
    /// </value>
    public IEnumerable<TopicMessageViewModel> Messages { get; set; }

    /// <summary>
    /// 聊天室建立時間
    /// </summary>
    /// <value>
    /// The create time.
    /// </value>
    public DateTime CreateTime { get; set; }
}
