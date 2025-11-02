namespace MinecraftJarScanner.BlazorApp.Models;

public class Scanner
{
    private readonly ILogger<Scanner> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public required Guid Id { get; init; }

    public required string ScannerPath { get; set; }

    public Scanner(ILogger<Scanner> logger)
    {
        _logger = logger;
    }

    public string TabName => Path.GetFileName(ScannerPath);

    public ScannerStatus Status { get; private set; } = ScannerStatus.Idle;

    public bool IsRunning => Status == ScannerStatus.Scanning;

    public async Task StartAsync()
    {
        _logger.LogInformation("Starting scanner {Id} on directory {ScannerPath}", Id, ScannerPath);
        Status = ScannerStatus.Scanning;

        await Task.Delay(2000, _cancellationTokenSource.Token);

        if (_cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        _logger.LogInformation("Completed scanner {Id} on directory {ScannerPath}", Id, ScannerPath);
        Status = ScannerStatus.Completed;
    }

    public async Task CancelAsync()
    {
        _cancellationTokenSource.Cancel();
        Status = ScannerStatus.Cancelled;
        _logger.LogInformation("Cancelled scanner {Id}", Id);
    }
}
