using MudBlazor.Services;
using PBS.Blazor.ServerFramework;
using PBS.Blazor.ServerFramework.Integrations;
using DSSUtilities = PBS.DSS.WebServices.Server.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

PBS.DataAccess.Core.ConfigurationManager.GetConnectionString = DSSUtilities.Utility.DBConnectionString;

var log = new Activity("", "ServerStarted");
log.LogMessage("PBS DSS Server Started");
log.LogMessage($"Connect Hub Message ID: {ConnectHubIntegration.GetMessageId()}");
log.Update();

app.Run();