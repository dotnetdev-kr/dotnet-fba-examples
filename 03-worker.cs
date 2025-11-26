#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Worker
#:property PublishAot=false
#:package Microsoft.Extensions.Hosting@10.*
#:package Aspire.StackExchange.Redis@13.0.0

using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.AddRedisClient("cache");

using var app = builder.Build();
app.Run();

public sealed class Worker(
    ILogger<Worker> logger, IConnectionMultiplexer cache
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var key = new RedisKey("LastUpdated");
        var database = cache.GetDatabase();
        while (true)
        {
            var current = DateTimeOffset.Now;
            await database.StringSetAsync(key, "Last updated: " + current.ToString());
            logger.LogInformation("Worker running at: {Time}", current);
            await Task.Delay(TimeSpan.FromSeconds(1d), stoppingToken);
        }
    }
}
