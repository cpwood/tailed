using Tailed;

if (!Environment.GetCommandLineArgs().Any())
{
    Console.WriteLine("Help");
    return;
}

var filename = Environment.GetCommandLineArgs()[0];

var monitor = new MonitorSession("localhost:44318");
await monitor.MonitorFileAsync(filename, Array.Empty<ColorizationRule>(), CancellationToken.None);