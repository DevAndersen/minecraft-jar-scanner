using System.Diagnostics.CodeAnalysis;

namespace MinecraftJarScanner.BlazorApp.Services.Scanning;

/// <summary>
/// Provides logic for managing scanners.
/// </summary>
public class ScannerService
{
    private readonly ILogger<ScannerService> _logger;
    private readonly ILoggerFactory _loggerFactory;

    private readonly List<Scanner> _scanners = [];
    private readonly JsonSerializerOptions _logFileSerializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public ScannerService(ILogger<ScannerService> logger, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Add a new <see cref="Scanner"/>.
    /// </summary>
    public void CreateNewScanner()
    {
        ILogger<Scanner> logger = _loggerFactory.CreateLogger<Scanner>();
        _scanners.Add(new Scanner(logger)
        {
            Id = Guid.CreateVersion7(),
            ScannerPath = string.Empty
        });
    }

    /// <summary>
    /// Attempt to retrieve the <see cref="Scanner"/> with the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool TryGetScanner(Guid id, [NotNullWhen(true)] out Scanner? scanner)
    {
        scanner = _scanners.FirstOrDefault(x => x.Id == id);
        return scanner != null;
    }

    /// <summary>
    /// Retrieve all <see cref="Scanner"/>s.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<Scanner> GetScanners()
    {
        return _scanners;
    }

    /// <summary>
    /// Delete the specified <paramref name="scanner"/>.
    /// </summary>
    /// <remarks>
    /// If <paramref name="scanner"/> is scanning, it will be cancelled.
    /// </remarks>
    /// <param name="scanner"></param>
    /// <returns></returns>
    public async Task DeleteScannerAsync(Scanner scanner)
    {
        _scanners.Remove(scanner);
        await scanner.CancelAsync();
        _logger.LogInformation("Deleted scanner {Id}", scanner.Id);
    }

    /// <summary>
    /// Returns the UTF-8 encoded log file content for <paramref name="scanner"/>.
    /// </summary>
    /// <param name="scanner"></param>
    /// <returns></returns>
    public byte[] GetLogFile(Scanner scanner)
    {
        return JsonSerializer.SerializeToUtf8Bytes(scanner.Results, _logFileSerializationOptions);
    }
}
