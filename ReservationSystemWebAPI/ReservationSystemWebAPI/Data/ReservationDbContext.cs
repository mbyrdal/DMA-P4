using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.DataAccess
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the reservation system.
    /// Provides access to all relevant entities and configures relationships and concurrency.
    /// </summary>
    public class ReservationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationDbContext"/> class
        /// with specified options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet representing bookable equipment (storage items).
        /// </summary>
        public DbSet<StorageItem> WEXO_DEPOT { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing the reservation history records.
        /// </summary>
        public DbSet<ReservationHistory> ReservationHistory { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing reservations.
        /// </summary>
        public DbSet<Reservation> Reservations { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing individual reservation items.
        /// </summary>
        public DbSet<ReservationItems> ReservationItems { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing system users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Configures the entity relationships and properties on model creation.
        /// Sets up cascade delete for reservation items and configures optimistic concurrency.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Reservation
            modelBuilder.Entity<Reservation>(entity =>
            {
                // Relationships
                entity.HasMany(r => r.Items)
                    .WithOne(i => i.Reservation)
                    .HasForeignKey(i => i.ReservationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Concurrency
                entity.Property(r => r.RowVersion)
                    .IsRowVersion();

                // Indexes
                entity.HasIndex(r => r.Email);
                entity.HasIndex(r => r.Status);
            });

            // Configure ReservationItems
            modelBuilder.Entity<ReservationItems>(entity =>
            {
                // Concurrency
                entity.Property(i => i.RowVersion)
                    .IsRowVersion();

                // Indexes
                entity.HasIndex(i => i.Equipment);

                // Data types
                entity.Property(i => i.Equipment)
                    .HasMaxLength(100);
            });

            // Configure ReservationHistory (if needed)
            modelBuilder.Entity<ReservationHistory>(entity =>
            {
                entity.HasIndex(h => h.ReservationId);
            });
        }

    }
}
