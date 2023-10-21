using CommandLine;
using CommandLine.Text;

namespace Tailed;

[Verb("defaultVerb", true)]
internal class TailOptions
{
    public enum DefaultRules
    {
        None,
        Serilog
    }

    [Value(0, MetaName = "Filename", Required = true, HelpText = "The path to the file which will be tailed.")]
    public string File { get; set; } = null!;

    [Option('c',"rulesFile", Required = false, Default = null, HelpText = "Optional path to a file containing JSON colorization rules.")]
    public string? ColorRulesFile { get; set; }

    [Option('r', "rules", Required = false, Default = DefaultRules.None, HelpText = "A default colorization rule set.")]
    public DefaultRules ColorRules { get; set; } = DefaultRules.None;

    [Option('h', "hostname", Required = false, Default = null, HelpText = "Optional override of the Tailed server hostname.")]
    public string? Hostname { get; set; }

    [Usage(ApplicationAlias = "tailed")]
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<Example> Examples
    {
        get
        {
            yield return new Example("Monitor a log file", new TailOptions
            {
                File = "/path/to/my/file.log"
            });

            yield return new Example("Monitor a log file and use Serilog colorization rules", new TailOptions
            {
                File = "/path/to/my/file.log",
                ColorRules = DefaultRules.Serilog
            });

            yield return new Example("Monitor a log file and use custom colorization rules", new TailOptions
            {
                File = "/path/to/my/file.log",
                ColorRulesFile = "/path/to/rules.json"
            });

            yield return new Example("Monitor a log file with a custom web server", new TailOptions
            {
                File = "/path/to/my/file.log",
                Hostname = "my.domain.com"
            });
        }
    }
}