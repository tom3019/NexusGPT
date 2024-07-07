using Microsoft.Extensions.DependencyInjection;

namespace NexusGPT.MainComponent;

public interface INexusGptBuilder
{
    /// <summary>
    /// Service collection
    /// </summary>
    public IServiceCollection ServiceCollection { get; }
}