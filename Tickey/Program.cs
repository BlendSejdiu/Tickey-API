using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Tickey.Endpoints;
using Tickey.Middleware;
using Tickey.Sevices;

var builder = WebApplication.CreateBuilder(args);

// metric, error, logging
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddLogging();

// db
builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("TickeyDB")!);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TickeyDB")));

//auth config
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
        ValidateIssuerSigningKey = true,
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("UserOnly", policy => policy.RequireRole("User"));

builder.Services.AddRateLimiter(options =>
{ 
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("Fixed", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromSeconds(30);     
    });

});

// services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

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

// ratelimiting middleware, aspire for metrics. Work more on events and ticket models. Add tests and more methods.

app.Run();