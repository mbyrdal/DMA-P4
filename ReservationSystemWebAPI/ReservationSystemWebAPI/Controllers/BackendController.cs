using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Services;

namespace ReservationSystemWebAPI.Controllers
{
    /// <summary>
    /// Controller til håndtering af CRUD-operationer på udstyr (StorageItem) i databasen.<br/>
    /// Denne controller modtager HTTP-forespørgsler og videresender dem til <see cref="IStorageItemService"/>,
    /// som håndterer den egentlige forretningslogik og dataadgang via repositorylaget.<br/>
    /// Alle metoder er asynkrone og returnerer relevante HTTP-statuskoder.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BackendController : ControllerBase
    {
        private readonly IStorageItemService _storageItemService;

        public BackendController(IStorageItemService storageItemService)
        {
            // Use DI to instantiate and make use of the existing ReservationDbContext.
            // ReservationDbContext contains the DbSet<StorageItem> StorageItem property (access to the database).
            _storageItemService = storageItemService;
        }

        // GET: api/Backend
        /// <summary>
        /// Henter alt udstyr i databasen.<br/>
        /// Ved succes returneres listen af genstande.<br/>
        /// Ved fejl returneres en 500-statuskode med fejlbesked.
        /// </summary>
        /// <returns>En liste af <see cref="StorageItem"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StorageItem>>> GetItems()
        {
            var items = await _storageItemService.GetAllItemsAsync();
            return Ok(items);
        }

        // GET: api/Backend/5
        /// <summary>
        /// Henter et enkelt stykke udstyr baseret på ID.<br/>
        /// Returnerer 404 hvis ikke fundet.
        /// </summary>
        /// <param name="id">ID for genstanden der ønskes hentet.</param>
        /// <returns>Et <see cref="StorageItem"/>-objekt.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<StorageItem>> GetItem(int id)
        {
            try
            {
                StorageItem udstyr = await _storageItemService.GetItemByIdAsync(id);
                return Ok(udstyr);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/Backend
        /// <summary>
        /// Opretter et nyt stykke udstyr i databasen.<br/>
        /// Returnerer 201 Created ved succes.
        /// </summary>
        /// <param name="newItem">Den nye genstand der skal tilføjes.</param>
        /// <returns>Statuskode og oprettet genstand.</returns>
        [HttpPost]
        public async Task<ActionResult<StorageItem>> CreateItem([FromBody] StorageItem newItem)
        {
            try
            {
                await _storageItemService.AddItemAsync(newItem);
                return CreatedAtAction(nameof(GetItem), new { id = newItem.Id }, newItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        // PUT: api/Backend/5
        /// <summary>
        /// Opdaterer et eksisterende stykke udstyr.<br/>
        /// Returnerer 204 NoContent ved succes.
        /// </summary>
        /// <param name="id">ID på genstanden der skal opdateres.</param>
        /// <param name="updatedItem">De nye data for genstanden.</param>
        /// <returns>Statuskode afhængigt af resultatet.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] StorageItem updatedItem)
        {
            if (id != updatedItem.Id)
            {
                return BadRequest(new { message = "Mismatch på ID'er" });
            }

            try
            {
                await _storageItemService.UpdateItemAsync(updatedItem);
                return NoContent(); // Status 204 ved succes
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Opdatering mislykkedes"))
            {
                // Handle optimistic concurrency conflict
                return Conflict(new { message = ex.Message }); // Status 409 if concurrency conflict occurs
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/Backend/5
        /// <summary>
        /// Sletter udstyr baseret på ID.<br/>
        /// Returnerer 204 NoContent ved succes.
        /// </summary>
        /// <param name="id">ID på den genstand der ønskes slettet.</param>
        /// <returns>Statuskode afhængigt af resultatet.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                await _storageItemService.DeleteItemAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
