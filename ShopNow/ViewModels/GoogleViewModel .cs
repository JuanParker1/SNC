using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{


    public class result
    {
        public string name { get; set; }
        public string international_phone_number { get; set; }
        public geometry geometry { get; set; }
        public string website { get; set; }
        public string rating { get; set; }
        public string user_ratings_total
        {
            get; set;
        }
        public opening_hours opening_hours { get; set; }
    }
    public class geometry
    {
        public location location { get; set; }
    }
    public class location
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class opening_hours
    {
        public List<string> weekday_text { get; set; }
    }

  

    public class Results
    {      
        public result result { get; set; }
    }
    public class BannerImages
    {
        public string Bannerpath { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }

    }
    public class NearShopImages
    {
        public List<shops> NearShops { get; set; }
        public class shops
        {
            public int id { get; set; }
            public string image { get; set; }
            public bool? IsOnline { get; set; }
            public TimeSpan? NextOnTime { get; set; }
        }


    }
        public class PlacesListView
    {
        //public List<Places> List { get; set; }
        public List<Places> ResturantList { get; set; }
        public List<Places> SuperMarketList { get; set; }
        public List<Places> GroceriesList { get; set; }
        public List<Places> HealthList { get; set; }
        public List<Places> ElectronicsList { get; set; }
        public List<Places> ServicesList { get; set; }
        public List<Places> OtherList { get; set; }
        public class Places
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public string DistrictName { get; set; }
            public double Rating { get; set; }
            public int ShopCategoryId { get; set; }
            public string ShopCategoryName { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
         
            public int Status { get; set; }
            public double Meters { get; set; }
            public bool ? isOnline { get; set; }
            public List<BannerImages> List { get; set; }
            public int ReviewCount { get; set; }
            public string Address { get; set; }
            public TimeSpan? NextOnTime { get; set; }
            // public List<Models.Banner> List { get; set; }
            public double OfferPercentage { get; set; }
            public bool IsShopRate { get; set; }
        }
    }

}

