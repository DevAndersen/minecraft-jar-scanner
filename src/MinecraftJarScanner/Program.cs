using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

if (!File.Exists(knownHashListFile))
{
    Console.WriteLine("Hash file not found, creating empty file.");
    File.Create(knownHashListFile).Dispose();
}
else
{
    knownHahes = File.ReadAllLines(knownHashListFile);
    Console.WriteLine($"Found {knownHahes.Length} hashes");
}

Console.Write("Search in: ");
string? baseDir = Console.ReadLine();
if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
{
    Console.WriteLine("Unable to find directory.");
    return;
}

Console.WriteLine($"Searching for files in: {baseDir}");
string[] files = FindFiles(baseDir, fileListFileName);

Console.WriteLine($"Total files found: {files.Length}");

foreach (string file in files)
{
    if (IsFileType(file, zipFileType))
    {
        await ScanZipFileAsync(file);
    }
    else if (IsFileType(file, jarFileType))
    {
        using FileStream fileStream = File.OpenRead(file);
        await ScanJarAsync(fileStream, [file]);
    }
}

Console.WriteLine("Done!");

try
{
    Console.Beep();
}
catch
{
}

public static partial class Program
{
    private const string outputFile = "output.txt";
    private const string knownHashListFile = "hash.txt";
    private const string fileListFileName = "filelist.txt";

    private static readonly string zipFileType = ".zip";
    private static readonly string jarFileType = ".jar";

    private static string[] knownHahes = [];

    private static string[] FindFiles(string baseDir, string outputFile)
    {
        if (File.Exists(outputFile))
        {
            return File.ReadAllLines(outputFile);
        }

        string[] allFiles = Directory.GetFiles(baseDir, "*.*", SearchOption.AllDirectories);
        File.WriteAllLines(outputFile, allFiles.Where(x => IsFileType(x, zipFileType) || IsFileType(x, jarFileType)));

        return allFiles;
    }

    private static async Task ScanZipFileAsync(string file)
    {
        LinkedList<string> nav = new LinkedList<string>([file]);

        try
        {
            using ZipArchive zip = ZipFile.Open(file, ZipArchiveMode.Read);
            await ScanZipEntriesAsync(zip.Entries, nav);
        }
        catch (Exception e)
        {
            Print($"""
                {string.Join(" -> ", nav)}
                    Error scanning zip file {file}
                    {e.GetType().FullName}
                {e.StackTrace}

                """);
        }
    }

    private static async Task ScanZipEntriesAsync(IEnumerable<ZipArchiveEntry> entries, LinkedList<string> nav)
    {
        foreach (ZipArchiveEntry entry in entries)
        {
            nav.AddLast(entry.FullName);

            if (IsFileType(entry.FullName, zipFileType))
            {

                using ZipArchive nestedArchive = await ZipArchive.CreateAsync(entry.Open(), ZipArchiveMode.Read, true, Encoding.UTF8);
                await ScanZipEntriesAsync(nestedArchive.Entries, nav);
            }
            else if (IsFileType(entry.FullName, jarFileType))
            {
                await ScanJarAsync(entry.Open(), nav);
            }

            nav.RemoveLast();
        }
    }

    private static bool IsFileType(string fileName, string extension)
    {
        return Path.GetExtension(fileName)
            .Equals(extension, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task ScanJarAsync(Stream stream, IEnumerable<string> nav)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        ms.Position = 0;

        Span<byte> hash = stackalloc byte[MD5.HashSizeInBytes];
        MD5.HashData(ms, hash);
        ms.Position = 0;
        string hashHex = Convert.ToHexString(hash);

        using ZipArchive jar = await ZipArchive.CreateAsync(ms, ZipArchiveMode.Read, true, Encoding.UTF8);
        try
        {
            bool isClientJar = IsMinecraftClientJar(jar);
            bool hasClientMetaInf = ContainsClientMetaInf(jar);

            bool isServerJar = IsMinecraftServerJar(jar);
            bool hasServerMetaInf = ContainsServerMetaInf(jar);

            bool isHashKnown = knownHahes.Contains(hashHex, StringComparer.OrdinalIgnoreCase);

            bool isClientCandidate = isClientJar && hasClientMetaInf;
            bool isServerCandidate = isServerJar && hasServerMetaInf;

            bool isCandidate = (isClientCandidate || isServerCandidate) && !isHashKnown;

            string candidateString = $"\e[{(isCandidate ? "92" : "91")}m{isCandidate}\e[0m";

            Print($"""
                {string.Join(" -> ", nav)}
                    MD5:        {hashHex}
                    IsClient    {isClientJar}
                    ClientMeta: {hasClientMetaInf}
                    IsServer    {isServerJar}
                    ServerMeta: {hasServerMetaInf}
                    Known:      {isHashKnown}
                    Candidate:  {candidateString}

                """);
        }
        catch (Exception e)
        {
            Print($"""
                {string.Join(" -> ", nav)}
                    Error reading file
                    {e.GetType().FullName}
                {e.StackTrace}

                """);
        }
    }

    private static bool IsMinecraftClientJar(ZipArchive jar)
    {
        return jar.GetEntry("net/minecraft/client/Minecraft.class") != null;
    }

    private static bool IsMinecraftServerJar(ZipArchive jar)
    {
        return jar.GetEntry("net/minecraft/server/MinecraftServer.class") != null;
    }

    private static bool ContainsClientMetaInf(ZipArchive jar)
    {
        string[] metaInfFileNames =
        [
            "META-INF/MANIFEST.MF",
            "META-INF/MOJANG_C.SF",
            "META-INF/MOJANG_C.DSA",
        ];

        foreach (string metaInfFileName in metaInfFileNames)
        {
            if (jar.GetEntry(metaInfFileName) == null)
            {
                return false;
            }
        }

        return true;
    }

    private static bool ContainsServerMetaInf(ZipArchive jar)
    {
        return jar.GetEntry("META-INF/MANIFEST.MF") != null;
    }

    private static void Print(string message)
    {
        Console.WriteLine(message);
        File.AppendAllText(outputFile, AnsiEscapeSequenceRegex().Replace(message, string.Empty) + '\n');
    }

    [GeneratedRegex(@"\e\[.+?m")]
    private static partial Regex AnsiEscapeSequenceRegex();
}
