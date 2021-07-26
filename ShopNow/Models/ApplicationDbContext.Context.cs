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
    
    public partial class ShopnowchatEntities : DbContext
    {
        public ShopnowchatEntities()
            : base("name=ShopnowchatEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccessPolicy> AccessPolicies { get; set; }
        public virtual DbSet<AddOnCategory> AddOnCategories { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BrandOwner> BrandOwners { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<CustomerReview> CustomerReviews { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DeliveryBoy> DeliveryBoys { get; set; }
        public virtual DbSet<DeliveryBoyShop> DeliveryBoyShops { get; set; }
        public virtual DbSet<DiscountCategory> DiscountCategories { get; set; }
        public virtual DbSet<DrugCompoundDetail> DrugCompoundDetails { get; set; }
        public virtual DbSet<MarketingAgent> MarketingAgents { get; set; }
        public virtual DbSet<MasterProduct> MasterProducts { get; set; }
        public virtual DbSet<MeasurementUnit> MeasurementUnits { get; set; }
        public virtual DbSet<NextSubCategory> NextSubCategories { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OtpVerification> OtpVerifications { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentsData> PaymentsDatas { get; set; }
        public virtual DbSet<PincodeRate> PincodeRates { get; set; }
        public virtual DbSet<PlatFormCreditRate> PlatFormCreditRates { get; set; }
        public virtual DbSet<Portion> Portions { get; set; }
        public virtual DbSet<ProductDishAddOn> ProductDishAddOns { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductSpecificationItem> ProductSpecificationItems { get; set; }
        public virtual DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<RefundsData> RefundsDatas { get; set; }
        public virtual DbSet<ShopCategory> ShopCategories { get; set; }
        public virtual DbSet<ShopCredit> ShopCredits { get; set; }
        public virtual DbSet<ShopDishAddOn> ShopDishAddOns { get; set; }
        public virtual DbSet<ShopMember> ShopMembers { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<Specification> Specifications { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<UserEnquiry> UserEnquiries { get; set; }
        public virtual DbSet<LocationDetail> LocationDetails { get; set; }
    
        [DbFunction("sncEntities", "GetTableVAlueString")]
        public virtual IQueryable<GetTableVAlueString_Result> GetTableVAlueString(string key)
        {
            var keyParameter = key != null ?
                new ObjectParameter("key", key) :
                new ObjectParameter("key", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<GetTableVAlueString_Result>("[sncEntities].[GetTableVAlueString](@key)", keyParameter);
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
    
        public virtual ObjectResult<GetDEliveryBoyList_Result> GetDEliveryBoyList()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetDEliveryBoyList_Result>("GetDEliveryBoyList");
        }
    
        public virtual int GetProductList(Nullable<double> longitude, Nullable<double> latitude, string str, Nullable<int> page, Nullable<int> pagesize)
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
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetProductList", longitudeParameter, latitudeParameter, strParameter, pageParameter, pagesizeParameter);
        }
    
        public virtual int GetProductListCount(Nullable<double> longitude, Nullable<double> latitude, string str)
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
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetProductListCount", longitudeParameter, latitudeParameter, strParameter);
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
    
        public virtual int GetShopCategoryProductCount(string shopCode, string categoryCode, string str)
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
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetShopCategoryProductCount", shopCodeParameter, categoryCodeParameter, strParameter);
        }
    
        public virtual int GetShopCategoryProducts(string shopCode, string categoryCode, string str, Nullable<int> page, Nullable<int> pageSize)
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
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetShopCategoryProducts", shopCodeParameter, categoryCodeParameter, strParameter, pageParameter, pageSizeParameter);
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
    
        public virtual int GetShopname(string shopcode)
        {
            var shopcodeParameter = shopcode != null ?
                new ObjectParameter("shopcode", shopcode) :
                new ObjectParameter("shopcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetShopname", shopcodeParameter);
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
    
        public virtual int test(string str)
        {
            var strParameter = str != null ?
                new ObjectParameter("str", str) :
                new ObjectParameter("str", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("test", strParameter);
        }
    }
}
