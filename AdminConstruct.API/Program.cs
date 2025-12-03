using AdminConstruct.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Habilitar comportamiento legado de timestamps para PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


// -----------------------------
// DATABASE
// -----------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// -----------------------------
// IDENTITY
// -----------------------------
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// -----------------------------
// JWT
// -----------------------------
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Missing Jwt:Key in appsettings.json");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = "JwtBearer";
    opt.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// -----------------------------
// AUTOMAPPER
// -----------------------------
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// -----------------------------
// EMAIL SERVICE
// -----------------------------
builder.Services.AddScoped<AdminConstruct.API.Services.IEmailService, AdminConstruct.API.Services.EmailService>();

// -----------------------------
// CONTROLLERS
// -----------------------------
builder.Services.AddControllers();

// -----------------------------
// CORS
// -----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// -----------------------------
// SWAGGER + JWT CONFIG
// -----------------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AdminConstruct API", Version = "v1" });

    // JWT in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token here"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// -----------------------------
// CREATE DEFAULT ROLES (Admin, Client)
// -----------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    // Note: Migrations are applied by the Web service which owns the migration files
    // This prevents concurrent migration attempts that cause database conflicts

    // Initialize Seed Data
    AdminConstruct.API.Data.DbInitializer.Initialize(context);

    string[] roles = { "Admin", "Cliente" }; // Keeping role names as they might be used in logic, but comments are translated.

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

// -----------------------------
// MIDDLEWARE
// -----------------------------
// Swagger enabled in all environments (Development and Production)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
