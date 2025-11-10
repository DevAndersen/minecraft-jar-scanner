namespace MinecraftJarScanner.Lib.Models;

public record JarFlags
{
    public required bool HasRubyDungClass { get; init; }

    public required bool HasMinecraftAppletClass { get; init; }

    public required bool HasMinecraftClass { get; init; }

    public required bool HasMinecraftServerClass { get; init; }

    public required bool HasMinecraftClientMetaInf { get; init; }
}
