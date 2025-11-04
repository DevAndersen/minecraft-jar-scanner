namespace MinecraftJarScanner.Lib.Models;

public interface IScannerResult
{
    string FullPath { get; }

    string Name => Path.GetFileName(FullPath);
}
