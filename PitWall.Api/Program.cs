using PitWall.Api;
using PitWall.Core.Data;

var builder = WebApplication.CreateBuilder(args);
var section = builder.Configuration.GetSection("PitWall");
var options = section.Get<PitWallOptions>() ?? new();
var dataDir = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, options.DataDir));

builder.Services.Configure<PitWallOptions>(section);
builder.Services.AddSingleton<ISessionData>(new FileSessionData(dataDir));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<LiveRace>();

var app = builder.Build();

app.Run();
