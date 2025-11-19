using System.Text.Json.Serialization;

namespace MinecraftJarScanner.Lib.Models;

[JsonDerivedType(typeof(ScannerResultError), typeDiscriminator: nameof(ScannerResultError))]
[JsonDerivedType(typeof(ScannerResultJarFile), typeDiscriminator: nameof(ScannerResultJarFile))]
[JsonDerivedType(typeof(ScannerResultZipFile), typeDiscriminator: nameof(ScannerResultZipFile))]
public interface IScannerResult
{
    string FullPath { get; }

    string Name => Path.GetFileName(FullPath);
}
