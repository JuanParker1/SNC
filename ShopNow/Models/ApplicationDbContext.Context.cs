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
        public virtual DbSet<AchievementSetting> AchievementSettings { get; set; }
        public virtual DbSet<AchievementShop> AchievementShops { get; set; }
        public virtual DbSet<AddOnCategory> AddOnCategories { get; set; }
        public virtual DbSet<Agency> Agencies { get; set; }
        public virtual DbSet<ApiSetting> ApiSettings { get; set; }
        public virtual DbSet<AppDetail> AppDetails { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<BillingCharge> BillingCharges { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BrandOwner> BrandOwners { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<CallRecord> CallRecords { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Crust> Crusts { get; set; }
        public virtual DbSet<CustomerAchievement> CustomerAchievements { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<CustomerFavorite> CustomerFavorites { get; set; }
        public virtual DbSet<CustomerPrescriptionImage> CustomerPrescriptionImages { get; set; }
        public virtual DbSet<CustomerPrescriptionItem> CustomerPrescriptionItems { get; set; }
        public virtual DbSet<CustomerReview> CustomerReviews { get; set; }
        public virtual DbSet<CustomerReviewReply> CustomerReviewReplies { get; set; }
        public virtual DbSet<CustomerSearchData> CustomerSearchDatas { get; set; }
        public virtual DbSet<CustomerWalletHistory> CustomerWalletHistories { get; set; }
        public virtual DbSet<DeliveryBoyShop> DeliveryBoyShops { get; set; }
        public virtual DbSet<DeliveryCharge> DeliveryCharges { get; set; }
        public virtual DbSet<DiscountCategory> DiscountCategories { get; set; }
        public virtual DbSet<DrugCompoundDetail> DrugCompoundDetails { get; set; }
        public virtual DbSet<KeywordData> KeywordDatas { get; set; }
        public virtual DbSet<MarketingAgent> MarketingAgents { get; set; }
        public virtual DbSet<MasterProduct> MasterProducts { get; set; }
        public virtual DbSet<MeasurementUnit> MeasurementUnits { get; set; }
        public virtual DbSet<NextSubCategory> NextSubCategories { get; set; }
        public virtual DbSet<OfferProduct> OfferProducts { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<OfferShop> OfferShops { get; set; }
        public virtual DbSet<OrderItemAddon> OrderItemAddons { get; set; }
        public virtual DbSet<OtpVerification> OtpVerifications { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PaymentsData> PaymentsDatas { get; set; }
        public virtual DbSet<PincodeRate> PincodeRates { get; set; }
        public virtual DbSet<PlatFormCreditRate> PlatFormCreditRates { get; set; }
        public virtual DbSet<Portion> Portions { get; set; }
        public virtual DbSet<ProductDishAddOn> ProductDishAddOns { get; set; }
        public virtual DbSet<ProductSpecificationItem> ProductSpecificationItems { get; set; }
        public virtual DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<ReferralSetting> ReferralSettings { get; set; }
        public virtual DbSet<RefundsData> RefundsDatas { get; set; }
        public virtual DbSet<SearchData> SearchDatas { get; set; }
        public virtual DbSet<ShopCategory> ShopCategories { get; set; }
        public virtual DbSet<ShopCredit> ShopCredits { get; set; }
        public virtual DbSet<ShopDishAddOn> ShopDishAddOns { get; set; }
        public virtual DbSet<ShopMember> ShopMembers { get; set; }
        public virtual DbSet<Specification> Specifications { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<tbl> tbls { get; set; }
        public virtual DbSet<UserEnquiry> UserEnquiries { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<CustomerAppInfo> CustomerAppInfoes { get; set; }
        public virtual DbSet<CustomerDeviceInfo> CustomerDeviceInfoes { get; set; }
        public virtual DbSet<CustomerPrescription> CustomerPrescriptions { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<ShopSchedule> ShopSchedules { get; set; }
        public virtual DbSet<DeliveryBoy> DeliveryBoys { get; set; }
        public virtual DbSet<ShopBillDetail> ShopBillDetails { get; set; }
        public virtual DbSet<ProductSchedule> ProductSchedules { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<CustomerBankDetail> CustomerBankDetails { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<FAQCategory> FAQCategories { get; set; }
        public virtual DbSet<CustomerGroceryUpload> CustomerGroceryUploads { get; set; }
        public virtual DbSet<CustomerGroceryUploadImage> CustomerGroceryUploadImages { get; set; }
        public virtual DbSet<FAQ> FAQs { get; set; }
        public virtual DbSet<CustomerSearchHistory> CustomerSearchHistories { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<ShopStaff> ShopStaffs { get; set; }
        public virtual DbSet<LocationDetail> LocationDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<TagCategory> TagCategories { get; set; }
        public virtual DbSet<CustomerGiftCard> CustomerGiftCards { get; set; }
    
        [DbFunction("sncEntities", "GetTableVAlueString")]
        public virtual IQueryable<GetTableVAlueString_Result> GetTableVAlueString(string key)
        {
            var keyParameter = key != null ?
                new ObjectParameter("key", key) :
                new ObjectParameter("key", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<GetTableVAlueString_Result>("[sncEntities].[GetTableVAlueString](@key)", keyParameter);
        }
    
        [DbFunction("sncEntities", "ufn_CSVToTable")]
        public virtual IQueryable<ufn_CSVToTable_Result> ufn_CSVToTable(string stringInput, string delimiter)
        {
            var stringInputParameter = stringInput != null ?
                new ObjectParameter("StringInput", stringInput) :
                new ObjectParameter("StringInput", typeof(string));
    
            var delimiterParameter = delimiter != null ?
                new ObjectParameter("Delimiter", delimiter) :
                new ObjectParameter("Delimiter", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<ufn_CSVToTable_Result>("[sncEntities].[ufn_CSVToTable](@StringInput, @Delimiter)", stringInputParameter, delimiterParameter);
        }
    
        [DbFunction("sncEntities", "ufn_StringTable")]
        public virtual IQueryable<ufn_StringTable_Result> ufn_StringTable(string stringInput, string delimiter)
        {
            var stringInputParameter = stringInput != null ?
                new ObjectParameter("StringInput", stringInput) :
                new ObjectParameter("StringInput", typeof(string));
    
            var delimiterParameter = delimiter != null ?
                new ObjectParameter("Delimiter", delimiter) :
                new ObjectParameter("Delimiter", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<ufn_StringTable_Result>("[sncEntities].[ufn_StringTable](@StringInput, @Delimiter)", stringInputParameter, delimiterParameter);
        }
    
        public virtual int getCategoryListbyShopcode(string code)
        {
            var codeParameter = code != null ?
                new ObjectParameter("code", code) :
                new ObjectParameter("code", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("getCategoryListbyShopcode", codeParameter);
        }
    
        public virtual int GetCategoryProductsList(string shopCode, string categoryCode, string str, Nullable<int> page, Nullable<int> pageSize)
        {
            var shopCodeParameter = shopCode != null ?
                new ObjectParameter("shopCode", shopCode) :
                new ObjectParameter("shopCode", typeof(string));
    
            var categoryCodeParameter = categoryCode != null ?
                new ObjectParameter("categoryCode", categoryCode) :
                new ObjectParameter("categoryCode", typeof(string));
    
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            var pageParameter = page.HasValue ?
                new ObjectParameter("page", page) :
                new ObjectParameter("page", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCategoryProductsList", shopCodeParameter, categoryCodeParameter, strParameter, pageParameter, pageSizeParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> GetCustomerCount()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetCustomerCount");
        }
    
        public virtual ObjectResult<GetDEliveryBoyList_Result> GetDEliveryBoyList()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetDEliveryBoyList_Result>("GetDEliveryBoyList");
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
    
        public virtual int GetShopCategoryList(string shop, string categoryCode, string str, Nullable<int> page, Nullable<int> size)
        {
            var shopParameter = shop != null ?
                new ObjectParameter("shop", shop) :
                new ObjectParameter("shop", typeof(string));
    
            var categoryCodeParameter = categoryCode != null ?
                new ObjectParameter("CategoryCode", categoryCode) :
                new ObjectParameter("CategoryCode", typeof(string));
    
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            var pageParameter = page.HasValue ?
                new ObjectParameter("Page", page) :
                new ObjectParameter("Page", typeof(int));
    
            var sizeParameter = size.HasValue ?
                new ObjectParameter("Size", size) :
                new ObjectParameter("Size", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetShopCategoryList", shopParameter, categoryCodeParameter, strParameter, pageParameter, sizeParameter);
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
    
        public virtual int GetShopCategoryProductsList(string shopCode, string categoryCode, string str, Nullable<int> page, Nullable<int> pageSize)
        {
            var shopCodeParameter = shopCode != null ?
                new ObjectParameter("shopCode", shopCode) :
                new ObjectParameter("shopCode", typeof(string));
    
            var categoryCodeParameter = categoryCode != null ?
                new ObjectParameter("categoryCode", categoryCode) :
                new ObjectParameter("categoryCode", typeof(string));
    
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            var pageParameter = page.HasValue ?
                new ObjectParameter("page", page) :
                new ObjectParameter("page", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetShopCategoryProductsList", shopCodeParameter, categoryCodeParameter, strParameter, pageParameter, pageSizeParameter);
        }
    
        public virtual ObjectResult<GetShopname_Result> GetShopname(Nullable<int> shopId)
        {
            var shopIdParameter = shopId.HasValue ?
                new ObjectParameter("shopId", shopId) :
                new ObjectParameter("shopId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetShopname_Result>("GetShopname", shopIdParameter);
        }
    
        public virtual int ReatillerPaymentReport(Nullable<System.DateTime> from, Nullable<System.DateTime> to, string shopcode, string user)
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
    
            var userParameter = user != null ?
                new ObjectParameter("user", user) :
                new ObjectParameter("user", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ReatillerPaymentReport", fromParameter, toParameter, shopcodeParameter, userParameter);
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
    
        public virtual int ReatillerPaymentReports(Nullable<System.DateTime> from, Nullable<System.DateTime> to, string shopcode, string user)
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
    
            var userParameter = user != null ?
                new ObjectParameter("user", user) :
                new ObjectParameter("user", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ReatillerPaymentReports", fromParameter, toParameter, shopcodeParameter, userParameter);
        }
    
        public virtual int Sp_TB_SelectMedicineName(string search, Nullable<int> pageNumber, Nullable<int> rowofpage)
        {
            var searchParameter = search != null ?
                new ObjectParameter("search", search) :
                new ObjectParameter("search", typeof(string));
    
            var pageNumberParameter = pageNumber.HasValue ?
                new ObjectParameter("pageNumber", pageNumber) :
                new ObjectParameter("pageNumber", typeof(int));
    
            var rowofpageParameter = rowofpage.HasValue ?
                new ObjectParameter("Rowofpage", rowofpage) :
                new ObjectParameter("Rowofpage", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Sp_TB_SelectMedicineName", searchParameter, pageNumberParameter, rowofpageParameter);
        }
    
        public virtual ObjectResult<Nullable<System.Guid>> SqlQueryNotificationStoredProcedure_35889fc1_4b38_49ec_a25f_35bceebcd9f3()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<System.Guid>>("SqlQueryNotificationStoredProcedure_35889fc1_4b38_49ec_a25f_35bceebcd9f3");
        }
    
        public virtual ObjectResult<Nullable<System.Guid>> SqlQueryNotificationStoredProcedure_413caca6_2c87_4361_9ff7_ee550ffbd0ed()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<System.Guid>>("SqlQueryNotificationStoredProcedure_413caca6_2c87_4361_9ff7_ee550ffbd0ed");
        }
    
        public virtual ObjectResult<Nullable<System.Guid>> SqlQueryNotificationStoredProcedure_85a2a887_f0ed_4901_b7d8_b32bab1a5e51()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<System.Guid>>("SqlQueryNotificationStoredProcedure_85a2a887_f0ed_4901_b7d8_b32bab1a5e51");
        }
    
        public virtual int test(string str)
        {
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("test", strParameter);
        }
    
        public virtual int GetAutoCompleteSearch(Nullable<double> longitude, Nullable<double> latitude, string str, Nullable<int> customerid)
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
    
            var customeridParameter = customerid.HasValue ?
                new ObjectParameter("customerid", customerid) :
                new ObjectParameter("customerid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetAutoCompleteSearch", longitudeParameter, latitudeParameter, strParameter, customeridParameter);
        }
    
        public virtual ObjectResult<GetDashBoardDetails_Result> GetDashBoardDetails()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetDashBoardDetails_Result>("GetDashBoardDetails");
        }
    
        public virtual ObjectResult<Nullable<int>> GetAllSearchResultCount(Nullable<double> longitude, Nullable<double> latitude, string str, Nullable<int> customerid)
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
    
            var customeridParameter = customerid.HasValue ?
                new ObjectParameter("customerid", customerid) :
                new ObjectParameter("customerid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetAllSearchResultCount", longitudeParameter, latitudeParameter, strParameter, customeridParameter);
        }
    
        public virtual ObjectResult<string> GetAllSearchResult(Nullable<double> longitude, Nullable<double> latitude, string str, Nullable<int> customerid, Nullable<int> page, Nullable<int> pageSize)
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
    
            var customeridParameter = customerid.HasValue ?
                new ObjectParameter("customerid", customerid) :
                new ObjectParameter("customerid", typeof(int));
    
            var pageParameter = page.HasValue ?
                new ObjectParameter("page", page) :
                new ObjectParameter("page", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("GetAllSearchResult", longitudeParameter, latitudeParameter, strParameter, customeridParameter, pageParameter, pageSizeParameter);
        }
    
        public virtual ObjectResult<GetShopProductSearch_Result> GetShopProductSearch(Nullable<int> shopId, string str)
        {
            var shopIdParameter = shopId.HasValue ?
                new ObjectParameter("shopId", shopId) :
                new ObjectParameter("shopId", typeof(int));
    
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetShopProductSearch_Result>("GetShopProductSearch", shopIdParameter, strParameter);
        }
    
        public virtual ObjectResult<GetShopCategoryProducts_Result> GetShopCategoryProducts(Nullable<int> shopCode, Nullable<int> categoryCode, string str, Nullable<int> page, Nullable<int> pageSize, Nullable<int> customerid)
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
    
            var customeridParameter = customerid.HasValue ?
                new ObjectParameter("customerid", customerid) :
                new ObjectParameter("customerid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetShopCategoryProducts_Result>("GetShopCategoryProducts", shopCodeParameter, categoryCodeParameter, strParameter, pageParameter, pageSizeParameter, customeridParameter);
        }
    }
}
