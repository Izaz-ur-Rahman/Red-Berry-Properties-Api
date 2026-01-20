using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RedBerryApi.Dtos
{
    public class AddPropertyDto
    {
        // Basic info
        public string Title { get; set; }
        public string PropertyType { get; set; }
        public string ListingType { get; set; }
        public string PriceCurrency { get; set; }
        public int? Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string City { get; set; }
        public string Neighborhood { get; set; }

        // Optional/nullable fields
        public decimal? SalePrice { get; set; }
        public decimal? OffPrice { get; set; }
        public decimal? RentalPrice { get; set; }

        // Images
        public IFormFile MainImage { get; set; }
        public List<IFormFile> SubImages { get; set; }
    }

}
