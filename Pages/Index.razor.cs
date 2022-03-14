using System.Runtime.Versioning;
using HACC.Applications;
using HACC.Enumerations;
using Microsoft.AspNetCore.Components;
using Console = HACC.Components.Console;

namespace HACC.Demo.Pages;

[SupportedOSPlatform(platformName: "browser")]
public partial class Index
{
    private ConsoleType ActiveConsole { get; } = ConsoleType.StandardOutput;

    [Inject] private ILogger Logger { get; set; } = null!;

    public readonly WebApplication WebApplication;

    // ReSharper disable once MemberInitializerValueIgnored
    public Console ConsoleReference { get; set; } = null!;

    public Index()
    {
        this.WebApplication = new WebApplication(
            logger: this.Logger,
            console: this.ConsoleReference!);
        this.WebApplication.Init();
    }
}