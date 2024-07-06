using NexusGPT.SeedWork;

namespace NexusGPT.Entities;

public class Topic : AggregateRoot<TopicId>, INullObject
{
    /// <summary>
    /// 訊息
    /// </summary>
    public IList<Message> Messages { get; private set; } = new List<Message>();

    /// <summary>
    /// 持有人
    /// </summary>
    public MemberId MemberId { get; private set; }

    /// <summary>
    /// 聊天室標題
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// 總問題Token數
    /// </summary>
    public int TotalQuestionTokenCount
    {
        get => Messages.Sum(x => x.QuestionTokenCount);
        private set { }
    }

    /// <summary>
    /// 總答案Token數
    /// </summary>
    public int TotalAnswerTokenCount
    {
        get => Messages.Sum(x => x.AnswerTokenCount);
        private set { }
    }

    /// <summary>
    /// 總Token數
    /// </summary>
    public int TotalTokenCount
    {
        get => TotalQuestionTokenCount + TotalAnswerTokenCount;
        private set { }
    }

    /// <summary>
    /// Create Time
    /// </summary>
    public DateTimeOffset CreateTime { get; private set; }

    /// <summary>
    /// 建立訊息頻道
    /// </summary>
    /// <param name="id"></param>
    /// <param name="memberId"></param>
    /// <param name="title"></param>
    /// <param name="timeProvider"></param>
    public Topic(TopicId id,
        MemberId memberId,
        string title,
        TimeProvider timeProvider)
    {
        Messages = new List<Message>();
        Apply(new CreateTopicEvent(id, memberId, title, timeProvider.GetLocalNow()));
    }

    /// <summary>
    /// 新增訊息
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="question"></param>
    /// <param name="answer"></param>
    /// <param name="questionTokenCount"></param>
    /// <param name="answerTokenCount"></param>
    /// <param name="timeProvider"></param>
    public void AddMessage(MessageId messageId,
        string question,
        string answer,
        int questionTokenCount,
        int answerTokenCount,
        TimeProvider timeProvider)
    {
        Apply(new AddMessageEvent(Id,
            answer,
            question,
            messageId,
            questionTokenCount,
            answerTokenCount,
            timeProvider.GetLocalNow()));
    }
    
    public void ChangeTitle(string title)
    {
        Apply(new ChangeTopicTitleEvent(Id, title));
    }

    /// <summary>
    /// Whens the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected override void When(DomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case CreateTopicEvent e:
                Id = e.Id;
                MemberId = e.MemberId;
                CreateTime = e.CreateTime;
                Title = e.Title;
                break;

            case AddMessageEvent e:
                var message = new Message(Apply);
                ApplyToEntity(message, e);
                Messages.Add(message);
                break;
            
            case ChangeTopicTitleEvent e:
                Title = e.Title;
                break;
        }
    }

    protected override void EnsureValidState()
    {
    }

    protected Topic()
    {
    }

    private static readonly NullTopic? _null;

    public static Topic Null = _null ??= new NullTopic();

    public virtual bool IsNull()
    {
        return false;
    }

    private class NullTopic : Topic
    {
        public override bool IsNull()
        {
            return true;
        }
    }
}