using System.Runtime.Versioning;
using HACC.Applications;
using HACC.Demo.Logging;
using Microsoft.AspNetCore.Components;

namespace HACC.Demo.Pages;
public partial class ConsoleDemo : ComponentBase
{

    [Inject] public WebApplication WebApplication { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        this.WebApplication.Shutdown();
        this.WebApplication.Init();
    }
}
