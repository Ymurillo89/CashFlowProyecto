using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Services;

namespace AngularApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetRole>>> GetRoles()
        {
            try
            {
                var response = await _service.GetRolesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching roles: {ex.Message}");
            }
        }
    }
}
