using MinecraftJarScanner.BlazorApp.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ScannerService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Attempt to add appsettings.json from embedded resource stream.
using Stream? appsettingsStream = typeof(Program).Assembly.GetManifestResourceStream($"{typeof(Program).Assembly.GetName().Name}.appsettings.json");
if (appsettingsStream != null)
{
    builder.Configuration.AddJsonStream(appsettingsStream);
}

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
