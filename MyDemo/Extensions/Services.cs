using Domain.Interface;
using Entity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyDemo.DAL;
using Repository.Repository;
using Repository.Services;
using System.Text;

namespace MyDemo.Extensions
{
    public static class Services
    {
        public static void ConfigureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Dependency Injection
            services.AddScoped<IAuthentication, AuthenticationRepo>()
                    .AddScoped<AuthenticationDAL>()
                    .AddScoped<Service>()
                    .AddScoped<JWTToken>()
                    .AddDataProtection();

            services.AddHttpContextAccessor();

            // Database Context
            services.AddDbContext<nEdit_DEVContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DbConnString")));

            // JWT Authentication
            ConfigureAuthentication(services, configuration);

            // Swagger Configuration
            ConfigureSwagger(services);

            // CORS Configuration
            ConfigureCors(services);
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                context.Request.Headers.TryGetValue("Session-Key", out var userId);
                                context.Token = context.Request.Cookies[$"AuthToken_{userId}"];
                                return Task.CompletedTask;
                            }
                        };
                    });
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {token}'",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, new string[] { } }
                });
            });
        }

        private static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policy =>
                    policy.WithOrigins("http://localhost:5000") // Adjust as needed
                          .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });
        }
    }
}
