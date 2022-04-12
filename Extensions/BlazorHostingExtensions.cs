using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HACC.Demo.Extensions;

public static class BlazorHostingExtensions
{
    public static WebAssemblyHostBuilder UseBlazor(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>(selector: "#app");
        builder.RootComponents.Add<HeadOutlet>(selector: "head::after");
        builder.Services.AddScoped(implementationFactory: sp => new HttpClient
            {BaseAddress = new Uri(uriString: builder.HostEnvironment.BaseAddress)});
        return builder;
    }
}