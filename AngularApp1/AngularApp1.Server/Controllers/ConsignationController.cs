using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Services;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ConsignationController : ControllerBase
    {
        private readonly IConsignationService _service;

        public ConsignationController(IConsignationService service)
        {
            _service = service;
        }


        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var userIdStr = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                long? userId = !string.IsNullOrEmpty(userIdStr) ? long.Parse(userIdStr) : null;

                var list = await _service.GetPendingConsignationsAsync(userId, role);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userIdStr = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                long? userId = !string.IsNullOrEmpty(userIdStr) ? long.Parse(userIdStr) : null;

                var list = await _service.GetAllConsignationsAsync(userId, role);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var obj = await _service.GetConsignationByIdAsync(id);
                if (obj == null) return NotFound();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PostConsignation request, IFormFile file)
        {
            try
            {
                var userIdStr = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                long createdBy = !string.IsNullOrEmpty(userIdStr) ? long.Parse(userIdStr) : 1;

                var id = await _service.SubmitConsignationAsync(request, file, createdBy);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("audit/{id}")]
        public async Task<IActionResult> Audit(long id, [FromBody] AuditConsignation request)
        {
            try
            {
                var userIdStr = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                long validatorId = !string.IsNullOrEmpty(userIdStr) ? long.Parse(userIdStr) : 1;

                var result = await _service.AuditConsignationAsync(id, request, validatorId);
                if (!result) return NotFound();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
