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
                    foreach (var pro  in result.items)
                    {
                        var produc = db.Products.FirstOrDefault(i => i.ItemId == Convert.ToInt32(pro.itemId)); // Product.GetItemId(pro.itemId);
                        if (produc == null && pro.status =="R")
                        {
                            product.ItemId = Convert.ToInt32(pro.itemId);
                            product.ShopId = model.ShopId;
                            product.ShopName = model.ShopName;
                            product.Name = pro.itemName;
                            product.IBarU = Convert.ToInt32(pro.iBarU);
                            product.TypeId = 1;
                            product.TypeName = "Medical";
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

        public int CheckCategory(string CategoryName,double percentage)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var category = db.DiscountCategories.FirstOrDefault(i => i.Name == CategoryName); // Category.GetName(CategoryName);
            if (category != null)
            {

                return category.Id;
            }
            else
            {
                DiscountCategory cat = new DiscountCategory();
                cat.Name = CategoryName;
                cat.CategoryType = 0;
                cat.Type = 0;
                cat.Percentage = percentage;
                cat.DateEncoded = DateTime.Now;
                cat.DateUpdated = DateTime.Now;
                cat.CreatedBy = user.Name;
                cat.UpdatedBy = user.Name;
                cat.Status = 0;
                db.DiscountCategories.Add(cat);
                db.SaveChanges();
                return cat.Id;
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
                    Product product = new Product();
                    foreach (var pro in result.items)
                    {
                        var produc = db.Products.FirstOrDefault(i => i.ItemId == Convert.ToInt32(pro.itemId)); // Product.GetItemId(pro.itemId);
                        if (produc == null && pro.status == "R")
                        {
                            product.ItemId = Convert.ToInt32(pro.itemId);
                            product.ShopId = model.ShopId;
                            product.ShopName = model.ShopName;
                            product.Name = pro.itemName;
                            product.IBarU = Convert.ToInt32(pro.iBarU);
                            product.TypeId = 1;
                            product.TypeName = "Medical";
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

        [AccessPolicy(PageCode = "SHNAPITSU003")]
        public ActionResult TSUpdate()
        {
            return View();
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
                            product.TypeId = 1;
                            product.TypeName = "Medical";
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


        [AccessPolicy(PageCode = "SHNCAPI001")]
        public async Task<JsonResult> GetShopSelect2(string pincode, string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.PinCode == pincode && a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                address =i.Address,
                phone = i.PhoneNumber,
                ownerphone = i.OwnerPhoneNumber
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}