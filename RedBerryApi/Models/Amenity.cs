using System;
using System.Collections.Generic;

namespace RedBerryApi.Models
{
    public class Amenity
    {
        public Amenity()
        {
            PropertyAmenities = new HashSet<PropertyAmenity>();
        }

        public int AmenityId { get; set; }
        public string AmenityName { get; set; }
        public string Icon { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual AmenityCategory AmenityCategory { get; set; }
        public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; }
    }
}
