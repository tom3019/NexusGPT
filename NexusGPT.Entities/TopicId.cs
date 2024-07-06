using NexusGPT.SeedWork;

namespace NexusGPT.Entities;

public record TopicId:ValueObject<TopicId>
{
    public Guid Value { get; }

    public TopicId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(value),"The value cannot be empty.");
        }
        
        Value = value;
    }
    public static implicit operator Guid(TopicId self) => self.Value;
    public static implicit operator TopicId(Guid self) => new(self);

    protected TopicId()
    {
        
    }
}