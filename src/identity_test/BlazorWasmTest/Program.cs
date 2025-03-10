using BlazorWasmTest;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(opts =>
{
    opts.ProviderOptions.Authority = "http://localhost:5000";
    opts.ProviderOptions.ClientId = "blazor_wasm_test";
    opts.ProviderOptions.ResponseType = "token";

    opts.AuthenticationPaths.LogInCallbackPath = "/oidc/callback";
});

await builder.Build().RunAsync();
