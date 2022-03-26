using HACC.Components;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleDemo : ComponentBase
{
    private WebConsole _webConsole = default!;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        this._webConsole.WebApplication.Shutdown();
        this._webConsole.WebApplication.Init();
        Application.Top.Add(view: new Label(text: "HACC Demo"));
        this._webConsole.WebApplication.Run();
    }
}