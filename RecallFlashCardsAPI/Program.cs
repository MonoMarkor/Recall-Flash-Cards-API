using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/test", () =>
{
    string prompt = "What is the capital of France?";
    return Results.Ok(new { question = prompt, answer = "Paris" });
});


app.Run();