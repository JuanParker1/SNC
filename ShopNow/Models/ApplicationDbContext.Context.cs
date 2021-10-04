﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopNow.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class sncEntities : DbContext
    {
        public sncEntities()
            : base("name=sncEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccessPolicy> AccessPolicies { get; set; }
        public virtual DbSet<AchievementProduct> AchievementProducts { get; set; }
        public virtual DbSet<AchievementShop> AchievementShops { get; set; }
        public virtual DbSet<AddOnCategory> AddOnCategories { get; set; }
        public virtual DbSet<ApiSetting> ApiSettings { get; set; }
        public virtual DbSet<AppDetail> AppDetails { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<BrandOwner> BrandOwners { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<CustomerReview> CustomerReviews { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DeliveryBoyShop> DeliveryBoyShops { get; set; }
        public virtual DbSet<DiscountCategory> DiscountCategories { get; set; }
        public virtual DbSet<DiscountCategories1> DiscountCategories1 { get; set; }
        public virtual DbSet<DrugCompoundDetail> DrugCompoundDetails { get; set; }
        public virtual DbSet<LocationDetail> LocationDetails { get; set; }
        public virtual DbSet<MarketingAgent> MarketingAgents { get; set; }
        public virtual DbSet<MeasurementUnit> MeasurementUnits { get; set; }
        public virtual DbSet<NextSubCategory> NextSubCategories { get; set; }
        public virtual DbSet<OfferProduct> OfferProducts { get; set; }
        public virtual DbSet<OfferShop> OfferShops { get; set; }
        public virtual DbSet<OtpVerification> OtpVerifications { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentsData> PaymentsDatas { get; set; }
        public virtual DbSet<PincodeRate> PincodeRates { get; set; }
        public virtual DbSet<PlatFormCreditRate> PlatFormCreditRates { get; set; }
        public virtual DbSet<Portion> Portions { get; set; }
        public virtual DbSet<ProductDishAddOn> ProductDishAddOns { get; set; }
        public virtual DbSet<Products1> Products1 { get; set; }
        public virtual DbSet<ProductSpecificationItem> ProductSpecificationItems { get; set; }
        public virtual DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<ReferralSetting> ReferralSettings { get; set; }
        public virtual DbSet<RefundsData> RefundsDatas { get; set; }
        public virtual DbSet<ShopCategory> ShopCategories { get; set; }
        public virtual DbSet<ShopCredit> ShopCredits { get; set; }
        public virtual DbSet<ShopDishAddOn> ShopDishAddOns { get; set; }
        public virtual DbSet<ShopMember> ShopMembers { get; set; }
        public virtual DbSet<Specification> Specifications { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<UserEnquiry> UserEnquiries { get; set; }
        public virtual DbSet<ShopSchedule> ShopSchedules { get; set; }
        public virtual DbSet<ProductSchedule> ProductSchedules { get; set; }
        public virtual DbSet<AchievementSetting> AchievementSettings { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Crust> Crusts { get; set; }
        public virtual DbSet<MasterProduct> MasterProducts { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<CustomerSearchData> CustomerSearchDatas { get; set; }
        public virtual DbSet<KeywordData> KeywordDatas { get; set; }
        public virtual DbSet<SearchData> SearchDatas { get; set; }
        public virtual DbSet<DeliveryCharge> DeliveryCharges { get; set; }
        public virtual DbSet<BillingCharge> BillingCharges { get; set; }
        public virtual DbSet<CustomerAchievement> CustomerAchievements { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<CustomerPrescriptionItem> CustomerPrescriptionItems { get; set; }
        public virtual DbSet<CustomerPrescription> CustomerPrescriptions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Agency> Agencies { get; set; }
        public virtual DbSet<DeliveryBoy> DeliveryBoys { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
    
        [DbFunction("sncEntities", "GetTableVAlueString")]
        public virtual IQueryable<GetTableVAlueString_Result> GetTableVAlueString(string key)
        {
            var keyParameter = key != null ?
                new ObjectParameter("key", key) :
                new ObjectParameter("key", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<GetTableVAlueString_Result>("[sncEntities].[GetTableVAlueString](@key)", keyParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> GetCustomerCount()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetCustomerCount");
        }
    
        public virtual ObjectResult<GetDEliveryBoyList_Result> GetDEliveryBoyList()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetDEliveryBoyList_Result>("GetDEliveryBoyList");
        }
    
        public virtual ObjectResult<Nullable<int>> GetProductListCount(Nullable<double> longitude, Nullable<double> latitude, string str)
        {
            var longitudeParameter = longitude.HasValue ?
                new ObjectParameter("Longitude", longitude) :
                new ObjectParameter("Longitude", typeof(double));
    
            var latitudeParameter = latitude.HasValue ?
                new ObjectParameter("Latitude", latitude) :
                new ObjectParameter("Latitude", typeof(double));
    
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetProductListCount", longitudeParameter, latitudeParameter, strParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> GetShopCategoryProductCount(Nullable<int> shopCode, Nullable<int> categoryCode, string str)
        {
            var shopCodeParameter = shopCode.HasValue ?
                new ObjectParameter("shopCode", shopCode) :
                new ObjectParameter("shopCode", typeof(int));
    
            var categoryCodeParameter = categoryCode.HasValue ?
                new ObjectParameter("categoryCode", categoryCode) :
                new ObjectParameter("categoryCode", typeof(int));
    
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetShopCategoryProductCount", shopCodeParameter, categoryCodeParameter, strParameter);
        }
    
        public virtual ObjectResult<GetShopname_Result> GetShopname(Nullable<int> shopId)
        {
            var shopIdParameter = shopId.HasValue ?
                new ObjectParameter("shopId", shopId) :
                new ObjectParameter("shopId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetShopname_Result>("GetShopname", shopIdParameter);
        }
    
        public virtual int ReatillerPaymentReportAdmin(Nullable<System.DateTime> from, Nullable<System.DateTime> to, string shopcode)
        {
            var fromParameter = from.HasValue ?
                new ObjectParameter("from", from) :
                new ObjectParameter("from", typeof(System.DateTime));
    
            var toParameter = to.HasValue ?
                new ObjectParameter("to", to) :
                new ObjectParameter("to", typeof(System.DateTime));
    
            var shopcodeParameter = shopcode != null ?
                new ObjectParameter("shopcode", shopcode) :
                new ObjectParameter("shopcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ReatillerPaymentReportAdmin", fromParameter, toParameter, shopcodeParameter);
        }
    
        public virtual ObjectResult<GetProductList_Result> GetProductList(Nullable<double> longitude, Nullable<double> latitude, string str, Nullable<int> page, Nullable<int> pagesize)
        {
            var longitudeParameter = longitude.HasValue ?
                new ObjectParameter("Longitude", longitude) :
                new ObjectParameter("Longitude", typeof(double));
    
            var latitudeParameter = latitude.HasValue ?
                new ObjectParameter("Latitude", latitude) :
                new ObjectParameter("Latitude", typeof(double));
    
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            var pageParameter = page.HasValue ?
                new ObjectParameter("page", page) :
                new ObjectParameter("page", typeof(int));
    
            var pagesizeParameter = pagesize.HasValue ?
                new ObjectParameter("pagesize", pagesize) :
                new ObjectParameter("pagesize", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetProductList_Result>("GetProductList", longitudeParameter, latitudeParameter, strParameter, pageParameter, pagesizeParameter);
        }
    
        public virtual ObjectResult<GetShopCategoryProducts_Result> GetShopCategoryProducts(Nullable<int> shopCode, Nullable<int> categoryCode, string str, Nullable<int> page, Nullable<int> pageSize)
        {
            var shopCodeParameter = shopCode.HasValue ?
                new ObjectParameter("shopCode", shopCode) :
                new ObjectParameter("shopCode", typeof(int));
    
            var categoryCodeParameter = categoryCode.HasValue ?
                new ObjectParameter("categoryCode", categoryCode) :
                new ObjectParameter("categoryCode", typeof(int));
    
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            var pageParameter = page.HasValue ?
                new ObjectParameter("page", page) :
                new ObjectParameter("page", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetShopCategoryProducts_Result>("GetShopCategoryProducts", shopCodeParameter, categoryCodeParameter, strParameter, pageParameter, pageSizeParameter);
        }
    }
}
