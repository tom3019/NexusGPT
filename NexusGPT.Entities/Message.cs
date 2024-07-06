using NexusGPT.SeedWork;

namespace NexusGPT.Entities;

public class Message : Entity<MessageId>
{
    /// <summary>
    /// 問題
    /// </summary>
    public string Question { get; private set; }

    /// <summary>
    /// 答案
    /// </summary>
    public string Answer { get; private set; }

    /// <summary>
    /// TopicId
    /// </summary>
    public Guid TopicId { get;private set; }
    
    /// <summary>
    /// 問題Token數
    /// </summary>
    public int QuestionTokenCount { get;private set; }
    
    /// <summary>
    /// 答案Token數
    /// </summary>
    public int AnswerTokenCount { get;private set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTimeOffset CreateTime { get;private set; }

    /// <summary>
    /// 總Token數
    /// </summary>
    public int TotalTokenCount
    {
        get => QuestionTokenCount + AnswerTokenCount;
        private set { }
    }

    public Message(Action<DomainEvent> applier) : base(applier)
    {
    }

    protected override void When(DomainEvent @event)
    {
        switch (@event)
        {
            case AddMessageEvent e:
                Id = e.MessageId;
                Question = e.Question;
                Answer = e.Answer;
                TopicId = e.TopicId;
                QuestionTokenCount = e.QuestionTokenCount;
                AnswerTokenCount = e.AnswerTokenCount;
                CreateTime = e.CreateTime;
                break;
        }
    }

    protected Message()
    {
    }
}