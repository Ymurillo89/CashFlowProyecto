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
    [Authorize]
    public class BankController : ControllerBase
    {
        private readonly IBankService _service;

        public BankController(IBankService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBank>>> GetBanks()
        {
            try
            {
                var response = await _service.GetBanksAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching banks: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetBank>> GetBank(short id)
        {
            try
            {
                var response = await _service.GetBankByIdAsync(id);
                if (response == null)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching bank: {ex.Message}");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<Result>> PostBank([FromBody] PostBank model)
        {
            try
            {
                var response = await _service.PostBankAsync(model);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating bank: {ex.Message}");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Result>> PutBank(short id, [FromBody] PostBank model)
        {
            try
            {
                var response = await _service.PutBankAsync(id, model);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating bank: {ex.Message}");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Result>> DeleteBank(short id)
        {
            try
            {
                var response = await _service.DeleteBankAsync(id);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting bank: {ex.Message}");
            }
        }
    }
}
