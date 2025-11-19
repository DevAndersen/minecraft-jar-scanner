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
    {
        // Copy content to memory stream, so the stream position can be reset.
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
            bool hasRubyDungRubyDungClass = jar.Has("com/mojang/rubydung/RubyDung.class");
            bool hasMinecraftRubyDungClass = jar.Has("com/mojang/minecraft/RubyDung.class");

            bool hasClientComMinecraftClass = jar.Has("com/mojang/minecraft/Minecraft.class");
            bool hasClientNetMainClass = jar.Has("net/minecraft/client/main.class");

            bool hasClientComAppletClass = jar.Has("com/mojang/minecraft/MinecraftApplet.class");
            bool hasClientNetAppletClass = jar.Has("net/minecraft/client/MinecraftApplet.class");

            bool hasServerComServerClass = jar.Has("com/mojang/minecraft/server/MinecraftServer.class");
            bool hasServerNetServerClass = jar.Has("net/minecraft/server/MinecraftServer.class");

            bool hasServerNetBundleMain = jar.Has("net/minecraft/bundler/Main.class");

            bool hasClientMetaInf = jar.Has(
                "META-INF/MANIFEST.MF",
                "META-INF/MOJANG_C.SF",
                "META-INF/MOJANG_C.DSA"
            );

            bool hasServerMetaInf = jar.Has("META-INF/MANIFEST.MF");

            JarFlags flags = new JarFlags
            {
                HasRubyDungRubyDungClass = hasRubyDungRubyDungClass,
                HasMinecraftRubyDungClass = hasMinecraftRubyDungClass,

                HasClientComMinecraftClass = hasClientComMinecraftClass,
                HasClientNetMainClass = hasClientNetMainClass,

                HasClientComAppletClass = hasClientComAppletClass,
                HasClientNetAppletClass = hasClientNetAppletClass,

                HasServerComServerClass = hasServerComServerClass,
                HasServerNetServerClass = hasServerNetServerClass,

                HasServerNetBundleMain = hasServerNetBundleMain,

                HasClientMetaInf = hasClientMetaInf,
                HasServerMetaInf = hasServerMetaInf,
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
        bool hasClientMetaInf = flags.HasClientMetaInf;
        bool hasServerMetaInf = flags.HasServerMetaInf && !flags.HasClientMetaInf;

        if (flags.HasRubyDungRubyDungClass || flags.HasMinecraftRubyDungClass)
        {
            return new JarEvaluation(JarKind.RubyDungClient, hasClientMetaInf);
        }

        if (flags.HasClientComMinecraftClass || flags.HasClientNetMainClass || flags.HasClientComAppletClass || flags.HasClientNetAppletClass)
        {
            return new JarEvaluation(JarKind.Client, hasClientMetaInf);
        }

        if (flags.HasServerComServerClass || flags.HasServerNetServerClass || flags.HasServerNetBundleMain)
        {
            return new JarEvaluation(JarKind.Server, hasServerMetaInf);
        }

        return new JarEvaluation(JarKind.UnrelatedJar, false);
    }
}

file static class Extensions
{
    // Todo: Replace with extension member syntax once https://github.com/dotnet/roslyn/issues/80024 has been resolved,
    // and move the extension block into the class above.
    public static bool Has(this ZipArchive jar, params ReadOnlySpan<string> files)
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
