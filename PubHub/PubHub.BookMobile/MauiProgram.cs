﻿using System.Reflection;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using PubHub.BookMobile.Auth;
using PubHub.BookMobile.Extensions;
using PubHub.BookMobile.Services;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;
using PubHub.Common.Models.Authentication;

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
                .AddSingleton(AudioManager.Current)
                .AddPubHubServices(options =>
                {
                    options.Address = builder.Configuration.GetSection("ApiSettings").GetValue<string>(ApiConstants.API_ENDPOINT) ?? throw new NullReferenceException("API base address couldn't be found.");
                    options.HttpClientName = ApiConstants.HTTPCLIENT_NAME;
                    options.AppId = builder.Configuration.GetSection("ApiSettings").GetValue<string>(ApiConstants.APP_ID) ?? throw new NullReferenceException("Application ID couldn't be found.");
                    options.ConfigureForMobile = true;
                    options.GetTokenInfoAsync = async (provider) =>
                    {
                        (bool IsSuccess, TokenInfo? tokens) = await User.TryGetCachedTokenAsync();
                        if (!IsSuccess)
                            return new TokenInfo { RefreshToken = string.Empty, Token = string.Empty };

                        return tokens!;
                    };
                    options.SetTokenInfo = async (provider, tokenInfo) =>
                    {
                        await User.SetAsync(new Common.Models.Accounts.TokenResponseModel(tokenInfo.Token, tokenInfo.RefreshToken));
                    };
                    options.RemoveTokenInfo = (provider) =>
                    {
                        User.Unset();
                    };
                });

            MauiHandlerService.AppendCustomMapping();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
