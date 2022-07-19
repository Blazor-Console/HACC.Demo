using HACC.Components;
using HACC.Extensions;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;
using static HACC.Demo.Pages.ConsoleScrollingDemo;

namespace HACC.Demo.Pages;

public partial class ConsoleMenuDemo : ComponentBase
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

        Label ml;
        int count = 0;
        ml = new Label(new Rect(1, 1, 50, 1), "Mouse: ");
        List<string> rme = new List<string>();

        var test = new Label(1, 2, "Se iniciará el análisis");
        win.Add(test);
        win.Add(ml);

        var rmeList = new ListView(rme)
        {
            X = Pos.Right(test) + 25,
            Y = Pos.Top(test) + 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill(),
            ColorScheme = Colors.TopLevel
        };
        win.Add(rmeList);

        Application.RootMouseEvent += delegate (MouseEvent me)
        {
            ml.Text = $"Mouse: ({me.X},{me.Y}) - {me.Flags} {count}";
            rme.Add($"({me.X},{me.Y}) - {me.Flags} {count++}");
            rmeList.MoveDown();
            Console.WriteLine($"RootMouseEvent(X:{me.X};Y:{me.Y};Flags:{me.Flags})");
        };

        // I have no idea what this was intended to show off in demo.c
        var drag = new Label("Drag: ") { X = 1, Y = 4 };
        var dragText = new TextField("")
        {
            X = Pos.Right(drag),
            Y = Pos.Top(drag),
            Width = 40
        };
        win.Add(drag, dragText);

        var scrollView = new ScrollView()//new Rect(1, 6, 20, 8))
        {
            X = 1,
            Y = Pos.AnchorEnd() - 8,
            Width = 20,
            Height = 8,
            ContentSize = new Size(20, 50),
            //ContentOffset = new Point (0, 0),
            ShowVerticalScrollIndicator = true,
            ShowHorizontalScrollIndicator = true
        };
        var filler = new Filler(new Rect(0, 0, 60, 40));
        scrollView.Add(filler);
        scrollView.DrawContent += (_) => scrollView.ContentSize = filler.GetContentSize();
        win.Add(scrollView);

        Application.Top.Add(win);
        await HaccExtensions.WebApplication.Run();
    }
}