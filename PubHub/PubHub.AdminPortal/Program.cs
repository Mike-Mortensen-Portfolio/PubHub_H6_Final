using PubHub.AdminPortal.Components;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddPubHubServices(options =>
{
    options.Address = builder.Configuration.GetValue<string>(ApiConstants.API_ENDPOINT) ?? throw new NullReferenceException("API base address couldn't be found.");
    options.HttpClientName = ApiConstants.HTTPCLIENT_NAME;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
