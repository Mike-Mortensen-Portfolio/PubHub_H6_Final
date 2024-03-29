using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Identity;

namespace PubHub.API
{
    public static class ServiceExtensions
    {
        // IServiceCollection extensions
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
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<PubHubContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, params OpenApiInfo[] docs)
        {
            return services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = $"Jwt authorization using '<i>Bearer</i>'heading sheme. Enter '<i>Bearer [Space] [Token]</i>' below.<br/><strong>Example:</strong> Bearer 123456abcdef",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "0auth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                foreach (var doc in docs)
                {
                    options.SwaggerDoc(doc.Version, doc);
                }

                // Uncommment to include XML documentation (Remember to add XML Doc file in Properties > Build > Output > Documentation File)
                //var xmlDocPath = Path.Combine(AppContext.BaseDirectory, "PubHub.API.xml");
                //options.IncludeXmlComments(xmlDocPath);
            });
        }

        public static IServiceCollection ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
            });

            return services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
        }

        // ApplicationBuilder extensions
        /// <summary>
        /// Configures a default exception response for unhandled exceptions that adheres to the <see href="https://datatracker.ietf.org/doc/html/rfc9110">RFC9110</see> standard
        /// </summary>
        /// <param name="app"></param>
        /// <returns>The <see cref="IApplicationBuilder"/> that was used to configure this handler</returns>
        public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/problem+json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        //Log.Error($"Something went wrong: {contextFeature.Error}");
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            type = "https://datatracker.ietf.org/doc/html/rfc9110#name-500-internal-server-error",
                            title = InternalServerErrorSpecification.TITLE,
                            status = InternalServerErrorSpecification.STATUS_CODE,
                            detail = "We didn't expect this to happen. Sorry. Please try again, or contact PubHub support if the problem perists."
                        }));
                    }
                });
            });
        }

        public static IApplicationBuilder ConfigureSwaggerUI(this IApplicationBuilder app)
        {
            var apiVersionDescriptionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger();
            return app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }
    }
}
