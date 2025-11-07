using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminConstruct.Razor.Controllers;

public class AccountController: Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            // Verificar el rol
            if (await _userManager.IsInRoleAsync(user, "Administrador"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (await _userManager.IsInRoleAsync(user, "Cliente"))
            {
                return RedirectToAction("Index", "Home");
            }

            // Si el usuario no tiene rol definido, lo deslogueamos por seguridad
            await _signInManager.SignOutAsync();
            ModelState.AddModelError("", "Tu cuenta no tiene un rol asignado. Contacta al administrador.");
            return View(model);
        }

        ModelState.AddModelError("", "Email o contraseña incorrectos");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    // GET: /Account/AccessDenied
    public IActionResult AccessDenied()
    {
        return View();
    }
}

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
