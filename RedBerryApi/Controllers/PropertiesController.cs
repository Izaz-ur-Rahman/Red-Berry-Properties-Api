using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RedBerryApi.Data;
using RedBerryApi.Models;

namespace RedBerryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly RedBerryDbContext _db;

        public PropertiesController(RedBerryDbContext db)
        {
            _db = db;
        }

        // =========================
        // Get Properties with filters
        // =========================
        [HttpGet("GetProperties")]
        public async Task<IActionResult> GetProperties(
            int pageNumber = 1,
            int pageSize = 6,
            string listingType = null,
            string city = null,
            string propertyType = null,
            string price = null,
            int? bedrooms = null)
        {
            try
            {
                var q = _db.PropertyListings.AsQueryable();

                if (!string.IsNullOrWhiteSpace(listingType))
                    q = q.Where(p => p.ListingType == listingType);

                if (!string.IsNullOrWhiteSpace(city))
                    q = q.Where(p => p.City == city);

                if (!string.IsNullOrWhiteSpace(propertyType))
                    q = q.Where(p => p.PropertyType == propertyType);

                if (bedrooms.HasValue)
                {
                    q = bedrooms == 3
                        ? q.Where(p => p.Bedrooms >= 3 || p.VillaBedrooms >= 3)
                        : q.Where(p => p.Bedrooms == bedrooms || p.VillaBedrooms == bedrooms);
                }

                if (!string.IsNullOrWhiteSpace(price) && price.Contains('-'))
                {
                    var parts = price.Split('-');
                    if (decimal.TryParse(parts[0], out var min))
                        q = q.Where(p => p.SalePrice >= min || p.RentalPrice >= min || p.OffPrice >= min);
                    if (decimal.TryParse(parts[1], out var max))
                        q = q.Where(p => p.SalePrice <= max || p.RentalPrice <= max || p.OffPrice <= max);
                }

                var totalCount = await q.CountAsync();
                var data = await q
                    .OrderByDescending(p => p.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new { TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize, Data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // =========================
        // Get Property by Slug
        // =========================
        [HttpGet("GetPropertyBySlug")]
        public async Task<IActionResult> GetPropertyBySlug(string name)
        {
            try
            {
                var property = await _db.PropertyListings
                    .Include(p => p.PropertyAmenities)
                        .ThenInclude(pa => pa.Amenity)
                            .ThenInclude(a => a.AmenityCategory)
                    .Where(p => p.PropertySlug.Trim() == name.Trim())
                    .Select(p => new
                    {
                        p.Id,
                        p.Title,
                        p.PropertyType,
                        p.ListingType,
                        p.PriceCurrency,
                        p.MainImage,
                        p.SubImage,
                        p.PaymentPlanAvailable,
                        p.PropertySlug,
                        p.PropertyCategory,
                        Amenities = p.PropertyAmenities.Select(pa => new
                        {
                            pa.AmenityId,
                            pa.IsAvailable,
                            pa.Amenity.Icon,
                            pa.Amenity.AmenityName,
                            Category = pa.Amenity.AmenityCategory.CategoryName
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (property == null) return NotFound();
                return Ok(property);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // =========================
        // Get Property by ID
        // =========================
        [HttpGet("GetPropertyById/{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            try
            {
                var property = await _db.PropertyListings.FindAsync(id);
                if (property == null) return NotFound();
                return Ok(property);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // =========================
        // Add Property
        // =========================
        [HttpPost("AddProperty")]
        public async Task<IActionResult> AddProperty([FromForm] string property,
                                                     [FromForm] IFormFile mainImage,
                                                     [FromForm] List<IFormFile> subImages)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<PropertyListing>(property);

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/RedBerryFiles/Propertiesfiles");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                // Main Image
                if (mainImage != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);
                    var path = Path.Combine(uploadPath, fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await mainImage.CopyToAsync(stream);
                    model.MainImage = "/RedBerryFiles/Propertiesfiles/" + fileName;
                }

                // Sub Images
                if (subImages != null && subImages.Count > 0)
                {
                    var subPaths = new List<string>();
                    foreach (var file in subImages.Take(4))
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(uploadPath, fileName);
                        using var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        subPaths.Add("/RedBerryFiles/Propertiesfiles/" + fileName);
                    }
                    model.SubImage = string.Join(",", subPaths);
                }

                model.CreatedDate = DateTime.UtcNow;
                model.Status = "Active";
                model.Off_plan = model.ListingType == "Off-Plan";
                model.Property_purpose = model.ListingType == "For Rent" ? "Rent" : "Sell";

                _db.PropertyListings.Add(model);
                await _db.SaveChangesAsync();

                return Ok(new { PropertyId = model.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // =========================
        // Update Property
        // =========================
        [HttpPost("UpdateProperty")]
        public async Task<IActionResult> UpdateProperty([FromForm] string property,
                                                        [FromForm] IFormFile mainImage,
                                                        [FromForm] List<IFormFile> subImages,
                                                        [FromForm] string existingMainImage,
                                                        [FromForm] string existingSubImage)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<PropertyListing>(property);

                var existingProperty = await _db.PropertyListings.FindAsync(model.Id);
                if (existingProperty == null) return NotFound("Property not found");

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/RedBerryFiles/Propertiesfiles");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                // Main Image
                if (mainImage != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);
                    var path = Path.Combine(uploadPath, fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await mainImage.CopyToAsync(stream);
                    existingProperty.MainImage = "/RedBerryFiles/Propertiesfiles/" + fileName;
                }
                else
                {
                    existingProperty.MainImage = existingMainImage;
                }

                // Sub Images
                if (subImages != null && subImages.Count > 0)
                {
                    var subPaths = new List<string>();
                    foreach (var file in subImages.Take(4))
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(uploadPath, fileName);
                        using var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        subPaths.Add("/RedBerryFiles/Propertiesfiles/" + fileName);
                    }
                    existingProperty.SubImage = string.Join(",", subPaths);
                }
                else
                {
                    existingProperty.SubImage = existingSubImage;
                }

                // TODO: Update other fields (copy all properties from model to existingProperty)
                _db.Entry(existingProperty).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return Ok(new { PropertyId = model.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // =========================
        // Delete Property
        // =========================
        [HttpDelete("DeleteProperty/{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            try
            {
                var property = await _db.PropertyListings.FindAsync(id);
                if (property == null) return NotFound();

                var relatedAmenities = _db.PropertyAmenities.Where(pa => pa.PropertyId == id);
                _db.PropertyAmenities.RemoveRange(relatedAmenities);

                _db.PropertyListings.Remove(property);
                await _db.SaveChangesAsync();

                return Ok("Deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // =========================
        // Update Property Amenity
        // =========================
        [HttpPost("UpdatePropertyAmenity")]
        public async Task<IActionResult> UpdatePropertyAmenity([FromBody] List<PropertyAmenity> model)
        {
            try
            {
                if (model == null || !model.Any())
                    return BadRequest("No amenities provided");

                int propertyId = model.First().PropertyId.Value;

                var existingAmenities = _db.PropertyAmenities.Where(pa => pa.PropertyId == propertyId);
                _db.PropertyAmenities.RemoveRange(existingAmenities);

                await _db.PropertyAmenities.AddRangeAsync(model);
                await _db.SaveChangesAsync();

                return Ok("Property Amenities Updated");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // =========================
        // Get Amenities by PropertyId
        // =========================
        [HttpGet("GetAmenityByPropertyId/{id}")]
        public async Task<IActionResult> GetAmenityByPropertyId(int id)
        {
            try
            {
                var amenities = await _db.PropertyAmenities
                    .Where(pa => pa.PropertyId == id)
                    .Include(pa => pa.Amenity)
                    .Select(pa => new
                    {
                        pa.AmenityId,
                        pa.IsAvailable,
                        pa.Amenity.AmenityName
                    })
                    .ToListAsync();

                return Ok(amenities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // =========================
        // Off Plan Properties
        // =========================
        [HttpGet("GetOffPlanProperties")]
        public async Task<IActionResult> GetOffPlanProperties(
            int pageNumber = 1,
            int pageSize = 6,
            string city = null,
            string propertyType = null,
            string price = null,
            int? bedrooms = null)
        {
            try
            {
                var q = _db.PropertyListings
                    .Where(p => p.ListingType == "Off-Plan")
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(city))
                    q = q.Where(p => p.City == city);

                if (!string.IsNullOrWhiteSpace(propertyType))
                    q = q.Where(p => p.PropertyType == propertyType);

                if (bedrooms.HasValue)
                    q = bedrooms == 3
                        ? q.Where(p => p.Bedrooms >= 3 || p.VillaBedrooms >= 3)
                        : q.Where(p => p.Bedrooms == bedrooms || p.VillaBedrooms == bedrooms);

                if (!string.IsNullOrWhiteSpace(price))
                {
                    if (price == "low")
                        q = q.OrderByDescending(p => p.SalePrice);
                    else
                        q = q.OrderBy(p => p.SalePrice);
                }

                var totalCount = await q.CountAsync();
                var data = await q
                    .OrderByDescending(p => p.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new { TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize, Data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
