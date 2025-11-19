namespace MinecraftJarScanner.Lib.Models;

public class ScannerResultZipFile : IScannerResult
{
    public required string FullPath { get; init; }

    public required IReadOnlyList<IScannerResult> Results { get; init; }
}
