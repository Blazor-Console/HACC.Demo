using HACC.Extensions;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace HACC.Demo.Pages;

public partial class ConsoleDemo : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        HaccExtensions.WebApplication.Shutdown();
        HaccExtensions.WebApplication.Init();
        Application.Top.Add(view: new Label(text: "HACC Demo"));
        HaccExtensions.WebApplication.Run();
    }
}