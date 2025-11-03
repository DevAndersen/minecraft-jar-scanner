namespace MinecraftJarScanner.Lib.Models;

public class ScannerResultError : IScannerResult
{
    public required string Path { get; init; }

    public required Exception Exception { get; set; }
}
