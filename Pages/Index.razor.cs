using HACC.Enumerations;
using Microsoft.AspNetCore.Components;
using Console = HACC.Components.Console;

namespace HACC.Demo.Pages;

public partial class Index
{
    private ConsoleType ActiveConsole { get; }

    [Inject] private ILogger Logger { get; set; } = null!;

    public Console ConsoleReference { get; set; } = null!;
}