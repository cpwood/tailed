using Tailed.Common;

namespace Tailed.Rules;

internal class SerilogList : IRulesList
{
    public ColorizationRule[] Rules
    {
        get
        {
            return new[]
            {
                new ColorizationRule
                {
                    Name = "Information",
                    Pattern = @"\[(?<c>INF)\]",
                    Foreground = AnsiCodes.Colors.Green,
                    Background = AnsiCodes.Colors.Default,
                    Mode = ColorizationRule.Modes.First
                },
                new ColorizationRule
                {
                    Name = "Warning",
                    Pattern = @"\[(?<c>WRN)\]",
                    Foreground = AnsiCodes.Colors.Black,
                    Background = AnsiCodes.Colors.Yellow,
                    Mode = ColorizationRule.Modes.First
                },
                new ColorizationRule
                {
                    Name = "Error",
                    Pattern = @"\[(?<c>ERR)\]",
                    Foreground = AnsiCodes.Colors.BrightRed,
                    Background = AnsiCodes.Colors.Default,
                    Mode = ColorizationRule.Modes.First
                },
                new ColorizationRule
                {
                    Name = "Fatal",
                    Pattern = @"\[(?<c>FTL)\]",
                    Foreground = AnsiCodes.Colors.BrightWhite,
                    Background = AnsiCodes.Colors.Red,
                    Mode = ColorizationRule.Modes.First
                },
                new ColorizationRule
                {
                    Name = "Datetime",
                    Pattern = @"[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}\.[0-9]{3} [\+\-]?[0-9]{2}:[0-9]{2}",
                    Foreground = AnsiCodes.Colors.Magenta,
                    Background = AnsiCodes.Colors.Default,
                    Mode = ColorizationRule.Modes.All
                }
            };
        }
    }
}