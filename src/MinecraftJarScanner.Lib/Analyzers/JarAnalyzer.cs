using MinecraftJarScanner.Lib.Models;
using System.Buffers;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace MinecraftJarScanner.Lib.Analyzers;

internal static class JarAnalyzer
{
    public static async Task<IScannerResult> AnalyzeAsync(string file, CancellationToken cancellationToken)
    {
        using FileStream fileStream = File.OpenRead(file);
        return await AnalyzeAsync(fileStream, file, cancellationToken);
    }

    public static async Task<IScannerResult> AnalyzeAsync(Stream stream, string path, CancellationToken cancellationToken)
    {        // Copy content to memory stream, so the stream position can be reset.
        using MemoryStream ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        ms.Position = 0;

        // Get file hash.
        string hashHex;
        byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(MD5.HashSizeInBytes);
        try
        {
            Memory<byte> hashBuffer = rentedBuffer.AsMemory()[..MD5.HashSizeInBytes];
            await MD5.HashDataAsync(ms, hashBuffer, cancellationToken);
            hashHex = Convert.ToHexString(hashBuffer.Span);
        }
        finally
        {
            ms.Position = 0;
            ArrayPool<byte>.Shared.Return(rentedBuffer);
        }

        // Analyze jar content.
        using ZipArchive jar = await ZipArchive.CreateAsync(ms, ZipArchiveMode.Read, true, Encoding.UTF8, cancellationToken);
        try
        {
            JarFlags flags = new JarFlags
            {
                HasMinecraftClientMetaInf = ContainsClientMetaInf(jar),
                HasRubyDungClass = HasRubyDungClass(jar),
                HasMinecraftAppletClass = HasMinecraftAppletClass(jar),
                HasMinecraftClass = HasMinecraftClass(jar),
                HasMinecraftServerClass = HasMinecraftServerClass(jar),
            };

            return new ScannerResultJarFile
            {
                Evaluation = EvaluateFlags(flags),
                Flags = flags,
                FullPath = path,
                Hash = hashHex
            };
        }
        catch (Exception e)
        {
            return new ScannerResultError
            {
                FullPath = path,
                Exception = e
            };
        }
    }

    private static JarEvaluation EvaluateFlags(JarFlags flags)
    {
        if (flags.HasRubyDungClass)
        {
            return JarEvaluation.RubyDungClient;
        }

        return JarEvaluation.UnrelatedJar;
    }

    private static bool HasRubyDungClass(ZipArchive archive)
    {
        return archive.Has("com/mojang/rubydung/RubyDung.class")
            || archive.Has("com/mojang/minecraft/RubyDung.class");
    }

    private static bool HasMinecraftAppletClass(ZipArchive archive)
    {
        return archive.Has("com/mojang/minecraft/MinecraftApplet.class");
    }

    private static bool HasMinecraftClass(ZipArchive archive)
    {
        return archive.Has("com/mojang/minecraft/Minecraft.class");
    }

    private static bool HasMinecraftServerClass(ZipArchive archive)
    {
        return archive.Has("com/mojang/minecraft/server/MinecraftServer.class");
    }

    private static bool ContainsClientMetaInf(ZipArchive jar)
    {
        return jar.Has([
            "META-INF/MANIFEST.MF",
            "META-INF/MOJANG_C.SF",
            "META-INF/MOJANG_C.DSA"
        ]);
    }
}

file static class Extensions
{
    extension(ZipArchive jar)
    {
        public bool Has(string file)
        {
            return jar.GetEntry(file) != null;
        }

        public bool Has(ReadOnlySpan<string> files)
        {
            foreach (string file in files)
            {
                if (jar.GetEntry(file) == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
