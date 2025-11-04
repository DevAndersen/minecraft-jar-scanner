using MinecraftJarScanner.Lib.Analyzers;
using MinecraftJarScanner.Lib.Helpers;
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
                IScannerResult? result = await ScanFileAsync(file, cancellationToken);
                if (result != null)
                {
                    _results.Add(result);
                }

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

            if (FileHelper.IsFileType(file, zipFileType) || FileHelper.IsFileType(file, jarFileType))
            {
                yield return file;
            }
        }
    }

    private static async Task<IScannerResult?> ScanFileAsync(string file, CancellationToken cancellationToken)
    {
        try
        {
            if (FileHelper.IsFileType(file, zipFileType))
            {
                return await ZipAnalyzer.AnalyzeAsync(file, cancellationToken);
            }
            else if (FileHelper.IsFileType(file, jarFileType))
            {
                return await JarAnalyzer.AnalyzeAsync(file, cancellationToken);
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            return new ScannerResultError
            {
                FullPath = file,
                Exception = e
            };
        }
    }

    public void SetStatus(ScannerStatus status, string? message = null)
    {
        Status = status;
        StatusMessage = message;
    }
}
