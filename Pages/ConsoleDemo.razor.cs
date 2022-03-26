using HACC.Components;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleDemo : ComponentBase
{
    private WebConsole _webConsole = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        this._webConsole!.WebConsoleDriver.ConsoleWeb = this._webConsole!;
        this._webConsole.WebApplication.Shutdown();
        this._webConsole.WebApplication.Init();
        Application.Top.Add(view: new Label(text: "HACC Demo"));
        this._webConsole.WebApplication.Run();
    }
}