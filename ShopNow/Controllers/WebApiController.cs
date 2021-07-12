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
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;


namespace ShopNow.Controllers
{
    public class WebApiController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        
        private static string _generatedCode(string _prefix)
        {
                return ShopNow.Helpers.DRC.Generate(_prefix);
        }

        [AccessPolicy(PageCode = "SHNCAPI001")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNCAPI001")]
        public ActionResult Create(WebApiCreateViewModel model)
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
                    ProductMedicalStock productMedicalStock = new ProductMedicalStock();
                    foreach (var pro  in result.items)
                    {
                        var produc = db.Products.FirstOrDefault(i => i.ItemId == pro.itemId); // Product.GetItemId(pro.itemId);
                        if (produc == null && pro.status =="R")
                        {
                            product.ItemId = pro.itemId;
                            product.ShopCode = model.ShopCode;
                            product.ShopName = model.ShopName;
                            product.Name = pro.itemName;
                            product.iBarU = pro.iBarU;
                            product.ProductType = "Medical";
                            product.DataEntry = 1;
                            product.Manufacturer = pro.manufacturer;
                            product.CreatedBy = user.Name;
                            product.UpdatedBy = user.Name;
                            if (pro.appliesOnline != null)
                            {
                                product.AppliesOnline = Convert.ToInt32(pro.appliesOnline);
                            }
                            else
                            {
                                product.AppliesOnline = 5;
                            }
                            product.Code = _generatedCode("PRO");
                            product.Status = 0;
                            product.DateEncoded = DateTime.Now;
                            product.DateUpdated = DateTime.Now;
                            db.Products.Add(product);
                            db.SaveChanges();
                           // Product.Add(product);
                            if(pro.stock != null) { 
                            foreach (var med in pro.stock)
                            {
 
                                if (med.outletId == "2")
                                {
                                        productMedicalStock.ItemId = pro.itemId;
                                        productMedicalStock.ProductCode = product.Code;
                                    productMedicalStock.ProductName = product.Name;
                                    if (med.stock != null)
                                    {
                                        productMedicalStock.Stock = Convert.ToDouble(med.stock);
                                    }
                                    else
                                    {
                                        productMedicalStock.Stock = 0;
                                    }
                                    productMedicalStock.SupplierName = med.supplierName;
                                    if (med.mrp != null)
                                    {
                                        productMedicalStock.MRP = Convert.ToDouble(med.mrp);
                                    }
                                    else
                                    {
                                        productMedicalStock.MRP = 0;
                                    }

                                    if (med.salePrice != null)
                                    {
                                        productMedicalStock.SalePrice = Convert.ToDouble(med.salePrice);
                                    }
                                    else
                                    {
                                        productMedicalStock.SalePrice = 0;
                                    }
                                    if (med.taxPercentage!= null)
                                    {

                                        productMedicalStock.TaxPercentage = Convert.ToDouble(med.taxPercentage);
                                    }
                                    else
                                    {
                                        productMedicalStock.TaxPercentage = 0;
                                    }
                                    if (med.discountpercentage != null)
                                    {
                                        productMedicalStock.DiscountPercentage = Convert.ToDouble(med.discountpercentage);
                                    }
                                    else
                                    {
                                        productMedicalStock.DiscountPercentage = 0;
                                    }
                                    if (med.loyaltypointsper100value != null)
                                    {
                                        productMedicalStock.LoyaltyPointsper100Value = Convert.ToDouble(med.loyaltypointsper100value);
                                    }
                                    else
                                    {
                                        productMedicalStock.LoyaltyPointsper100Value = 0;
                                    }
                                    if (med.minimumloyaltyreductionpercentage != null)
                                    {

                                        productMedicalStock.MinimumLoyaltyReducationPercentage = Convert.ToDouble(med.minimumloyaltyreductionpercentage);
                                    }
                                    else
                                    {
                                        productMedicalStock.MinimumLoyaltyReducationPercentage = 0;
                                    }
                                    if (med.specialcostfordelivery!= null)
                                    {

                                        productMedicalStock.SpecialCostOfDelivery = Convert.ToDouble(med.specialcostfordelivery);
                                    }
                                    else
                                    {
                                        productMedicalStock.SpecialCostOfDelivery = 0;
                                    }
                                    if (med.specialPrice!= null)
                                    {
                                        productMedicalStock.SpecialPrice = Convert.ToDouble(med.specialPrice);
                                    }
                                    else
                                    {
                                        productMedicalStock.SpecialPrice = 0;
                                    }

                                    if (med.outletId != null)
                                    {
                                        productMedicalStock.OutLetId = Convert.ToInt32(med.outletId);
                                    }
                                    else
                                    {
                                        productMedicalStock.OutLetId = 0;
                                    }
                                    if (med.minSaleQuantity != null)
                                    {
                                        productMedicalStock.MinSaleQty = Convert.ToInt32(med.minSaleQuantity);
                                    }
                                    else
                                    {
                                        productMedicalStock.MinSaleQty = 0;
                                    }
                                    productMedicalStock.CreatedBy = user.Name;
                                    productMedicalStock.UpdatedBy = user.Name;
                                    productMedicalStock.CategoryName1 = med.Cat1;
                                    productMedicalStock.CategoryName2 = med.Cat2;
                                    productMedicalStock.CategoryName3 = med.Cat3;
                                    productMedicalStock.CategoryName4 = med.Cat4;
                                    productMedicalStock.CategoryName5 = med.Cat5;
                                    productMedicalStock.CategoryName6 = med.Cat6;
                                    productMedicalStock.CategoryName7 = med.Cat7;
                                    productMedicalStock.CategoryName8 = med.Cat8;
                                    productMedicalStock.CategoryName9 = med.Cat9;
                                    productMedicalStock.CategoryName10 = med.Cat10;
                                    productMedicalStock.ItemTimeStamp = med.itemTimeStamp;
                                    if (model.CategoryType==1)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat1, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat1;
                                        productMedicalStock.OfferCategoryType = 1;
                                    }
                                    else if (model.CategoryType ==2)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat2, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat2;
                                        productMedicalStock.OfferCategoryType = 2;
                                    }
                                    else if (model.CategoryType ==3)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat3, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat3;
                                        productMedicalStock.OfferCategoryType = 3;
                                    }
                                    else if (model.CategoryType ==4)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat4, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat4;
                                        productMedicalStock.OfferCategoryType = 4;
                                    }
                                    else if (model.CategoryType ==5)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat5, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat5;
                                        productMedicalStock.OfferCategoryType = 5;
                                    }
                                    else if (model.CategoryType ==6)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat6, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat6;
                                        productMedicalStock.OfferCategoryType = 6;
                                    }
                                    else if (model.CategoryType ==7)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat7, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat7;
                                        productMedicalStock.OfferCategoryType = 7;
                                    }
                                    else if (model.CategoryType ==8)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat8, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat8;
                                        productMedicalStock.OfferCategoryType = 8;
                                    }
                                    else if (model.CategoryType ==9)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat9, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat9;
                                        productMedicalStock.OfferCategoryType = 9;
                                    }
                                    else if (model.CategoryType ==10)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat10, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat10;
                                        productMedicalStock.OfferCategoryType = 10;
                                    }
                                        productMedicalStock.Code = _generatedCode("PMS");
                                        productMedicalStock.Status = 0;
                                        productMedicalStock.DateEncoded = DateTime.Now;
                                        productMedicalStock.DateUpdated = DateTime.Now;
                                        productMedicalStock.productid = product.Id; 
                                        db.ProductMedicalStocks.Add(productMedicalStock);
                                        db.SaveChanges();
                                       // ProductMedicalStock.Add(productMedicalStock, out errorCode);
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

        public string CheckCategory(string CategoryName, string ProductType)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var category = db.Categories.FirstOrDefault(i => i.Name == CategoryName); // Category.GetName(CategoryName);
            if (category != null)
            {

                return category.Code;
            }
            else
            {
                Category cat = new Category();
                cat.Name = CategoryName;
                cat.ProductType = ProductType;
                cat.DateEncoded = DateTime.Now;
                cat.DateUpdated = DateTime.Now;
                cat.CreatedBy = user.Name;
                cat.UpdatedBy = user.Name;
                cat.Code = _generatedCode("CAT");
                cat.Status = 0;
                db.Categories.Add(cat);
                db.SaveChanges();
               // cat.Code = Category.Add(cat, out int error);
                return cat.Code;
            }

        }

        [AccessPolicy(PageCode = "SHNUAPI002")]
        public ActionResult Update()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNUAPI002")]
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

                    foreach (var pro in result.items)
                    {
                        var product = db.Products.FirstOrDefault(i => i.ItemId == pro.itemId); //GetItemId(pro.itemId);
                        if (product != null && pro.status == "R")
                        {
                         //   product.ItemId = pro.itemId;
                          //  product.ShopCode = model.ShopCode;
                          //  product.ShopName = model.ShopName;
                            product.Name = pro.itemName;
                            product.iBarU = pro.iBarU;
                           // product.ProductType = "Medical";
                          //  product.DataEntry = 1;
                           // product.Manufacturer = pro.manufacturer;
                            product.CreatedBy = user.Name;
                            product.UpdatedBy = user.Name;
                            product.DateUpdated = DateTime.Now;
                          //  product.AppliesOnline = pro.appliesOnline;
                           // Product.Edit(product, out errorCode);
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            foreach (var med in pro.stock)
                            {
                                var productMedicalStock = db.ProductMedicalStocks.FirstOrDefault(i => i.productid == product.Id); //GetProduct(product.Code);
                                if (productMedicalStock!=null) { 
                                if (med.outletId == "2")
                                {
                                        productMedicalStock.ItemId = pro.itemId;
                                        //  productMedicalStock.ProductCode = product.Code;
                                        // productMedicalStock.ProductName = product.Name;
                                        if (med.stock != null)
                                    {
                                        productMedicalStock.Stock = Convert.ToDouble(med.stock);
                                    }
                                    else
                                    {
                                        productMedicalStock.Stock = 0;
                                    }
                                //    productMedicalStock.SupplierName = med.supplierName;
                                    if (med.mrp != null)
                                    {
                                        productMedicalStock.MRP = Convert.ToDouble(med.mrp);
                                    }
                                    else
                                    {
                                        productMedicalStock.MRP = 0;
                                    }

                                    //if (med.salePrice != null)
                                    //{
                                    //    productMedicalStock.SalePrice = Convert.ToDouble(med.salePrice);
                                    //}
                                    //else
                                    //{
                                    //    productMedicalStock.SalePrice = 0;
                                    //}
                                    //if (med.taxPercentage != null)
                                    //{

                                    //    productMedicalStock.TaxPercentage = Convert.ToDouble(med.taxPercentage);
                                    //}
                                    //else
                                    //{
                                    //    productMedicalStock.TaxPercentage = 0;
                                    //}
                                    //if (med.discountpercentage != null)
                                    //{
                                    //    productMedicalStock.DiscountPercentage = Convert.ToDouble(med.discountpercentage);                                                                           
                                    //}
                                    //else
                                    //{
                                    //    productMedicalStock.DiscountPercentage = 0;
                                    //}
                                    //if (med.loyaltypointsper100value != null)
                                    //{
                                    //    productMedicalStock.LoyaltyPointsper100Value = Convert.ToDouble(med.loyaltypointsper100value);
                                    //}
                                    //else
                                    //{
                                    //    productMedicalStock.LoyaltyPointsper100Value = 0;
                                    //}
                                    //if (med.minimumloyaltyreductionpercentage != null)
                                    //{
         
                                    //    productMedicalStock.MinimumLoyaltyReducationPercentage = Convert.ToDouble(med.minimumloyaltyreductionpercentage);
                                    //}
                                    //else
                                    //{
                                    //    productMedicalStock.MinimumLoyaltyReducationPercentage = 0;
                                    //}
                                    //if (med.specialcostfordelivery != null)
                                    //{
 
                                    //    productMedicalStock.SpecialCostOfDelivery = Convert.ToDouble(med.specialcostfordelivery);
                                    //}
                                    //else
                                    //{
                                    //    productMedicalStock.SpecialCostOfDelivery = 0;
                                    //}
                                    //if (med.specialPrice != null)
                                    //{
                                    //    productMedicalStock.SpecialPrice = Convert.ToDouble(med.specialPrice);
                                    //}
                                    //else
                                    //{
                                    //    productMedicalStock.SpecialPrice = 0;
                                    //}

                                    //if (med.outletId != null)
                                    //{
                                    //    productMedicalStock.OutLetId = Convert.ToInt32(med.outletId);
                                    //}
                                    //else
                                    //{
                                    //    productMedicalStock.OutLetId = 0;
                                    //}
                                    //if (med.minSaleQuantity != null)
                                    //{
                                    //    productMedicalStock.MinSaleQty = Convert.ToInt32(med.minSaleQuantity);
                                    //}
                                    //else
                                    //{
                                    //    productMedicalStock.MinSaleQty = 0;
                                    //}


                           
                                    productMedicalStock.CreatedBy = user.Name;
                                    productMedicalStock.UpdatedBy = user.Name;
                                    productMedicalStock.CategoryName1 = med.Cat1;
                                    productMedicalStock.CategoryName2 = med.Cat2;
                                    productMedicalStock.CategoryName3 = med.Cat3;
                                    productMedicalStock.CategoryName4 = med.Cat4;
                                    productMedicalStock.CategoryName5 = med.Cat5;
                                    productMedicalStock.CategoryName6 = med.Cat6;
                                    productMedicalStock.CategoryName7 = med.Cat7;
                                    productMedicalStock.CategoryName8 = med.Cat8;
                                    productMedicalStock.CategoryName9 = med.Cat9;
                                    productMedicalStock.CategoryName10 = med.Cat10;
                                    productMedicalStock.ItemTimeStamp = med.itemTimeStamp;
                                    if (model.CategoryType == 1)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat1, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat1;
                                        productMedicalStock.OfferCategoryType = 1;
                                    }
                                    else if (model.CategoryType == 2)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat2, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat2;
                                        productMedicalStock.OfferCategoryType = 2;
                                    }
                                    else if (model.CategoryType == 3)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat3, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat3;
                                        productMedicalStock.OfferCategoryType = 3;
                                    }
                                    else if (model.CategoryType == 4)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat4, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat4;
                                        productMedicalStock.OfferCategoryType = 4;
                                    }
                                    else if (model.CategoryType == 5)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat5, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat5;
                                        productMedicalStock.OfferCategoryType = 5;
                                    }
                                    else if (model.CategoryType == 6)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat6, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat6;
                                        productMedicalStock.OfferCategoryType = 6;
                                    }
                                    else if (model.CategoryType == 7)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat7, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat7;
                                        productMedicalStock.OfferCategoryType = 7;
                                    }
                                    else if (model.CategoryType == 8)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat8, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat8;
                                        productMedicalStock.OfferCategoryType = 8;
                                    }
                                    else if (model.CategoryType == 9)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat9, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat9;
                                        productMedicalStock.OfferCategoryType = 9;
                                    }
                                    else if (model.CategoryType == 10)
                                    {
                                        productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat10, "11");
                                        productMedicalStock.OfferCategoryName = med.Cat10;
                                        productMedicalStock.OfferCategoryType = 10;
                                    }
                                        productMedicalStock.DateUpdated = DateTime.Now;
                                        db.Entry(productMedicalStock).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //ProductMedicalStock.Edit(productMedicalStock, out errorCode);
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

        [AccessPolicy(PageCode = "SHNAPITSU003")]
        public ActionResult TSUpdate()
        {
            return View();
        }
        ProductMedicalStock pms = new ProductMedicalStock();
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNAPITSU003")]
        public ActionResult TSUpdate1(WebApiTSViewModel model)
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
                   
                    string getList = myData.DownloadString("http://joyrahq.gofrugal.com/RayMedi_HQ/api/v1/items?q=itemTimeStamp>="+model.timeSpan+ ",status==R,outletId==2");
                    dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(getList, new ExpandoObjectConverter());
                    string id = "";
                    foreach (var enabledEndpoint in ((IEnumerable<dynamic>)config.items).Where(t => t.status == "R"))
                    {
                        id = Convert.ToString(enabledEndpoint.itemId);
                        var model1 = db.Products.Where(C => C.ItemId == id && C.Status == 0).ToList();
                        if (enabledEndpoint.status == "R")
                        {

                            foreach (var med in enabledEndpoint.stock[0])
                            {
                                var modelpms = db.ProductMedicalStocks.Where(m => m.productid == model1[0].Id && m.Status == 0).ToList();

                                if (modelpms != null)
                                {
                                    if (enabledEndpoint.stock[0].stock != null)
                                    {
                                        pms.Stock = Convert.ToDouble(enabledEndpoint.stock[0].stock);
                                    }
                                    else
                                    {
                                        pms.Stock = 0;
                                    }

                                    if (med.mrp != null)
                                    {
                                        pms.MRP = Convert.ToDouble(enabledEndpoint.stock[0].mrp);
                                    }
                                    else
                                    {
                                        pms.MRP = 0;
                                    }



                                    pms.CategoryName1 = enabledEndpoint.stock[0].Cat1;
                                    pms.CategoryName2 = enabledEndpoint.stock[0].Cat2;
                                    pms.CategoryName3 = enabledEndpoint.stock[0].Cat3;
                                    pms.CategoryName4 = enabledEndpoint.stock[0].Cat4;
                                    pms.CategoryName5 = enabledEndpoint.stock[0].Cat5;
                                    pms.CategoryName6 = enabledEndpoint.stock[0].Cat6;
                                    pms.CategoryName7 = enabledEndpoint.stock[0].Cat7;
                                    pms.CategoryName8 = enabledEndpoint.stock[0].Cat8;
                                    pms.CategoryName9 = enabledEndpoint.stock[0].Cat9;
                                    pms.CategoryName10 = enabledEndpoint.stock[0].Cat10;
                                    pms.ItemTimeStamp = enabledEndpoint.stock[0].itemTimeStamp;
                                    pms.ItemId = enabledEndpoint.itemId;
                                    pms.OfferCategoryCode = CheckCategory(enabledEndpoint.stock[0].Cat1, "11");
                                    pms.OfferCategoryName = enabledEndpoint.stock[0].Cat1;
                                    pms.OfferCategoryType = 1;
                            
                                    //db.Entry(pms).State = EntityState.Modified;
                                    //db.SaveChanges();
                                    //ProductMedicalStock.Edit(productMedicalStock, out errorCode);
                                }
                            }
                        }
                    }
                        var result = JsonConvert.DeserializeObject<RootObject>(getList);
                    
                    //foreach (var pro in result.items)
                    //{
                    //    var product = Product.GetItemId(pro.itemId);
                    //    if (product != null && pro.status == "R")
                    //    {
                           
                    //        product.Name = pro.itemName;
                    //        product.iBarU = pro.iBarU;
                            
                    //        product.CreatedBy = user.Name;
                    //        product.UpdatedBy = user.Name;
                           
                    //        Product.Edit(product, out errorCode);
                    //        foreach (var med in pro.stock)
                    //        {
                    //            var productMedicalStock = ProductMedicalStock.GetProduct(product.Code);
                    //            if (productMedicalStock != null)
                    //            {
                    //                if (med.outletId == "2")
                    //                {
                    //                     if (med.stock != null)
                    //                    {
                    //                        productMedicalStock.Stock = Convert.ToDouble(med.stock);
                    //                    }
                    //                    else
                    //                    {
                    //                        productMedicalStock.Stock = 0;
                    //                    }
                                       
                    //                    if (med.mrp != null)
                    //                    {
                    //                        productMedicalStock.MRP = Convert.ToDouble(med.mrp);
                    //                    }
                    //                    else
                    //                    {
                    //                        productMedicalStock.MRP = 0;
                    //                    }

                                       
                    //                    productMedicalStock.CreatedBy = user.Name;
                    //                    productMedicalStock.UpdatedBy = user.Name;
                    //                    productMedicalStock.CategoryName1 = med.Cat1;
                    //                    productMedicalStock.CategoryName2 = med.Cat2;
                    //                    productMedicalStock.CategoryName3 = med.Cat3;
                    //                    productMedicalStock.CategoryName4 = med.Cat4;
                    //                    productMedicalStock.CategoryName5 = med.Cat5;
                    //                    productMedicalStock.CategoryName6 = med.Cat6;
                    //                    productMedicalStock.CategoryName7 = med.Cat7;
                    //                    productMedicalStock.CategoryName8 = med.Cat8;
                    //                    productMedicalStock.CategoryName9 = med.Cat9;
                    //                    productMedicalStock.CategoryName10 = med.Cat10;
                    //                    productMedicalStock.ItemTimeStamp = med.itemTimeStamp;
                    //                    productMedicalStock.ItemId = pro.itemId;
                    //                    productMedicalStock.OfferCategoryCode = CheckCategory(med.Cat1, "11");
                    //                        productMedicalStock.OfferCategoryName = med.Cat1;
                    //                        productMedicalStock.OfferCategoryType = 1;
                                        
                                       
                    //                   ProductMedicalStock.Edit(productMedicalStock, out errorCode);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
                return RedirectToAction("TSUpdate", "WebApi");

            }
            catch (Exception)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNAPITSU003")]
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
                    //string getList = myData.DownloadString("http://joyrahq.gofrugal.com/RayMedi_HQ/api/v1/items?q=itemTimeStamp>=" + model.timeSpan + "&limit=100000");
                    dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(getList, new ExpandoObjectConverter());
                    int s = 0;
                    foreach (var pro in ((IEnumerable<dynamic>)config.items).Where(t => t.status == "R"))
                    {

                        //    foreach (var pro in result.items)
                        //{
                        String ss = Convert.ToString(pro.itemId);
                        var product = db.Products.FirstOrDefault(i => i.ItemId == ss); //Product.GetItemId(Convert.ToString(pro.itemId));
                        if (product != null)
                        {

                            product.Name = pro.itemName;
                            product.iBarU = Convert.ToString(pro.iBarU);

                            product.CreatedBy = user.Name;
                            product.UpdatedBy = user.Name;

                            product.DateUpdated = DateTime.Now;
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                           // Product.Edit(product, out errorCode);
                            foreach (var med in pro.stock)
                            {
                                var productMedicalStock = db.ProductMedicalStocks.FirstOrDefault(i => i.productid == product.Id); // ProductMedicalStock.GetProduct(product.Code);
                                if (productMedicalStock != null)
                                {
                                    if (Convert.ToString(med.outletId) == "2")
                                    {
                                        if (Convert.ToDouble(med.stock) != null)
                                        {
                                            productMedicalStock.Stock = Convert.ToDouble(med.stock);
                                            product.Qty = Convert.ToDouble(med.stock);
                                        }
                                        else
                                        {
                                            productMedicalStock.Stock = 0;
                                        }

                                        if (Convert.ToDouble(med.mrp) != null)
                                        {
                                            productMedicalStock.MRP = Convert.ToDouble(med.mrp);
                                            product.MenuPrice = Convert.ToDouble(med.mrp);
                                        }
                                        else
                                        {
                                            productMedicalStock.MRP = 0;
                                        }


                                        productMedicalStock.CreatedBy = user.Name;
                                        productMedicalStock.UpdatedBy = user.Name;
                                        productMedicalStock.CategoryName1 = Convert.ToString(med.Cat1);
                                        productMedicalStock.CategoryName2 = Convert.ToString(med.Cat2);
                                        productMedicalStock.CategoryName3 = Convert.ToString(med.Cat3);
                                        productMedicalStock.CategoryName4 = Convert.ToString(med.Cat4);
                                        productMedicalStock.CategoryName5 = Convert.ToString(med.Cat5);
                                        productMedicalStock.CategoryName6 = Convert.ToString(med.Cat6);
                                        productMedicalStock.CategoryName7 = Convert.ToString(med.Cat7);
                                        productMedicalStock.CategoryName8 = Convert.ToString(med.Cat8);
                                        productMedicalStock.CategoryName9 = Convert.ToString(med.Cat9);
                                        productMedicalStock.CategoryName10 = Convert.ToString(med.Cat10);
                                        productMedicalStock.ItemTimeStamp = Convert.ToString(med.itemTimeStamp);

                                        productMedicalStock.OfferCategoryCode = CheckCategory(Convert.ToString(med.Cat1), "11");
                                        productMedicalStock.OfferCategoryName = Convert.ToString(med.Cat1);
                                        productMedicalStock.OfferCategoryType = 1;
                                        
                                        productMedicalStock.DateUpdated = DateTime.Now;
                                        db.Entry(productMedicalStock).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                       // ProductMedicalStock.Edit(productMedicalStock, out errorCode);
                                    }
                                }
                            }
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
        public JsonResult GetLastTS(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var last1 = (from p in db.Products
                         join s in db.ProductMedicalStocks on p.Id equals s.productid
                         where p.ShopCode == code
                         select s.ItemTimeStamp).ToList();
            var data = "";
            if (last1.Count > 0)
                data = last1.Max();
            else
                data = "empty";

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCAPI001")]
        public async Task<JsonResult> GetShopSelect2(string pincode, string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.PinCode == pincode && a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name,
                address =i.Address,
                phone = i.PhoneNumber,
                ownerphone = i.OwnerPhoneNumber
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult SampleTS()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]

        //public ActionResult SampleTS(WebApiCreateViewModel model)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    int errorCode = 0;
        //    try
        //    {

        //        using (WebClient myData = new WebClient())
        //        {
        //            myData.Headers["X-Auth-Token"] = "62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";
        //            myData.Headers[HttpRequestHeader.Accept] = "application/json";
        //            myData.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        //            string getList = myData.DownloadString("http://joyrahq.gofrugal.com/RayMedi_HQ/api/v1/items?q=itemTimeStamp%3E=1&limit=100000");
        //            var result = JsonConvert.DeserializeObject<RootObject>(getList);

        //            foreach (var pro in result.items)
        //            {
        //                Sample product = new Sample();
        //                product.ItemId = pro.itemId;
        //                product.ShopCode = pro.manufacturer;
        //                product.ShopName = pro.status;
        //                product.Name = pro.itemName;
        //                Sample.Add(product, out errorCode);
        //            }

        //        }
        //        return RedirectToAction("List", "Product");

        //    }
        //    catch (Exception ex)
        //    {
        //        return HttpNotFound("Error Code: " + errorCode);
        //    }
        //}
   

    }
}