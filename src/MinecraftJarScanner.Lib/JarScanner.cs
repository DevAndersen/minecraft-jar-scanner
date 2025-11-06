using MinecraftJarScanner.Lib.Analyzers;
using MinecraftJarScanner.Lib.Helpers;
using MinecraftJarScanner.Lib.Models;
using System.Runtime.CompilerServices;

namespace MinecraftJarScanner.Lib;

public class JarScanner
{
    private readonly List<IScannerResult> _results = [];

    public ScannerStatus Status { get; private set; }

    public string? StatusMessage { get; private set; }

    public int FilesScanned { get; private set; }

    public Exception? Exception { get; private set; }

    public IReadOnlyList<IScannerResult> Results => _results;

    /// <summary>
    /// Starts scanning <paramref name="directory"/> for results.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ScanAsync(string directory, CancellationToken cancellationToken = default)
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
            SetStatus(ScannerStatus.Error, $"An {e.GetType().Name} occurred: {e.Message}");
        }
        finally
        {
            if (cancellationToken.IsCancellationRequested)
            {
                SetStatus(ScannerStatus.Cancelled);
            }
        }
    }

    /// <summary>
    /// Returns an asynchronously iterator over all files in <paramref name="directory"/>, including nested directories.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async IAsyncEnumerable<string> ScanDirectoryAsync(string directory, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Find all files in the directory.
        IEnumerable<string> files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            if (FileHelper.IsFileType(file, ScannerConstants.ZipFileType) || FileHelper.IsFileType(file, ScannerConstants.JarFileType))
            {
                yield return file;
            }
        }
    }

    /// <summary>
    /// Scans <paramref name="file"/> and determines if it contains any data of interest.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<IScannerResult?> ScanFileAsync(string file, CancellationToken cancellationToken)
    {
        try
        {
            if (FileHelper.IsFileType(file, ScannerConstants.ZipFileType))
            {
                return await ZipAnalyzer.AnalyzeAsync(file, cancellationToken);
            }
            else if (FileHelper.IsFileType(file, ScannerConstants.JarFileType))
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

    /// <summary>
    /// Updates the status of the scanner.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="message"></param>
    private void SetStatus(ScannerStatus status, string? message = null)
    {
        Status = status;
        StatusMessage = message;
    }
}
