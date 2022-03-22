using HACC.Applications;
using HACC.Components;
using HACC.Demo;
using HACC.Demo.Extensions;
using HACC.Models;
using HACC.Models.Drivers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args: args);
builder.RootComponents.Add<App>(selector: "#app");
builder.RootComponents.Add<HeadOutlet>(selector: "head::after");

builder.Services.AddScoped(implementationFactory: sp => new HttpClient
{ BaseAddress = new Uri(uriString: builder.HostEnvironment.BaseAddress) });

builder.Logging.ClearProviders();
builder.Logging.AddCustomLogging(configure: configuration =>
{
    configuration.LogLevels.Add(
        key: LogLevel.Warning, value: ConsoleColor.DarkMagenta);
    configuration.LogLevels.Add(
        key: LogLevel.Error, value: ConsoleColor.Red);
});
builder.Logging.SetMinimumLevel(level: LogLevel.Debug);

//builder.Services.AddSingleton(implementationFactory: serviceProvider => new WebApplication(webConsoleDriver: new WebConsoleDriver(
//    logger: serviceProvider.GetService<ILoggerFactory>()!.CreateLogger("Logging"))));
//builder.Services.AddSingleton(implementationFactory: serviceProvider => new WebConsole(
//    logger: serviceProvider.GetService<ILoggerFactory>()!.CreateLogger("Logging")));
//builder.Services.AddSingleton(implementationFactory: serviceProvider => new WebApplication(
//    logger: serviceProvider.GetService<ILoggerFactory>()!.CreateLogger("Logging")));

await builder.Build().RunAsync();