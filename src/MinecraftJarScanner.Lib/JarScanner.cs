using MinecraftJarScanner.Lib.Models;
using System.Runtime.CompilerServices;

namespace MinecraftJarScanner.Lib;

public class JarScanner
{
    private static readonly string zipFileType = ".zip";
    private static readonly string jarFileType = ".jar";

    private readonly List<IScannerResult> _results = [];

    public ScannerStatus Status { get; private set; }

    public string? StatusMessage { get; private set; }

    public int FilesScanned { get; private set; }

    public Exception? Exception { get; private set; }

    public IReadOnlyList<IScannerResult> Results => _results;

    public async Task ScanAsync(string directory, CancellationToken cancellationToken)
    {
        if (Status != ScannerStatus.Idle)
        {
            return;
        }

        SetStatus(ScannerStatus.Scanning);

        try
        {
            if (!Directory.Exists(directory))
            {
                SetStatus(ScannerStatus.Error, "Directory was not found");
                return;
            }

            IAsyncEnumerable<string> files = ScanDirectoryAsync(directory, cancellationToken);
            await foreach (string file in files)
            {
                _results.Add(await ScanFileAsync(file, cancellationToken));
                FilesScanned++;
            }

            SetStatus(ScannerStatus.Completed);
        }
        catch (Exception e)
        {
            Exception = e;
            SetStatus(ScannerStatus.Error, "An error occurred");
        }
        finally
        {
            if (cancellationToken.IsCancellationRequested)
            {
                SetStatus(ScannerStatus.Cancelled);
            }
        }
    }

    private static async IAsyncEnumerable<string> ScanDirectoryAsync(string directory, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            if (IsFileType(file, zipFileType) || IsFileType(file, jarFileType))
            {
                yield return file;
            }
        }
    }

    private static async Task<IScannerResult> ScanFileAsync(string file, CancellationToken cancellationToken)
    {
        try
        {
            // Todo
            return new ScannerResultJarFile
            {
                Path = file,
                Hash = "TODO" // Todo
            };
        }
        catch (Exception e)
        {
            return new ScannerResultError
            {
                Path = file,
                Exception = e
            };
        }
    }

    public void SetStatus(ScannerStatus status, string? message = null)
    {
        Status = status;
        StatusMessage = message;
    }

    private static bool IsFileType(string fileName, string extension)
    {
        return Path.GetExtension(fileName)
            .Equals(extension, StringComparison.OrdinalIgnoreCase);
    }
}
