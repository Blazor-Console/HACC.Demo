using HACC.Components;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleDemo : ComponentBase
{
    /// <summary>
    ///     This is NULL until after render
    /// </summary>
    private WebConsole? _webConsole;

    protected void InitApp()
    {
        if (this._webConsole is null)
            throw new InvalidOperationException(message: "_webConsole reference was not set");

        this._webConsole.WebApplication!.Shutdown();
        this._webConsole.WebApplication.Init();

        var label = new Label(text: "Enter your name:")
        {
            X = Pos.Center(),
            Y = 0,
        };
        var text = new TextField("gui.cs:")
        {
            X = Pos.Center(),
            Y = 2,
            Width = 20,
        };
        var button = new Button(text: "Say Hello")
        {
            X = Pos.Center(),
            Y = 4
        };
        var text2 = new TextField("this is horiz/vert centered")
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = 30,
        };
        var win = new Window()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        win.Add(label, text, button, text2);
        Application.Top.Add(win);
        this._webConsole.WebApplication.Run();
    }
}