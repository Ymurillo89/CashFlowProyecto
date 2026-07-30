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
                var list = await _service.GetPendingConsignationsAsync();
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
                var list = await _service.GetAllConsignationsAsync();
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
                var id = await _service.SubmitConsignationAsync(request, file);
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
                var result = await _service.AuditConsignationAsync(id, request);
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
