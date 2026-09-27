using PitWall.Api;

var builder = WebApplication.CreateBuilder(args);

var section = builder.Configuration.GetSection("PitWall");
var options = section.Get<PitWallOptions>() ?? new();
var dataDir = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, options.DataDir));

builder.Services.Configure<PitWallOptions>(section);

Console.WriteLine($"Daten: {dataDir} | Session: {options.SessionKey} | Speed: {options.Speed}");

var app = builder.Build();

app.Run();
