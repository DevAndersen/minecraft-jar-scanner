namespace MinecraftJarScanner.Lib.Helpers;

internal static class FileHelper
{
    public static bool IsFileType(string fileName, string extension)
    {
        return Path.GetExtension(fileName)
            .Equals(extension, StringComparison.OrdinalIgnoreCase);
    }
}
