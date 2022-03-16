using System.Runtime.Versioning;
using HACC.Applications;
using HACC.Components;
using HACC.Enumerations;
using HACC.Models;
using HACC.Models.Drivers;
using Microsoft.AspNetCore.Components;

namespace HACC.Demo.Pages;
[SupportedOSPlatform(platformName: "browser")]
public partial class ConsoleDemo : ComponentBase
{

    [Inject] private ILogger Logger { get; set; } = null!;

    [Inject] private WebClipboard WebClipboard { get; set; } = null!;

    public readonly WebApplication WebApplication;

    // ReSharper disable once MemberInitializerValueIgnored
    public WebConsole ConsoleReference { get; set; } = null!;

    public ConsoleDemo()
    {
        this.WebApplication = new WebApplication(
            logger: this.Logger,
            webClipboard: this.WebClipboard,
            console: this.ConsoleReference);

        this.WebApplication.Init();
        //this.WebApplication.Run();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        //this.WebApplication.Init();
    }
}
