using Microsoft.AspNetCore.Identity;

namespace AdminConstruct.Web;

public class SeedData
{
    public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Roles por defecto
        string[] roles = { "Administrador", "Cliente" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Usuario administrador
        string adminEmail = "admin@firmeza.com";
        string adminPass = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, adminPass);
            await userManager.AddToRoleAsync(admin, "Administrador");
        }
    }
}
