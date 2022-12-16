using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MockDoor.Client;
using MockDoor.Client.Constants;
using MockDoor.Client.Services;
using MockDoor.Client.State;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

Console.WriteLine("builder.HostEnvironment.BaseAddress: " + builder.HostEnvironment.BaseAddress);

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddScoped<ConfigurationService>();
builder.Services.AddScoped<MicroserviceService>();
builder.Services.AddScoped<MockTestingService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<ResponseService>();
builder.Services.AddScoped<ServiceGroupService>();
builder.Services.AddScoped<SetupWizardService>();
builder.Services.AddScoped<SimulateTimeService>();
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<UtilityService>();

builder.Services.AddSingleton<PageHistoryState>();


builder.Services.AddBlazoredLocalStorage();

//set base path dynamically

Environment.SetEnvironmentVariable(ConfigurationConstants.BasePathKey,
    new Uri(builder.HostEnvironment.BaseAddress).AbsolutePath);

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();