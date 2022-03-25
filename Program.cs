using HACC.Demo.Extensions;
using HACC.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

await WebAssemblyHostBuilder
    .CreateDefault(args: args)
    .UseBlazor()
    .UseHacc()
    .Build()
    .RunAsync();