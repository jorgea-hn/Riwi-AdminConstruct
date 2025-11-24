using AdminConstruct.API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdminConstruct.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AdminConstruct.API.Services.IEmailService _emailService;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, AdminConstruct.API.Services.IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        // --------------------- Register ---------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Verificar si ya existe el usuario
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                return BadRequest("El usuario ya existe.");

            // Crear nuevo usuario
            var user = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Asignar rol Cliente
            await _userManager.AddToRoleAsync(user, "Cliente");

            // Enviar correo de bienvenida
            try
            {
                await _emailService.SendEmailAsync(dto.Email, "Bienvenido a AdminConstruct", "<h1>¡Bienvenido!</h1><p>Gracias por registrarte en nuestra plataforma.</p>");
            }
            catch (Exception ex)
            {
                // Loguear error pero no fallar el registro
                Console.WriteLine($"Error enviando correo: {ex.Message}");
            }

            return Ok("Usuario registrado correctamente.");
        }

        // --------------------- Login ---------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Usuario o contraseña inválidos.");

            var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!validPassword) return Unauthorized("Usuario o contraseña inválidos.");

            var token = await GenerateJwtToken(user);

            return Ok(token);
        }

        // --------------------- Generar JWT ---------------------
        private async Task<AuthResponseDto> GenerateJwtToken(IdentityUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Agregar roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
