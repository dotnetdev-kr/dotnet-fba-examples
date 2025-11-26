#!/usr/bin/env dotnet

#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false
#:package Microsoft.Agents.AI.Hosting.AGUI.AspNetCore@1.0.0-preview.251114.1
#:package Microsoft.Extensions.AI.OpenAI@10.0.0-preview.1.25560.10

using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

// Setup secret key with below command:
// dotnet user-secrets --id 06-agui-server set openrouter-key "secret key"

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets("06-agui-server");
builder.Services.AddSingleton<MathTools>();
builder.Services.AddChatClient(
	new OpenAIClient(
		new ApiKeyCredential(builder.Configuration["openrouter-key"] ?? throw new Exception("openrouter-key is missing.")),
		new OpenAIClientOptions { Endpoint = new Uri("https://openrouter.ai/api/v1", UriKind.Absolute), }
	).GetChatClient("x-ai/grok-4.1-fast").AsIChatClient());

using var app = builder.Build();
var mathTool = app.Services.GetRequiredService<MathTools>();
var agent = app.Services.GetRequiredService<IChatClient>().CreateAIAgent(
	name: "AGUIAssistant",
	instructions: "You are a helpful assistant.",
	tools: [
		AIFunctionFactory.Create(mathTool.Add, nameof(mathTool.Add), "Add two numbers"),
		AIFunctionFactory.Create(mathTool.Add, nameof(mathTool.Sub), "Subtract two numbers"),
	]);
app.MapAGUI("/aguitest", agent);
app.Run();

public class MathTools(ILogger<MathTools> logger)
{
	public int Add(int a, int b)
	{
		logger.LogInformation("Add tool called: {a}, {b}", a, b);
		return a + b;
	}

	public int Sub(int a, int b)
	{
		logger.LogInformation("Subtool called: {a}, {b}", a, b);
		return a - b;
	}
}
