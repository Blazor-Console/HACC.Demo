using System.Runtime.Versioning;
using HACC.Applications;
using HACC.Enumerations;
using Microsoft.AspNetCore.Components;
using Console = HACC.Components.Console;

namespace HACC.Demo.Pages;

[SupportedOSPlatform(platformName: "browser")]
public partial class Index
{
    private ConsoleType ActiveConsole { get; }

    [Inject] private ILogger Logger { get; set; } = null!;

    public readonly WebApplication WebApplication;

    public Console ConsoleReference { get; set; } = null!;
    
    public Index()
    {
        ActiveConsole = ConsoleType.StandardOutput;
        this.WebApplication = new WebApplication(
            logger: Logger,
            console: ConsoleReference);
    }
}