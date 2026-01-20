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
        public DbSet<PropertyListing> PropertyListing { get; set; }
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<AmenityCategory> AmenityCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map the PropertyListing entity to the exact table name in DB
            modelBuilder.Entity<PropertyListing>().ToTable("PropertyListing"); // exact table name
                                                                               // PropertyAmenity → PropertyListing
            modelBuilder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.PropertyListing)
                .WithMany(p => p.PropertyAmenities)
                .HasForeignKey(pa => pa.PropertyId)    // ✅ use actual column name

                .OnDelete(DeleteBehavior.Cascade);     // optional

            // PropertyAmenity → Amenity
            modelBuilder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.Amenity)
                .WithMany(a => a.PropertyAmenities)
                .HasForeignKey(pa => pa.AmenityId)    // ✅ use actual column name
                .OnDelete(DeleteBehavior.Cascade);

            // Amenity → AmenityCategory
            modelBuilder.Entity<Amenity>()
                .HasOne(a => a.AmenityCategory)
                .WithMany(c => c.Amenities)
                .HasForeignKey(a => a.CategoryId);    // ✅ use actual column name
        }

    }
}
