using System.Text.RegularExpressions;
using TailedLive;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseDefaultFiles();
app.UseStaticFiles();

var tailHtml = await File.ReadAllTextAsync("wwwroot/tail.html");

app.Map("/", (HttpContext context) =>
{
    context.Response.Redirect(Environment.GetEnvironmentVariable("CUSTOM_HOMEPAGE") ??
                              "https://docs.tailed.live/");
});

app.MapWhen(x => Regex.IsMatch(x.Request.Path.ToString(), @"^/[a-zA-Z0-9_\-=]{22}$"), 
    config =>
{
    config.Run(async context => await context.Response.WriteAsync(tailHtml));
});


app.MapHub<TailHub>("/api/tail");

app.Run();
