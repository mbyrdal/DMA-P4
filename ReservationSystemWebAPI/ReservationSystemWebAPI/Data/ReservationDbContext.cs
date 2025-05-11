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
        
        public DbSet<StorageItem> WEXO_DEPOT { get; set; }
        public DbSet<ReservationHistory> ReservationHistory { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationItems> ReservationItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Items)
                .WithOne(i => i.Reservation)
                .HasForeignKey(i => i.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
