using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TalentoPlus.Infrastructure.Data.Context;
using TalentoPlus.Infrastructure.Identity.Identity;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Infrastructure.Data.Repositories;
using TalentoPlus.Infrastructure.Identity.Seed;
using TalentoPlus.Infrastructure.Identity.Services;
using TalentoPlus.Application.Services;
using TalentoPlus.Infrastructure.Integrations;
using TalentoPlus.Infrastructure.Integrations.Pdf;
using TalentoPlus.Infrastructure.Integrations.Email;
using TalentoPlus.Infrastructure.Integrations.AI;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ================================================
// Load .env (optional)
// ================================================
Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();

// ================================================
// DbContext - PostgreSQL
// ================================================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

// ================================================
// Dependency Injection
// ================================================
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// ❌ Repositorios eliminados (ya no existen tablas para enums)
// builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
// builder.Services.AddScoped<IEducationLevelRepository, EducationLevelRepository>();

// Services
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();

// Integrations
builder.Services.AddScoped<ExcelImporter>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAiService, AiService>(); // ← AI Service

// ================================================
// Identity
// ================================================
builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ================================================
// JWT Authentication
// ================================================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

// ================================================
// Controllers & JSON options (Enums as strings)
// ================================================
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ================================================
// Swagger + JWT Support
// ================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TalentoPlus API",
        Version = "v1",
        Description = "API para gestión de empleados con enums, Identity y PostgreSQL."
    });

    // JWT Bearer Config
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa tu JWT con el formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ================================================
// CORS
// ================================================
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", p =>
    {
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowAnyOrigin();
    });
});

var app = builder.Build();

// ================================================
// Middleware
// ================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ================================================
// SEEDER
// ================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var db = services.GetRequiredService<AppDbContext>();

    await IdentitySeeder.SeedAsync(userManager, roleManager, db);
}

app.Run();