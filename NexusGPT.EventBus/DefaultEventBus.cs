using NexusGPT.SeedWork;

namespace NexusGPT.EventBus;

public class DefaultEventBus : IDomainEventBus
{
    public Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : DomainEvent
    {
        return Task.CompletedTask;
    }

    public Task DispatchDomainEventsAsync<TId>(AggregateRoot<TId> aggregateRoot) where TId : ValueObject<TId>
    {
        return Task.CompletedTask;
    }
}