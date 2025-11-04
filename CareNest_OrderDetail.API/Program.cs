using CareNest_OrderDetail.API.Middleware;
using CareNest_OrderDetail.Application.Common;
using CareNest_OrderDetail.Application.Features.Commands.Create;
using CareNest_OrderDetail.Application.Features.Commands.Delete;
using CareNest_OrderDetail.Application.Features.Commands.Update;
using CareNest_OrderDetail.Application.Features.Queries.GetAllPaging;
using CareNest_OrderDetail.Application.Features.Queries.GetById;
using CareNest_OrderDetail.Application.Interfaces.CQRS;
using CareNest_OrderDetail.Application.Interfaces.CQRS.Commands;
using CareNest_OrderDetail.Application.Interfaces.CQRS.Queries;
using CareNest_OrderDetail.Application.Interfaces.Services;
using CareNest_OrderDetail.Application.Common.Options;
using CareNest_OrderDetail.Application.Interfaces.UOW;
using CareNest_OrderDetail.Application.UseCases;
using CareNest_OrderDetail.Domain.Entitites;
using CareNest_OrderDetail.Domain.Repositories;
using CareNest_OrderDetail.Infrastructure.Persistences.Configuration;
using CareNest_OrderDetail.Infrastructure.Persistences.Database;
using CareNest_OrderDetail.Infrastructure.Persistences.Repository;
using CareNest_OrderDetail.Infrastructure.Services;
using CareNest_OrderDetail.Infrastructure.UOW;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Lấy DatabaseSettings ưu tiên từ ENV (DATABASE_URL), fallback sang DB_* hoặc appsettings, có try/catch an toàn
string connectionString;
try
{
    var config = builder.Configuration;
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") ?? config["DATABASE_URL"];

    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        // Parse postgres://user:password@host:port/dbname
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);
        var user = userInfo.Length > 0 ? userInfo[0] : "postgres";
        var password = userInfo.Length > 1 ? userInfo[1] : "postgres";
        var host = uri.Host;
        var portVal = uri.Port > 0 ? uri.Port : 5432;
        var dbName = uri.AbsolutePath.TrimStart('/');

        var ssl = Environment.GetEnvironmentVariable("POSTGRES_SSL");
        var sslPart = (!string.IsNullOrWhiteSpace(ssl) && ssl.Equals("true", StringComparison.OrdinalIgnoreCase))
            ? "SSL Mode=Require;Trust Server Certificate=true;"
            : string.Empty;

        connectionString = $"Host={host};Port={portVal};Database={dbName};Username={user};Password={password};{sslPart}Pooling=true;Maximum Pool Size=5;Minimum Pool Size=0;Timeout=15;";
    }
    else
    {
        // Fallback: đọc từ DB_* hoặc appsettings
        var host = config["DB_HOST"] ?? config["DatabaseSettings:Ip"] ?? "localhost";
        var portVal = int.TryParse(config["DB_PORT"], out var parsedPort)
            ? parsedPort
            : (config.GetSection("DatabaseSettings").GetValue<int?>("Port") ?? 5432);
        var user = config["DB_USER"] ?? config["DatabaseSettings:User"] ?? "postgres";
        var password = config["DB_PASSWORD"] ?? config["DatabaseSettings:Password"] ?? "postgres";
        var dbName = config["DB_NAME"] ?? config["DatabaseSettings:Database"] ?? "order-detail";

        DatabaseSettings dbSettings = new DatabaseSettings
        {
            Ip = host,
            Port = portVal,
            User = user,
            Password = password,
            Database = dbName
        };
        if (builder.Environment.IsDevelopment())
        {
            dbSettings.Display();
        }
        var ssl = Environment.GetEnvironmentVariable("POSTGRES_SSL");
        var sslPart = (!string.IsNullOrWhiteSpace(ssl) && ssl.Equals("true", StringComparison.OrdinalIgnoreCase))
            ? "SSL Mode=Require;Trust Server Certificate=true;"
            : string.Empty;

        connectionString = dbSettings.GetConnectionString() + $";{sslPart}Pooling=true;Maximum Pool Size=5;Minimum Pool Size=0;Timeout=15;";
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[Startup] Fallback DB config due to: {ex.Message}");
    connectionString = "Host=localhost;Port=5432;Database=order-detail;Username=postgres;Password=postgres;Pooling=true;Maximum Pool Size=5;Minimum Pool Size=0;Timeout=15;";
}


// Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
        // Chỉ định assembly chứa EF Core migrations
        npgsqlOptions.MigrationsAssembly("CareNest_OrderDetail.Infrastructure");
    }));

builder.Services.AddTransient<DatabaseSeeder>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Đăng ký service thêm chú thích cho api
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    //ADD JWT BEARER SECURITY DEFINITION
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        //Type = SecuritySchemeType.ApiKey,
        Type = SecuritySchemeType.Http,//ko cần thêm token phía trước
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer"
            },
            new List<string>()
        }
    });
});

// Đăng ký các repository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//command
builder.Services.AddScoped<ICommandHandler<CreateCommand, OrderDetail>, CreateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateCommand, OrderDetail>, UpdateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteCommand>, DeleteCommandHandler>();
//query
builder.Services.AddScoped<IQueryHandler<GetAllPagingQuery, PageResult<OrderDetailResponse>>, GetAllPagingQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetByIdQuery, OrderDetail>, GetByIdQueryHandler>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);


//Đăng ký cho FE
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:4200",
                "https://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});


//Đăng ký lấy thông tin từ token
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

//Đăng ký HttpClient
//builder.Services.AddHttpClient<IAccountService, AccountService>(client =>
//{
//    client.BaseAddress = new Uri("https://authorize-api-dev.lighttail.com/api/");
//}).AddPolicyHandler(HttpPolicyExtensions
//    .HandleTransientHttpError()
//    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2)));

