using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Controllers
{
    // Route base for this controller
    [Route("api/[controller]")]
    [ApiController]
    public class BackendController : ControllerBase
    {
        private readonly ReservationDbContext _dbContext;

        public BackendController(ReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Backend
        // Get all items from the database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WEXO_DEPOT>>> GetItems()
        {
            var items = await _dbContext.WEXO_DEPOT.ToListAsync();
            return Ok(items);  // Return items with 200 OK status
        }

        // GET: api/Backend/5
        // Get a specific item by id
        [HttpGet("{id}")]
        public async Task<ActionResult<WEXO_DEPOT>> GetItem(int id)
        {
            var item = await _dbContext.WEXO_DEPOT.FindAsync(id);

            if (item == null)
            {
                return NotFound();  // Return 404 if item not found
            }

            return Ok(item);  // Return the item with 200 OK status
        }
    }
}
