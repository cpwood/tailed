using System.Text.Json.Serialization;
using Tailed.Common;

namespace Tailed;

public class ColorizationRule
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Modes
    {
        First,
        All
    }

    public string? Name { get; set; }
    public string Pattern { get; set; } = null!;
    public bool IgnoreCase { get; set; }
    public AnsiCodes.Colors Background { get; set; } = AnsiCodes.Colors.Default;
    public AnsiCodes.Colors Foreground { get; set; } = AnsiCodes.Colors.Default;
    public Modes Mode { get; set; } = Modes.First;
}