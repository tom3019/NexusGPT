namespace NexusGPT.UseCase.Port.Out;

public class MessageChannelDataModel
{
    /// <summary>
    /// Id
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    /// 聊天室名稱
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; }

    /// <summary>
    /// 聊天室建立時間
    /// </summary>
    /// <value>
    /// The create time.
    /// </value>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 最後訊息時間
    /// </summary>
    public DateTime? LastMessageCreateTime { get; set; }
}