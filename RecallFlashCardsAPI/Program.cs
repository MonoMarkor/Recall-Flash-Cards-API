using BLL.Services;
using Domain.IRepositories;
using Domain.IServices;
using Google.GenAI;
using Infrastructure.Gemini;
using Infrastructure.Gemini.Repositories;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.IRepositories;
using Infrastructure.SQL.Repositories;
using Microsoft.EntityFrameworkCore;
using Minio;
using RecallFlashCardsAPI.RouteGroups;
using System.Net;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostgreSQLDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

var geminiOptions = builder.Configuration.GetSection("Gemini").Get<GeminiOptions>();
if (geminiOptions != null && !string.IsNullOrEmpty(geminiOptions.ApiKey))
{
    builder.Services.AddScoped<Client>(sp => new Client(apiKey: geminiOptions.ApiKey));
}
else
{
    Console.WriteLine("Gemini API key is not configured. Please set the 'Gemini:ApiKey' in appsettings.json or environment variables.");
}
builder.Services.AddScoped<IGenerativeAIRepository, GenerativeAIRepository>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
builder.Services.AddScoped<IFlashCardService, FlashCardService>();
builder.Services.AddScoped<IFlashCardRepository, FlashCardRepository>();
builder.Services.AddScoped<IMinioService, MinioService>();

builder.Services.AddMinio(options =>
    options.WithEndpoint(builder.Configuration.GetSection("Minio")["Endpoint"])
    .WithCredentials(
        builder.Configuration.GetSection("Minio")["AccessKey"],
        builder.Configuration.GetSection("Minio")["SecretKey"]
    )
    .WithSSL(builder.Configuration.GetSection("Minio").GetValue<bool>("Secure"))
);

string fixedFileUploadPolicy = "fixedFileUploads";
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.AddPolicy<string>(fixedFileUploadPolicy, httpContext =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 4,
                Window = TimeSpan.FromSeconds(12),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var minioService = scope.ServiceProvider.GetRequiredService<IMinioService>();
    string imageBucket = builder.Configuration["Minio:ImageBucket"]
        ?? throw new InvalidOperationException("Minio:ImageBucket configuration is missing."); ;
    string audioBucket = builder.Configuration["Minio:AudioBucket"]
        ?? throw new InvalidOperationException("Minio:AudioBucket configuration is missing.");
    await minioService.InitBucketAsync(imageBucket);
    await minioService.InitBucketAsync(audioBucket);
}

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddCollectionEndpoints();
app.AddFlashCardEndpoints(fixedFileUploadPolicy);

app.MapGet("/test", async ( IGenerativeAIRepository classification) =>
{
    string answer = await classification.GetClassificationAsync("cat", ["cute", "dangerous", "creepy"]);

    return Results.Ok(answer);
});

app.Run();