using MinecraftJarScanner.Lib.Helpers;
using MinecraftJarScanner.Lib.Models;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

namespace MinecraftJarScanner.Lib.Analyzers;

internal static class ZipAnalyzer
{
    public static async Task<ScannerResultZipFile> AnalyzeAsync(string file, CancellationToken cancellationToken)
    {
        using ZipArchive zip = ZipFile.Open(file, ZipArchiveMode.Read);
        IEnumerable<IScannerResult> results = await ScanZipEntriesAsync(zip.Entries, cancellationToken);

        return new ScannerResultZipFile
        {
            FullPath = file,
            Results = results.ToArray()
        };
    }

    private static async Task<IEnumerable<IScannerResult>> ScanZipEntriesAsync(IEnumerable<ZipArchiveEntry> entries, CancellationToken cancellationToken)
    {
        List<IScannerResult> results = [];

        foreach (ZipArchiveEntry entry in entries)
        {
            if (FileHelper.IsFileType(entry.FullName, ScannerConstants.ZipFileType))
            {
                using ZipArchive nestedZipStream = await ZipArchive.CreateAsync(entry.Open(), ZipArchiveMode.Read, true, Encoding.UTF8, cancellationToken);
                IEnumerable<IScannerResult> nestedResults = await ScanZipEntriesAsync(nestedZipStream.Entries, cancellationToken);

                results.Add(new ScannerResultZipFile
                {
                    FullPath = entry.FullName,
                    Results = nestedResults.ToArray()
                });
            }
            else if (FileHelper.IsFileType(entry.FullName, ScannerConstants.JarFileType))
            {
                using Stream jarStream = entry.Open();
                results.Add(await JarAnalyzer.AnalyzeAsync(jarStream, entry.FullName, cancellationToken));
            }
        }

        return results;
    }
}
