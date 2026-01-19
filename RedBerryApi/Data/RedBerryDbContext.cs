using Microsoft.EntityFrameworkCore;
using RedBerryApi.Models;

namespace RedBerryApi.Data
{
    public class RedBerryDbContext : DbContext
    {
        public RedBerryDbContext(DbContextOptions<RedBerryDbContext> options)
            : base(options)
        {
        }

        // ADD YOUR TABLES HERE
        public DbSet<PropertyListing> PropertyListings { get; set; }
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<AmenityCategory> AmenityCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Fluent API configurations
            // modelBuilder.Entity<PropertyListing>().ToTable("PropertyListings");
        }
    }
}
