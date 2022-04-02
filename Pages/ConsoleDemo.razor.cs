using HACC.Components;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleDemo : ComponentBase
{
    /// <summary>
    /// This is NULL until after render
    /// </summary>
    private WebConsole _webConsole = default!;

    protected async Task InitApp()
    {
        if (_webConsole is null)
            throw new InvalidOperationException("_webConsole reference was not set");

        this._webConsole.WebApplication!.Shutdown();
        this._webConsole.WebApplication.Init();
        Application.Top.Add(view: new Label(text: "HACC Demo"));
        this._webConsole.WebApplication.Run();
    }
}