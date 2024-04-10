using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using System.Reflection;
using PubHub.BookMobile.Extensions;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;
using PubHub.BookMobile.Auth;
using PubHub.Common.Models.Authentication;
using Android.Content.Res;
using Microsoft.Maui.Controls.PlatformConfiguration;
using static Java.Text.Normalizer;

namespace PubHub.BookMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            var services = builder.Services;

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .LoadAppSettingsConfiguration(Assembly.GetExecutingAssembly())
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("icofont.ttf", "IcoFont");
                });

            services
                .AddRoutes()
                .AddPubHubServices(options =>
                {
                    options.Address = builder.Configuration.GetSection("ApiSettings").GetValue<string>(ApiConstants.API_ENDPOINT) ?? throw new NullReferenceException("API base address couldn't be found.");
                    options.HttpClientName = ApiConstants.HTTPCLIENT_NAME;
                    options.AppId = builder.Configuration.GetSection("ApiSettings").GetValue<string>(ApiConstants.APP_ID) ?? throw new NullReferenceException("Application ID couldn't be found.");
                    options.ConfigureForMobile = true;
                    options.TokenInfoAsync = (provider) =>
                    {
                        if (User.TryGetCachedToken(out TokenInfo? result))
                            return Task.FromResult(User.GetChachedToken());
                        return Task.FromResult(new TokenInfo { RefreshToken = string.Empty, Token = string.Empty });
                    };
                });

            //  Remove underline from entries on Android
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) => handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent));

#if DEBUG
            builder.Logging.AddDebug();
#endif


            return builder.Build();
        }
    }
}
