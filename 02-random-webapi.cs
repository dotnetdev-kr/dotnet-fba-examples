#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Random>(Random.Shared);
using var app = builder.Build();
app.MapGet("/", (Random rnd) =>
	new { ts = DateTimeOffset.UtcNow, val = rnd.Next() % 10, });
app.Run();
