using NexusGPT.SeedWork;

namespace NexusGPT.Entities;

public record MessageId : ValueObject<MessageId>
{
    public Guid Value { get; }
    
    public MessageId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(value),"The value cannot be empty.");
        }
        
        Value = value;
    }
    
    public static implicit operator Guid(MessageId self) => self.Value;
    public static implicit operator MessageId(Guid self) => new(self);

    protected MessageId()
    {
        
    }
}