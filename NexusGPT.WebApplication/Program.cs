using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using NexusGPT.Adapter.Out;
using NexusGPT.EventBus.Extensions;
using NexusGPT.MainComponent;
using NexusGPT.WebApplication.Hubs;
using NexusGPT.WebApplication.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "NexusGPT API",
            Version = "v1",
        });

    var basePath = AppContext.BaseDirectory;
    var xmlFiles = Directory.EnumerateFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly);
    foreach (var xmlFile in xmlFiles)
    {
        c.IncludeXmlComments(xmlFile);
    }

    c.DocumentFilter<SwaggerEnumDocumentFilter>();
    c.AddSignalRSwaggerGen();
});
builder.Services.AddApiVersioning(option =>
{
    option.ReportApiVersions = true;
    option.AssumeDefaultVersionWhenUnspecified = true; //此選項將用於在沒有版本的情況下提供請求
    option.DefaultApiVersion = new ApiVersion(1, 0); //預設版本號
    option.ApiVersionSelector = new CurrentImplementationApiVersionSelector(option);
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddHttpClient("NexusGPT");

builder.Services.AddNexusGptModule(b=>b.UseLocalImageStorage())
    .AddEventBusModule();

builder.Services.AddDbContext<NexusGptDbContext>(
    o => 
        o.UseSqlServer(builder.Configuration.GetConnectionString("NexusGPT")));

builder.Services.AddCors(o =>
    o.AddPolicy("cors", b =>
    {
        b.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins("http://localhost:8080");
    }));

builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("cors");
app.MapHub<MessageHub>("/MessageHub");
app.MapHub<TopicHub>("/TopicHub");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Images")),
    RequestPath = "/Images"
});

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();