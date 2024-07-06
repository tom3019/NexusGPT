namespace NexusGPT.WebApplication.Models.ResultViewModel;

public class TopicMessageViewModel
{
    
    /// <summary>
    /// MessageId
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid MessageId { get; set; }

    /// <summary>
    /// 問題
    /// </summary>
    /// <value>
    /// The input message.
    /// </value>
    public string Question { get; set; }

    /// <summary>
    /// 答案
    /// </summary>
    /// <value>
    /// The output message.
    /// </value>
    public string Answer { get; set; }

    /// <summary>
    /// 問題Token數
    /// </summary>
    public int QuestionTokenCount { get; set; }

    /// <summary>
    /// 答案Token數
    /// </summary>
    public int AnswerTokenCount { get; set; }

    /// <summary>
    /// 總Token數
    /// </summary>
    public int TotalTokenCount { get; set; }

    /// <summary>
    /// 訊息對話建立時間
    /// </summary>
    /// <value>
    /// The create time.
    /// </value>
    public DateTime CreateTime { get; set; }

}
