namespace NexusGPT.SeedWork;

public interface IDomainEventBus
{
    /// <summary>
    /// Publishes the domain event
    /// </summary>
    /// <typeparam name="TEvent">The event</typeparam>
    /// <param name="domainEvent">The domain event</param>
    Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : DomainEvent;

    /// <summary>
    /// DispatchDomainEventsAsync
    /// </summary>
    /// <param name="aggregateRoot"></param>
    /// <typeparam name="TId"></typeparam>
    /// <returns></returns>
    Task DispatchDomainEventsAsync<TId>(AggregateRoot<TId> aggregateRoot) where TId : ValueObject<TId>;
}