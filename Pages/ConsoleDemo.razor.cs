using System.Runtime.Versioning;
using HACC.Applications;
using HACC.Enumerations;
using Microsoft.AspNetCore.Components;

namespace HACC.Demo.Pages;
[SupportedOSPlatform(platformName: "browser")]
public partial class ConsoleDemo : ComponentBase
{
    private ConsoleType ActiveConsole { get; } = ConsoleType.StandardOutput;

    [Inject] private ILogger Logger { get; set; } = null!;

    public readonly WebApplication WebApplication;

    // ReSharper disable once MemberInitializerValueIgnored
    public HACC.Components.Console ConsoleReference { get; set; } = null!;

    public ConsoleDemo()
    {
        this.WebApplication = new WebApplication(
            logger: this.Logger,
            console: this.ConsoleReference!);

        WebApplication.Init();
        WebApplication.Run();

    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        //this.WebApplication.Init();
    }
}