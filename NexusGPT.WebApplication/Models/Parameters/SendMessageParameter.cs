using NexusGPT.WebApplication.Models.Parameters.Enums;

namespace NexusGPT.WebApplication.Models.Parameters;


/// <summary>
/// SendMessageParameter
/// </summary>
public class SendMessageParameter
{
    /// <summary>
    /// 聊天室Id
    /// </summary>
    /// <value>
    /// The topic identifier.
    /// </value>
    public Guid TopicId { get; set; }

    /// <summary>
    /// 問題訊息
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string Message { get; set; }

    /// <summary>
    /// 訊息Id
    /// </summary>
    public Guid MessageId { get; set; }

}
