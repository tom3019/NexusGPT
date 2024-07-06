using Microsoft.Extensions.DependencyInjection;
using NexusGPT.SeedWork;

namespace NexusGPT.EventBus.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddEventBusModule(this IServiceCollection services)
    {
        services.AddSingleton<IDomainEventBus, DefaultEventBus>();
        return services;
    }
}