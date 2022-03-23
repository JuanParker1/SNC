using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace ShopNow.Controllers
{
    public class WebApiController : Controller
    {
        private sncEntities db = new sncEntities();
        
        private static string _generatedCode(string _prefix)
        {
                return ShopNow.Helpers.DRC.Generate(_prefix);
        }

        [AccessPolicy(PageCode = "SNCWATSU296")]
        public ActionResult TimeStampUpdate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCWATSU296")]
        public ActionResult TimeStampUpdate(WebApiTimeStampViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            try
            {
                sncEntities db = new sncEntities();

                var api = db.ApiSettings.Where(m => m.Status == 0 && m.ShopId == model.ShopId).FirstOrDefault();

                        var shop = db.Shops.Where(m => m.Id == model.ShopId).Select(i => new
                        {
                            Id = i.Id,
                            shopname = i.Name,
                            shopcategoryid = i.ShopCategoryId,
                            shopcategoryname = i.ShopCategoryName
                        }).FirstOrDefault();

                        var productMedicalStocktime = (from p in db.Products
                                                       where p.ShopId == model.ShopId
                                                       orderby p.ItemTimeStamp descending
                                                       select p.ItemTimeStamp).FirstOrDefault();
                        string s = "";
                        string Url = api.Url + "items?q=itemTimeStamp>="+ model.TimeStamp+ ",status==R,outletId==" + api.OutletId + "&limit=200000&selectAll=true";
                        using (WebClient client = new WebClient())
                        {
                            client.Headers["X-Auth-Token"] = api.AuthKey; //"62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";

                            s = client.DownloadString(Url);
                        }
                        var lst = db.Products.Where(m => m.ShopId == api.ShopId).ToList();
                        List<Product> updateList = new List<Product>();
                        List<Product> createList = new List<Product>();
                      
                        List<DiscountCategory> createCategoryList = new List<DiscountCategory>();
                        DiscountCategory dc = new DiscountCategory();
                        Product varProduct = new Product();
                        List<DiscountCategory> lstDiscount = new List<DiscountCategory>();
                        goto GetDiscoutCatecories;
                    GetDiscoutCatecories:
                        lstDiscount = db.DiscountCategories.Where(m => m.ShopId == api.ShopId).ToList();
                        dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(s, new ExpandoObjectConverter());
                        foreach (var pro in ((IEnumerable<dynamic>)config.items).Where(t => t.status == "R"))
                        {

                            varProduct.ItemId = Convert.ToInt32(pro.itemId);
                            varProduct.Name = pro.itemName;
                            varProduct.IBarU = Convert.ToInt32(pro.iBarU);
                            varProduct.DateUpdated = DateTime.Now;
                            varProduct.ShopCategoryId = 4;
                            varProduct.ShopId = api.ShopId;

                            varProduct.ProductTypeId = 0;
                            varProduct.MinSelectionLimit = 0;
                            varProduct.MaxSelectionLimit = 0;
                            varProduct.Customisation = false;
                            varProduct.Percentage = 0;
                            varProduct.DiscountCategoryId = 0;
                            varProduct.DataEntry = 1;
                            varProduct.IsOnline = true;
                            varProduct.HoldOnStok = 0;
                            varProduct.PackingType = 0;
                            varProduct.SpecialCostOfDelivery = 0;
                            varProduct.LoyaltyPoints = 0;
                            varProduct.PackingCharge = 0;
                            varProduct.Status = 0;
                            varProduct.DateEncoded = DateTime.Now;
                            varProduct.CreatedBy = "Admin";
                            varProduct.UpdatedBy = "Admin";
                            foreach (var med in pro.stock)
                            {
                                varProduct.Qty = Convert.ToInt32(Math.Floor(Convert.ToDecimal(med.stock)));
                                varProduct.MenuPrice = Convert.ToDouble(med.mrp);
                                varProduct.Price = Convert.ToDouble(med.salePrice);
                                varProduct.TaxPercentage = Convert.ToDouble(med.taxPercentage);
                                varProduct.ItemTimeStamp = Convert.ToString(med.itemTimeStamp);
                                varProduct.AppliesOnline = Convert.ToInt32(pro.appliesOnline);
                                varProduct.OutletId = Convert.ToInt32(med.outletId);
                                if (api.Category == 1)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat1);
                                else if (api.Category == 2)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat2);
                                else if (api.Category == 3)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat3);
                                else if (api.Category == 4)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat4);
                                else if (api.Category == 5)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat5);
                                else if (api.Category == 6)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat6);
                                else if (api.Category == 7)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat7);
                                else if (api.Category == 8)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat8);
                                else if (api.Category == 9)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat9);
                                else if (api.Category == 10)
                                    varProduct.DiscountCategoryName = Convert.ToString(med.Cat10);

                                if (varProduct.DiscountCategoryName != null)
                                    varProduct.DiscountCategoryName = varProduct.DiscountCategoryName.Trim();
                                else
                                    varProduct.DiscountCategoryName = varProduct.DiscountCategoryName;
                                if (api.ShopId == 123)
                                {
                                    if (Convert.ToString(med.Cat2) != "MEDICINES" && Convert.ToString(med.Cat2) != "MEDICINE" && Convert.ToString(med.Cat2) != "MEDICINES(10)" && Convert.ToString(med.Cat2) != "MEDICINE(10)")
                                    {
                                        varProduct.DiscountCategoryName = Convert.ToString(med.Cat1);
                                    }

                                }

                                var catCout = lstDiscount.Where(c => c.ShopId == api.ShopId && c.Name == varProduct.DiscountCategoryName).Count();
                                if (catCout <= 0)
                                {
                                    if (varProduct.DiscountCategoryName != null)
                                        dc.Name = varProduct.DiscountCategoryName.Trim();
                                    else
                                        dc.Name = varProduct.DiscountCategoryName;
                                    dc.ShopId = api.ShopId;
                                    dc.ShopName = shop.shopname;
                                    dc.CreatedBy = "Admin";
                                    dc.UpdatedBy = "Admin";
                                    dc.DateEncoded = DateTime.Now;
                                    dc.DateUpdated = DateTime.Now;
                                    db.DiscountCategories.Add(dc);
                                    db.SaveChanges();
                                    varProduct.DiscountCategoryId = dc.Id;
                                    lstDiscount = db.DiscountCategories.Where(m => m.ShopId == api.ShopId).ToList();
                                }
                                else
                                {
                                    var catId = lstDiscount.Where(c => c.ShopId == api.ShopId && c.Name == varProduct.DiscountCategoryName).Select(c => c.Id).ToList();
                                    varProduct.DiscountCategoryId = Convert.ToInt32(catId[0]);
                                }

                            }

                            int idx = lst.FindIndex(a => a.ItemId == pro.itemId);
                            if (idx >= 0)
                            {
                                updateList.Add(new Product
                                {
                                    Id = lst[idx].Id,
                                    Name = varProduct.Name,
                                    MasterProductId = lst[idx].MasterProductId,
                                    CategoryId = lst[idx].CategoryId,
                                    ShopId = shop.Id,
                                    ShopName = shop.shopname,
                                    ShopCategoryId = shop.shopcategoryid,
                                    ShopCategoryName = shop.shopcategoryname,
                                    Price = varProduct.Price,
                                    Qty = varProduct.Qty,
                                    ProductTypeName = lst[idx].ProductTypeName,
                                    ProductTypeId = lst[idx].ProductTypeId,
                                    MinSelectionLimit = lst[idx].MinSelectionLimit,
                                    MaxSelectionLimit = lst[idx].MaxSelectionLimit,
                                    Customisation = lst[idx].Customisation,
                                    MenuPrice = varProduct.MenuPrice,
                                    IBarU = Convert.ToInt32(pro.iBarU),
                                    ItemId = varProduct.ItemId,
                                    Percentage = lst[idx].Percentage,
                                    DiscountCategoryId = varProduct.DiscountCategoryId,
                                    DiscountCategoryName = varProduct.DiscountCategoryName,
                                    DataEntry = lst[idx].DataEntry,
                                    AppliesOnline = varProduct.AppliesOnline,
                                    IsOnline = lst[idx].IsOnline,
                                    HoldOnStok = lst[idx].HoldOnStok,
                                    PackingType = lst[idx].PackingType,
                                    TaxPercentage = varProduct.TaxPercentage,
                                    SpecialCostOfDelivery = lst[idx].SpecialCostOfDelivery,
                                    OutletId = varProduct.OutletId,
                                    ItemTimeStamp = varProduct.ItemTimeStamp,
                                    LoyaltyPoints = lst[idx].LoyaltyPoints,
                                    PackingCharge = lst[idx].PackingCharge,
                                    BrandOwnerMiddlePercentage = lst[idx].BrandOwnerMiddlePercentage,
                                    ShopOwnerPrice = lst[idx].ShopOwnerPrice,
                                    HasSchedule = lst[idx].HasSchedule,
                                    NextOnTime = lst[idx].NextOnTime,
                                    IsPreorder = lst[idx].IsPreorder,
                                    PreorderHour = lst[idx].PreorderHour,
                                    OfferQuantityLimit = lst[idx].OfferQuantityLimit,
                                    MappedDate = lst[idx].MappedDate,
                                    Status = lst[idx].Status,
                                    DateEncoded = lst[idx].DateEncoded,
                                    DateUpdated = DateTime.Now,
                                    UpdatedBy = "Admin",
                                    CreatedBy = lst[idx].CreatedBy

                                });
                                //update
                            }
                            else
                            {
                                //long masterid = 0;
                                long id = 0;
                                createList.Add(new Product
                                {
                                    Id = id,
                                    Name = varProduct.Name,
                                    ShopId = shop.Id,
                                    ShopName = shop.shopname,
                                    ShopCategoryId = shop.shopcategoryid,
                                    ShopCategoryName = shop.shopcategoryname,
                                    Price = varProduct.Price,
                                    Qty = varProduct.Qty,
                                    ProductTypeId = 0,
                                    MinSelectionLimit = varProduct.MinSelectionLimit,
                                    MaxSelectionLimit = varProduct.MaxSelectionLimit,
                                    Customisation = false,
                                    MenuPrice = varProduct.MenuPrice,
                                    IBarU = Convert.ToInt32(pro.iBarU),
                                    ItemId = varProduct.ItemId,
                                    Percentage = 0,
                                    DiscountCategoryId = varProduct.DiscountCategoryId,
                                    DiscountCategoryName = varProduct.DiscountCategoryName,
                                    DataEntry = varProduct.DataEntry,
                                    AppliesOnline = varProduct.AppliesOnline,
                                    IsOnline = true,
                                    HoldOnStok = 0,
                                    PackingType = 0,
                                    TaxPercentage = varProduct.TaxPercentage,
                                    SpecialCostOfDelivery = 0,
                                    OutletId = varProduct.OutletId,
                                    ItemTimeStamp = varProduct.ItemTimeStamp,
                                    LoyaltyPoints = 0,
                                    PackingCharge = 0,
                                    BrandOwnerMiddlePercentage = 0,
                                    ShopOwnerPrice = 0,
                                    HasSchedule = false,
                                    NextOnTime = null,
                                    IsPreorder = false,
                                    PreorderHour = 0,
                                    OfferQuantityLimit = 0,
                                    Status = 0,
                                    DateEncoded = DateTime.Now,
                                    DateUpdated = DateTime.Now,
                                    CreatedBy = "Admin"
                                });
                            }

                        }
                        db.BulkInsert(createList);
                        if (updateList.Count > 0)
                            db.BulkUpdate(updateList);
                        db.BulkSaveChanges();
                   

                db.Dispose();
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        [AccessPolicy(PageCode = "SNCWAC294")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCWAC294")]
        public ActionResult Create(WebApiCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {
                model.ShopId = 123;
                model.ShopName = "JOYRA MEDICALS+SURGICALS";

                
                using (WebClient myData = new WebClient())
                {
                    myData.Headers[model.AuthName] = model.AuthKey;
                    myData.Headers[HttpRequestHeader.Accept] = "application/json";
                    myData.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string getList = myData.DownloadString(model.Link);
                    var result = JsonConvert.DeserializeObject<RootObject>(getList);
                    Product product = new Product();
                    foreach (var pro  in result.items)
                    {
                        var produc = db.Products.FirstOrDefault(i => i.ItemId == pro.itemId); 
                        if (produc == null && pro.status =="R")
                        {
                            product.ItemId = pro.itemId;
                            product.ShopId = model.ShopId;
                            product.ShopName = model.ShopName;
                            product.Name = pro.itemName;
                            product.IBarU = Convert.ToInt32(pro.iBarU);
                            product.ProductTypeId = 3;
                            product.ProductTypeName = "Medical";
                            product.DataEntry = 1;
                            
                            if (pro.appliesOnline != null)
                            {
                                product.AppliesOnline = Convert.ToInt32(pro.appliesOnline);
                            }
                            else
                            {
                                product.AppliesOnline = 5;
                            }

                            if (pro.stock != null)
                            {
                                foreach (var med in pro.stock)
                                {

                                    if (med.outletId == "2")
                                    {
                                        if (med.stock != null)
                                        {
                                            product.Qty = Convert.ToInt32(Math.Floor(Convert.ToDouble(med.stock)));
                                        }
                                        else
                                        {
                                            product.Qty = 0;
                                        }

                                        if (med.mrp != null)
                                        {
                                            product.MenuPrice = Convert.ToDouble(med.mrp);
                                        }
                                        else
                                        {
                                            product.MenuPrice = 0;
                                        }

                                        if (med.salePrice != null)
                                        {
                                            product.Price = Convert.ToDouble(med.salePrice);
                                        }
                                        else
                                        {
                                            product.Price = 0;
                                        }
                                        if (med.taxPercentage != null)
                                        {

                                            product.TaxPercentage = Convert.ToDouble(med.taxPercentage);
                                        }
                                        else
                                        {
                                            product.TaxPercentage = 0;
                                        }
                                        //if (med.discountpercentage != null)
                                        //{
                                        //    product.DiscountCategoryPercentage = Convert.ToDouble(med.discountpercentage);
                                        //}
                                        //else
                                        //{
                                        //    product.DiscountCategoryPercentage = 0;
                                        //}
                                        if (med.loyaltypointsper100value != null)
                                        {
                                            product.LoyaltyPoints = Convert.ToDouble(med.loyaltypointsper100value);
                                        }
                                        else
                                        {
                                            product.LoyaltyPoints = 0;
                                        }
                                        //if (med.minimumloyaltyreductionpercentage != null)
                                        //{

                                        //        product.m = Convert.ToDouble(med.minimumloyaltyreductionpercentage);
                                        //}
                                        //else
                                        //{
                                        //    productMedicalStock.MinimumLoyaltyReducationPercentage = 0;
                                        //}
                                        if (med.specialcostfordelivery != null)
                                        {

                                            product.SpecialCostOfDelivery = Convert.ToDouble(med.specialcostfordelivery);
                                        }
                                        else
                                        {
                                            product.SpecialCostOfDelivery = 0;
                                        }
                                        //if (med.specialPrice!= null)
                                        //{
                                        //        product.SpecialPrice = Convert.ToDouble(med.specialPrice);
                                        //}
                                        //else
                                        //{
                                        //    productMedicalStock.SpecialPrice = 0;
                                        //}

                                        if (med.outletId != null)
                                        {
                                            product.OutletId = Convert.ToInt32(med.outletId);
                                        }
                                        else
                                        {
                                            product.OutletId = 0;
                                        }
                                        //if (med.minSaleQuantity != null)
                                        //{
                                        //    product.min = Convert.ToInt32(med.minSaleQuantity);
                                        //}
                                        //else
                                        //{
                                        //    productMedicalStock.MinSaleQty = 0;
                                        //}
                                        //product.CategoryNameMain = med.Cat1;
                                        //product.DiscountCategoryId= CheckCategory(med.Cat1, Convert.ToDouble(med.discountpercentage));
                                        product.DiscountCategoryName = med.Cat1;
                                        //product.DiscountCategoryType = 1;
                                        //product.DiscountType = 1;
                                        product.ItemTimeStamp = med.itemTimeStamp;
                                        product.Status = 0;
                                        product.CreatedBy = user.Name;
                                        product.UpdatedBy = user.Name;
                                        product.DateEncoded = DateTime.Now;
                                        product.DateUpdated = DateTime.Now;
                                        db.Products.Add(product);
                                        db.SaveChanges();
                                    }

                                }
                            }
                        }
                    }
                }
                return RedirectToAction("List", "Product");

            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SNCWAU295")]
        public ActionResult Update()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCWAU295")]
        public ActionResult Update(WebApiCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {

                using (WebClient myData = new WebClient())
                {
                    myData.Headers[model.AuthName] = model.AuthKey;
                    myData.Headers[HttpRequestHeader.Accept] = "application/json";
                    myData.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string getList = myData.DownloadString(model.Link);
                    var result = JsonConvert.DeserializeObject<RootObject>(getList);
                    Product product = new Product();
                    foreach (var pro in result.items)
                    {
                        var produc = db.Products.FirstOrDefault(i => i.ItemId == Convert.ToInt32(pro.itemId)); 
                        if (produc == null && pro.status == "R")
                        {
                            product.ItemId = Convert.ToInt32(pro.itemId);
                            product.ShopId = model.ShopId;
                            product.ShopName = model.ShopName;
                            product.Name = pro.itemName;
                            product.IBarU = Convert.ToInt32(pro.iBarU);
                            product.ProductTypeId = 3;
                            product.ProductTypeName = "Medical";
                            product.DataEntry = 1;

                            if (pro.appliesOnline != null)
                            {
                                product.AppliesOnline = Convert.ToInt32(pro.appliesOnline);
                            }
                            else
                            {
                                product.AppliesOnline = 5;
                            }

                            if (pro.stock != null)
                            {
                                foreach (var med in pro.stock)
                                {

                                    if (med.outletId == "2")
                                    {
                                        if (med.stock != null)
                                        {
                                            product.Qty = Convert.ToInt32(Math.Floor(Convert.ToDouble(med.stock)));
                                        }
                                        else
                                        {
                                            product.Qty = 0;
                                        }

                                        if (med.mrp != null)
                                        {
                                            product.MenuPrice = Convert.ToDouble(med.mrp);
                                        }
                                        else
                                        {
                                            product.MenuPrice = 0;
                                        }

                                        if (med.salePrice != null)
                                        {
                                            product.Price = Convert.ToDouble(med.salePrice);
                                        }
                                        else
                                        {
                                            product.Price = 0;
                                        }
                                        if (med.taxPercentage != null)
                                        {

                                            product.TaxPercentage = Convert.ToDouble(med.taxPercentage);
                                        }
                                        else
                                        {
                                            product.TaxPercentage = 0;
                                        }
                                        //if (med.discountpercentage != null)
                                        //{
                                        //    product.DiscountCategoryPercentage = Convert.ToDouble(med.discountpercentage);
                                        //}
                                        //else
                                        //{
                                        //    product.DiscountCategoryPercentage = 0;
                                        //}
                                        if (med.loyaltypointsper100value != null)
                                        {
                                            product.LoyaltyPoints = Convert.ToDouble(med.loyaltypointsper100value);
                                        }
                                        else
                                        {
                                            product.LoyaltyPoints = 0;
                                        }
                                        //if (med.minimumloyaltyreductionpercentage != null)
                                        //{

                                        //        product.m = Convert.ToDouble(med.minimumloyaltyreductionpercentage);
                                        //}
                                        //else
                                        //{
                                        //    productMedicalStock.MinimumLoyaltyReducationPercentage = 0;
                                        //}
                                        if (med.specialcostfordelivery != null)
                                        {

                                            product.SpecialCostOfDelivery = Convert.ToDouble(med.specialcostfordelivery);
                                        }
                                        else
                                        {
                                            product.SpecialCostOfDelivery = 0;
                                        }
                                        //if (med.specialPrice!= null)
                                        //{
                                        //        product.SpecialPrice = Convert.ToDouble(med.specialPrice);
                                        //}
                                        //else
                                        //{
                                        //    productMedicalStock.SpecialPrice = 0;
                                        //}

                                        if (med.outletId != null)
                                        {
                                            product.OutletId = Convert.ToInt32(med.outletId);
                                        }
                                        else
                                        {
                                            product.OutletId = 0;
                                        }
                                        //if (med.minSaleQuantity != null)
                                        //{
                                        //    product.min = Convert.ToInt32(med.minSaleQuantity);
                                        //}
                                        //else
                                        //{
                                        //    productMedicalStock.MinSaleQty = 0;
                                        //}
                                        //product.CategoryNameMain = med.Cat1;
                                        //product.DiscountCategoryId = CheckCategory(med.Cat1, Convert.ToDouble(med.discountpercentage));
                                        product.DiscountCategoryName = med.Cat1;
                                        //product.DiscountCategoryType = 1;
                                        //product.DiscountType = 1;
                                        product.ItemTimeStamp = med.itemTimeStamp;
                                        product.Status = 0;
                                        product.CreatedBy = user.Name;
                                        product.UpdatedBy = user.Name;
                                        product.DateEncoded = DateTime.Now;
                                        product.DateUpdated = DateTime.Now;
                                        db.Entry(product).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }
                            }
                        }
                    }
                }
                return RedirectToAction("List", "Product");

            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SNCWATSU2966")]
        public ActionResult TSUpdate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCWATSU2966")]
        public ActionResult TSUpdate(WebApiTSViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {

                using (WebClient myData = new WebClient())
                {

                    myData.Headers["X-Auth-Token"] = "62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";
                    myData.Headers[HttpRequestHeader.Accept] = "application/json";
                    myData.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string getList = myData.DownloadString("http://joyrahq.gofrugal.com/RayMedi_HQ/api/v1/items?q=itemTimeStamp>=" + model.timeSpan + ",status==R,outletId==2&limit=105000");
                    dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(getList, new ExpandoObjectConverter());
                    int s = 0;
                    foreach (var pro in ((IEnumerable<dynamic>)config.items).Where(t => t.status == "R"))
                    {

                        int ss = Convert.ToInt32(pro.itemId);
                        var product = db.Products.FirstOrDefault(i => i.ItemId == ss); //Product.GetItemId(Convert.ToString(pro.itemId));
                        if (product != null)
                        {
                            product.ItemId = Convert.ToInt32(pro.itemId);
                            product.Name = pro.itemName;
                            product.IBarU = Convert.ToInt32(pro.iBarU);
                            product.ProductTypeId = 3;
                            product.ProductTypeName = "Medical";
                            product.DataEntry = 1;

                            if (pro.appliesOnline != null)
                            {
                                product.AppliesOnline = Convert.ToInt32(pro.appliesOnline);
                            }
                            else
                            {
                                product.AppliesOnline = 5;
                            }

                            if (pro.stock != null)
                            {
                                foreach (var med in pro.stock)
                                {

                                    if (med.outletId == "2")
                                    {
                                        if (med.stock != null)
                                        {
                                            product.Qty = Convert.ToInt32(Math.Floor(Convert.ToDouble(med.stock)));
                                        }
                                        else
                                        {
                                            product.Qty = 0;
                                        }

                                        if (med.mrp != null)
                                        {
                                            product.MenuPrice = Convert.ToDouble(med.mrp);
                                        }
                                        else
                                        {
                                            product.MenuPrice = 0;
                                        }

                                        if (med.salePrice != null)
                                        {
                                            product.Price = Convert.ToDouble(med.salePrice);
                                        }
                                        else
                                        {
                                            product.Price = 0;
                                        }
                                        if (med.taxPercentage != null)
                                        {

                                            product.TaxPercentage = Convert.ToDouble(med.taxPercentage);
                                        }
                                        else
                                        {
                                            product.TaxPercentage = 0;
                                        }
                                        //if (med.discountpercentage != null)
                                        //{
                                        //    product.DiscountCategoryPercentage = Convert.ToDouble(med.discountpercentage);
                                        //}
                                        //else
                                        //{
                                        //    product.DiscountCategoryPercentage = 0;
                                        //}
                                        if (med.loyaltypointsper100value != null)
                                        {
                                            product.LoyaltyPoints = Convert.ToDouble(med.loyaltypointsper100value);
                                        }
                                        else
                                        {
                                            product.LoyaltyPoints = 0;
                                        }
                                        if (med.specialcostfordelivery != null)
                                        {

                                            product.SpecialCostOfDelivery = Convert.ToDouble(med.specialcostfordelivery);
                                        }
                                        else
                                        {
                                            product.SpecialCostOfDelivery = 0;
                                        }

                                        if (med.outletId != null)
                                        {
                                            product.OutletId = Convert.ToInt32(med.outletId);
                                        }
                                        else
                                        {
                                            product.OutletId = 0;
                                        }
                                        //product.CategoryNameMain = med.Cat1;
                                        //product.DiscountCategoryId = CheckCategory(med.Cat1, Convert.ToDouble(med.discountpercentage));
                                        product.DiscountCategoryName = med.Cat1;
                                        //product.DiscountCategoryType = 1;
                                        //product.DiscountType = 1;
                                        product.ItemTimeStamp = med.itemTimeStamp;
                                        product.Status = 0;
                                        product.CreatedBy = user.Name;
                                        product.UpdatedBy = user.Name;
                                        product.DateEncoded = DateTime.Now;
                                        product.DateUpdated = DateTime.Now;
                                        db.Entry(product).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }
                            }

                            //product.Name = pro.itemName;
                            //product.iBarU = Convert.ToString(pro.iBarU);

                            //product.CreatedBy = user.Name;
                            //product.UpdatedBy = user.Name;

                            //product.DateUpdated = DateTime.Now;
                            //db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                            //db.SaveChanges();
                            //foreach (var med in pro.stock)
                            //{
                            //    var productMedicalStock = db.ProductMedicalStocks.FirstOrDefault(i => i.productid == product.Id); // ProductMedicalStock.GetProduct(product.Code);
                            //    if (productMedicalStock != null)
                            //    {
                            //        if (Convert.ToString(med.outletId) == "2")
                            //        {
                            //            if (Convert.ToDouble(med.stock) != null)
                            //            {
                            //                productMedicalStock.Stock = Convert.ToDouble(med.stock);
                            //                product.Qty = Convert.ToDouble(med.stock);
                            //            }
                            //            else
                            //            {
                            //                productMedicalStock.Stock = 0;
                            //            }

                            //            if (Convert.ToDouble(med.mrp) != null)
                            //            {
                            //                productMedicalStock.MRP = Convert.ToDouble(med.mrp);
                            //                product.MenuPrice = Convert.ToDouble(med.mrp);
                            //            }
                            //            else
                            //            {
                            //                productMedicalStock.MRP = 0;
                            //            }


                            //            productMedicalStock.CreatedBy = user.Name;
                            //            productMedicalStock.UpdatedBy = user.Name;
                            //            productMedicalStock.CategoryName1 = Convert.ToString(med.Cat1);
                            //            productMedicalStock.CategoryName2 = Convert.ToString(med.Cat2);
                            //            productMedicalStock.CategoryName3 = Convert.ToString(med.Cat3);
                            //            productMedicalStock.CategoryName4 = Convert.ToString(med.Cat4);
                            //            productMedicalStock.CategoryName5 = Convert.ToString(med.Cat5);
                            //            productMedicalStock.CategoryName6 = Convert.ToString(med.Cat6);
                            //            productMedicalStock.CategoryName7 = Convert.ToString(med.Cat7);
                            //            productMedicalStock.CategoryName8 = Convert.ToString(med.Cat8);
                            //            productMedicalStock.CategoryName9 = Convert.ToString(med.Cat9);
                            //            productMedicalStock.CategoryName10 = Convert.ToString(med.Cat10);
                            //            productMedicalStock.ItemTimeStamp = Convert.ToString(med.itemTimeStamp);

                            //            productMedicalStock.OfferCategoryCode = CheckCategory(Convert.ToString(med.Cat1), "11");
                            //            productMedicalStock.OfferCategoryName = Convert.ToString(med.Cat1);
                            //            productMedicalStock.OfferCategoryType = 1;

                            //            productMedicalStock.DateUpdated = DateTime.Now;
                            //            db.Entry(productMedicalStock).State = System.Data.Entity.EntityState.Modified;
                            //            db.SaveChanges();
                            //           // ProductMedicalStock.Edit(productMedicalStock, out errorCode);
                            //        }
                            //    }
                            //}
                        }
                        s = s + 1;

                    }
                }
                return RedirectToAction("List", "Product");

            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && (a.Id == 123 || a.Id== 203)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}