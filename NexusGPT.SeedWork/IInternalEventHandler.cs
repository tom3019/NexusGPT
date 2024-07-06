namespace NexusGPT.SeedWork;

public interface IInternalEventHandler
{
    void Handle(DomainEvent domainEvent);
}