using CommunityToolkit.Maui;
using PubHub.BookMobile.Views;
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
                .AddTransient<Home, HomeViewModel>()

                .AddTransientWithShellRoute<Login, LoginViewModel>("Login")
                .AddTransientWithShellRoute<Logout>("Logout")
                .AddTransientWithShellRoute<Register, RegisterViewModel>("Register")
                .AddTransientWithShellRoute<Library, LibraryViewModel>("Library")
                .AddTransientWithShellRoute<PersonalLibrary, PersonalLibraryViewModel>("PersonalLibrary")
                .AddTransientWithShellRoute<BookInfo, BookInfoViewModel>("BookInfo")
                .AddTransientWithShellRoute<Profile, ProfileViewModel>("Profile");

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
