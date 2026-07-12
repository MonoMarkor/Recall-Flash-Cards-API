using System.Net.Security;
using Domain.IRepositories;
using Domain.DTOs;
using Domain.IServices;
using Google.GenAI;
using Infrastructure.Gemini;
using Infrastructure.Gemini.Repositories;
using RecallFlashCardsAPI.Models;
using BLL.Services;
using Infrastructure.SQL.Repositories;
using Infrastructure.SQL.Database;
using Microsoft.EntityFrameworkCore;
using Minio;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostgreSQLDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

var geminiOptions = builder.Configuration.GetSection("Gemini").Get<GeminiOptions>();
builder.Services.AddScoped<Client>(sp => new Client(apiKey: geminiOptions.ApiKey));
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var minioService = scope.ServiceProvider.GetRequiredService<IMinioService>();
    await minioService.InitBucketAsync("flashcard-images");
    await minioService.InitBucketAsync("flashcard-audios");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/test", async ( IGenerativeAIRepository classification) =>
{
    string answer = await classification.GetClassificationAsync("cat", ["cute", "dangerous", "creepy"]);

    return Results.Ok(answer);
});

app.MapPost("/collection", async (ICollectionService collectionService, Collection collection) =>
{
    var newCollection = new CollectionDto
    {
        Name = collection.Name,
        Description = collection.Description
    };
    return Results.Ok(newCollection.Id);
});


app.Run();