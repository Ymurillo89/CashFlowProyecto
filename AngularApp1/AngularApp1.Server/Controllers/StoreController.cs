using AngularApp1.Server.Models.Entities;
using AngularApp1.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngularApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetStores()
        {
            var stores = await _storeService.GetAllStoresAsync();
            return Ok(stores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(long id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        [HttpPost]
        public async Task<ActionResult<Store>> PostStore(Store store)
        {
            var createdStore = await _storeService.CreateStoreAsync(store);
            return CreatedAtAction(nameof(GetStore), new { id = createdStore.Id }, createdStore);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore(long id, Store store)
        {
            var updated = await _storeService.UpdateStoreAsync(id, store);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(long id)
        {
            var deleted = await _storeService.DeleteStoreAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
