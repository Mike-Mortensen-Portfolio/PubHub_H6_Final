using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using System.Reflection;
using PubHub.BookMobile.Extensions;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;

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
                    options.ConfigureForMobile = true;
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif


            return builder.Build();
        }
    }
}
