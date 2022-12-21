using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Mockdoor.Data.Sqlite.Services;
using Mockdoor.Data.SqlServer.Services;
using MockDoor.Abstractions.ConfigurationServices;
using MockDoor.Abstractions.MockServices;
using MockDoor.Abstractions.ProxyServices;
using MockDoor.Abstractions.Repositories;
using MockDoor.Api.Controllers.AdminControllers;
using MockDoor.Data.Contexts;
using MockDoor.Data.Repositories;
using MockDoor.Server.Constants;
using MockDoor.Server.Services;
using MockDoor.Services.Hubs;
using MockDoor.Services.MockServices;
using MockDoor.Services.ProxyServices;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Models.Configuration;
using MockDoor.Shared.Models.Utility;
using Newtonsoft.Json;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
// fix poorly documented behaviour where any environment other than development will not load web assets
builder.WebHost.ConfigureAppConfiguration((ctx, _) => {
    // https://github.com/dotnet/aspnetcore/blob/main/src/DefaultBuilder/src/WebHost.cs#L219
    if (!ctx.HostingEnvironment.IsEnvironment("Development")) {
        // This call inserts "StaticWebAssetsFileProvider" into the static file middleware
        StaticWebAssetsLoader.UseStaticWebAssets(ctx.HostingEnvironment, ctx.Configuration);
    }
});
#endif

builder.Host.ConfigureAppConfiguration((_, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Secrets.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();
});

var deploymentConfiguration =
    builder.Configuration.GetSection("DeploymentConfiguration").Get<DeploymentConfiguration>();

builder.Services.Configure<DeploymentConfiguration>(builder.Configuration.GetSection("DeploymentConfiguration"));

Console.WriteLine($"version: {SharedConstants.MockdoorVersion}");

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

switch (deploymentConfiguration.DatabaseConfig.Provider)
{
    case DatabaseProvider.Sqlite:
        {
            builder.Services.AddDbContext<MockDoorMainContext>(options => options.UseSqlite(deploymentConfiguration.DatabaseConfig.ConnectionString, b => b.MigrationsAssembly("Mockdoor.Data.Sqlite")));
            builder.Services.AddScoped<IDatabaseConfigurationService, SqliteDatabaseConnectionService>();
        } break;
    case DatabaseProvider.SqlServer:
        {
            builder.Services.AddDbContext<MockDoorMainContext>(options => options.UseSqlServer(deploymentConfiguration.DatabaseConfig.ConnectionString, b => b.MigrationsAssembly("Mockdoor.Data.SqlServer")));
            builder.Services.AddScoped<IDatabaseConfigurationService, SqlServerDatabaseConfigurationService>();
        } break;
    default: throw new ArgumentException("Invalid database provider set");
}

builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IServiceGroupRepository, ServiceGroupRepository>();
builder.Services.AddScoped<IMicroserviceRepository, MicroserviceRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IMockResponseRepository, MockResponseRepository>();

builder.Services.AddScoped<IMockService, MockService>();
builder.Services.AddScoped<ISimulateTimeService, SimulateTimeService>();
builder.Services.AddScoped<IProxyService, ProxyService>();
builder.Services.AddScoped<IHttpService, HttpService>();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddMvc()
    .AddApplicationPart(typeof(ConfigurationController).Assembly)
    .AddControllersAsServices()
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(deploymentConfiguration.SeedOnStartup))
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var baseRepository = services.GetRequiredService<IBaseRepository>();
        var seedJson = File.ReadAllText(deploymentConfiguration.SeedOnStartup);

        var databaseToSeed = JsonConvert.DeserializeObject<FullDatabaseDto>(seedJson);

        var databaseConfigurationService = services.GetRequiredService<IDatabaseConfigurationService>();

        var databaseStatus = databaseConfigurationService
            .TestConnectionStringWorkAsync(deploymentConfiguration.DatabaseConfig.ConnectionString).GetAwaiter().GetResult();

        if (databaseStatus.ConnectionStringStatus == ConnectionStringStatus.ConnectNoDatabase || databaseStatus.PendingMigrations?.Count() > 0)
        {
            databaseConfigurationService.ApplyMigrationsAsync(deploymentConfiguration.DatabaseConfig.ConnectionString).GetAwaiter().GetResult();
        }
        
        baseRepository.ImportDatabase(databaseToSeed, true);
    }
}

app.Use(async (context, next) =>
{
    //enable buffering of the request to enable multiple reads of request Bodies
    context.Request.EnableBuffering();

    await next();
});

if (deploymentConfiguration.PathBase != null)
{
    app.UsePathBase(deploymentConfiguration.PathBase);
    app.Use((context, next) =>
    {
        context.Request.PathBase = new PathString($"/{deploymentConfiguration.PathBase.TrimStart('/')}");
        return next();
    });
    app.Use(async (context, next) =>
    {
        context.Response.Headers.TryAdd(HttpConstants.CustomBasePathHeaderKey, deploymentConfiguration.PathBase);

        await next();
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

    if (deploymentConfiguration.ForceHttps)
    {
        app.UseHsts();
    }
}

var basePath = deploymentConfiguration.PathBase ?? "/";
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        if (!httpReq.Headers.ContainsKey("X-Original-Host"))
            return;

        var serverUrl = $"{httpReq.Headers["X-Original-Proto"]}://" + $"{httpReq.Headers["X-Original-Host"]}/" + $"{httpReq.Headers["X-Original-Prefix"]}";

        Console.WriteLine(serverUrl);
            
        swaggerDoc.Servers = new List<OpenApiServer>()
        {
            new OpenApiServer { Url = serverUrl }
        };
    });
});
app.UseSwaggerUI(c => c.SwaggerEndpoint($"{basePath.TrimEnd('/')}/swagger/v1/swagger.json", "Mockdoor API v0.50.03"));

if (deploymentConfiguration.ForceHttps)
{
    app.UseHttpsRedirection();
}

app.UseCors(cpb => cpb.AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<RequestHub>("/requesthub");
app.MapFallbackToFile("index.html");

app.Run();
