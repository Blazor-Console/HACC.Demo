using HACC.Components;
using HACC.Extensions;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.ComponentModel;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleMdiDemo : ComponentBase
{
    /// <summary>
    ///     This is NULL until after render
    /// </summary>
    [Parameter] public WebConsole? _webConsole { get; set; }

    protected async Task InitAppAsync()
    {
        if (this._webConsole is null)
            throw new InvalidOperationException(message: "_webConsole reference was not set");

        await HaccExtensions.WebApplication!.Init();

        await HaccExtensions.WebApplication.Run<MdiMain>();
    }

    class MdiMain : Toplevel
    {
        private WorkerApp _workerApp;
        private bool _canOpenWorkerApp;
        MenuBar _menu;

        public MdiMain()
        {
            Data = "MdiMain";

            IsMdiContainer = true;

            _workerApp = new WorkerApp() { Visible = false };

            _menu = new MenuBar(new MenuBarItem[] {
                    new MenuBarItem ("_Options", new MenuItem [] {
                        new MenuItem ("_Run Worker", "", () => _workerApp.RunWorker(), null, null, Key.CtrlMask | Key.R),
                        new MenuItem ("_Cancel Worker", "", () => _workerApp.CancelWorker(), null, null, Key.CtrlMask | Key.C),
                        null!,
                        new MenuItem ("_Quit", "", () => Quit(), null, null, Key.CtrlMask | Key.Q)
                    }),
                    new MenuBarItem ("_View", new MenuItem [] { }),
                    new MenuBarItem ("_Window", new MenuItem [] { })
                });
            _menu.MenuOpening += Menu_MenuOpening;
            Add(_menu);

            var statusBar = new StatusBar(new[] {
                    new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Exit", () => Quit()),
                    new StatusItem(Key.CtrlMask | Key.R, "~^R~ Run Worker", () => _workerApp.RunWorker()),
                    new StatusItem(Key.CtrlMask | Key.C, "~^C~ Cancel Worker", () => _workerApp.CancelWorker())
                });
            Add(statusBar);

            Activate += MdiMain_Activate;
            Deactivate += MdiMain_Deactivate;

            Closed += MdiMain_Closed;

            Application.Iteration += async () =>
            {
                var mdiTop = Application.MdiTop;
                if (_canOpenWorkerApp && !_workerApp.Running && mdiTop.Running)
                {
                    await HaccExtensions.WebApplication.Run(_workerApp);
                }
            };
        }

        private void MdiMain_Closed(Toplevel obj)
        {
            _workerApp.Dispose();
            Dispose();
        }

        private void Menu_MenuOpening(MenuOpeningEventArgs menu)
        {
            if (!_canOpenWorkerApp)
            {
                _canOpenWorkerApp = true;
                return;
            }
            if (menu.CurrentMenu.Title == "_Window")
            {
                menu.NewMenuBarItem = OpenedWindows();
            }
            else if (menu.CurrentMenu.Title == "_View")
            {
                menu.NewMenuBarItem = View();
            }
        }

        private void MdiMain_Deactivate(Toplevel top)
        {
            _workerApp.WriteLog($"{top.Data} deactivate.");
        }

        private void MdiMain_Activate(Toplevel top)
        {
            _workerApp.WriteLog($"{top.Data} activate.");
        }

        private MenuBarItem View()
        {
            List<MenuItem> menuItems = new List<MenuItem>();
            var item = new MenuItem()
            {
                Title = "WorkerApp",
                CheckType = MenuItemCheckStyle.Checked
            };
            var top = Application.MdiChildes?.Find((x) => x.Data.ToString() == "WorkerApp");
            if (top != null)
            {
                item.Checked = top.Visible;
            }
            item.Action += () =>
            {
                var mdiTop = Application.MdiTop;
                var mdiChildes = Application.MdiChildes;
                var top = mdiChildes!.Find((x) => x.Data.ToString() == "WorkerApp");
                item.Checked = top!.Visible = !item.Checked;
                if (top.Visible)
                {
                    top.ShowChild();
                }
                else
                {
                    mdiTop.SetNeedsDisplay();
                }
            };
            menuItems.Add(item);
            return new MenuBarItem("_View",
                new List<MenuItem[]>() { menuItems.Count == 0 ? new MenuItem[] { } : menuItems.ToArray() });
        }

        private MenuBarItem OpenedWindows()
        {
            var index = 1;
            List<MenuItem> menuItems = new List<MenuItem>();
            var sortedChildes = Application.MdiChildes;
            sortedChildes.Sort(new ToplevelComparer());
            foreach (var top in sortedChildes)
            {
                if (top.Data.ToString() == "WorkerApp" && !top.Visible)
                {
                    continue;
                }
                var item = new MenuItem();
                item.Title = top is Window ? $"{index} {((Window) top).Title}" : $"{index} {top.Data}";
                index++;
                item.CheckType |= MenuItemCheckStyle.Checked;
                var topTitle = top is Window ? ((Window) top).Title : top.Data.ToString();
                var itemTitle = item.Title.Substring(index.ToString().Length + 1);
                if (top == top.GetTopMdiChild() && topTitle == itemTitle)
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }
                item.Action += () =>
                {
                    top.ShowChild();
                };
                menuItems.Add(item);
            }
            if (menuItems.Count == 0)
            {
                return new MenuBarItem("_Window", "", null);
            }
            else
            {
                return new MenuBarItem("_Window", new List<MenuItem[]>() { menuItems.ToArray() });
            }
        }

        private void Quit()
        {
            RequestStop();
        }
    }

    class WorkerApp : Toplevel
    {
        private List<string> _log = new List<string>();
        private ListView _listLog;
        private Dictionary<Staging, BackgroundWorker>? _stagingWorkers;
        private List<StagingUIController>? _stagingsUI;

        public WorkerApp()
        {
            Data = "WorkerApp";

            Width = Dim.Percent(80);
            Height = Dim.Percent(50);

            ColorScheme = Colors.Base;

            var label = new Label("Worker collection Log")
            {
                X = Pos.Center(),
                Y = 0
            };
            Add(label);

            _listLog = new ListView(_log)
            {
                X = 0,
                Y = Pos.Bottom(label),
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            Add(_listLog);
        }

        public async void RunWorker()
        {
            var stagingUI = new StagingUIController() { Modal = true };

            Staging? staging = null;
            var worker = new BackgroundWorker() { WorkerSupportsCancellation = true };

            worker.DoWork += async (s, e) =>
            {
                string? error = null;
                var cancel = false;
                var stageResult = new List<string>();
                await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        for (int i = 0; i < 500; i++)
                        {
                            stageResult.Add(
                                $"Worker {i} started at {DateTime.Now}");
                            e.Result = stageResult;
                            await Task.Delay(10);
                            if (worker.CancellationPending)
                            {
                                cancel = true;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }

                    if (error != null)
                    {
                        // Failed
                        WriteLog($"Exception occurred {error} on Worker {staging!.StartStaging}.{staging.StartStaging:fff} at {DateTime.Now}");
                    }
                    else if (cancel)
                    {
                        // Canceled
                        WriteLog($"Worker {staging!.StartStaging}.{staging.StartStaging:fff} was canceled at {DateTime.Now}!");
                    }
                    else
                    {
                        // Passed
                        WriteLog($"Worker {staging!.StartStaging}.{staging.StartStaging:fff} was completed at {DateTime.Now}.");
                        Application.Refresh();

                        var stagingUI = new StagingUIController(staging, (List<string>) e.Result!)
                        {
                            Modal = false,
                            Title = $"Worker started at {staging.StartStaging}.{staging.StartStaging:fff}",
                            Data = $"{staging.StartStaging}.{staging.StartStaging:fff}"
                        };

                        stagingUI.ReportClosed += StagingUI_ReportClosed;

                        if (_stagingsUI == null)
                        {
                            _stagingsUI = new List<StagingUIController>();
                        }
                        _stagingsUI.Add(stagingUI);
                        _stagingWorkers?.Remove(staging);

                        await stagingUI.RunAsync();
                    }
                });
            };

            await HaccExtensions.WebApplication.Run(stagingUI);

            if (stagingUI.Staging != null && stagingUI.Staging.StartStaging != null)
            {
                staging = new Staging(stagingUI.Staging.StartStaging);
                WriteLog($"Worker is started at {staging.StartStaging}.{staging.StartStaging:fff}");
                if (_stagingWorkers == null)
                {
                    _stagingWorkers = new Dictionary<Staging, BackgroundWorker>();
                }
                _stagingWorkers.Add(staging, worker);
                await Task.Run(() => worker.RunWorkerAsync());
                stagingUI.Dispose();
            }
        }

        private void StagingUI_ReportClosed(StagingUIController obj)
        {
            WriteLog($"Report {obj.Staging!.StartStaging}.{obj.Staging.StartStaging:fff} closed.");
            _stagingsUI!.Remove(obj);
        }

        public async void CancelWorker()
        {
            if (_stagingWorkers == null || _stagingWorkers.Count == 0)
            {
                WriteLog($"Worker is not running at {DateTime.Now}!");
                return;
            }

            foreach (var sw in _stagingWorkers)
            {
                var key = sw.Key;
                var value = sw.Value;
                if (!key.Completed)
                {
                    await Task.Run(() => value.CancelAsync());
                }
                WriteLog($"Worker {key.StartStaging}.{key.StartStaging:fff} is canceling at {DateTime.Now}!");

                _stagingWorkers.Remove(sw.Key);
            }
        }

        public void WriteLog(string msg)
        {
            _log.Add(msg);
            _listLog.MoveDown();
        }
    }

    class StagingUIController : Window
    {
        private Label _label;
        private ListView _listView;
        private Button _start;
        private Button _close;
        public Staging? Staging { get; private set; }

        public event Action<StagingUIController>? ReportClosed;

        public StagingUIController(Staging staging, List<string> list) : this()
        {
            Staging = staging;
            _label.Text = "Work list:";
            _listView.SetSource(list);
            _listView.MoveEnd();
            _start.Visible = false;
            Id = "";
        }

        public StagingUIController()
        {
            X = Pos.Center();
            Y = Pos.Center();
            Width = Dim.Percent(85);
            Height = Dim.Percent(85);

            ColorScheme = Colors.Dialog;

            Title = "Run Worker";

            _label = new Label("Press start to do the work or close to exit.")
            {
                X = Pos.Center(),
                Y = 1,
                ColorScheme = Colors.Dialog
            };
            Add(_label);

            _listView = new ListView()
            {
                X = 0,
                Y = 2,
                Width = Dim.Fill(),
                Height = Dim.Fill(2)
            };
            Add(_listView);

            _start = new Button("Start") { IsDefault = true };
            _start.Clicked += () =>
            {
                Staging = new Staging(DateTime.Now);
                RequestStop();
            };
            Add(_start);

            _close = new Button("Close");
            _close.Clicked += OnReportClosed;
            Add(_close);

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == Key.Esc)
                {
                    OnReportClosed();
                }
            };

            LayoutStarted += (_) =>
            {
                var btnsWidth = _start.Bounds.Width + _close.Bounds.Width + 2 - 1;
                var shiftLeft = Math.Max((Bounds.Width - btnsWidth) / 2 - 2, 0);

                shiftLeft += _close.Bounds.Width + 1;
                _close.X = Pos.AnchorEnd(shiftLeft);
                _close.Y = Pos.AnchorEnd(1);

                shiftLeft += _start.Bounds.Width + 1;
                _start.X = Pos.AnchorEnd(shiftLeft);
                _start.Y = Pos.AnchorEnd(1);
            };
        }

        private void OnReportClosed()
        {
            if (Staging?.StartStaging != null)
            {
                ReportClosed?.Invoke(this);
            }
            RequestStop();
        }

        public async Task RunAsync()
        {
            await HaccExtensions.WebApplication.Run(this);
        }
    }

    class Staging
    {
        public DateTime? StartStaging { get; private set; }
        public bool Completed { get; }

        public Staging(DateTime? startStaging, bool completed = false)
        {
            StartStaging = startStaging;
            Completed = completed;
        }
    }
}