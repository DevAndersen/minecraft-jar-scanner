namespace MinecraftJarScanner.Lib.Models;

public class ScannerResultZipFile : IScannerResult
{
    public required string Path { get; init; }

    public required IReadOnlyList<IScannerResult> Results { get; init; }
}
