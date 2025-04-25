using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Controllers
{
    /// <summary>
    /// Controller for managing StorageItem items.<br/>
    /// It allows for CRUD operations on items in storage, ie. CREATE, READ, UPDATE and DELETE.<br/>
    /// Methods are conded to work asynchronously.<br/>
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BackendController : ControllerBase
    {
        private readonly ReservationDbContext _dbContext;

        public BackendController(ReservationDbContext dbContext)
        {
            // Use DI to instantiate and make use of the existing ReservationDbContext.
            // ReservationDbContext contains the DbSet<StorageItem> StorageItem property (access to the database).
            _dbContext = dbContext;
        }

        // GET: api/Backend
        /// <summary>
        /// This method retrieves all items from the StorageItem table.<br/>
        /// <returns>Returns a list of all StorageItem items in its respective Item table.</returns>
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StorageItem>>> GetItems() // TODO: Add proper error handling.
        {
            var items = await _dbContext.WEXO_DEPOT.ToListAsync();
            return Ok(items);
        }

        // GET: api/Backend/5
        /// <summary>
        /// This method retrieves a specific item from the StorageItem table based on its identifier (ID).
        /// </summary>
        /// <param name="id">The identifier used to categorize its position in the table.</param>
        /// <returns>A StorageItem item.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<StorageItem>> GetItem(int id) // TODO: Add proper error handling.
        {
            var item = await _dbContext.WEXO_DEPOT.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST: api/Backend
        /// <summary>
        /// This method creates a new item instance in the StorageItem table.<br/>
        /// The details of the item are provided in the request body.<br/>
        /// </summary>
        /// <param name="newItem">The request body to be used to create a new instance.</param>
        /// <returns>A status code depending on the outcome of the method. 200 OK if successful.</returns>
        [HttpPost]
        public async Task<ActionResult<StorageItem>> CreateItem([FromBody] StorageItem newItem) // TODO: Add proper error handling.
        {
            _dbContext.WEXO_DEPOT.Add(newItem);
            await _dbContext.SaveChangesAsync(); // SQL INSERT

            return CreatedAtAction(nameof(GetItem), new { id = newItem.Id }, newItem);
        }

        // PUT: api/Backend/5
        /// <summary>
        /// This method updates an existing item in the StorageItem table.<br/>
        /// The updated details are provided by the request body.<br/>
        /// </summary>
        /// <param name="id">ID of the existing item to be updated.</param>
        /// <param name="updatedItem">The request body containing the new details to be used.</param>
        /// <returns>A status code depending on the outcome of the method. <br/>.
        /// 204 NoContent if successful.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] StorageItem updatedItem) // TODO: Add proper error handling.
        {
            if (id != updatedItem.Id)
            {
                return BadRequest("ID mismatch.");
            }

            // EntityState is used to mark changes to the entity/database.
            // Modified: An item has been updated in the database, hence SQL UPDATE.
            _dbContext.Entry(updatedItem).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync(); // SQL UPDATE
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.WEXO_DEPOT.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Backend/5
        /// <summary>
        /// This method deletes an existing item in the database.
        /// </summary>
        /// <param name="id">The identifier of an existing item entry to be deleted.</param>
        /// <returns>A status code depending on the outcome.<br/>
        /// 204 NoContent if successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id) // TODO: Add proper error handling.
        {
            var item = await _dbContext.WEXO_DEPOT.FindAsync(id);
            if (item == null) return NotFound();

            _dbContext.WEXO_DEPOT.Remove(item);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
