using System;
using System.Collections.Generic;

namespace RedBerryApi.Models
{
    public class AmenityCategory
    {
        public AmenityCategory()
        {
            Amenities = new HashSet<Amenity>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        // Navigation property
        public virtual ICollection<Amenity> Amenities { get; set; }
    }
}
