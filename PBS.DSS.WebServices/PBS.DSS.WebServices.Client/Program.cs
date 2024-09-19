using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudExtensions.Services;
using Blazored.SessionStorage;
using PBS.DSS.Shared.Services;
using PBS.DSS.WebServices.Client.Pages;
using PBS.DSS.WebServices.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddMudExtensions();
builder.Services.AddLocalization();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<InterOpService>();
builder.Services.AddScoped<SessionStorageService>();

var host = builder.Build();

var sessionStorageService = host.Services.GetRequiredService<SessionStorageService>();
var culture = await sessionStorageService.GetSessionCulture();

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();