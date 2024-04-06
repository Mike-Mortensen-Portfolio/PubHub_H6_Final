﻿using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using PubHub.API;
using PubHub.API.Controllers.Problems;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

// Add services to the container.
builder.Services.ConfigureDatabase(builder.Configuration, "Local");
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger(new OpenApiInfo { Title = "PubHub API v1", Version = "v1" });

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.ConfigureSwaggerUI();
app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
