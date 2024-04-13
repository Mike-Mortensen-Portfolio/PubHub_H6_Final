using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using PubHub.AdminPortal.Components;
using PubHub.AdminPortal.Components.Auth;
using PubHub.AdminPortal.Components.Helpers;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;
using PubHub.Common.Helpers;
using PubHub.Common.Models.Authentication;
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
    options.AppId = builder.Configuration.GetValue<string>(ApiConstants.APP_ID) ?? throw new NullReferenceException("Application ID couldn't be found.");
    options.TokenInfoAsync = async (sp) =>
    {
        var localStorageService = sp.GetRequiredService<ILocalStorageService>();
        return new TokenInfo()
        {
            Token = await localStorageService.GetItemAsync<string>("token") ?? string.Empty,
            RefreshToken = await localStorageService.GetItemAsync<string>("refreshToken") ?? string.Empty
        };
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Operator", policy => policy.Requirements.Add(new CustomClaimRequirement("accountType", builder.Configuration.GetValue<string>("Operator") ?? throw new NullReferenceException("Operator couldn't be found."))));
    options.AddPolicy("Publisher", policy => policy.Requirements.Add(new CustomClaimRequirement("accountType", builder.Configuration.GetValue<string>("Publisher") ?? throw new NullReferenceException("Publisher couldn't be found."))));
});

builder.Services.AddSingleton<IAuthorizationHandler, CustomClaimRequirementHandler>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddHsts(options =>
{ 
    options.MaxAge = TimeSpan.FromDays(365);    // Long-term security assurance and reduced vulnerability window.
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
