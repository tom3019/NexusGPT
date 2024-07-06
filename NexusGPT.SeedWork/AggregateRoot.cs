using System.Collections.ObjectModel;

namespace NexusGPT.SeedWork;

public abstract class AggregateRoot<TId> where TId : ValueObject<TId>
{
     /// <summary>
    /// Gets or sets the value of the is deleted
    /// </summary>
    protected bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public TId Id { get; protected set; }

    /// <summary>
    /// Gets the version.
    /// </summary>
    public long Version { get; private set; } = -1;

    /// <summary>
    /// The domain events
    /// </summary>
    private IList<DomainEvent> UncommittedEvents { get; } = new List<DomainEvent>();

    /// <summary>
    /// Gets the uncommitted events
    /// </summary>
    /// <returns>A read only list of i domain event</returns>
    public IReadOnlyList<DomainEvent> GetUncommittedEvents()
    {
        return new ReadOnlyCollection<DomainEvent>(UncommittedEvents);
    }

    /// <summary>
    /// Describes whether this instance is changed
    /// </summary>
    /// <returns>The bool</returns>
    public bool IsChanged()
    {
        return UncommittedEvents.Any();
    }

    /// <summary>
    /// Clears the domain events.
    /// </summary>
    public void ClearUncommittedEvents()
    {
        UncommittedEvents.Clear();
    }

    /// <summary>
    /// Applies to entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="domainEvent">The domain event.</param>
    protected void ApplyToEntity(IInternalEventHandler entity, DomainEvent domainEvent)
    {
        entity?.Handle(domainEvent);
    }

    /// <summary>
    /// Raises the event using the specified domain event
    /// </summary>
    /// <param name="domainEvent">The domain event</param>
    protected void RaiseEvent(DomainEvent domainEvent)
    {
        UncommittedEvents.Add(domainEvent);
    }

    /// <summary>
    /// Applies the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected void Apply(DomainEvent domainEvent)
    {
        When(domainEvent);
        EnsureValidState();
        RaiseEvent(domainEvent);
    }

    /// <summary>
    /// Whens the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected abstract void When(DomainEvent domainEvent);

    /// <summary>
    /// Ensures the state of the valid.
    /// </summary>
    protected abstract void EnsureValidState();

    /// <summary>
    /// Loads the specified history.
    /// </summary>
    /// <param name="history">The history.</param>
    public void Load(IEnumerable<DomainEvent> history)
    {
        if (history is null)
        {
            throw new ArgumentNullException(nameof(history));
        }

        foreach (var domainEvent in history)
        {
            When(domainEvent);
            Version++;
        }
    } 
}