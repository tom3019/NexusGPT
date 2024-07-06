namespace NexusGPT.WebApplication.Models.ResultViewModel;

/// <summary>
/// MessageViewModel
/// </summary>
public class MessageViewModel
{
    public Guid MessageId { get; set; }
    
    /// <summary>
    /// 答案
    /// </summary>
    /// <value>
    /// The output message.
    /// </value>
    public string Answer { get; set; }

    /// <summary>
    /// 訊息對話建立時間
    /// </summary>
    /// <value>
    /// The create time.
    /// </value>
    public DateTime CreateTime { get; set; }
}
