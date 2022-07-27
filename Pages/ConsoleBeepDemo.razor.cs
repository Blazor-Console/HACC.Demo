using HACC.Components;
using HACC.Enumerations;
using HACC.Extensions;
using Microsoft.AspNetCore.Components;
using NStack;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleBeepDemo : ComponentBase
{
    /// <summary>
    ///     This is NULL until after render
    /// </summary>
    private WebConsole? _webConsole;
    private Timer? _timer = null;
    private BeepType _selected;
    private float? _duration;
    private float? _frequency;
    private float? _volume;
    private RadioGroup? _rg;
    private CheckBox? _ckbRandom;

    protected async Task InitAppAsync()
    {
        if (this._webConsole is null)
            throw new InvalidOperationException(message: "_webConsole reference was not set");

        await HaccExtensions.WebApplication!.Init();

        var win = new Window();

        var label = new Label("Duration:")
        {
            Width = 10,
            AutoSize = false
        };
        win.Add(label);

        var tfDuration = new TextField(Configuration.Defaults.BeepDurationMsec.ToString())
        {
            X = Pos.Right(label) + 1,
            Width = 5
        };
        win.Add(tfDuration);

        label = new Label("Frequency:")
        {
            Y = Pos.Bottom(label) + 1,
            Width = 10,
            AutoSize = false
        };
        win.Add(label);

        var tfFrequency = new TextField(Configuration.Defaults.BeepFrequency.ToString())
        {
            X = Pos.Right(label) + 1,
            Y = Pos.Top(label),
            Width = 5
        };
        win.Add(tfFrequency);

        label = new Label("Volume:")
        {
            Y = Pos.Bottom(label) + 1,
            Width = 10,
            AutoSize = false
        };
        win.Add(label);

        var tfVolume = new TextField(Configuration.Defaults.BeepVolume.ToString())
        {
            X = Pos.Right(label) + 1,
            Y = Pos.Top(label),
            Width = 5
        };
        win.Add(tfVolume);

        var beepTypes = Enum.GetValues(typeof(BeepType)).Cast<BeepType>().ToList();
        _rg = new RadioGroup(beepTypes.Select(b => ustring.Make(b.ToString())).ToArray())
        {
            Y = Pos.Bottom(label) + 1
        };
        _rg.SelectedItemChanged += e =>
        {

        };
        win.Add(_rg);

        var ckbContinuous = new CheckBox("Continuous beep")
        {
            Y = Pos.Bottom(_rg) + 1
        };
        win.Add(ckbContinuous);

        _ckbRandom = new CheckBox("Random")
        {
            Y = Pos.Bottom(ckbContinuous) + 1
        };
        win.Add(_ckbRandom);

        var btnStart = new Button("Start")
        {
            Y = Pos.Bottom(_ckbRandom) + 1
        };
        btnStart.Clicked += async () =>
        {
            if (_ckbRandom.Checked)
                _selected = (BeepType) GetRandomBeepType();
            else
                _selected = (BeepType) _rg.SelectedItem;
            if (ustring.IsNullOrEmpty(tfDuration.Text))
                _duration = null;
            else
                _duration = float.Parse(tfDuration.Text.ToString()!);
            if (ustring.IsNullOrEmpty(tfFrequency.Text))
                _frequency = null;
            else
                _frequency = float.Parse(tfFrequency.Text.ToString()!);
            if (ustring.IsNullOrEmpty(tfVolume.Text))
                _volume = null;
            else
                _volume = float.Parse(tfVolume.Text.ToString()!);

            if (ckbContinuous.Checked)
            {
                this.StartTimer();
                btnStart.Enabled = false;
                return;
            }
            await HaccExtensions.WebApplication.WebConsoleDriver!.Beep(_selected != BeepType.Custom
                ? _selected : null, _duration, _frequency, _volume);
        };
        win.Add(btnStart);

        var btnStop = new Button("St_op")
        {
            X = Pos.Right(btnStart) + 1,
            Y = Pos.Top(btnStart)
        };
        btnStop.Clicked += () =>
        {
            if (_timer != null)
            {
                this.StopTimer();
                btnStart.Enabled = true;
            }
        };
        win.Add(btnStop);

        Application.Top.Add(win);
        HaccExtensions.WebApplication.WebVisibilityChanged += this.WebApplication_WebVisibilityChanged;
        HaccExtensions.WebApplication.WebFocusChanged += this.WebApplication_WebVisibilityChanged;
        await HaccExtensions.WebApplication.Run();
        HaccExtensions.WebApplication.WebVisibilityChanged -= this.WebApplication_WebVisibilityChanged;
        HaccExtensions.WebApplication.WebFocusChanged -= this.WebApplication_WebVisibilityChanged;
    }

    private void WebApplication_WebVisibilityChanged(bool obj)
    {
        if (obj)
            this.StartTimer();
        else
            this.StopTimer();
    }

    private async void ContinuousBeep()
    {
        if (_ckbRandom!.Checked)
            _selected = (BeepType) GetRandomBeepType();
        else
            _selected = (BeepType) _rg!.SelectedItem;

        await HaccExtensions.WebApplication.WebConsoleDriver!.Beep(_selected != BeepType.Custom
            ? _selected : null, _duration, _frequency, _volume);
    }

    private int GetRandomBeepType()
    {
        var rnd = new Random();
        return rnd.Next(1, 4);
    }

    private void StartTimer()
    {
        if (_timer == null)
            _timer = new Timer((_) => this.ContinuousBeep(), null, 1000, 1000);
    }

    private void StopTimer()
    {
        if (_timer != null)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.DisposeAsync();
            _timer = null;
        }
    }
}