using NexusGPT.SeedWork;

namespace NexusGPT.Entities;

public record MemberId:ValueObject<MemberId>
{
    public Guid Value { get; }
    
    public MemberId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(value),"The value cannot be empty.");
        }
        
        Value = value;
    }
    
    public static implicit operator Guid(MemberId self) => self.Value;
    public static implicit operator MemberId(Guid self) => new(self);

    protected MemberId()
    {
        
    }
}