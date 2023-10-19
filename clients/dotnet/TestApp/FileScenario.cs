﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Tailed;
using Tailed.Rules;

namespace TestApp;

internal static class FileScenario
{
    public static async Task RunAsync()
    {
        var list = new SerilogList();

        if (File.Exists("test.log"))
            File.Delete("test.log");

        await using var session = new MonitorSession("localhost:44318");
        _ = session.MonitorFileAsync("test.log", list.Rules, CancellationToken.None);

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File("test.log")
            .CreateLogger();

        var serviceProvider = new ServiceCollection()
            .AddLogging((loggingBuilder) => loggingBuilder
                .SetMinimumLevel(LogLevel.Debug)
                .AddSerilog(dispose:true)
            )
            .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILoggerFactory>()!.CreateLogger<Program>();

        logger.LogInformation("Started The Test");

        for (var i = 0; i < 500; i++)
        {
            logger.Log(TestData.GetRandomLogLevel(), TestData.GetRandomSentence());
            await Task.Delay(100);
        }
    }
}