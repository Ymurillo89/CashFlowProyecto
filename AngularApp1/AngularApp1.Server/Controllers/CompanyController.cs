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
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _service;

        public CompanyController(CompanyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCompany>>> GetCompanies()
        {
            try
            {
                var response = await _service.GetCompaniesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching companies: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetCompany>> GetCompany(long id)
        {
            try
            {
                var response = await _service.GetCompanyByIdAsync(id);
                if (response == null)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching company: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Result>> PostCompany([FromBody] PostCompany model)
        {
            try
            {
                var response = await _service.PostCompanyAsync(model);
                if (response.Success)
                    return Ok(response);
                
                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating company: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result>> PutCompany(long id, [FromBody] PostCompany model)
        {
            try
            {
                var response = await _service.UpdateCompanyAsync(id, model);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating company: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result>> DeleteCompany(long id)
        {
            try
            {
                var response = await _service.DeleteCompanyAsync(id);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting company: {ex.Message}");
            }
        }
    }
}
