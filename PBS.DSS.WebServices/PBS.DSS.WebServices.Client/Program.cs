using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudExtensions.Services;
using System.Globalization;
using PBS.DSS.Shared.Services;
using PBS.DSS.WebServices.Client.Pages;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddMudExtensions();
builder.Services.AddLocalization();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<InterOpService>();

var host = builder.Build();

var interOpService = host.Services.GetRequiredService<InterOpService>();
var culture = new CultureInfo("en-CA");
var cultureString = await interOpService.GetCulture();

if (!string.IsNullOrEmpty(cultureString))
    culture = new CultureInfo(cultureString);

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();