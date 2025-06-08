using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FitnessVibe.Infrastructure.Data;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Infrastructure.Data.Repositories;
using FitnessVibe.Application.Services;
using FitnessVibe.Infrastructure.Services;
using MediatR;
using FluentValidation;
using Serilog;
using System.Reflection;

namespace FitnessVibe.API
{
    /// <summary>
    /// Main application startup and configuration.
    /// Think of this as the master control center that sets up all the systems
    /// and services needed to run our fitness center's digital infrastructure.
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure Serilog for structured logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/fitnessvibe-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting FitnessVibe API server");
                
                var builder = WebApplication.CreateBuilder(args);
                
                // Configure services
                ConfigureServices(builder);
                
                var app = builder.Build();
                
                // Configure middleware pipeline
                await ConfigureMiddleware(app);
                
                // Ensure database is created and seeded
                await EnsureDatabaseSetup(app);
                
                Log.Information("FitnessVibe API server started successfully");
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "FitnessVibe API server failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Configure all application services.
        /// Like setting up all the different departments and staff in our fitness center.
        /// </summary>
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            // Add Serilog
            builder.Host.UseSerilog();

            // Database Configuration
            services.AddDbContext<FitnessVibeDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("FitnessVibe.Infrastructure")
                )
            );

            // Repository Pattern
            services.AddScoped<IUserRepository, UserRepository>();

            // Application Services
            services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            services.AddScoped<ITokenService, TokenService>();

            // MediatR for CQRS
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(FitnessVibe.Application.Commands.Users.RegisterUserCommand))!);
            });

            // FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(FitnessVibe.Application.Commands.Users.RegisterUserCommand))!);

            // AutoMapper
            services.AddAutoMapper(Assembly.GetAssembly(typeof(FitnessVibe.Application.Commands.Users.RegisterUserCommand))!);

            // JWT Authentication
            ConfigureAuthentication(services, configuration);

            // API Controllers
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // API Documentation
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { 
                    Title = "FitnessVibe API", 
                    Version = "v1",
                    Description = "API for the FitnessVibe fitness tracking application"
                });
                
                // Include XML comments for better documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // JWT Authentication in Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // CORS Configuration
            services.AddCors(options =>
            {
                options.AddPolicy("FitnessVibePolicy", builder =>
                {
                    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? 
                                        new[] { "http://localhost:4200" };
                    
                    builder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            // Health Checks
            services.AddHealthChecks()
                .AddDbContext<FitnessVibeDbContext>("database");

            // Response Compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Rate Limiting (basic implementation)
            services.AddMemoryCache();
        }

        /// <summary>
        /// Configure JWT Authentication.
        /// Like setting up the digital security system for our fitness center.
        /// </summary>
        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Log.Debug("JWT Token validated for user: {User}", context.Principal?.Identity?.Name);
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();
        }

        /// <summary>
        /// Configure the middleware pipeline.
        /// Like setting up the flow of operations in our fitness center.
        /// </summary>
        private static async Task ConfigureMiddleware(WebApplication app)
        {
            // Exception handling (should be first)
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FitnessVibe API v1");
                    c.RoutePrefix = "docs"; // Access Swagger at /docs instead of /swagger
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // Security headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                await next();
            });

            // HTTPS Redirection
            app.UseHttpsRedirection();

            // Response Compression
            app.UseResponseCompression();

            // CORS
            app.UseCors("FitnessVibePolicy");

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Request logging
            app.UseSerilogRequestLogging();

            // Health checks
            app.UseHealthChecks("/health");

            // API Routes
            app.MapControllers();

            // Default route for SPA
            app.MapFallbackToFile("index.html");
        }

        /// <summary>
        /// Ensure database is created and seeded with initial data.
        /// Like setting up the gym with all the initial equipment and memberships.
        /// </summary>
        private static async Task EnsureDatabaseSetup(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FitnessVibeDbContext>();
            
            try
            {
                Log.Information("Ensuring database is created and up to date");
                await context.Database.MigrateAsync();
                
                Log.Information("Database setup completed successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to setup database");
                throw;
            }
        }
    }
}
