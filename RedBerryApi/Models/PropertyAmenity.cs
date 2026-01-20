using System;
using System.Text.Json.Serialization;

namespace RedBerryApi.Models
{
    public class PropertyAmenity
    {
        public int Id { get; set; }
        public int? PropertyId { get; set; }
        public int? AmenityId { get; set; }
        public bool? IsAvailable { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual PropertyListing PropertyListing { get; set; }
        [JsonIgnore]
        public virtual Amenity Amenity { get; set; }
    }
}
