using System;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using PubHub.BookMobile.Pages;
using PubHub.BookMobile.ViewModels;

namespace PubHub.BookMobile.Extensions
{
    internal static class ServiceExtensions
    {
        /// <summary>
        /// Registeres all the routes used within the application
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The <see cref="IServiceCollection"/> used with this call</returns>
        public static IServiceCollection AddRoutes(this IServiceCollection services)
        {
            services
                .AddSingletonWithShellRoute<Home, HomeViewModel>("Home")

                //  TODO (MSM): Re-add missing class???
                //.AddTransientWithShellRoute<Login, LoginViewModel>("Login")
                .AddTransientWithShellRoute<Library, LibraryViewModel>("Library")
                .AddTransientWithShellRoute<BookInfo, BookInfoViewModel>("BookInfo");

            return services;
        }

        public static IServiceCollection AddSingletonWithShellRoute<TView>(this IServiceCollection services, string route, RouteFactory factory = null!) where TView : NavigableElement
        {
            services.AddSingleton<TView>();

            RegisterShellRoute<TView>(route, factory);

            return services;
        }
        public static IServiceCollection AddSingletonWithShellRoute<TView>(this IServiceCollection services, string route, Func<IServiceProvider, TView> implementationFactory, RouteFactory factory = null!) where TView : NavigableElement
        {
            services.AddSingleton(implementationFactory);

            RegisterShellRoute<TView>(route, factory);

            return services;
        }

        public static IServiceCollection AddTransientWithShellRoute<TView>(this IServiceCollection services, string route, RouteFactory factory = null!) where TView : NavigableElement
        {
            services.AddTransient<TView>();

            RegisterShellRoute<TView>(route, factory);

            return services;
        }
        public static IServiceCollection AddTransientWithShellRoute<TView>(this IServiceCollection services, string route, Func<IServiceProvider, TView> implementationFactory, RouteFactory factory = null!) where TView : NavigableElement
        {
            services.AddTransient<TView>(implementationFactory);

            RegisterShellRoute<TView>(route, factory);

            return services;
        }

        ///// <summary>
        ///// Add <typeparamref name="TService"/> configured with <typeparamref name="TSettings"/> to the DI container as a <strong>singleton</strong>
        ///// </summary>
        ///// <typeparam name="TService"></typeparam>
        ///// <typeparam name="TSettings"></typeparam>
        ///// <param name="services"></param>
        ///// <returns></returns>
        //public static IServiceCollection AddSingletonServiceWithSettings<TService, TSettings>(this IServiceCollection services) where TService : ServiceBase where TSettings : IServiceSettings
        //{
        //    return services.AddSingleton(provider =>
        //    {
        //        var appSettings = provider.GetRequiredService<TSettings>();

        //        var service = Activator.CreateInstance(typeof(TService), appSettings) as TService;
        //        return service;
        //    });
        //}

        private static void RegisterShellRoute<TView>(string route, RouteFactory factory = null!) where TView : NavigableElement
        {
            if (factory is null)
            {
                Routing.RegisterRoute(route, typeof(TView));
            }
            else
            {
                Routing.RegisterRoute(route, factory);
            }
        }
    }
}
