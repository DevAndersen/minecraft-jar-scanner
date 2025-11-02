namespace MinecraftJarScanner.BlazorApp.Models;

public class Scanner
{
    private readonly ILogger<Scanner> _logger;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public required Guid Id { get; init; }

    public required string ScannerPath { get; set; }

    public Scanner(ILogger<Scanner> logger)
    {
        _logger = logger;
    }

    public string TabName => Path.GetFileName(ScannerPath);

    public ScannerStatus Status { get; private set; } = ScannerStatus.Idle;

    public string? StatusMessage { get; private set; }

    public bool IsRunning => Status == ScannerStatus.Scanning;

    public async Task StartAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _logger.LogInformation("Starting scanner {Id} on directory {ScannerPath}", Id, ScannerPath);
        Status = ScannerStatus.Scanning;

        SetStatus(ScannerStatus.Scanning, "Debug wait");
        await Task.Delay(2000, _cancellationTokenSource.Token);

        if (_cancellationTokenSource.IsCancellationRequested)
        {
            SetStatus(ScannerStatus.Cancelled, "Scan was cancelled");
            return;
        }

        _logger.LogInformation("Completed scanner {Id} on directory {ScannerPath}", Id, ScannerPath);
        SetStatus(ScannerStatus.Completed, "Scan completed");
    }

    public void SetStatus(ScannerStatus status, string? message = null)
    {
        Status = status;
        StatusMessage = message;
    }

    public async Task CancelAsync()
    {
        if (!IsRunning)
        {
            return;
        }

        _cancellationTokenSource.Cancel();
        Status = ScannerStatus.Cancelled;
        _logger.LogInformation("Cancelled scanner {Id}", Id);
    }
}
