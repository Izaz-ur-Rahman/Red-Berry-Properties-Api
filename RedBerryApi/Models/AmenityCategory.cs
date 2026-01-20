using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace RedBerryApi.Models
{
    public class AmenityCategory
    {
        public AmenityCategory()
        {
            Amenities = new HashSet<Amenity>();
        }
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        // Navigation property
        [JsonIgnore]
        public virtual ICollection<Amenity> Amenities { get; set; }
    }
}
