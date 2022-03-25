using HACC.Applications;
using HACC.Extensions;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleDemo : ComponentBase
{
    public readonly WebApplication WebApplication;

    public ConsoleDemo()
    {
        this.WebApplication = HaccExtensions.GetService<WebApplication>();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        this.WebApplication.Shutdown();
        this.WebApplication.Init();
        Application.Top.Add(view: new Label(text: "HACC Demo"));
        this.WebApplication.Run();
    }
}