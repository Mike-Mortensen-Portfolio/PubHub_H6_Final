using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain;
using PubHub.API.Domain.Identity;

namespace PubHub.API
{
    public static class ServiceExtensions
    {
        //  IServiceCollection extensions
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration, string connectionId)
        {
            return services.AddDbContext<PubHubContext>(options =>
            {
                options.EnableSensitiveDataLogging(true)
              .UseSqlServer(configuration.GetConnectionString(connectionId));
            });
        }
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<Account, IdentityRole<int>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<PubHubContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
