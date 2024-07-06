using NexusGPT.SeedWork;

namespace NexusGPT.Entities;

public record CreateMessageChannelEvent(Guid Id, Guid MemberId, string Title,DateTimeOffset CreateTime)
    : DomainEvent;

public record AddMessageEvent(Guid ChannelId, 
    string Answer, 
    string Question,
    Guid MessageId,
    int QuestionTokenCount,
    int AnswerTokenCount,
    DateTimeOffset CreateTime)
    : DomainEvent;
    
public record ChangeMessageChannelTitleEvent(MessageChannelId Id, string Title) : DomainEvent;