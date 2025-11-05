namespace MinecraftJarScanner.BlazorApp.Helpers;

/// <summary>
/// Contains helper logic related to <see cref="ScannerStatus"/>.
/// </summary>
public static class StatusHelper
{
    /// <summary>
    /// Returns the Bootstrap color name for <paramref name="status"/>.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static string GetColorName(ScannerStatus status)
    {
        return status switch
        {
            ScannerStatus.Idle => "primary",
            ScannerStatus.Scanning => "warning",
            ScannerStatus.Completed => "success",
            ScannerStatus.Error => "danger",
            _ => "danger",
        };
    }
}
