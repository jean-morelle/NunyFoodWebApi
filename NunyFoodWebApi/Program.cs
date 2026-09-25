using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NunyFoodWebApi.Application;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Infrastructure;
using NunyFoodWebApi.Infrastructure.Security;
using NunyFoodWebApi.Middleware;
using NunyFoodWebApi.RateLimiting;
using NunyFoodWebApi.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext());

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUser, CurrentUser>();

    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
        ?? throw new InvalidOperationException("JWT settings not configured.");

    // Le secret ne doit jamais être dans appsettings.json (versionné) : user-secrets en dev,
    // variable d'environnement Jwt__Secret en production.
    if (Encoding.UTF8.GetByteCount(jwtSettings.Secret) < 32)
        throw new InvalidOperationException(
            "Jwt:Secret manquant ou trop court (32 octets minimum). En développement : " +
            "dotnet user-secrets set \"Jwt:Secret\" \"<valeur aléatoire>\" --project NunyFoodWebApi");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // Garder les noms de claims du JWT tels quels ("sub", "role") au lieu de les
            // convertir en URI ClaimTypes.*, sinon RoleClaimType = "role" ne trouve aucun rôle.
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                RoleClaimType = "role"
            };
        });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("NunyFoodFrontend", policy =>
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173",
                           "http://localhost:5174", "https://localhost:5174")
                  .AllowAnyHeader()
                  .AllowAnyMethod());
    });

    builder.Services.AddNunyFoodRateLimiting(builder.Configuration);

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Enums en texte ("Paid", "PayPal") : c'est ce qu'attend le frontend. Les nombres restent acceptés en entrée.
    builder.Services.AddControllers()
        .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "NunyFood API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Entrez votre token JWT (sans le préfixe Bearer)"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                []
            }
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "NunyFood API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseCors("NunyFoodFrontend");
    // Les preuves de livraison ne sont pas servies en fichiers statiques : voir GET /api/deliveries/{id}/photo.
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRateLimiter();
    app.MapControllers();
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
