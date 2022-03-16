using System.Runtime.Versioning;
using HACC.Applications;
using HACC.Components;
using HACC.Enumerations;
using HACC.Models.Drivers;
using Microsoft.AspNetCore.Components;

namespace HACC.Demo.Pages;
[SupportedOSPlatform(platformName: "browser")]
public partial class ConsoleDemo : ComponentBase
{

    [Inject] private ILogger Logger { get; set; } = null!;

    public readonly WebApplication WebApplication;

    // ReSharper disable once MemberInitializerValueIgnored
    public WebConsole ConsoleReference { get; set; } = null!;

    public ConsoleDemo()
    {
        this.WebApplication = new WebApplication(
            logger: this.Logger,
            console: this.ConsoleReference);

        WebApplication.Init();
        //WebApplication.Run();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        //this.WebApplication.Init();
    }
}