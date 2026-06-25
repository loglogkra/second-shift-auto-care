using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SecondShiftAutoCare.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices(config =>
{
    // The app only declares one MudPopoverProvider, but prerender/reload edge cases can
    // cause MudBlazor to detect a transient duplicate and crash the WebAssembly UI.
    config.PopoverOptions.ThrowOnDuplicateProvider = false;
});

await builder.Build().RunAsync();
