using Localio.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Escuchar en 0.0.0.0 para Azure App Service Linux.
// El puerto se resuelve en orden: PORT → WEBSITES_PORT → 8080.
var port = Environment.GetEnvironmentVariable("PORT")
           ?? Environment.GetEnvironmentVariable("WEBSITES_PORT")
           ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ISiteConfigService, SiteConfigService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