//builder.Services.AddHttpClient<IScreenServicce, ScreenService>(client =>
//{
//    client.BaseAddress = new Uri("https://authorize-api-dev.lighttail.com/swagger/index.html");
//}).AddPolicyHandler(HttpPolicyExtensions
//    .HandleTransientHttpError()
//    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2)));

//builder.Services.Configure<RouteOptions>(options =>
//{
//    options.LowercaseUrls = true;
//});

//builder.Services.AddSwaggerGen(c =>
//{
//    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//    c.IncludeXmlComments(xmlPath);
//});

//var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
//builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidIssuer = jwtSettings!.Issuer,
//        ValidAudience = jwtSettings.Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,

//        RoleClaimType = ClaimTypes.Role
//    };

//    options.Events = new JwtBearerEvents
//    {
//        OnChallenge = async context =>
//        {
//            context.HandleResponse();
//            if (!context.Response.HasStarted)
//            {
//                context.Response.StatusCode = 401;
//                context.Response.ContentType = "application/json";
//                await context.Response.WriteAsync(JsonSerializer.Serialize(new
//                {
//                    statusCode = 401,
//                    message = "Unauthorized – Token missing or invalid.",
//                    timestamp = DateTime.UtcNow
//                }));
//            }
//        },
//        OnForbidden = async context =>
//        {
//            if (!context.Response.HasStarted)
//            {
//                context.Response.StatusCode = 403;
//                context.Response.ContentType = "application/json";
//                await context.Response.WriteAsync(JsonSerializer.Serialize(new
//                {
//                    statusCode = 403,
//                    message = "Forbidden – You don't have permission.",
//                    timestamp = DateTime.UtcNow
//                }));
//            }
//        }
//    };
//});

builder.Services.AddScoped<IUseCaseDispatcher, UseCaseDispatcher>();

// Options cho APIService generic - ưu tiên ENV, fallback sang config
builder.Services.Configure<APIServiceOption>(options =>
{
    var config = builder.Configuration;
    options.BaseUrlShop = config["SHOP_API_URL"] ?? config["ShopApi:BaseUrl"] ?? "http://localhost:8015";
    options.BaseUrlProduct = config["PRODUCT_DETAIL_API_URL"] ?? config["ProductDetailApi:BaseUrl"] ?? "http://localhost:8016";
});

// HttpClient generic
builder.Services.AddHttpClient<IAPIService, APIService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
var swaggerEnv = Environment.GetEnvironmentVariable("SWAGGER_ENABLED");
var swaggerEnabled = app.Environment.IsDevelopment()
    || builder.Configuration.GetValue<bool>("Swagger:Enabled")
    || (!string.IsNullOrWhiteSpace(swaggerEnv) && swaggerEnv.Equals("true", StringComparison.OrdinalIgnoreCase));
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Ở môi trường Koyeb (đứng sau proxy), có thể không xác định được HTTPS port
// Chỉ bật redirect HTTPS ở Development; Production để proxy xử lý TLS
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

// Chạy migrate có điều kiện qua ENV RUN_MIGRATIONS=true, có log và bắt lỗi rõ ràng
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    var runMigrations = Environment.GetEnvironmentVariable("RUN_MIGRATIONS");
    if (!string.IsNullOrWhiteSpace(runMigrations) && runMigrations.Equals("true", StringComparison.OrdinalIgnoreCase))
    {
        try
        {
            Console.WriteLine("[Startup] Applying EF Core migrations...");
            var pending = context.Database.GetPendingMigrations().ToList();
            Console.WriteLine($"[Startup] Pending migrations: {(pending.Count == 0 ? "<none>" : string.Join(", ", pending))}");
            context.Database.Migrate();
            Console.WriteLine("[Startup] EF Core migrations completed successfully.");

            // Nếu hoàn toàn không có migration nào (trường hợp build thiếu migrations), fallback EnsureCreated
            var allMigrations = context.Database.GetMigrations().ToList();
            var applied = context.Database.GetAppliedMigrations().ToList();
            if (allMigrations.Count == 0 && applied.Count == 0)
            {
                Console.WriteLine("[Startup] No migrations found. Running EnsureCreated() as fallback...");
                context.Database.EnsureCreated();
                Console.WriteLine("[Startup] EnsureCreated completed.");
            }

            // Đảm bảo bảng orderdetails tồn tại (tránh lệch tên hoa/thường giữa migration và model)
            Console.WriteLine("[Startup] Verifying table 'orderdetails' exists...");
            var ensureSql = @"DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_catalog.pg_class c
        JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace
        WHERE n.nspname='public' AND c.relname='orderdetails'
    ) THEN
        CREATE TABLE public.orderdetails (
            \"Id\" text NOT NULL,
            \"ProductDetailId\" text NULL,
            \"OrderId\" text NULL,
            \"Quantity\" integer NOT NULL,
            \"TotalAmount\" double precision NOT NULL,
            \"CreatedBy\" text NULL,
            \"UpdatedBy\" text NULL,
            \"DeletedBy\" text NULL,
            \"CreatedAt\" timestamp with time zone NULL,
            \"UpdatedAt\" timestamp with time zone NULL,
            \"DeleteAt\" timestamp with time zone NULL,
            CONSTRAINT \"PK_orderdetails\" PRIMARY KEY (\"Id\")
        );
    END IF;
END $$;";
            context.Database.ExecuteSqlRaw(ensureSql);
            Console.WriteLine("[Startup] Table 'orderdetails' verification completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Startup] EF Core migration failed: {ex.Message}");
            throw;
        }
    }
}

app.Run();