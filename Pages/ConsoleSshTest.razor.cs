using HACC.Components;
using Microsoft.AspNetCore.Components;
using Renci.SshNet;
using Renci.SshNet.Common;
using Terminal.Gui;
namespace HACC.Demo.Pages;

public partial class ConsoleSshTest : ComponentBase
{
    /// <summary>
    ///     This is NULL until after render
    /// </summary>
    private WebConsole? _webConsole;
    private Window? _window;
    private SshClient? sshClient;
    private ShellStream? shellStream;
    [Inject]
    private ILogger? logger { get; set; }

    protected void OnClick()
    {
        var driver = this._webConsole!.WebConsoleDriver!;
        this.sshClient = new Renci.SshNet.SshClient(
            host: "",
            username: "",
            password: "");
        this.sshClient.Connect();
        this.shellStream = this.sshClient.CreateShellStream(
             terminalName: "xterm-256color",
             columns: (uint) driver.BufferColumns,
             rows: (uint) driver.BufferRows,
             width: (uint) driver.WindowWidthPixels,
             height: (uint) driver.WindowHeightPixels,
             bufferSize: 4096000);
        this.shellStream.DataReceived += this.SshConnection_DataReceived;
        this.shellStream.ErrorOccurred += this.ShellStream_ErrorOccurred;
        this.logger!.Log(logLevel: LogLevel.Information, message: $"Connected: {Convert.ToString(this.sshClient.IsConnected)}");
    }
    protected void InitApp()
    {
        if (this._webConsole is null)
            throw new InvalidOperationException(message: "_webConsole reference was not set");

        this._webConsole.WebApplication!.Shutdown();
        this._webConsole.WebApplication.Init();

        var button = new Button(text: "Run");
        button.Clicked += this.OnClick;

        Application.RootMouseEvent = (e) =>
        {
        };
        Application.RootKeyEvent = (e) =>
        {
            var mk = ShortcutHelper.GetModifiersKey(e);
            //lblKey.Text = $"Key:{e.Key};KeyValue:{e.KeyValue};KeyChar:{(char) e.KeyValue}\nAlt:{mk.HasFlag(Key.AltMask)};Ctrl:{mk.HasFlag(Key.CtrlMask)};Shift:{mk.HasFlag(Key.ShiftMask)};Count:{++keyCount}";
            return false;
        };

        _window = new Window("SSH Test")
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };


        _window.Add(button);
        Application.Top.Add(_window);
        this._webConsole.WebApplication.Run();
    }

    private void ShellStream_ErrorOccurred(object? sender, ExceptionEventArgs e)
    {
        this.logger!.LogError(exception: e.Exception, e.ToString(), e);
    }

    private void SshConnection_DataReceived(object? sender, ShellDataEventArgs e)
    {
        var data = new byte[this.shellStream!.Length];
        var newData = this.shellStream.Read(data, 0, data.Length);
        this.shellStream.Flush();
        var driver = this._webConsole!.WebConsoleDriver!;
        var row = driver.CursorTop;
        var col = driver.CursorLeft;
        foreach (var d in data)
        {
            this._window!.AddRune(
            col: col,
            row: row,
            ch: new Rune(ch: (char) d));
            if (col++ == driver.WindowColumns)
            {
                row++;
            }
        }
    }
}