#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false
#:package Aspire.StackExchange.Redis@13.0.0

using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.AddRedisClient("cache");
using var app = builder.Build();

app.MapGet("/", async ([FromServices] IConnectionMultiplexer cache) =>
{
    var database = cache.GetDatabase();
    var messageKey = new RedisKey("Message");
    var lastUpdatedKey = new RedisKey("LastUpdated");
    if (!await database.KeyExistsAsync(messageKey))
        await database.StringSetAsync(messageKey, new RedisValue("Hello, World!"));
    var message = (await database.StringGetAsync(messageKey)).ToString();
    if (await database.KeyExistsAsync(lastUpdatedKey))
        message += " / " + (await database.StringGetAsync(lastUpdatedKey)).ToString();
    return Results.Ok(message);
});
app.Run();
