using MinecraftJarScanner.Lib.Models;

namespace MinecraftJarScanner.Lib.Analyzers;

internal static class JarAnalyzer
{
    public static async Task<ScannerResultJarFile> AnalyzeAsync(string file, CancellationToken cancellationToken)
    {
        using FileStream fileStream = File.OpenRead(file);
        return await AnalyzeAsync(fileStream, file, cancellationToken);
    }

    public static async Task<ScannerResultJarFile> AnalyzeAsync(Stream stream, string path, CancellationToken cancellationToken)
    {
        // Todo
        return new ScannerResultJarFile
        {
            FullPath = path,
            Hash = "TODO"
        };
    }
}
