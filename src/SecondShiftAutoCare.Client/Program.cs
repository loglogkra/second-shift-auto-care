using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SecondShiftAutoCare.Client;
using SecondShiftAutoCare.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    var baseAddress = builder.HostEnvironment.IsDevelopment()
        ? new Uri("http://localhost:7071/")
        : new Uri(builder.HostEnvironment.BaseAddress);

    return new HttpClient { BaseAddress = baseAddress };
});

builder.Services.AddScoped<ServiceRequestClient>();
builder.Services.AddScoped<StaticWebAppsAuthClient>();

builder.Services.AddMudServices(config =>
{
    // The app only declares one MudPopoverProvider, but prerender/reload edge cases can
    // cause MudBlazor to detect a transient duplicate and crash the WebAssembly UI.
    config.PopoverOptions.ThrowOnDuplicateProvider = false;
});


await builder.Build().RunAsync();
