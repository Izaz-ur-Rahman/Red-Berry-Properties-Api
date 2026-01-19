using System;

namespace RedBerryApi.Models
{
    public class PropertyAmenity
    {
        public int Id { get; set; }
        public int? PropertyId { get; set; }
        public int? AmenityId { get; set; }
        public bool? IsAvailable { get; set; }

        // Navigation properties
        public virtual PropertyListing PropertyListing { get; set; }
        public virtual Amenity Amenity { get; set; }
    }
}
