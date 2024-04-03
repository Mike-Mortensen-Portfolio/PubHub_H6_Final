using System.Reflection;
using Serilog;
using Serilog.Events;

namespace PubHub.API
{
    public static class WebApplicationBuilderExtensions
    {
        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            string assembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "INVALID PATH";
            string logPath = Path.Combine(assembly, @"Logs\log-.txt");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    path: logPath,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information
                 )
                .WriteTo.Console(
#if DEBUG
                    restrictedToMinimumLevel: LogEventLevel.Debug
#else
                    restrictedToMinimumLevel: LogEventLevel.Information
#endif
                 )
                .CreateLogger();

            builder.Host.UseSerilog();
            builder.Logging.ClearProviders();
        }
    }
}
