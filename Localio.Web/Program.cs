using Localio.Web.Models;
using Localio.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Escuchar en 0.0.0.0 solo en Azure App Service Linux (cuando PORT o WEBSITES_PORT están definidos).
// En desarrollo local, Kestrel usa los puertos de launchSettings.json.
var azurePort = Environment.GetEnvironmentVariable("PORT")
                ?? Environment.GetEnvironmentVariable("WEBSITES_PORT");
if (!string.IsNullOrEmpty(azurePort))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{azurePort}");
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ISiteConfigService, SiteConfigService>();
builder.Services.Configure<AnalyticsOptions>(
    builder.Configuration.GetSection(AnalyticsOptions.SectionName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();