using System.Text.RegularExpressions;
using Tailed.Common;

namespace Tailed;

internal class MonitorSession : SessionBase
{
    public MonitorSession(string hostname) : base(hostname)
    {
    }

    public async Task MonitorFileAsync(string filename, ColorizationRule[] rules, CancellationToken cancellationToken)
    {
        RenderSessionInformation();

        if (!File.Exists(filename))
        {
            Console.WriteLine($"'{filename}' doesn't exist yet. Waiting for file creation..");

            while (!File.Exists(filename) && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
                return;
        }

        Console.WriteLine($"Monitoring '{filename}' ..");

        using var reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrEmpty(line))
                continue;

            foreach (var rule in rules)
            {
                if (rule.Mode == ColorizationRule.Modes.First)
                {
                    line = ProcessRule(rule, line).Line;
                }
                else
                {
                    var success = true;

                    while (success)
                    {
                        var result = ProcessRule(rule, line);
                        line = result.Line;
                        success = result.Success;
                    }
                }
            }

            await Client.SendLineAsync($"{line}\n");
        }
    }

    private RuleResult ProcessRule(ColorizationRule rule, string line)
    {
        var match = Regex.Match(line, rule.Pattern,
            rule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

        var index = match.Index;
        var length = match.Length;

        if (match.Groups.Count > 1)
        {
            index = match.Groups[1].Index;
            length = match.Groups[1].Length;
        }

        if (!match.Success)
            return new RuleResult(false, line);

        line = line.Insert(index + length,
            $"{AnsiCodes.BlackBackground}{AnsiCodes.WhiteForeground}");

        line = line.Insert(index,
            $"{AnsiCodes.ColorToAnsi(AnsiCodes.ColorPosition.Foreground, rule.Foreground)}{AnsiCodes.ColorToAnsi(AnsiCodes.ColorPosition.Background, rule.Background)}");

        return new RuleResult(true, line);
    }

    private class RuleResult
    {
        public bool Success { get; }
        public string Line { get; }

        public RuleResult(bool success, string line)
        {
            Success = success;
            Line = line;
        }
    }
}