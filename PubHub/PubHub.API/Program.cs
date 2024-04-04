using Microsoft.OpenApi.Models;
using PubHub.API;
using System.Text.Json.Serialization;

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
