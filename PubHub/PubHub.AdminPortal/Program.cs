using PubHub.AdminPortal.Components;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddPubHubServices(options =>
{
    string address = builder.Configuration.GetValue<string>(ApiConstants.API_ENDPOINT);
    if (address == null)
        throw new ArgumentNullException(nameof(address), "Api base address couldn't be found.");

    options.Address = address;
    options.HttpClientName = ApiConstants.HTTPCLIENT_NAME;
});

builder.Services.AddRadzenComponents();

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
