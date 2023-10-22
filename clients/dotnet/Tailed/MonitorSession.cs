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
                line = rule.Mode == ColorizationRule.Modes.First
                    ? ProcessRuleFirst(rule, line)
                    : ProcessRuleAll(rule, line);
            }

            await Client.SendLineAsync($"{line}\n");
        }
    }

    private string ProcessRuleFirst(ColorizationRule rule, string line)
    {
        var match = Regex.Match(line, rule.Pattern,
            rule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

        return ProcessMatch(match, rule, line);
    }
    
    private string ProcessRuleAll(ColorizationRule rule, string line)
    {
        var matches = Regex.Matches(line, rule.Pattern,
            rule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

        for (var i = matches.Count - 1; i >= 0; i--)
        {
            line = ProcessMatch(matches[i], rule, line);
        }

        return line;
    }

    private string ProcessMatch(Match match, ColorizationRule rule, string line)
    {
        if (!match.Success)
            return line;
        
        var index = match.Index;
        var length = match.Length;

        if (match.Groups["c"].Success)
        {
            index = match.Groups["c"].Index;
            length = match.Groups["c"].Length;
        }
        
        line = line.Insert(index + length,
            $"{AnsiCodes.DefaultBackground}{AnsiCodes.DefaultForeground}");

        line = line.Insert(index,
            $"{AnsiCodes.ColorToAnsi(AnsiCodes.ColorPosition.Foreground, rule.Foreground)}{AnsiCodes.ColorToAnsi(AnsiCodes.ColorPosition.Background, rule.Background)}");

        return line;
    }
}