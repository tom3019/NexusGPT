using Microsoft.Extensions.DependencyInjection;
using NexusGPT.Adapter.Out.Implements;
using NexusGPT.UseCase;
using NexusGPT.UseCase.Port;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.In.AddMessage;
using NexusGPT.UseCase.Port.Out;
using OpenAI.Extensions;

namespace NexusGPT.MainComponent;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 加入NexusGPT模組
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public static IServiceCollection AddNexusGPTModule(this IServiceCollection service)
    {
        service.AddSingleton(TimeProvider.System);
        service.AddOpenAIService(o => o.ApiKey =
           Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new ArgumentNullException("OPENAI_API_KEY"));
        return service.AddNexusGPTInput()
            .AddNexusGPTOutput();
    }
    
    
    /// <summary>
    /// 加入NexusGPT輸入
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    private static IServiceCollection AddNexusGPTInput(this IServiceCollection service)
    {
        service.AddScoped<ICreateTopicService, CreateTopicService>();
        service.AddScoped<IAddMessageService, AddMessageService>();
        service.AddScoped<IAddMessageAsStreamService, AddMessageAsStreamService>();
        service.AddScoped<IAddImageMessageService, AddImageMessageService>();
        service.AddScoped<IChangeTitleService, ChangeTitleService>();
        service.AddScoped<IDeleteTopicService, DeleteTopicService>();
        service.AddScoped<ITopicQueryService, TopicQueryService>();
        service.AddScoped<IShareTopicService, ShareTopicService>();
        return service;
    }
    
    
    /// <summary>
    /// 加入NexusGPT輸出
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    private static IServiceCollection AddNexusGPTOutput(this IServiceCollection service)
    {
        service.AddScoped<ITopicOutPort, TopicRepository>();
        service.AddScoped<IMessageOutPort, MessageRepository>();
        service.AddScoped<IImageStorageOutPort, LocalStorageRepository>();
        return service;
    }
}