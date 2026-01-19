using System;
using System.Collections.Generic;

namespace RedBerryApi.Models
{
    public class PropertyListing
    {
        public PropertyListing()
        {
            PropertyAmenities = new HashSet<PropertyAmenity>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string PropertyType { get; set; }
        public string ListingType { get; set; }
        public string PriceCurrency { get; set; }
        public string MainImage { get; set; }
        public string SubImage { get; set; }
        public bool? PaymentPlanAvailable { get; set; }
        public string PaymentPlanAvailableDescription { get; set; }
        public string PropertySlug { get; set; }
        public string PropertyCategory { get; set; }
        public string DeveloperName { get; set; }
        public string HandoverDate { get; set; }
        public string PaymentPlanStructure { get; set; }
        public string PostHandoverPayment { get; set; }
        public string ConstructionStatus { get; set; }
        public string ModelUnitImages { get; set; }
        public string ServiceCharges { get; set; }
        public bool? ShowroomAvailable { get; set; }
        public string PropertyAge { get; set; }
        public string TenancyStatus { get; set; }
        public string MortgageStatus { get; set; }
        public bool? PreviousOwnership { get; set; }
        public bool? TitleDeedAvailable { get; set; }
        public decimal? RentalPrice { get; set; }
        public string PaymentTerms { get; set; }
        public string ChequesAccepted { get; set; }
        public string CommissionFee { get; set; }
        public string TenancyContractLength { get; set; }
        public string VacatingNoticePeriod { get; set; }
        public bool? Deposit { get; set; }
        public int? Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string FloorLevel { get; set; }
        public string BalconyTerrace { get; set; }
        public string PropertyView { get; set; }
        public string BuiltUpArea { get; set; }
        public string ParkingSpaces { get; set; }
        public string KitchenType { get; set; }
        public string MaidsRoom { get; set; }
        public string SmartHomeFeatures { get; set; }
        public string StorageRoom { get; set; }
        public string PlotSize { get; set; }
        public string PrivateGarden { get; set; }
        public string PrivatePool { get; set; }
        public string Furnished { get; set; }
        public string UnitType { get; set; }
        public string PantryKitchen { get; set; }
        public string Washroom { get; set; }
        public string LicenseTypeSupport { get; set; }
        public bool? SwimmingPool { get; set; }
        public string CommunityName { get; set; }
        public string City { get; set; }
        public string Neighborhood { get; set; }
        public string ProximityToMetro { get; set; }
        public string NearbyLandmarks { get; set; }
        public string SchoolHospitalProximity { get; set; }
        public decimal? ROI { get; set; }
        public decimal? RentalYield { get; set; }
        public decimal? AnnualServiceCharges { get; set; }
        public string MortgageCalculator { get; set; }
        public string RentalPaymentOptions { get; set; }
        public string ListingId { get; set; }
        public string RealEstateAgencyName { get; set; }
        public string RERAPermitNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string DLDTransferFee { get; set; }
        public string CoolingSystem { get; set; }
        public string PetPolicy { get; set; }
        public string TenancyContractStatus { get; set; }
        public DateTime? LeaseExpiryDate { get; set; }
        public string BoostListing { get; set; }
        public string PriorityPlacement { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public decimal? OffPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public int? VillaBedrooms { get; set; }
        public string VillaBathrooms { get; set; }
        public string VillaParkingSpaces { get; set; }
        public string VillaFloorLevel { get; set; }
        public string VillaBuiltUpArea { get; set; }
        public string VillaMaidsRoom { get; set; }
        public string VillaDeveloperName { get; set; }
        public string VillaFurnished { get; set; }
        public string CommercialBuiltUpArea { get; set; }
        public string CommercialParkingSpaces { get; set; }
        public string CommercialFloorLevel { get; set; }
        public string CommercialDeveloperName { get; set; }
        public string CommercialFurnished { get; set; }
        public string Description { get; set; }
        public string ChequesAcceptedRental { get; set; }
        public string Status { get; set; }
        public string Property_Title_AR { get; set; }
        public string Property_purpose { get; set; }
        public decimal? Property_Size { get; set; }
        public string Property_Size_Unit { get; set; }
        public decimal? plotArea { get; set; }
        public string Features { get; set; }
        public bool? Off_plan { get; set; }
        public string Portals { get; set; }
        public string Property_Description_AR { get; set; }
        public string Rent_Frequency { get; set; }
        public string offplanDetails_saleType { get; set; }
        public string offplanDetails_dldWaiver { get; set; }
        public decimal? offplanDetails_originalPrice { get; set; }
        public decimal? offplanDetails_amountPaid { get; set; }
        public string Videos { get; set; }
        public string Locality { get; set; }
        public string Sub_Locality { get; set; }
        public string Tower_Name { get; set; }
        public string Listing_Agent { get; set; }
        public string Listing_Agent_Phone { get; set; }
        public string Listing_Agent_Email { get; set; }

        // NAVIGATION PROPERTY
        public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; }
    }
}
