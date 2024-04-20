﻿using System.Globalization;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PubHub.API;
using PubHub.API.Controllers.Problems;
using PubHub.Common;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

// Add services to the container.
builder.Services.ConfigureDatabase(builder.Configuration, "Local");
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity(builder.Configuration);
builder.Services.ConfigureJwt();
builder.Services.ConfigureCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger(new OpenApiInfo { Title = "PubHub API v1", Version = "v1" });

builder.Services.AddRateLimiter(rateLimterOptions =>
{
    rateLimterOptions.AddPolicy("limit-by-app-id", context =>
    {
        var appId = context.Request.Headers["appId"].ToString();
        return RateLimitPartition.GetTokenBucketLimiter(appId, factory: _ => new TokenBucketRateLimiterOptions
        {
            AutoReplenishment = true,
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
            TokenLimit = 1,
            TokensPerPeriod = 1
        });
    });

    rateLimterOptions.AddPolicy("limit-by-consumer-id", context =>
    {
        var consumerId = (context.User.Identity?.Name) ?? throw new Exception("No identity found");
        return RateLimitPartition.GetConcurrencyLimiter(consumerId,
            factory: _ => new ConcurrencyLimiterOptions
            {
                PermitLimit = 5,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 1
            });
    });

    rateLimterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());    //  Allowing enums to be displayed and used as string values
    });

builder.Services.ConfigureVersioning();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var problemDetails = new ValidationProblemDetails(actionContext.ModelState);

        var result = new BadRequestObjectResult(new
        {
            Status = ValidationProblemSpecification.STATUS_CODE,
            Title = ValidationProblemSpecification.TITLE,
            Type = ValidationProblemSpecification.TYPE,
            Extensions = problemDetails.Errors.ToDictionary()
        });
        result.ContentTypes.Add("application/problem+json");

        return result;
    };
});

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);    // Long-term security assurance and reduced vulnerability window.
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Enable HSTS middleware for non-development environments
}

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.ConfigureSwaggerUI();
app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
