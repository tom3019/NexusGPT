using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NexusGPT.Adapter.Out.ImageStorage.Local;
using NexusGPT.Adapter.Out.ImageStorage.S3;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.MainComponent;

public static class NexusGptBuilderExtensions
{
    public static INexusGptBuilder UseLocalImageStorage(this INexusGptBuilder builder)
    {
        builder.ServiceCollection.AddScoped<IImageStorageOutPort, LocalStorageRepository>();
        return builder;
    }
    
    public static INexusGptBuilder UseS3ImageStorage(this INexusGptBuilder builder)
    {
        builder.ServiceCollection.AddOptions<AwsS3StorageOptions>()
            .BindConfiguration(nameof(AwsS3StorageOptions));

        var buildServiceProvider = builder.ServiceCollection.BuildServiceProvider();
        var options = buildServiceProvider.GetRequiredService<IOptions<AwsS3StorageOptions>>().Value;
        var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(options.Region);

        var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESSKEY") 
                        ?? throw new ArgumentNullException(nameof(UseS3ImageStorage),"AWS_ACCESSKEY is null");
        
        var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET") 
                        ?? throw new ArgumentNullException(nameof(UseS3ImageStorage),"AWS_SECRET is null");
        
        var credentials = new BasicAWSCredentials(accessKey,secretKey);
        builder.ServiceCollection.AddScoped<IAmazonS3>(x=>
            new AmazonS3Client(credentials,regionEndpoint));
        
        builder.ServiceCollection.AddScoped<IImageStorageOutPort, S3StorageRepository>();
        return builder;
    }
}