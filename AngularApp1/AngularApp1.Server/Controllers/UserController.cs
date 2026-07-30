using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Services;

namespace AngularApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUser>>> GetUsers()
        {
            try
            {
                var response = await _service.GetUsersAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching users: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUser>> GetUser(long id)
        {
            try
            {
                var response = await _service.GetUserByIdAsync(id);
                if (response == null)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching user: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Result>> PostUser([FromBody] PostUser model)
        {
            try
            {
                var response = await _service.PostUserAsync(model);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating user: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result>> PutUser(long id, [FromBody] PostUser model)
        {
            try
            {
                var response = await _service.PutUserAsync(id, model);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating user: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result>> DeleteUser(long id)
        {
            try
            {
                var response = await _service.DeleteUserAsync(id);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting user: {ex.Message}");
            }
        }
    }
}
