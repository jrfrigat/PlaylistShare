
using Community.Microsoft.Extensions.Caching.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PlaylistShare.Api.Data;
using PlaylistShare.Api.Entities;
using PlaylistShare.Api.Extensions;
using PlaylistShare.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

namespace PlaylistShare.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        // DbContext. The provider is selectable via Database:Provider (env Database__Provider):
        // "SqlServer" (default) or "Postgres". Each provider has its own DbContext subclass carrying a
        // provider-specific EF Core migration set (Data/Migrations vs Data/Migrations/Postgres); the app
        // always injects ApplicationDbContext, which is mapped to the active one.
        var dbProvider = builder.Configuration["Database:Provider"] ?? "SqlServer";
        var usePostgres = string.Equals(dbProvider, "Postgres", StringComparison.OrdinalIgnoreCase);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (usePostgres)
        {
            builder.Services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(connectionString));
            builder.Services.AddScoped<ApplicationDbContext>(sp => sp.GetRequiredService<PostgresDbContext>());
        }
        else
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        }
        // Identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Session store (distributed cache), kept in the same database as the chosen provider so
        // session ids survive an API restart, exactly as on SQL Server.
        if (usePostgres)
        {
            builder.Services.AddDistributedPostgreSqlCache(options =>
            {
                options.ConnectionString = connectionString;
                options.SchemaName = "public";
                options.TableName = "SessionCache";
                options.CreateInfrastructure = true; // auto-create the cache table (no EF migration for it)
            });
        }
        else
        {
            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connectionString;
                options.SchemaName = "dbo";
                options.TableName = "SessionCache";
            });
        }

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromDays(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.MaxAge = TimeSpan.FromDays(30); // persistent across browser restarts
        });

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();  // trust all proxies in Docker network
            options.KnownProxies.Clear();
        });

        // JWT
        var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key missing");
        builder.Services.AddAuthentication(options =>
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
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<JwtService>();

        // Data Protection key ring lives on a mounted volume (see docker-compose.yml), NOT in the
        // database. The path comes from DataProtection:KeysPath (env DataProtection__KeysPath, e.g.
        // "/keys" in Docker) and falls back to a folder under the content root for local development.
        var dpKeysPath = builder.Configuration["DataProtection:KeysPath"]
            ?? Path.Combine(builder.Environment.ContentRootPath, "dp-keys");
        Directory.CreateDirectory(dpKeysPath);
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(dpKeysPath))
            .SetApplicationName("PlaylistShare.Api");

        // YandexMusic client (scoped IYandexMusicClient over a pooled IHttpClientFactory handler).
        builder.Services.AddYandexMusic();
        builder.Services.AddMemoryCache(); // holds in-progress QR sign-in attempts for their lifetime
        builder.Services.AddScoped<YandexApiService>();
        builder.Services.AddScoped<YandexMusicService>();
        builder.Services.AddScoped<YandexAuthService>();
        builder.Services.AddScoped<SharedPlaylistService>();
        builder.Services.AddScoped<TrackAdditionLogService>();
        builder.Services.AddScoped<TrackRemovalLogService>();
        builder.Services.AddScoped<FavoritesService>();

        builder.Services.AddHttpClient();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Production", policy =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    // Dev: allow any localhost origin so local tools work without hard-coding every
                    // scheme+port (Swagger on the API's http/https port, the PWA dev server on its own
                    // port, etc.). AllowCredentials needs a specific origin, so the match is reflected.
                    policy.SetIsOriginAllowed(origin =>
                              Uri.TryCreate(origin, UriKind.Absolute, out var u)
                              && (u.Host == "localhost" || u.Host == "127.0.0.1"))
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
                else
                {
                    policy.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
            });
        });

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // Единая обработка необработанных исключений: до неё они уходили клиенту голым 500 без тела
        // и без записи в логах. AddProblemDetails нужен, чтобы и остальные ошибки конвейера
        // (404, 415 и прочие) отвечали в том же формате, а не пустотой.
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi(options =>
        {
            // Advertise JWT Bearer in the OpenAPI doc so Swagger shows an "Authorize" button and sends
            // "Authorization: Bearer <token>" - otherwise [Authorize] endpoints just return 401.
            options.AddDocumentTransformer((document, context, ct) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Paste your JWT access token (from POST /api/account/login). The 'Bearer ' prefix is added automatically."
                };
                document.Security ??= new List<OpenApiSecurityRequirement>();
                document.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                });
                return Task.CompletedTask;
            });
        });

        var app = builder.Build();

        // Apply pending EF Core migrations on startup so a fresh deployment self-provisions its schema
        // without a manual `dotnet ef database update`. Retried so the API can start before the database
        // container is ready (Docker) and wait for it instead of crash-looping.
        using (var scope = app.Services.CreateScope())
        {
            var db = usePostgres
                ? scope.ServiceProvider.GetRequiredService<PostgresDbContext>()
                : scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            const int maxAttempts = 12;
            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    db.Database.Migrate();
                    app.Logger.LogInformation("Database schema is up to date (checked on attempt {Attempt}).", attempt);
                    break;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    app.Logger.LogWarning("Database not ready (attempt {Attempt}/{Max}): {Error}. Retrying in 5s...",
                        attempt, maxAttempts, ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
        }

        // Первым в конвейере: он ловит только то, что упало НИЖЕ по цепочке, поэтому всё, что должно
        // быть под его защитой, обязано регистрироваться после.
        app.UseExceptionHandler();

        app.MapOpenApi();
        // The .NET OpenAPI package only serves the raw document (/openapi/v1.json); Swashbuckle's UI
        // renders it as interactive Swagger at /swagger.
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "PlaylistShare API v1"));

        app.UseForwardedHeaders();
        app.UseCors("Production");

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }


        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
