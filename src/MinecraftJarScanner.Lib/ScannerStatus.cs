namespace MinecraftJarScanner.Lib;

/// <summary>
/// The different statuses that a <see cref="JarScanner"/> can be in."/>
/// </summary>
public enum ScannerStatus
{
    /// <summary>
    /// The scan has not yet been run.
    /// </summary>
    Idle,

    /// <summary>
    /// The scan is currently running.
    /// </summary>
    Scanning,

    /// <summary>
    /// The scan successfully ran to completion.
    /// </summary>
    Completed,

    /// <summary>
    /// The scan was unable to finish successfully.
    /// </summary>
    Error,

    /// <summary>
    /// The scan was cancelled by the user.
    /// </summary>
    Cancelled
}
