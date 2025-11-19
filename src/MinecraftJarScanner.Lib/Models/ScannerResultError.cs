namespace MinecraftJarScanner.Lib.Models;

public class ScannerResultError : IScannerResult
{
    public required string FullPath { get; init; }

    public required Exception Exception { get; set; }
}
