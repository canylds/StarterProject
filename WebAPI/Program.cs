
using Core.CrossCuttingConcerns.Exception.Extensions;
using Core.Security.Encryption;
using Core.Security.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Application;
using Infrastructure;
using Persistence;

namespace WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddApplicationServices();
        builder.Services.AddPersistenceServices(builder.Configuration);
        builder.Services.AddInfrastructureServices();

        builder.Services.AddHttpContextAccessor();

        const string tokenOptionsConfigurationSection = "TokenOptions";
        TokenOptions tokenOptions =
            builder.Configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new InvalidOperationException($"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                };
            });

        builder.Services.AddDistributedMemoryCache();
        //builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = "localhost:6379");

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddCors(opt => opt.AddDefaultPolicy(p =>
        {
            p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }));

        builder.Services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345.54321\""
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]
                    {

                    }
                }
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.DocExpansion(DocExpansion.None);
            });
        }

        if (app.Environment.IsProduction())
            app.ConfigureCustomExceptionMiddleware();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        const string webApiConfigurationSection = "WebAPIConfiguration";
        WebApiConfiguration webApiConfiguration =
            app.Configuration.GetSection(webApiConfigurationSection).Get<WebApiConfiguration>()
            ?? throw new InvalidOperationException($"\"{webApiConfigurationSection}\" section cannot found in configuration.");

        app.UseCors(opt => opt.WithOrigins(webApiConfiguration.AllowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

        app.Run();
    }
}
