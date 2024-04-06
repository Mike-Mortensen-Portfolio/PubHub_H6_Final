using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using PubHub.AdminPortal.Components;
using PubHub.AdminPortal.Components.Auth;
using PubHub.AdminPortal.Components.Helpers;
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
    options.Address = builder.Configuration.GetValue<string>(ApiConstants.API_ENDPOINT) ?? throw new NullReferenceException("API base address couldn't be found.");
    options.HttpClientName = ApiConstants.HTTPCLIENT_NAME;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Operator", policy => policy.Requirements.Add(new CustomClaimRequirement("accountType", "63A74B4F-C3A5-8267-87E9-018EB4E51304")));
    options.AddPolicy("Publisher", policy => policy.Requirements.Add(new CustomClaimRequirement("accountType", "B6008023-AD79-8F03-87E8-018EB4E51304")));
});
    

builder.Services.AddSingleton<IAuthorizationHandler, CustomClaimRequirementHandler>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<FileHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
