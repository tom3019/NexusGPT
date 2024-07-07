using Microsoft.Extensions.DependencyInjection;
using NexusGPT.Adapter.Out.ImageStorage.Local;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.MainComponent;

public static class NexusGptBuilderExtensions
{
    public static INexusGptBuilder UseLocalStorage(this INexusGptBuilder builder)
    {
        builder.ServiceCollection.AddScoped<IImageStorageOutPort, LocalStorageRepository>();
        return builder;
    }
}