using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text.Json.Serialization;
using Admission.Auth.Api.Swagger;
using Admission.Auth.Application;
using Admission.Auth.Domain.Entities;
using Admission.Auth.Initialization;
using Admission.Auth.Messaging;
using Admission.Auth.Options;
using Admission.Auth.Persistence;
using Admission.Auth.Security;
using Admission.Auth.Security.Signing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;

namespace Admission.Auth.Composition;

public static class DependencyInjection
{
    public static void AddAuthCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));
        services.Configure<PasswordPolicyOptions>(configuration.GetSection(PasswordPolicyOptions.SectionName));
        services.Configure<SecurityOptions>(configuration.GetSection(SecurityOptions.SectionName));
        services.Configure<SigningKeyOptions>(configuration.GetSection(SigningKeyOptions.SectionName));
        services.Configure<AdminSeedOptions>(configuration.GetSection(AdminSeedOptions.SectionName));
        services.Configure<AuthDebugOptions>(configuration.GetSection(AuthDebugOptions.SectionName));

        services.AddAuthPersistence(configuration);
        services.AddAuthMessaging(configuration);

        services.AddSingleton<ISigningKeyCache, SigningKeyCache>();
        services.AddScoped<ISigningKeyLifecycleService, SigningKeyLifecycleService>();

        services.AddHostedService<SigningKeyBootstrapHostedService>();
        services.AddHostedService<AdminSeedHostedService>();

        services.AddScoped<IPasswordHasher<AuthUser>, PasswordHasher<AuthUser>>();
        services.AddScoped<IPasswordPolicyValidator, PasswordPolicyValidator>();
        services.AddScoped<IRandomTokenGenerator, RandomTokenGenerator>();
        services.AddScoped<ITokenHasher, TokenHasher>();
        services.AddScoped<IAccessTokenFactory, AccessTokenFactory>();
        services.AddScoped<AuthService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<ISigningKeyCache, IConfiguration>((options, signingKeyCache, cfg) =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = cfg[$"{AuthOptions.SectionName}:Issuer"],
                    ValidAudience = cfg[$"{AuthOptions.SectionName}:Audience"],
                    RoleClaimType = AuthClaimNames.Role,
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    IssuerSigningKeyResolver = (_, _, _, _) => signingKeyCache.GetValidationKeys()
                };

                var skew = cfg.GetValue<int?>($"{AuthOptions.SectionName}:ClockSkewSeconds") ?? 30;
                options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(skew);

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var sub = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                        var secVerClaim = context.Principal?.FindFirst(AuthClaimNames.SecurityVersion)?.Value;

                        if (!Guid.TryParse(sub, out var userId) || !int.TryParse(secVerClaim, out var securityVersion))
                        {
                            context.Fail("Token claims are invalid.");
                            return;
                        }

                        var dbContext = context.HttpContext.RequestServices.GetRequiredService<AuthDbContext>();
                        var user = await dbContext.Users.FindAsync([userId], context.HttpContext.RequestAborted);
                        if (user is null || !user.IsActive || user.SecurityVersion != securityVersion)
                        {
                            context.Fail("Token has been revoked.");
                        }
                    }
                };
            });

        services.AddAuthorization();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        services.AddOpenApi();
        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste only JWT token value (without Bearer prefix)."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
            });

            options.IncludeXmlComments(Assembly.GetExecutingAssembly(), includeControllerXmlComments: true);

            options.OperationFilter<AuthorizeOperationFilter>();
        });
    }
}
