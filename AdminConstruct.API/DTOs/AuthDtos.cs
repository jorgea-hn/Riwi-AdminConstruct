namespace AdminConstruct.API.DTOs
{
    // DTO para registrar un usuario
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }

    // DTO para login
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // DTO para respuesta de autenticación
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}