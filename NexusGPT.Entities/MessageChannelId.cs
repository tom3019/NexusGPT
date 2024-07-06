using NexusGPT.SeedWork;

namespace NexusGPT.Entities;

public record MessageChannelId:ValueObject<MessageChannelId>
{
    public Guid Value { get; }

    public MessageChannelId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(value),"The value cannot be empty.");
        }
        
        Value = value;
    }
    public static implicit operator Guid(MessageChannelId self) => self.Value;
    public static implicit operator MessageChannelId(Guid self) => new(self);

    protected MessageChannelId()
    {
        
    }
}