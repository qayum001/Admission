using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Admission.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace Admission.Api.Auth;

public static class AuthExtension
{
    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetSection("AuthConfiguration").Get<AuthConfiguration>()
            ?? throw new ArgumentNullException($"{nameof(AuthConfiguration)}:{nameof(AuthConfiguration)}");
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.RequireHttpsMetadata = false;
            
            options.MetadataAddress = authOptions.MetadataAddress;

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = authOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = authOptions.Audience,

                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                RoleClaimType = authOptions.RoleClaimType,
                NameClaimType = JwtRegisteredClaimNames.Sub
            };
        });
        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

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
