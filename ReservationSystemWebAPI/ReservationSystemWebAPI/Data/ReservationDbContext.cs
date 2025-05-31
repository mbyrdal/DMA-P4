using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.DataAccess
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the reservation system.
    /// Provides access to entity sets and configures relationships, constraints, and concurrency behavior.
    /// </summary>
    public class ReservationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationDbContext"/> class
        /// using the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the entity set for bookable storage items (equipment).
        /// </summary>
        public DbSet<StorageItem> WEXO_DEPOT { get; set; }

        /// <summary>
        /// Gets or sets the entity set for reservation history records.
        /// </summary>
        public DbSet<ReservationHistory> ReservationHistory { get; set; }

        /// <summary>
        /// Gets or sets the entity set for reservations.
        /// </summary>
        public DbSet<Reservation> Reservations { get; set; }

        /// <summary>
        /// Gets or sets the entity set for individual reservation items.
        /// </summary>
        public DbSet<ReservationItems> ReservationItems { get; set; }

        /// <summary>
        /// Gets or sets the entity set for system users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Configures entity mappings, relationships, constraints, and concurrency settings.
        /// </summary>
        /// <param name="modelBuilder">Provides a simple API for configuring entity types and relationships.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Reservation entity
            modelBuilder.Entity<Reservation>(entity =>
            {
                // Relationships
                entity.HasMany(r => r.Items)
                      .WithOne(i => i.Reservation)
                      .HasForeignKey(i => i.ReservationId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Concurrency token
                entity.Property(r => r.RowVersion)
                      .IsRowVersion();

                // Indexes
                entity.HasIndex(r => r.Email);
                entity.HasIndex(r => r.Status);
            });

            // Configure ReservationItems entity
            modelBuilder.Entity<ReservationItems>(entity =>
            {
                // Concurrency token
                entity.Property(i => i.RowVersion)
                      .IsRowVersion();

                // Indexes
                entity.HasIndex(i => i.Equipment);

                // Field constraints
                entity.Property(i => i.Equipment)
                      .HasMaxLength(100);
            });

            // Configure ReservationHistory entity
            modelBuilder.Entity<ReservationHistory>(entity =>
            {
                entity.HasIndex(h => h.ReservationId);
            });
        }
    }
}