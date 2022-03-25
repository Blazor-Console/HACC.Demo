using System.Runtime.Versioning;
using HACC.Applications;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;
public partial class ConsoleDemo : ComponentBase
{
    [Inject] private ILoggerFactory LoggerFactory { get; set; }

    private static ILogger _logger;

    [Inject] public static WebApplication WebApplication { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _logger = LoggerFactory.CreateLogger("Logging");
        WebApplication?.Shutdown();
        WebApplication = new WebApplication(_logger);
        WebApplication.Init();

        Application.Top.Add(new Label("HACC Demo"));

        WebApplication.Run();
    }
    public ConsoleDemo()
    {

    }

    //public ConsoleDemo()
    //{
    //    if (_logger == null)
    //    {
    //        var serviceProvider = new ServiceCollection()
    //          .AddLogging()
    //          .BuildServiceProvider();

    //        _logger = serviceProvider.GetService<ILoggerFactory>()!
    //                     .CreateLogger("Logging");
    //    }
    //    WebApplication?.Shutdown();
    //    WebApplication = new WebApplication(_logger);
    //    WebApplication.Init();

    //    Application.Top.Add(new Label("HACC Demo"));

    //    WebApplication.Run();
    //}
}
