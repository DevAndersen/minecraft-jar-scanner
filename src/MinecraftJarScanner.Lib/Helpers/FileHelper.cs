namespace MinecraftJarScanner.Lib.Helpers;

/// <summary>
/// Contains helper logic related to files.
/// </summary>
internal static class FileHelper
{
    /// <summary>
    /// Determines if <paramref name="fileName"/> has the specified <paramref name="extension"/>.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static bool IsFileType(string fileName, string extension)
    {
        return Path.GetExtension(fileName)
            .Equals(extension, StringComparison.OrdinalIgnoreCase);
    }
}
