namespace MinecraftJarScanner.Lib.Models;

public class ScannerResultJarFile : IScannerResult
{
    public required JarEvaluation Evaluation { get; init; }

    public required JarFlags Flags { get; init; }

    public required string FullPath { get; init; }

    public required string Hash { get; init; }
}
