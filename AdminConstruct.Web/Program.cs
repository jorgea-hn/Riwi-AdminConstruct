using AdminConstruct.Web;
using AdminConstruct.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


using QuestPDF.Infrastructure;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Habilitar comportamiento legado de timestamps para PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configurar licencias
QuestPDF.Settings.License = LicenseType.Community;
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


// conection with postgres
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar servicios de Identity con roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


// Add services to the container.
builder.Services.AddScoped<AdminConstruct.Web.Services.RentalValidationService>();

builder.Services.AddControllersWithViews();

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// *** AÑADIDO: INICIO DE LA CONFIGURACIÓN DE VISTAS ***
// Esto le enseña al motor de vistas a buscar también en la carpeta /Views/Admin/
builder.Services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(options =>
{
    // {0} = Nombre de la Acción (ej. Index)
    // {1} = Nombre del Controlador (ej. Machinery)
    options.ViewLocationFormats.Add("/Views/Admin/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Admin/Shared/{0}.cshtml");
});
// *** FIN DE LA CONFIGURACIÓN DE VISTAS ***


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("Cliente", policy => policy.RequireRole("Cliente"));
});

// *** CORREGIDO: Ruta de Acceso Denegado ***
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/AccessDenied"; 
});


var app = builder.Build();


// Crear roles y usuario administrador por defecto
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    var context = services.GetRequiredService<ApplicationDbContext>();

    await SeedData.InitializeAsync(userManager, roleManager, context);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


// 👇 Importante: activar autenticación antes de autorización
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
