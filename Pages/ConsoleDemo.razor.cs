using System.Runtime.Versioning;
using HACC.Applications;
using HACC.Demo.Logging;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;
public partial class ConsoleDemo : ComponentBase
{
    private static ILogger _logger;

    [Inject] public static WebApplication WebApplication { get; set; } = default!;

    //protected override async Task OnInitializedAsync()
    //{
    //    await base.OnInitializedAsync();
    //    //var logger = serviceCollection..GetService<ILoggerFactory>()!.CreateLogger("Logging")
    //    if (WebApplication != null)
    //    {
    //        WebApplication.Shutdown();
    //    }
    //    //WebApplication = new WebApplication();

    //    //this.WebApplication.Shutdown();
    //    //this.WebApplication.Init();
    //}

    //public ConsoleDemo(ILogger logger)
    //{
    //    _logger = logger;
    //}

    public ConsoleDemo()
    {
        if (_logger == null)
        {
            var serviceProvider = new ServiceCollection()
              .AddLogging()
              .BuildServiceProvider();

           _logger = serviceProvider.GetService<ILoggerFactory>()!
                        .CreateLogger<Program>();
        }
        if (WebApplication != null)
        {
            WebApplication.Shutdown();
        }
        WebApplication = new WebApplication(_logger);
        WebApplication.Init();

        Application.Top.Add(new Label("HACC Demo"));

        WebApplication.Run();
    }
}
