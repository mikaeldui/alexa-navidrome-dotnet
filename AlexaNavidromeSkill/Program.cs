using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using Amazon.Lambda.Core;
using SubSonicMedia;
using SubSonicMedia.Models;
using SubSonicMedia.Responses.Search.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Essentially a C# pot of https://github.com/rosskouk/asknavidrome/blob/main/skill/app.py

string? NAVI_SKILL_ID = Environment.GetEnvironmentVariable("NAVI_SKILL_ID");
string? NAVI_URL = Environment.GetEnvironmentVariable("NAVI_URL");
string? NAVI_PORT = Environment.GetEnvironmentVariable("NAVI_PORT");
string? NAVI_USER = Environment.GetEnvironmentVariable("NAVI_USER");
string? NAVI_PASS = Environment.GetEnvironmentVariable("NAVI_PASS");
string? NAVI_API_PATH = Environment.GetEnvironmentVariable("NAVI_API_PATH");

using var navidrome = new SubsonicClient(new SubsonicConnectionInfo($"{NAVI_URL}:{NAVI_PORT}", NAVI_USER, NAVI_PASS));

// Test connection
await navidrome.System.PingAsync();


Song[] songs;
{
    var response = await navidrome.Search.Search3Async("\"\"", artistCount: 0, albumCount: 0, songCount: 10000);
    songs = response.SearchResult.Songs.ToArray();
}

app.MapGet("/", (SkillRequest input) =>
{
    var intentRequest = input.Request as IntentRequest;
        
    if(input.Request is LaunchRequest || intentRequest?.Intent.Name == "AMAZON.NavigateHomeIntent")
    {
        return ResponseBuilder.Ask("What do you want me to play?", new Reprompt());
    }

    if (intentRequest?.Intent.Name == "NaviSonicPlayMusicRandom")
    {
        
    }

    if (input.Request is AudioPlayerRequest audioRequest && audioRequest.AudioRequestType == AudioRequestType.PlaybackNearlyFinished)
    {
        return ResponseBuilder.AudioPlayerPlay(PlayBehavior.ReplaceAll, navidrome.Media.DownloadAsync() );
    }
    
    // your function logic goes here
    return new SkillResponse("OK");
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}