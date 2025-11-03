namespace MinecraftJarScanner.Lib.Models;

public class ScannerResultJarFile : IScannerResult
{
    public required string Path { get; init; }

    public required string Hash { get; init; }
}
