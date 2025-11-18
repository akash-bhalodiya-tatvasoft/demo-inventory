using System.Text;
using Inventory.API.Filter;
using Inventory.API.Middleware;
using Inventory.Context;
using Inventory.Services.Auth;
using Inventory.Services.Implementations;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// Add Swagger
builder.Services.AddSwaggerGen();

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

// DbContext
builder.Services.AddSingleton<ADODbContext>();

// Middleware
builder.Services.AddTransient<ExceptionMiddleware>();

// Filter
builder.Services.AddScoped<PermissionFilter>();

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

// Generate Swagger UI
builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,

                            },
                            new List<string>()
                        }
                });
            });

// Add HttpClient
builder.Services.AddHttpClient<IPostService, PostService>(option =>
{
    option.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("InventoryCORS");

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<UserContextMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();