using System.Text.Json.Serialization;

namespace MinecraftJarScanner.Lib.Models;

public record JarEvaluation([property: JsonConverter(typeof(JsonStringEnumConverter))] JarKind Kind, bool HasMetaInf);
