using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.DataAccess
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
            : base(options) 
        {
            
        }
        
        public DbSet<WEXO_DEPOT> WEXO_DEPOT { get; set; }
            
    }
}
