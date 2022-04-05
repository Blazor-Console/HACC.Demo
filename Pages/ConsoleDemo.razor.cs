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
        var label = new Label(text: "HACC Demo")
        {
            X = Pos.Center(),
            Y = 0
        };
        var text = new TextField(text: "Enter your name")
        {
            X = Pos.Center(),
            Y = 2,
            Width = 20
        };
        var button = new Button(text: "Say Hello")
        {
            X = Pos.Center(),
            Y = 4
        };
        Application.Top.Add(label, text, button);
        this._webConsole.WebApplication.Run();
    }
}