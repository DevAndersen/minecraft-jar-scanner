using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using MinecraftJarScanner.Components;
using System.Diagnostics;

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

// Startup event.
app.Lifetime.ApplicationStarted.Register(() =>
{
    IServer server = app.Services.GetRequiredService<IServer>();
    ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

    IServerAddressesFeature? addresses = server.Features.Get<IServerAddressesFeature>();
    string? firstAddress = addresses?.Addresses.FirstOrDefault();

    if (firstAddress == null)
    {
        logger.LogError("Failed to determine host port");
    }
    else
    {
        logger.LogInformation("Starting Minecraft Jar Scanner on: {address}", firstAddress);

        // Attempt to navigate to the site with the default web launcher.
        if (!Debugger.IsAttached)
        {
            try
            {
                Process.Start(new ProcessStartInfo(firstAddress)
                {
                    UseShellExecute = true
                });
            }
            catch
            {
            }
        }
    }

    logger.LogInformation("Press Ctrl+C to shut down");
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// The API endpoint for saving log files.
app.MapGet("/scanner/{scannerId:guid}/log", (Guid scannerId, ScannerService scannerService) =>
{
    if (!scannerService.TryGetScanner(scannerId, out Scanner? scanner))
    {
        return Results.NotFound();
    }

    byte[] json = scannerService.GetLogFile(scanner);

    return Results.File(
        json,
        "application/json",
        $"minecraft-jar-scanner-log_{DateTimeOffset.UtcNow:yyyy-MM-dd_HH-mm-ss}.json");
});

app.Run();
