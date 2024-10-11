using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.SessionStorage;
using PBS.Blazor.ClientFramework.Services;
using PBS.Blazor.ClientFramework.Extensions;
using PBS.DSS.WebServices.Client.Pages.App;
using PBS.DSS.WebServices.Client.Services;
using PBS.DSS.Shared.Models.States;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddLocalization();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddSharedState<SharedState>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<InterOpService>();
builder.Services.AddScoped<SessionStorageService>();
builder.Services.AddScoped<NavigationManagerService>();
builder.Services.AddScoped<ControllerAPIService>();
builder.Services.AddScoped<DocumentSignatureService>();

var host = builder.Build();

var sessionStorageService = host.Services.GetRequiredService<SessionStorageService>();
var culture = await sessionStorageService.GetSessionCulture();

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();