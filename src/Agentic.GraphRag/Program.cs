using Agentic.GraphRag.Components;
using Agentic.GraphRag.Extensions;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureOptions();

if (builder.Environment.IsDevelopment())
{
    builder.DumpConfiguration();
}

builder.AddAIServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .RegisterServices()
    .RegisterBlazorPersistenceServices()
    .RegisterResiliencePipelines()
    .RegisterHttpClients()
    .RegisterGraphDatabase();

builder.Services.AddHsts(options =>
{
    options.Preload = false;
    options.MaxAge = TimeSpan.FromDays(60);
});

var useDevUI = builder.Environment.IsDevelopment();
if (useDevUI)
{
    // Register services for OpenAI responses and conversations (also required for DevUI)
    builder.Services.AddOpenAIResponses();
    builder.Services.AddOpenAIConversations();
}

var app = builder.Build();

app.MapDefaultEndpoints();

if (useDevUI)
{
    // Map AI-related endpoints needed by DevUI
    app.MapOpenAIResponses();
    app.MapOpenAIConversations();

    // Map DevUI endpoint to /devui
    app.MapDevUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();

    // Map DevUI endpoint to /devui
    app.MapDevUI();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync().ConfigureAwait(false);
