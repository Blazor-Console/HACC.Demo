using HACC.Demo;
using HACC.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args: args);
builder.RootComponents.Add<App>(selector: "#app");
builder.RootComponents.Add<HeadOutlet>(selector: "head::after");
builder.Services.AddScoped(implementationFactory: sp => new HttpClient
    {BaseAddress = new Uri(uriString: builder.HostEnvironment.BaseAddress)});
builder.UsingHacc();
await builder.Build().RunAsync();