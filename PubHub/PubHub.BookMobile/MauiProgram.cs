using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;

namespace PubHub.BookMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("icofont.ttf", "IcoFont");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddPubHubServices(options =>
            {
                // TODO (SIA): Use appsettings.json
                options.Address = "https://localhost:7097/{0}";//builder.Configuration.GetValue<string>(ApiConstants.API_ENDPOINT) ?? throw new NullReferenceException("API base address couldn't be found.");
                options.HttpClientName = ApiConstants.HTTPCLIENT_NAME;
                options.ConfigureForMobile = true;
            });

            return builder.Build();
        }
    }
}
