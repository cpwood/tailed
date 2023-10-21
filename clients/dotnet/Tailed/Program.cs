using CommandLine;
using System.Text.Json;
using System.Text.RegularExpressions;
using CommandLine.Text;
using Tailed;
using Tailed.Rules;

var p = new Parser(x =>
{
    x.CaseInsensitiveEnumValues = true;
    x.CaseSensitive = false;
    //x.AutoHelp = false;
    x.HelpWriter = null;
});

var result = p.ParseArguments<TailOptions>(args);

await result.WithParsedAsync(async (options) =>
{
    try
    {
        var rules = Array.Empty<ColorizationRule>();

        if (!string.IsNullOrEmpty(options.ColorRulesFile))
        {
            if (!File.Exists(options.ColorRulesFile))
                throw new FileNotFoundException($"The file '{options.File}' cannot be found.");
            rules = JsonSerializer.Deserialize<ColorizationRule[]>(
                await File.ReadAllTextAsync(options.ColorRulesFile)) ?? Array.Empty<ColorizationRule>();
        }
        else if (options.ColorRules != TailOptions.DefaultRules.None)
        {
            switch (options.ColorRules)
            {
                case TailOptions.DefaultRules.Serilog:
                    rules = new SerilogList().Rules;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid colorization rule set name.");
            }
        }

        if (!string.IsNullOrEmpty(options.Hostname))
        {
            if (!Regex.IsMatch(options.Hostname, @"(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]", RegexOptions.IgnoreCase))
            {
                throw new ArgumentException("The hostname must be a valid domain name.");
            }
        }

        var monitor = new MonitorSession(options.Hostname ?? "tailed.live");
        await monitor.MonitorFileAsync(options.File, rules, CancellationToken.None);
    }
    catch (ArgumentOutOfRangeException ex)
    {
        await Console.Error.WriteLineAsync(ex.Message);
    }
    catch (JsonException ex)
    {
        await Console.Error.WriteLineAsync(ex.Message);
    }
    catch (ArgumentException ex)
    {
        await Console.Error.WriteLineAsync(ex.Message);
    }
    catch (Exception ex)
    {
        await Console.Error.WriteLineAsync(ex.ToString());
    }
});

result.WithNotParsed(errs =>
{
    var helpText = HelpText.AutoBuild(result, h =>
    {
        h.AddNewLineBetweenHelpSections = true;
        h.AddEnumValuesToHelpText = true;
        h.AdditionalNewLineAfterOption = false;
        h.AddPostOptionsLine("See the documentation at https://docs.tailed.live");
        return HelpText.DefaultParsingErrorsHandler(result, h);
    });

    Console.WriteLine(Regex.Replace(helpText, @"defaultVerb\s", string.Empty));
});