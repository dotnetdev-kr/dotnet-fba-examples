#!/usr/bin/env dotnet
#:sdk Aspire.AppHost.Sdk@13.0.0
#:property PublishAot=false
#:package Aspire.Hosting.Garnet@13.0.0

#pragma warning disable ASPIRECSHARPAPPS001

using Microsoft.Extensions.Configuration;

// To specify password/secrets:
// dotnet user-secrets set --id=apphosttest ConfigName Value

var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("03-apphost.json")
    .AddUserSecrets("apphosttest")
    .AddEnvironmentVariables();

var garnet = builder.AddGarnet("cache");

_ = builder.AddCSharpApp("worker", "03-worker.cs")
    .WithReference(garnet)
    .WaitFor(garnet);

_ = builder.AddCSharpApp("minapi", "03-minapi.cs")
    .WithReference(garnet)
    .WaitFor(garnet)
    .WithHttpsEndpoint()
    .WithHttpEndpoint();

using var app = builder.Build();
app.Run();
