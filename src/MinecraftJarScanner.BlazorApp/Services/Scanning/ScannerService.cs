namespace MinecraftJarScanner.BlazorApp.Services.Scanning;

public class ScannerService
{
    private readonly ILogger<ScannerService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly List<Scanner> _scanners = [];

    public ScannerService(ILogger<ScannerService> logger, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public void CreateNewScanner()
    {
        ILogger<Scanner> logger = _loggerFactory.CreateLogger<Scanner>();
        _scanners.Add(new Scanner(logger)
        {
            Id = Guid.CreateVersion7(),
            ScannerPath = string.Empty
        });
    }

    public Scanner? GetScanner(Guid id)
    {
        return _scanners.FirstOrDefault(x => x.Id == id);
    }

    public IReadOnlyList<Scanner> GetScanners()
    {
        return _scanners;
    }

    public async Task DeleteScannerAsync(Scanner scanner)
    {
        _scanners.Remove(scanner);
        await scanner.CancelAsync();
        _logger.LogInformation("Deleted scanner {Id}", scanner.Id);
    }
}
