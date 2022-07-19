using HACC.Components;
using HACC.Extensions;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleDemo : ComponentBase
{
    /// <summary>
    ///     This is NULL until after render
    /// </summary>
    private WebConsole? _webConsole;

    protected async Task InitAppAsync()
    {
        if (this._webConsole is null)
            throw new InvalidOperationException(message: "_webConsole reference was not set");

        await HaccExtensions.WebApplication!.Init();

        var win = new Window();

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

        HaccExtensions.WebApplication.RunStateEnding += (v) =>
        {
        };

        var text2 = new TextField("this is horiz/vert centered")
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = 30,
        };
        button.Clicked += () =>
        {
            HaccExtensions.WebApplication.RunStateEnding += WebApplication_RunStateEnding;
            var b = MessageBox.Query("Say Hello", $"Welcome {text.Text}", "Yes", "No");
        };
        void WebApplication_RunStateEnding(Toplevel obj)
        {
            var b = MessageBox.Clicked;
            if (b == -1)
                text2.Text = "Canceled!";
            else if (b == 0)
                text2.Text = "You choose 'Yes'";
            else if (b == 1)
                text2.Text = "You choose 'No'";
            HaccExtensions.WebApplication.RunStateEnding -= WebApplication_RunStateEnding;
        }
        var lblMouse = new Label()
        {
            Y = Pos.Center() + 2,
            Height = 2,
            AutoSize = true
        };
        var mouseCount = 0;
        Application.RootMouseEvent = (e) =>
        {
            lblMouse.Text = $"Mouse: X:{e.X};Y:{e.Y};Button:{e.Flags};\nView:{e.View};Count:{++mouseCount}";
        };
        var lblKey = new Label()
        {
            Y = Pos.Center() + 5,
            Height = 2,
            AutoSize = true
        };
        var keyCount = 0;
        Application.RootKeyEvent = (e) =>
        {
            var mk = ShortcutHelper.GetModifiersKey(e);
            lblKey.Text = $"Key:{e.Key};KeyValue:{e.KeyValue};KeyChar:{(char) e.KeyValue}\nAlt:{mk.HasFlag(Key.AltMask)};Ctrl:{mk.HasFlag(Key.CtrlMask)};Shift:{mk.HasFlag(Key.ShiftMask)};Count:{++keyCount}";
            return false;
        };

        win.Add(label, text, button, text2, lblMouse, lblKey);
        Application.Top.Add(win);
        await HaccExtensions.WebApplication.Run();
    }
}