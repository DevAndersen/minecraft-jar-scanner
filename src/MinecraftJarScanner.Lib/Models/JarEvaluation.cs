namespace MinecraftJarScanner.Lib.Models;

public enum JarEvaluation
{
    UnrelatedJar
        = 0b_0000_0000_0000_0000_0000,

    MissingMetaInf
        = 0b_0000_0000_0000_0000_0001,

    RubyDungClient
        = 0b_0000_0000_0000_0001_0000,

    ClassicClient
        = 0b_0000_0000_0000_0010_0000,

    ClassicServer
        = 0b_0000_0000_0000_0100_0000,

    IndevClient
        = 0b_0000_0000_0000_1000_0000,

    InfdevServer
        = 0b_0000_0000_0001_0000_0000,

    AlphaClient
        = 0b_0000_0000_0010_0000_0000,

    AlphaServer
        = 0b_0000_0000_0100_0000_0000,

    BetaClient
        = 0b_0000_0000_1000_0000_0000,

    BetaServer
        = 0b_0000_0001_0000_0000_0000,

    ReleaseClient
        = 0b_0000_0010_0000_0000_0000,

    ReleaseServer
        = 0b_0000_0100_0000_0000_0000,
}
