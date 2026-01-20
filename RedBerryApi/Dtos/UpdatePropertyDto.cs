using Microsoft.AspNetCore.Mvc;

namespace RedBerryApi.Dtos
{
    public class UpdatePropertyDto
    {
        [FromForm(Name = "Property")]
        public string Property { get; set; }              // JSON string for PropertyListing
        public IFormFile MainImage { get; set; }          // Optional new main image
        public List<IFormFile> SubImages { get; set; }    // Optional new sub-images
        public string ExistingMainImage { get; set; }     // Existing main image URL
        public string ExistingSubImage { get; set; }      // Existing sub-images URLs (comma-separated)

    }

}
