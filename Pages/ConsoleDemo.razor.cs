using System.Runtime.Versioning;
using HACC.Applications;
using HACC.Demo.Logging;
using Microsoft.AspNetCore.Components;

namespace HACC.Demo.Pages;
[SupportedOSPlatform(platformName: "browser")]
public partial class ConsoleDemo : ComponentBase
{

    [Inject] private ILogger Logger { get; set; } = null!;

    private static WebApplication? _instance;

    public readonly WebApplication WebApplication;

    public ConsoleDemo()
    {
        if (_instance != null)
        {
            _instance.Shutdown();
        }
        this.Logger = new CustomLogger("Test", GetConfig);
        this.WebApplication = new WebApplication(
            logger: this.Logger);

        _instance = this.WebApplication;
        this.WebApplication.Init();
        //this.WebApplication.Run();
    }

    private LoggingConfiguration GetConfig()
    {
        throw new NotImplementedException();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        //this.WebApplication.Init();
    }
}
