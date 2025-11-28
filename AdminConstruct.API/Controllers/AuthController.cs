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
            // Check if user already exists
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                return BadRequest("User already exists.");

            // Create new user
            var user = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign Client role
            await _userManager.AddToRoleAsync(user, "Cliente");

            // Create associated Customer
            var customer = new AdminConstruct.Web.Models.Customer
            {
                Name = dto.Name,
                Document = dto.Document,
                Email = dto.Email,
                Phone = dto.Phone,
                UserId = user.Id
            };

            // Save customer to database
            var context = HttpContext.RequestServices.GetRequiredService<AdminConstruct.Web.Data.ApplicationDbContext>();
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            // Send welcome email
            try
            {
                await _emailService.SendEmailAsync(dto.Email, "Welcome to AdminConstruct", "<h1>Welcome!</h1><p>Thanks for registering on our platform.</p>");
            }
            catch (Exception ex)
            {
                // Log error but do not fail registration
                Console.WriteLine($"Error sending email: {ex.Message}");
            }

            return Ok(new { message = "User registered successfully.", customerId = customer.Id });
        }

        // --------------------- Login ---------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid user or password.");

            var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!validPassword) return Unauthorized("Invalid user or password.");

            var token = await GenerateJwtToken(user);

            return Ok(token);
        }

        // --------------------- Generate JWT ---------------------
        private async Task<AuthResponseDto> GenerateJwtToken(IdentityUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),  // IMPORTANT: use user.Id instead of email
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            // Add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(24); // Extended to 24 hours

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            Console.WriteLine($"[AuthController] Token generated for user: {user.Email}");
            Console.WriteLine($"[AuthController] User ID: {user.Id}");
            Console.WriteLine($"[AuthController] Expiration: {expiration}");

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
