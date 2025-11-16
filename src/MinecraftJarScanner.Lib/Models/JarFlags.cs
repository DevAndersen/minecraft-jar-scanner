namespace MinecraftJarScanner.Lib.Models;

public record JarFlags
{
    /// <summary>
    /// Does the <c>.jar</c> file contain <c>com/mojang/rubydung/RubyDung.class</c>
    /// </summary>
    public required bool HasRubyDungRubyDungClass { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain <c>com/mojang/minecraft/RubyDung.class</c>
    /// </summary>
    public required bool HasMinecraftRubyDungClass { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain <c>com/mojang/minecraft/Minecraft.class</c>
    /// </summary>
    public required bool HasClientComMinecraftClass { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain <c>net/minecraft/client/main.class</c>
    /// </summary>
    public required bool HasClientNetMainClass { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain <c>com/mojang/minecraft/MinecraftApplet.class</c>
    /// </summary>
    public required bool HasClientComAppletClass { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain <c>net/minecraft/client/MinecraftApplet.class</c>
    /// </summary>
    public required bool HasClientNetAppletClass { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain <c>com/mojang/minecraft/server/MinecraftServer.class</c>
    /// </summary>
    public required bool HasServerComServerClass { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain <c>net/minecraft/server/MinecraftServer.class</c>
    /// </summary>
    public required bool HasServerNetServerClass { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain <c>net/minecraft/bundler/Main.class</c>
    /// </summary>
    public required bool HasServerNetBundleMain { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain
    /// <list type="bullet">
    /// <item><c>META-INF/MANIFEST.MF</c></item>
    /// <item><c>META-INF/MOJANG_C.SF</c></item>
    /// <item><c>META-INF/MOJANG_C.DSA</c></item>
    /// </list>
    /// </summary>
    public required bool HasClientMetaInf { get; init; }

    /// <summary>
    /// Does the <c>.jar</c> file contain <c>META-INF/MANIFEST.MF</c>
    /// </summary>
    public required bool HasServerMetaInf { get; init; }
}
