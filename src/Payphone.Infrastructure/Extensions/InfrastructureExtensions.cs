using Payphone.Application.Options;
using Payphone.Application.Services.Core;
using Payphone.Application.Services.Users;
using Payphone.Infrastructure.EF.Seeds;

namespace Payphone.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
     public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
        string xml)
    {
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddSwaggerConfig(xml);

        services.AddPersistence(configuration)
            .AddConfigOptions(configuration)
            .AddCorsWithConfiguration(configuration)
            .AddJwtConfig(configuration)
            .ConfigureIdentity()
            .AddValidators()
            .AddRepositories()
            .AddServices()
            .AddAutoMapper(typeof(UserMapper).Assembly);
            
        return services;
    }

    public static void UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseCors();
        app.ApplySeedConfiguration();
        app.UseAuthentication();
        app.UseAuthorization();
    }
    
    
    private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();
        
        services.Configure<IdentityOptions>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 6;
            opt.User.RequireUniqueEmail = true;
        });

        return services;
    }
    

    private static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            opt.EnableSensitiveDataLogging(false)
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Warning);
        });

        return services;
    }
    
    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }

    private static IServiceCollection AddCorsWithConfiguration(this
        IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = new CorsConfigOption();
        configuration.GetSection(nameof(CorsConfigOption)).Bind(corsOptions);

        var originsAllowed = corsOptions.OriginsAllowed ?? ["*"];
        var methodsAllowed = corsOptions.MethodsAllowed ?? ["*"];

        var corsPolicy = new CorsPolicyBuilder()
            .WithOrigins(string.Join(",", originsAllowed))
            .AllowAnyHeader()
            .WithMethods(string.Join(",", methodsAllowed))
            .Build();

        services.AddCors(c => c.AddDefaultPolicy(corsPolicy));

        return services;
    }
    
    private static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorsConfigOption>(configuration.GetSection(nameof(CorsConfigOption)));
        services.Configure<JwtOption>(configuration.GetSection(nameof(JwtOption)));
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection service)
    {
        service.AddScoped<IApplicationContext, ApplicationContext>();
        service.AddScoped<IUserService, UserService>();
        
        return service;
    }
    
    private static IServiceCollection AddRepositories(this IServiceCollection service)
    {
        return service;
    }

    private static void AddSwaggerConfig(this IServiceCollection services, string xml)
    {
        services.AddSwaggerGen(config =>
        {
            // Set the comments path for the Swagger JSON and UI.
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
            config.IncludeXmlComments(xmlPath);
            

            config.SwaggerDoc("v1", new()
            {
                Version = "v1.0",
                Title = "PAYPHONE APP API",
                Description = "user and password user:payphone@payphone.com, password: 123@payphone",
            });

            config.AddSecurityDefinition("Bearer", new()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT token"
            });

            config.AddSecurityRequirement(new()
            {
                {
                    new()
                    {
                        Reference = new()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
    
    private static IServiceCollection AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection(nameof(JwtOption));

        services.AddAuthorizationBuilder()
            .AddPolicy(JwtBearerDefaults.AuthenticationScheme, p =>
            {
                p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config[nameof(JwtOption.Issuer)],
                    ValidAudience = config[nameof(JwtOption.Audience)],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config[nameof(JwtOption.Key)] ?? ""))
                };
                options.UseSecurityTokenValidators = true;
            });

        return services;
    }

}