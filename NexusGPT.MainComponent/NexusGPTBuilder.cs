using Microsoft.Extensions.DependencyInjection;

namespace NexusGPT.MainComponent;

public class NexusGptBuilder : INexusGptBuilder
{
    public NexusGptBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    public IServiceCollection ServiceCollection { get; }
}