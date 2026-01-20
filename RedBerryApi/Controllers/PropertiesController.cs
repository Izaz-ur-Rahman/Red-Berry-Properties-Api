using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RedBerryApi.Data;
using RedBerryApi.Dtos;
using RedBerryApi.Models;

namespace RedBerryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly RedBerryDbContext _db;
        private readonly IWebHostEnvironment _env;

        public PropertiesController(RedBerryDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: api/Properties/GetProperties
        [HttpGet("GetProperties")]
        public IActionResult GetProperties(int pageNumber = 1,
                                           int pageSize = 6,
                                           string listingType = null,
                                           string city = null,
                                           string propertyType = null,
                                           string price = null,
                                           int? bedrooms = null)
        {
            try
            {
                var query = _db.PropertyListing.AsQueryable();

                if (!string.IsNullOrWhiteSpace(listingType))
                    query = query.Where(p => p.ListingType == listingType);

                if (!string.IsNullOrWhiteSpace(city))
                    query = query.Where(p => p.City == city);

                if (!string.IsNullOrWhiteSpace(propertyType))
                    query = query.Where(p => p.PropertyType == propertyType);

                if (bedrooms.HasValue)
                    query = bedrooms == 3
                        ? query.Where(p => p.Bedrooms >= 3 || p.VillaBedrooms >= 3)
                        : query.Where(p => p.Bedrooms == bedrooms || p.VillaBedrooms == bedrooms);

                if (!string.IsNullOrWhiteSpace(price) && price.Contains('-'))
                {
                    var parts = price.Split('-');
                    if (decimal.TryParse(parts[0], out var min))
                        query = query.Where(p => p.SalePrice >= min || p.RentalPrice >= min || p.OffPrice >= min);
                    if (decimal.TryParse(parts[1], out var max))
                        query = query.Where(p => p.SalePrice <= max || p.RentalPrice <= max || p.OffPrice <= max);
                }

                query = query.OrderByDescending(p => p.Id);
                var totalCount = query.Count();
                var data = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                // You can log using your own logging mechanism
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Properties/GetPropertyBySlug?name=slug
        [HttpGet("GetPropertyBySlug")]
        public IActionResult GetPropertyBySlug(string name)
        {
            try
            {
                var property = _db.PropertyListing
                    .Include(p => p.PropertyAmenities)
                        .ThenInclude(pa => pa.Amenity)
                            .ThenInclude(a => a.AmenityCategory)
                            .Where(p => p.PropertySlug != null && p.PropertySlug.Trim() == name.Trim())

                    //.Where(p => p.PropertySlug.Trim() == name.Trim())
                    .Select(property => new
                    {
                        property.Id,
                        property.Title,
                        property.PropertyType,
                        property.ListingType,
                        property.PriceCurrency,
                        property.MainImage,
                        property.SubImage,
                        property.PaymentPlanAvailable,
                        property.PropertySlug,
                        property.PropertyCategory,
                        property.DeveloperName,
                        property.HandoverDate,
                        property.PaymentPlanStructure,
                        property.PostHandoverPayment,
                        property.ConstructionStatus,
                        property.ModelUnitImages,
                        property.ServiceCharges,
                        property.ShowroomAvailable,
                        property.PropertyAge,
                        property.TenancyStatus,
                        property.MortgageStatus,
                        property.PreviousOwnership,
                        property.TitleDeedAvailable,
                        property.RentalPrice,
                        property.PaymentTerms,
                        property.ChequesAccepted,
                        property.CommissionFee,
                        property.TenancyContractLength,
                        property.VacatingNoticePeriod,
                        property.Deposit,
                        property.Bedrooms,
                        property.Bathrooms,
                        property.FloorLevel,
                        property.BalconyTerrace,
                        property.PropertyView,
                        property.BuiltUpArea,
                        property.ParkingSpaces,
                        property.KitchenType,
                        property.MaidsRoom,
                        property.SmartHomeFeatures,
                        property.StorageRoom,
                        property.PlotSize,
                        property.PrivateGarden,
                        property.PrivatePool,
                        property.Furnished,
                        property.UnitType,
                        property.PantryKitchen,
                        property.Washroom,
                        property.LicenseTypeSupport,
                        property.SwimmingPool,
                        property.CommunityName,
                        property.City,
                        property.Neighborhood,
                        property.ProximityToMetro,
                        property.NearbyLandmarks,
                        property.SchoolHospitalProximity,
                        property.ROI,
                        property.RentalYield,
                        property.AnnualServiceCharges,
                        property.MortgageCalculator,
                        property.RentalPaymentOptions,
                        property.ListingId,
                        property.RealEstateAgencyName,
                        property.RERAPermitNumber,
                        property.PaymentMethod,
                        property.DLDTransferFee,
                        property.CoolingSystem,
                        property.PetPolicy,
                        property.TenancyContractStatus,
                        property.LeaseExpiryDate,
                        property.BoostListing,
                        property.PriorityPlacement,
                        property.CreatedDate,
                        property.ModifiedDate,
                        property.CreatedBy,
                        property.OffPrice,
                        property.SalePrice,
                        property.VillaBedrooms,
                        property.VillaBathrooms,
                        property.VillaBuiltUpArea,
                        property.VillaParkingSpaces,
                        property.VillaFloorLevel,
                        property.VillaFurnished,
                        property.VillaMaidsRoom,
                        property.VillaDeveloperName,
                        property.CommercialBuiltUpArea,
                        property.CommercialDeveloperName,
                        property.CommercialParkingSpaces,
                        property.CommercialFloorLevel,
                        property.CommercialFurnished,
                        property.Description,
                        property.PaymentPlanAvailableDescription,
                        property.ChequesAcceptedRental,
                        property.Property_Title_AR,
                        property.Property_Size,
                        property.Listing_Agent,
                        property.Listing_Agent_Phone,
                        property.Listing_Agent_Email,

                        Amenities = property.PropertyAmenities.Select(pa => new
                        {
                            pa.AmenityId,
                            pa.IsAvailable,
                            Icon = pa.Amenity.Icon,
                            AmenityName = pa.Amenity.AmenityName,
                            Category = pa.Amenity.AmenityCategory.CategoryName
                        }).ToList()
                    })
                    .FirstOrDefault();

                return Ok(property);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Properties/GetPropertyById/5
        [HttpGet("GetPropertyById/{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            var property = await _db.PropertyListing
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
                return NotFound("Property not found.");

            var result = new
            {
                property.Id,
                property.Title,
                property.PropertyType,
                property.ListingType,
                property.PriceCurrency,
                property.Bedrooms,
                property.Bathrooms,
                property.City,
                property.Neighborhood,

                MainImage = string.IsNullOrEmpty(property.MainImage)
                    ? null
                    : property.MainImage,

                SubImages = string.IsNullOrEmpty(property.SubImage)
                    ? new List<string>()
                    : property.SubImage.Split(',').ToList(),

                property.Description,
                property.Status,
                property.CreatedDate
            };

            return Ok(result);
        }

        //[HttpGet("GetPropertyById/{id}")]
        //public IActionResult GetPropertyById(int id)
        //{
        //    try
        //    {
        //        var property = _db.PropertyListing.Find(id);
        //        return Ok(property);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // POST: api/Properties/AddProperty
        //[HttpPost("AddProperty")]
        //[DisableRequestSizeLimit]

        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> AddProperty([FromForm] string property, [FromForm] IFormFile mainImage, [FromForm] List<IFormFile> subImages)

        //    {
        //    try
        //    {
        //        var model = JsonConvert.DeserializeObject<PropertyListing>(property);
        //        var uploadPath = Path.Combine(_env.WebRootPath, "RedBerryFiles", "Propertiesfiles");

        //        if (!Directory.Exists(uploadPath))
        //            Directory.CreateDirectory(uploadPath);

        //        // Main image
        //        if (mainImage != null)
        //        {
        //            var mainFileName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);
        //            var mainImagePath = Path.Combine(uploadPath, mainFileName);
        //            using (var stream = new FileStream(mainImagePath, FileMode.Create))
        //            {
        //                await mainImage.CopyToAsync(stream);
        //            }
        //            model.MainImage = "/RedBerryFiles/Propertiesfiles/" + mainFileName;
        //        }

        //        // Sub images
        //        var subImagePaths = new List<string>();
        //        if (subImages != null && subImages.Any())
        //        {
        //            foreach (var file in subImages.Take(4))
        //            {
        //                var subFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        //                var subImagePath = Path.Combine(uploadPath, subFileName);
        //                using (var stream = new FileStream(subImagePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(stream);
        //                }
        //                subImagePaths.Add("/RedBerryFiles/Propertiesfiles/" + subFileName);
        //            }
        //            model.SubImage = string.Join(",", subImagePaths);
        //        }

        //        model.CreatedDate = DateTime.UtcNow;
        //        model.Status = "Active";
        //        model.Off_plan = model.ListingType == "Off-Plan";
        //        model.Property_purpose = model.ListingType == "For Rent" ? "Rent" : "Sell";

        //        _db.PropertyListing.Add(model);
        //        _db.SaveChanges();

        //        return Ok(new { PropertyId = model.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpPost("AddProperty")]
        //[DisableRequestSizeLimit]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> AddProperty([FromForm] AddPropertyDto dto)
        //{
        //    try
        //    {
        //        var model = JsonConvert.DeserializeObject<PropertyListing>(dto.Property);

        //        var uploadPath = Path.Combine(_env.WebRootPath, "RedBerryFiles", "Propertiesfiles");
        //        if (!Directory.Exists(uploadPath))
        //            Directory.CreateDirectory(uploadPath);

        //        // Main image
        //        if (dto.MainImage != null)
        //        {
        //            var mainFileName = Guid.NewGuid() + Path.GetExtension(dto.MainImage.FileName);
        //            var mainImagePath = Path.Combine(uploadPath, mainFileName);
        //            using (var stream = new FileStream(mainImagePath, FileMode.Create))
        //            {
        //                await dto.MainImage.CopyToAsync(stream);
        //            }
        //            model.MainImage = "/RedBerryFiles/Propertiesfiles/" + mainFileName;
        //        }

        //        // Sub images
        //        var subImagePaths = new List<string>();
        //        if (dto.SubImages != null && dto.SubImages.Any())
        //        {
        //            foreach (var file in dto.SubImages.Take(4))
        //            {
        //                var subFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        //                var subImagePath = Path.Combine(uploadPath, subFileName);
        //                using (var stream = new FileStream(subImagePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(stream);
        //                }
        //                subImagePaths.Add("/RedBerryFiles/Propertiesfiles/" + subFileName);
        //            }
        //            model.SubImage = string.Join(",", subImagePaths);
        //        }

        //        model.CreatedDate = DateTime.UtcNow;
        //        model.Status = "Active";
        //        model.Off_plan = model.ListingType == "Off-Plan";
        //        model.Property_purpose = model.ListingType == "For Rent" ? "Rent" : "Sell";

        //        _db.PropertyListing.Add(model);
        //        _db.SaveChanges();

        //        return Ok(new { PropertyId = model.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpPost("AddProperty")]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProperty([FromForm] AddPropertyDto dto)
        {
            try
            {
                var model = new PropertyListing
                {
                    Title = dto.Title,
                    PropertyType = dto.PropertyType,
                    ListingType = dto.ListingType,
                    PriceCurrency = dto.PriceCurrency,

                    Bedrooms = dto.Bedrooms,
                    Bathrooms = dto.Bathrooms,
                    City = dto.City,
                    Neighborhood = dto.Neighborhood,
                    CreatedDate = DateTime.UtcNow,
                    Status = "Active",
                    Off_plan = dto.ListingType == "Off-Plan",
                    Property_purpose = dto.ListingType == "For Rent" ? "Rent" : "Sell",

                    // ✅ FIX: Generate slug
                    PropertySlug = GenerateUniqueSlug(dto.Title)
                };

                var uploadPath = Path.Combine(_env.WebRootPath, "RedBerryFiles", "Propertiesfiles");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // ===== MAIN IMAGE =====
                if (dto.MainImage != null)
                {
                    var mainFileName = Guid.NewGuid() + Path.GetExtension(dto.MainImage.FileName);
                    var mainImagePath = Path.Combine(uploadPath, mainFileName);

                    using (var stream = new FileStream(mainImagePath, FileMode.Create))
                    {
                        await dto.MainImage.CopyToAsync(stream);
                    }

                    model.MainImage = "/RedBerryFiles/Propertiesfiles/" + mainFileName;
                }

                // ===== SUB IMAGES =====
                if (dto.SubImages != null && dto.SubImages.Any())
                {
                    var subImagePaths = new List<string>();

                    foreach (var file in dto.SubImages.Take(4))
                    {
                        var subFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var subImagePath = Path.Combine(uploadPath, subFileName);

                        using (var stream = new FileStream(subImagePath, FileMode.Create))
                            await file.CopyToAsync(stream);

                        subImagePaths.Add("/RedBerryFiles/Propertiesfiles/" + subFileName);
                    }

                    model.SubImage = string.Join(",", subImagePaths);
                }

                _db.PropertyListing.Add(model);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    PropertyId = model.Id,
                    Slug = model.PropertySlug   // 👈 return slug for frontend
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        //[HttpPost("AddProperty")]
        //[DisableRequestSizeLimit]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> AddProperty([FromForm] AddPropertyDto dto)
        //{
        //    try
        //    {
        //        var model = new PropertyListing
        //        {
        //            Title = dto.Title,
        //            PropertyType = dto.PropertyType,
        //            ListingType = dto.ListingType,
        //            PriceCurrency = dto.PriceCurrency,

        //            Bedrooms = dto.Bedrooms,
        //            Bathrooms = dto.Bathrooms,
        //            City = dto.City,
        //            Neighborhood = dto.Neighborhood,
        //            CreatedDate = DateTime.UtcNow,
        //            Status = "Active",
        //            Off_plan = dto.ListingType == "Off-Plan",
        //            Property_purpose = dto.ListingType == "For Rent" ? "Rent" : "Sell"
        //        };

        //        var uploadPath = Path.Combine(_env.WebRootPath, "RedBerryFiles", "Propertiesfiles");
        //        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        //        // Main image
        //        if (dto.MainImage != null)
        //        {
        //            var mainFileName = Guid.NewGuid() + Path.GetExtension(dto.MainImage.FileName);
        //            var mainImagePath = Path.Combine(uploadPath, mainFileName);
        //            using (var stream = new FileStream(mainImagePath, FileMode.Create))
        //            {
        //                await dto.MainImage.CopyToAsync(stream);
        //            }
        //            model.MainImage = "/RedBerryFiles/Propertiesfiles/" + mainFileName;
        //        }

        //        // Sub images
        //        if (dto.SubImages != null && dto.SubImages.Any())
        //        {
        //            var subImagePaths = new List<string>();
        //            foreach (var file in dto.SubImages.Take(4))
        //            {
        //                var subFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        //                var subImagePath = Path.Combine(uploadPath, subFileName);
        //                using (var stream = new FileStream(subImagePath, FileMode.Create))
        //                    await file.CopyToAsync(stream);

        //                subImagePaths.Add("/RedBerryFiles/Propertiesfiles/" + subFileName);
        //            }
        //            model.SubImage = string.Join(",", subImagePaths);
        //        }

        //        _db.PropertyListing.Add(model);
        //        _db.SaveChanges();

        //        return Ok(new { PropertyId = model.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("UpdateProperty")]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProperty([FromForm] UpdatePropertyDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Property))
                    return BadRequest("Property JSON is required.");

                // Deserialize JSON
                //var model = JsonConvert.DeserializeObject<PropertyListing>(dto.Property);
                if (string.IsNullOrWhiteSpace(dto.Property))
                    return BadRequest("Property JSON is missing.");

                PropertyListing model;
                try
                {
                    model = JsonConvert.DeserializeObject<PropertyListing>(dto.Property);
                }
                catch (Exception ex)
                {
                    return BadRequest("Invalid Property JSON: " + ex.Message);
                }

                var existingProperty = await _db.PropertyListing.FindAsync(model.Id);
                if (existingProperty == null)
                    return NotFound("Property not found");

                var uploadPath = Path.Combine(_env.WebRootPath, "RedBerryFiles", "Propertiesfiles");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // ===== MAIN IMAGE =====
                if (dto.MainImage != null)
                {
                    var mainFileName = Guid.NewGuid() + Path.GetExtension(dto.MainImage.FileName);
                    var mainImagePath = Path.Combine(uploadPath, mainFileName);

                    using (var stream = new FileStream(mainImagePath, FileMode.Create))
                        await dto.MainImage.CopyToAsync(stream);

                    existingProperty.MainImage = "/RedBerryFiles/Propertiesfiles/" + mainFileName;
                }
                else
                {
                    existingProperty.MainImage = dto.ExistingMainImage;
                }

                // ===== SUB IMAGES =====
                if (dto.SubImages != null && dto.SubImages.Any())
                {
                    var subImagePaths = new List<string>();

                    foreach (var file in dto.SubImages.Take(4))
                    {
                        var subFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var subImagePath = Path.Combine(uploadPath, subFileName);

                        using (var stream = new FileStream(subImagePath, FileMode.Create))
                            await file.CopyToAsync(stream);

                        subImagePaths.Add("/RedBerryFiles/Propertiesfiles/" + subFileName);
                    }

                    existingProperty.SubImage = string.Join(",", subImagePaths);
                }
                else
                {
                    existingProperty.SubImage = dto.ExistingSubImage;
                }

                // ===== COPY ALL FIELDS (OLD PROJECT STYLE) =====

                existingProperty.Title = model.Title;
                existingProperty.PropertyType = model.PropertyType;
                existingProperty.ListingType = model.ListingType;
                existingProperty.PriceCurrency = model.PriceCurrency;

                existingProperty.PaymentPlanAvailable = model.PaymentPlanAvailable;
                existingProperty.PropertySlug = model.PropertySlug;
                existingProperty.PreviousOwnership = model.PreviousOwnership;
                existingProperty.TitleDeedAvailable = model.TitleDeedAvailable;

                existingProperty.UnitType = model.UnitType;
                existingProperty.PantryKitchen = model.PantryKitchen;
                existingProperty.LicenseTypeSupport = model.LicenseTypeSupport;
                existingProperty.Washroom = model.Washroom;

                existingProperty.Bedrooms = model.Bedrooms;
                existingProperty.Bathrooms = model.Bathrooms;
                existingProperty.BuiltUpArea = model.BuiltUpArea;
                existingProperty.PlotSize = model.PlotSize;
                existingProperty.Furnished = model.Furnished;
                existingProperty.FloorLevel = model.FloorLevel;
                existingProperty.PropertyView = model.PropertyView;
                existingProperty.KitchenType = model.KitchenType;
                existingProperty.ParkingSpaces = model.ParkingSpaces;
                existingProperty.StorageRoom = model.StorageRoom;
                existingProperty.SmartHomeFeatures = model.SmartHomeFeatures;

                existingProperty.DeveloperName = model.DeveloperName;
                existingProperty.HandoverDate = model.HandoverDate;
                existingProperty.PaymentPlanStructure = model.PaymentPlanStructure;
                existingProperty.PostHandoverPayment = model.PostHandoverPayment;
                existingProperty.PaymentTerms = model.PaymentTerms;
                existingProperty.CommissionFee = model.CommissionFee;

                existingProperty.TenancyContractLength = model.TenancyContractLength;
                existingProperty.VacatingNoticePeriod = model.VacatingNoticePeriod;
                existingProperty.Deposit = model.Deposit;
                existingProperty.ShowroomAvailable = model.ShowroomAvailable;

                existingProperty.MaidsRoom = model.MaidsRoom;
                existingProperty.BalconyTerrace = model.BalconyTerrace;
                existingProperty.CommunityName = model.CommunityName;
                existingProperty.City = model.City;
                existingProperty.Neighborhood = model.Neighborhood;

                existingProperty.ProximityToMetro = model.ProximityToMetro;
                existingProperty.NearbyLandmarks = model.NearbyLandmarks;
                existingProperty.SchoolHospitalProximity = model.SchoolHospitalProximity;
                existingProperty.SwimmingPool = model.SwimmingPool;

                existingProperty.ROI = model.ROI;
                existingProperty.RentalYield = model.RentalYield;
                existingProperty.AnnualServiceCharges = model.AnnualServiceCharges;
                existingProperty.MortgageCalculator = model.MortgageCalculator;
                existingProperty.RentalPaymentOptions = model.RentalPaymentOptions;

                existingProperty.ChequesAccepted = model.ChequesAccepted;
                existingProperty.ListingId = model.ListingId;
                existingProperty.RealEstateAgencyName = model.RealEstateAgencyName;
                existingProperty.RERAPermitNumber = model.RERAPermitNumber;
                existingProperty.PaymentMethod = model.PaymentMethod;
                existingProperty.DLDTransferFee = model.DLDTransferFee;
                existingProperty.CoolingSystem = model.CoolingSystem;
                existingProperty.PetPolicy = model.PetPolicy;
                existingProperty.TenancyContractStatus = model.TenancyContractStatus;
                existingProperty.LeaseExpiryDate = model.LeaseExpiryDate;

                existingProperty.BoostListing = model.BoostListing;
                existingProperty.PriorityPlacement = model.PriorityPlacement;

                existingProperty.RentalPrice = model.RentalPrice;
                existingProperty.OffPrice = model.OffPrice;
                existingProperty.SalePrice = model.SalePrice;

                existingProperty.VillaBedrooms = model.VillaBedrooms;
                existingProperty.VillaBathrooms = model.VillaBathrooms;
                existingProperty.VillaBuiltUpArea = model.VillaBuiltUpArea;
                existingProperty.VillaFurnished = model.VillaFurnished;
                existingProperty.VillaDeveloperName = model.VillaDeveloperName;
                existingProperty.VillaParkingSpaces = model.VillaParkingSpaces;
                existingProperty.VillaMaidsRoom = model.VillaMaidsRoom;

                existingProperty.CommercialBuiltUpArea = model.CommercialBuiltUpArea;
                existingProperty.CommercialDeveloperName = model.CommercialDeveloperName;
                existingProperty.CommercialFurnished = model.CommercialFurnished;
                existingProperty.CommercialParkingSpaces = model.CommercialParkingSpaces;
                existingProperty.CommercialFloorLevel = model.CommercialFloorLevel;

                existingProperty.Description = model.Description;
                existingProperty.ChequesAcceptedRental = model.ChequesAcceptedRental;

                existingProperty.Property_Title_AR = model.Property_Title_AR;
                existingProperty.Property_Size = model.Property_Size;
                existingProperty.Property_Size_Unit = model.Property_Size_Unit;
                existingProperty.plotArea = model.plotArea;

                existingProperty.Portals = model.Portals;
                existingProperty.Rent_Frequency = model.Rent_Frequency;

                existingProperty.offplanDetails_saleType = model.offplanDetails_saleType;
                existingProperty.offplanDetails_dldWaiver = model.offplanDetails_dldWaiver;
                existingProperty.offplanDetails_originalPrice = model.offplanDetails_originalPrice;
                existingProperty.offplanDetails_amountPaid = model.offplanDetails_amountPaid;

                existingProperty.Videos = model.Videos;
                existingProperty.Locality = model.Locality;
                existingProperty.Sub_Locality = model.Sub_Locality;
                existingProperty.Tower_Name = model.Tower_Name;

                existingProperty.Listing_Agent = model.Listing_Agent;
                existingProperty.Listing_Agent_Phone = model.Listing_Agent_Phone;
                existingProperty.Listing_Agent_Email = model.Listing_Agent_Email;
                // ===== SYSTEM FIELDS =====

                // ✅ Regenerate slug if title changed
                if (!string.Equals(existingProperty.Title, model.Title, StringComparison.OrdinalIgnoreCase))
                {
                    existingProperty.PropertySlug = GenerateUniqueSlug(model.Title);
                }

                existingProperty.Off_plan = model.ListingType == "Off-Plan";
                existingProperty.Property_purpose = model.ListingType == "For Rent" ? "Rent" : "Sell";
                existingProperty.ModifiedDate = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return Ok(new { PropertyId = model.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        // POST: api/Properties/UpdatePropertyAmenity
        [HttpPost("UpdatePropertyAmenity")]
        public IActionResult UpdatePropertyAmenity(List<PropertyAmenity> model)
        {
            try
            {
                if (model == null || !model.Any()) return BadRequest("No amenities provided");

                int propertyId = (int)model.First().PropertyId;

                var existing = _db.PropertyAmenities.Where(pa => pa.PropertyId == propertyId).ToList();
                _db.PropertyAmenities.RemoveRange(existing);

                _db.PropertyAmenities.AddRange(model);
                _db.SaveChanges();

                return Ok("Property Amenities Updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Properties/AddPropertyAmenity
        [HttpPost("AddPropertyAmenity")]
        public IActionResult AddPropertyAmenity(List<PropertyAmenity> model)
        {
            try
            {
                _db.PropertyAmenities.AddRange(model);
                _db.SaveChanges();
                return Ok("Property Amenities Added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Properties/GetAmenityByPropertyId/5
        [HttpGet("GetAmenityByPropertyId/{id}")]
        public IActionResult GetAmenityByPropertyId(int id)
        {
            try
            {
                var property = _db.PropertyAmenities
                                  .Where(pa => pa.PropertyId == id)
                                  .Select(pa => new
                                  {
                                      pa.AmenityId,
                                      pa.IsAvailable,
                                      pa.Amenity.AmenityName
                                  }).ToList();

                return Ok(property);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Properties/DeleteProperty/5
        [HttpDelete("DeleteProperty/{id}")]
        public IActionResult DeleteProperty(int id)
        {
            try
            {
                var property = _db.PropertyListing.Include(p => p.PropertyAmenities)
                                                   .FirstOrDefault(p => p.Id == id);
                if (property == null) return NotFound("Property not found");

                _db.PropertyAmenities.RemoveRange(property.PropertyAmenities);
                _db.PropertyListing.Remove(property);
                _db.SaveChanges();

                return Ok("Deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Properties/GetOffPlanProperties
        [HttpGet("GetOffPlanProperties")]
        public IActionResult GetOffPlanProperties(int pageNumber = 1,
                                                  int pageSize = 6,
                                                  string city = null,
                                                  string propertyType = null,
                                                  string price = null,
                                                  int? bedrooms = null)
        {
            try
            {
                var query = _db.PropertyListing
                               .Where(p => p.ListingType == "Off-Plan")
                               .AsQueryable();

                if (!string.IsNullOrWhiteSpace(city))
                    query = query.Where(p => p.City == city);

                if (!string.IsNullOrWhiteSpace(propertyType))
                    query = query.Where(p => p.PropertyType == propertyType);

                if (bedrooms.HasValue)
                    query = bedrooms == 3
                        ? query.Where(p => p.Bedrooms >= 3 || p.VillaBedrooms >= 3)
                        : query.Where(p => p.Bedrooms == bedrooms || p.VillaBedrooms == bedrooms);

                if (!string.IsNullOrWhiteSpace(price))
                {
                    if (price == "low")
                        query = query.OrderByDescending(p => p.SalePrice.Value);
                    else
                        query = query.OrderBy(p => p.SalePrice.Value);
                }

                var totalCount = query.Count();
                var data = query.OrderByDescending(p => p.Id)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

                return Ok(new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            var slug = title.ToLowerInvariant().Trim();

            slug = System.Text.RegularExpressions.Regex
                   .Replace(slug, @"[^a-z0-9\s-]", "");

            slug = slug.Replace(" ", "-");

            slug = System.Text.RegularExpressions.Regex
                   .Replace(slug, @"-+", "-");

            return slug;
        }

        private string GenerateUniqueSlug(string title)
        {
            var baseSlug = GenerateSlug(title);
            if (string.IsNullOrWhiteSpace(baseSlug))
                return null;

            var slug = baseSlug;
            int count = 1;

            while (_db.PropertyListing.Any(p => p.PropertySlug == slug))
            {
                slug = $"{baseSlug}-{count}";
                count++;
            }

            return slug;
        }

    }
}
