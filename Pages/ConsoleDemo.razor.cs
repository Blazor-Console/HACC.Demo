using HACC.Applications;
using HACC.Extensions;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleDemo : ComponentBase
{
    public static readonly WebApplication WebApplication = HaccExtensions.GetService<WebApplication>();


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        WebApplication.Shutdown();
        WebApplication.Init();
        Application.Top.Add(view: new Label(text: "HACC Demo"));
        WebApplication.Run();
    }
}