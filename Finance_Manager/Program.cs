using Asp.Versioning;
using Finance_Manager_BAL;
using Finance_Manager_BAL.Interfaces;
using Finance_Manager_Core.Interface;
using Finance_Manager_Core.Middleware;
using Finance_Manager_Core.Services;
using Finance_Manager_DAL.Context;
using Finance_Manager_DAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using StackExchange.Redis;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()

    // File Logging
    .WriteTo.File(
        path: "logs/log-.txt",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7
    )

    // MySQL Logging
    .WriteTo.MySQL(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        tableName: "log"
    )
    .CreateLogger();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetTokenBucketLimiter(
            // Use client IP as partition key (each IP gets its own bucket)
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",

            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 100,
                TokensPerPeriod = 100,
                // Interval at which tokens are replenished
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                AutoReplenishment = true,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            }));


    options.AddTokenBucketLimiter("AuthPolicy", opt =>
    {
        opt.TokenLimit = 5;
        opt.TokensPerPeriod = 5;
        opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
        opt.AutoReplenishment = true;
    });

    options.OnRejected = async (context, token) =>
    {
        var httpContext = context.HttpContext;
        var ip = httpContext.Connection.RemoteIpAddress?.ToString();

        // Write log
        Log.ForContext("ClientIP", ip)
            .Warning("Rate limit exceeded | IP: {IP} | Path: {Path} | Method: {Method}",
                ip,
                httpContext.Request.Path,
                httpContext.Request.Method);

        // Set HTTP status code to 429 (Too Many Requests)
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        // Optional: Add Retry-After header (in seconds)
        context.HttpContext.Response.Headers["Retry-After"] = "60";

        // Custom API response body
        var response = new ApiResponse<string>
        {
            Success = false,
            Message = "Too many requests. Please try again after some time."
        };

        // Return JSON response
        await context.HttpContext.Response.WriteAsJsonAsync(response, token);
    };
});


// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen();

// Mysql
builder.Services.AddSingleton<IDbConnectionFactory>(dbFactory =>
    new OrmLiteConnectionFactory(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        MySqlDialect.Provider // or SqlServerDialect.Provider
    ));

// Redis 
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("Redis")
    ));



builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Your API",
        Version = "v1"
    });

    // ?? Add Bearer token support
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token.\nExample: Bearer abc123xyz"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});


//DI

builder.Services.AddScoped<IBLAuthHandler, BLAuthHandler>();
builder.Services.AddScoped<IDLAuthContext, DLAuthContext>();

builder.Services.AddScoped<IBLUserHandler, BLUserHandler>();
builder.Services.AddScoped<IDLUserContext, DLUserContext>();

builder.Services.AddScoped<IBLRoleHandler, BLRoleHandler>();
builder.Services.AddScoped<IDLRoleContext, DLRoleContext>();

builder.Services.AddScoped<IBLPermissionHandler, BLPermissionHandler>();
builder.Services.AddScoped<IDLPermissionContext, DLPermissionContext>();

builder.Services.AddScoped<IBLCategoryHandler, BLCategoryHandler>();
builder.Services.AddScoped<IDLCategoryContext, DLCategoryContext>();

builder.Services.AddScoped<IBLTransactionHandler, BLTransactionHandler>();
builder.Services.AddScoped<IDLTransactionContext, DLTransactionContext>();

builder.Services.AddScoped<IBLDashboardHandler, BLDashboardHandler>();
builder.Services.AddScoped<IDLDashboardContext, DLDashboardContext>();


builder.Services.AddScoped<IDTOPOCOMapper, DTOPOCOMapper>();

builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<RedisService>();
builder.Services.AddScoped<PermissionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
