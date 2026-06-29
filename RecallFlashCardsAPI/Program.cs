using System.Net.Security;
using Domain.IRepositories;
using Google.GenAI;
using Infrastructure.Gemini;
using Infrastructure.Gemini.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var geminiOptions = builder.Configuration.GetSection("Gemini").Get<GeminiOptions>();
builder.Services.AddScoped<Client>(sp => new Client(apiKey: geminiOptions.ApiKey));
builder.Services.AddScoped<IClassification, Classification>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/test", async ( IClassification classification) =>
{
    string answer = await classification.GetClassificationAsync("cat");

    return Results.Ok(answer);
});


app.Run();