using System.Text;
using System.Threading.RateLimiting;
using Inventory.API.Filter;
using Inventory.API.Middleware;
using Inventory.Common.Responses;
using Inventory.Context;
using Inventory.Services.Auth;
using Inventory.Services;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Mapster;
using MapsterMapper;
using Inventory.API.Mapping;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    }).AddFluentValidation(option =>
    {
        option.RegisterValidatorsFromAssemblyContaining<OrderRequestValidator>();
    });

// Connect to Database
builder.Services.AddDbContext<AppDbContext>((options) => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultString")));

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserActivityService, UserActivityService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Memory caching service
builder.Services.AddSingleton<IMemoryCacheService, MemoryCacheService>();

// DbContext
builder.Services.AddSingleton<ADODbContext>();

// Middleware
builder.Services.AddTransient<ExceptionMiddleware>();

// Filter
builder.Services.AddScoped<PermissionFilter>();

// Background Jobs
builder.Services.AddHostedService<UserInactiveService>();
builder.Services.AddHostedService<LoadProductInCache>();


// Mapster
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(typeof(OrderMappingConfig).Assembly);

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// NLog
NLog.LogManager.Configuration.Variables["DefaultString"] =
        builder.Configuration.GetConnectionString("DefaultString");

// JWT AUthentication
builder.Services.AddAuthentication(configureOptions =>
{
    configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer((x) =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWTToken:SecretKey"))),
        ValidateAudience = false,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JWTToken:Issuer"),
        ClockSkew = TimeSpan.Zero
    };
});

// CORS
builder.Services.AddCors(options => options.AddPolicy("InventoryCORS", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:3000")));

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("Fixed", httpContext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "localhost",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 3,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        }));
    options.OnRejected = async (context, cancellationToken) =>
        {
            context.HttpContext.Response.StatusCode =
              StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/json";

            var response = ApiResponse<string>.Failure(
                StatusCodes.Status429TooManyRequests,
                "Rate limit exceeded."
            );

            await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        };
});

// Add HttpClient
builder.Services.AddHttpClient<IPostService, PostService>(option =>
{
    option.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

// Enable caching
builder.Services.AddMemoryCache();

builder.Services.AddResponseCaching();

// API version
builder.Services.AddApiVersioning((options) =>
{
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddVersionedApiExplorer((option) =>
{
    option.GroupNameFormat = "'v'VVV";
    option.SubstituteApiVersionInUrl = true;
});

// Add Swagger
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions.Reverse())
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("InventoryCORS");

app.UseRateLimiter();

app.UseAuthentication();

app.UseMiddleware<UserContextMiddleware>();

app.UseAuthorization();

app.UseResponseCaching();

app.MapControllers();

app.Run();