using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using Tickey.Endpoints;
using Tickey.Middleware;
using Tickey.Sevices;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// metric, logging
builder.Services.AddErrorHandlingAndLogs();

builder.Services.AddOpenApi();

// db
builder.Services.AddDatabase(builder.Configuration);

//auth 
builder.Services.AddAuth(builder.Configuration);

//rate limiting
builder.Services.AddRateLimiting();

// services
builder.Services.DependentServices();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// endpoints
app.MapTicketEndpoints();
app.MapEventEndpoints();
app.MapAuthEndpoints();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

//  aspire for metrics. Work more on events and ticket models. Add tests and more methods.

app.Run();