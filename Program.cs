using ChromaSampleUI.Components;
using ChromaSampleUI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ChromaService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var chromaUrl = config["ChromaDb:Url"] ?? "http://localhost:8000";
    return new ChromaService(chromaUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
