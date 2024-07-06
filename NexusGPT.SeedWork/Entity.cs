namespace NexusGPT.SeedWork;

public abstract class Entity<TId>:IInternalEventHandler where TId : ValueObject<TId>
{
    private readonly Action<DomainEvent> _applier;

    public TId Id { get; protected set; }

    protected Entity(Action<DomainEvent> applier) => _applier = applier;

    protected Entity()
    {
    }

    protected abstract void When(DomainEvent @event);

    protected void Apply(DomainEvent @event)
    {
        When(@event);
        _applier(@event);
    }

    void IInternalEventHandler.Handle(DomainEvent @event) => When(@event);
}