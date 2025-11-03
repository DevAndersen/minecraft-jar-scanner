using MinecraftJarScanner.Lib;
using MinecraftJarScanner.Lib.Models;

namespace MinecraftJarScanner.BlazorApp.Models;

public class Scanner
{
    private readonly ILogger<Scanner> _logger;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private JarScanner? _scanner;

    public required Guid Id { get; init; }

    public required string ScannerPath { get; set; }

    public IReadOnlyList<IScannerResult> Results => _scanner?.Results ?? [];

    public Scanner(ILogger<Scanner> logger)
    {
        _logger = logger;
    }

    public string TabName => Path.GetFileName(ScannerPath);

    public ScannerStatus Status => _scanner?.Status ?? ScannerStatus.Idle;

    public string? StatusMessage => _scanner?.StatusMessage;

    public bool IsRunning => _scanner?.Status == ScannerStatus.Scanning;

    public async Task StartAsync()
    {
        if (IsRunning)
        {
            return;
        }

        _logger.LogInformation("Starting scanner {Id} on directory {ScannerPath}", Id, ScannerPath);

        _cancellationTokenSource = new CancellationTokenSource();

        _scanner = new JarScanner();
        await _scanner.ScanAsync(ScannerPath, _cancellationTokenSource.Token);

        if (_cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        _logger.LogInformation("Completed scanner {Id} on directory {ScannerPath}", Id, ScannerPath);
    }

    public async Task CancelAsync()
    {
        if (!IsRunning || _cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        _cancellationTokenSource.Cancel();
        _logger.LogInformation("Cancelled scanner {Id}", Id);
    }
}
