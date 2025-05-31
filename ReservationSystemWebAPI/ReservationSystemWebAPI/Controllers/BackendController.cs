using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Services;

namespace ReservationSystemWebAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling CRUD operations on StorageItem entities.
    /// Receives HTTP requests and forwards them to the <see cref="IStorageItemService"/>,
    /// which manages business logic and data access through the repository layer.
    /// All methods are asynchronous and return appropriate HTTP status codes.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BackendController : ControllerBase
    {
        private readonly IStorageItemService _storageItemService;

        public BackendController(IStorageItemService storageItemService)
        {
            // Dependency Injection of service layer that encapsulates business logic and data access.
            _storageItemService = storageItemService;
        }

        // GET: api/Backend
        /// <summary>
        /// Retrieves all storage items from the database.
        /// Returns 200 OK with the list of items on success.
        /// Returns 500 Internal Server Error on failure.
        /// </summary>
        /// <returns>List of <see cref="StorageItem"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StorageItem>>> GetItems()
        {
            var items = await _storageItemService.GetAllItemsAsync();
            return Ok(items);
        }

        // GET: api/Backend/5
        /// <summary>
        /// Retrieves a single storage item by ID.
        /// Returns 200 OK with the item if found.
        /// Returns 404 Not Found if the item does not exist.
        /// </summary>
        /// <param name="id">ID of the item.</param>
        /// <returns><see cref="StorageItem"/> object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<StorageItem>> GetItem(int id)
        {
            try
            {
                StorageItem item = await _storageItemService.GetItemByIdAsync(id);
                return Ok(item);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // "Ingen genstand fundet med det angivne ID."
            }
        }

        // POST: api/Backend
        /// <summary>
        /// Creates a new storage item in the database.
        /// Returns 201 Created on success with the Location header pointing to the new resource.
        /// Returns 400 Bad Request on validation error.
        /// Returns 500 Internal Server Error on unexpected failure.
        /// </summary>
        /// <param name="newItem">DTO containing data for the new item.</param>
        /// <returns>The newly created <see cref="StorageItem"/>.</returns>
        [HttpPost]
        public async Task<ActionResult<StorageItem>> CreateItem([FromBody] StorageItemCreateDto newItem)
        {
            try
            {
                await _storageItemService.AddItemAsync(newItem);
                return CreatedAtAction(nameof(GetItem), new { id = newItem.Id }, newItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // "Ugyldige data tilføjet."
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message }); // "Fejl ved oprettelse af genstand."
            }
        }

        // PUT: api/Backend/5
        /// <summary>
        /// Updates an existing storage item by ID.
        /// Returns 204 No Content on success.
        /// Returns 400 Bad Request if URL ID and data ID do not match or if RowVersion format is invalid.
        /// Returns 404 Not Found if the item does not exist.
        /// Returns 409 Conflict if a concurrency conflict occurs.
        /// Returns 500 Internal Server Error on unexpected failure.
        /// </summary>
        /// <param name="id">ID of the item to update.</param>
        /// <param name="dto">DTO with updated data including RowVersion for concurrency.</param>
        /// <returns>HTTP status code.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] StorageItemUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "ID i URL matcher ikke ID i data" });
            }

            try
            {
                await _storageItemService.UpdateItemAsync(dto);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Opdatering mislykkedes, da en anden bruger har lavet ændringer. Genindlæs data og prøv igen." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // "Genstanden blev ikke fundet."
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Ugyldig RowVersion format. Skal være Base64-kodet." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message }); // "Fejl ved opdatering af genstand."
            }
        }

        // DELETE: api/Backend/5
        /// <summary>
        /// Deletes a storage item by ID.
        /// Returns 204 No Content on success.
        /// Returns 404 Not Found if the item does not exist.
        /// Returns 500 Internal Server Error on unexpected failure.
        /// </summary>
        /// <param name="id">ID of the item to delete.</param>
        /// <returns>HTTP status code.</returns>
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
                return NotFound(new { message = ex.Message }); // "Genstanden blev ikke fundet."
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message }); // "Fejl ved sletning af genstand."
            }
        }
    }
}