using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace PubHub.BookMobile
{
    internal static class BuilderExtensions
    {
        /// <summary>
        /// Imports the <i>appsettings.json</i> file and adds it to the <see cref="IConfiguration"/> collection
        /// <br/>
        /// <br/>
        /// <strong>Note:</strong> This will attempt to load an <i>appsettings.json</i> file relative to the current environment configuration set in visual studio.
        /// <br/>
        /// This means that <strong>Release</strong> will load <i>appsettings.json</i> while <strong>Demo</strong> will load <i>appsettings.demo.json</i>
        /// and <strong>Debug</strong> will load <i>appsettings.development.json</i>
        /// <br/>
        /// If a Demo/Debug file is not found this will fall back to loading <i>appsettings.json</i>
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="executingAssembly">The assembly that executes the code (<i>This would often be the assembly that contains the program.cs file</i>)</param>
        /// <returns></returns>
        public static MauiAppBuilder LoadAppSettingsConfiguration(this MauiAppBuilder appBuilder, Assembly executingAssembly)
        {
            var shortAssemblyName = executingAssembly!.FullName!.Split(',')[0];
            var productionSettings = $"{shortAssemblyName}.appsettings.json";
#if DEBUG
            var appsettingsPath = $"{shortAssemblyName}.appsettings.development.json";
#elif DEMO
            var appsettingsPath = $"{shortAssemblyName}.appsettings.demo.json";
#else
            var appsettingsPath = productionSettings;
#endif

            var stream = executingAssembly.GetManifestResourceStream(appsettingsPath);

            if (stream is null)
            {
#if DEBUG || DEMO
                appsettingsPath = productionSettings;
                stream = executingAssembly.GetManifestResourceStream(appsettingsPath);

                if (stream is null)
#endif

                    throw new FileNotFoundException("No appsettings.json file was found. Did you forget to mark it as an EmbeddedRessource?");
            }

            using var resStream = stream;

            var config = new ConfigurationBuilder()
                .AddJsonStream(resStream)
                .Build();

            appBuilder.Configuration.AddConfiguration(config);

            return appBuilder;
        }
    }
}
