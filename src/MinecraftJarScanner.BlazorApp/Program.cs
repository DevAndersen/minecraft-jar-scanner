using MinecraftJarScanner.BlazorApp.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Service registrations.
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

// The API endpoint for downloading log files.
app.MapGet("/scanner/{scannerId:guid}/log", (Guid scannerId, ScannerService scannerService) =>
{
    Scanner? scanner = scannerService.GetScanner(scannerId);
    if (scanner == null)
    {
        return Results.NotFound();
    }

    byte[] json = scannerService.GetLogFile(scanner);

    return Results.File(
        json,
        "application/json",
        $"minecraft-jar-log_{DateTimeOffset.UtcNow:yyyy-MM-dd_HH-mm-ss}.txt");
});

app.Run();
