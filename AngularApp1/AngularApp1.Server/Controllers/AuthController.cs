using Microsoft.AspNetCore.Mvc;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Services;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "El correo y la contraseña son obligatorios." });

            var response = await _authService.LoginAsync(request);

            if (response == null)
                return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo." });

            return Ok(response);
        }
    }
}
