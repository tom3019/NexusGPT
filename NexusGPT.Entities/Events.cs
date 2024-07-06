using NexusGPT.SeedWork;

namespace NexusGPT.Entities;

public record CreateTopicEvent(Guid Id, Guid MemberId, string Title,DateTimeOffset CreateTime)
    : DomainEvent;

public record AddMessageEvent(Guid TopicId, 
    string Answer, 
    string Question,
    Guid MessageId,
    int QuestionTokenCount,
    int AnswerTokenCount,
    DateTimeOffset CreateTime)
    : DomainEvent;
    
public record ChangeTopicTitleEvent(TopicId Id, string Title) : DomainEvent;