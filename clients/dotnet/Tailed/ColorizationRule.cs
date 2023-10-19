using Tailed.Common;

namespace Tailed;

public class ColorizationRule
{
    public enum Modes
    {
        First,
        All
    }

    public string? Name { get; set; }
    public string Pattern { get; set; } = null!;
    public bool IgnoreCase { get; set; }
    public AnsiCodes.Colors Background { get; set; } = AnsiCodes.Colors.Black;
    public AnsiCodes.Colors Foreground { get; set; } = AnsiCodes.Colors.White;
    public Modes Mode { get; set; } = Modes.First;
}