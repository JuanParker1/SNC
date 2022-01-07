using Z.EntityFramework.Extensions;
using ShopNow.Global;
using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ShopNow.Filters;

namespace ShopNow.Controllers
{
    public class StockUpdateController : Controller
    {
        private sncEntities db = new sncEntities();
       
        [AccessPolicy(PageCode = "SNCSUSM284")]
        public ActionResult StockMaintenance()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "SNCSUDM285")]
        public ActionResult DiscountMaintenance()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCSUU286")]
        public ActionResult Update(FormCollection formCollection, stockmodel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            try
            {
                var shopmodel = db.Shops.FirstOrDefault(a => a.Id == model.shopid);
                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (Request.Files[0].ContentLength > 0)
                    {
                        //int shopid = shopmodel[0].id;
                        string extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();
                        string Name = Request.Files[0].FileName.Replace(extension, Convert.ToString(shopmodel.Id));
                        Name = Name + extension;
                        string connString = "";
                        string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                        string path1 = string.Format("{0}{1}", Server.MapPath("~/Content/ExcelUpload/"), Name);
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Content/ExcelUpload/"));
                        }
                        if (validFileTypes.Contains(extension))
                        {
                            if (System.IO.File.Exists(path1))
                            {
                                System.IO.File.Delete(path1);
                            }
                            Request.Files[0].SaveAs(path1);
                            DataTable dt = new DataTable();
                            if (extension == ".csv")
                            {
                                dt = Utility.ConvertCSVtoDataTable(path1);
                            }
                            else if (extension.Trim() == ".xls")
                            {
                                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                                dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                            }
                            else if (extension.Trim() == ".xlsx")
                            {
                                connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                                dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                            }
                            var lstStock = new List<Models.Product>();
                            List<Models.Product> updateList = new List<Models.Product>();
                            List<Models.Product> createList = new List<Models.Product>();
                            //foreach (DataRow dr in dt.Rows)   
                            //{
                            //    var stock = new Stocks();
                            //if (!dr[1].ToString().EndsWith("*"))
                            //    {

                            var lst = db.Products.Where(m => m.ShopId == shopmodel.Id).ToList();
                            //var newSortq = from row in dt.AsEnumerable()
                            //                       where Convert.ToString(row.ItemArray[1])[Convert.ToString(row.ItemArray[1]).Length - 1] != '*'
                            //                       select new { 
                            //                      itemid = Convert.ToInt32(row.ItemArray[0]),
                            //                      stock  = Convert.ToInt32(row.ItemArray[3]),
                            //                      Mrp= Convert.ToDouble(row.ItemArray[2]),
                            //                      Iu = Convert.ToInt32(row.ItemArray[4]),
                            //                      Name = Convert.ToString(row.ItemArray[1]),
                            //                      itemcode = "PRO"+ Convert.ToString(row.ItemArray[0])

                            //                      };
                            var newSortq = from row in dt.AsEnumerable()
                                           where Convert.ToString(row.ItemArray[1])[Convert.ToString(row.ItemArray[1]).Length - 1] != '*'
                                           group row by Convert.ToInt32(row.ItemArray[0]) into g
                                           select new
                                           {
                                               itemid = Convert.ToInt32(g.Key),
                                               stock = g.Sum(i => Convert.ToInt32(i.ItemArray[3])),
                                               Mrp = g.Max(i => Convert.ToInt32(i.ItemArray[2])),//Convert.ToDouble(row.ItemArray[2]),
                                               Iu = g.FirstOrDefault().ItemArray[4], //Convert.ToInt32(row.ItemArray[4]),
                                               Name = g.FirstOrDefault().ItemArray[1].ToString()
                                           };
                            if (newSortq.Count() > 0)
                            {
                                foreach (var s in newSortq)
                                {
                                    int idx = lst.FindIndex(a => a.ItemId == s.itemid);
                                    if (idx >= 0)
                                    {

                                        updateList.Add(new Models.Product
                                        {
                                            Id = lst[idx].Id,
                                            ItemId = lst[idx].ItemId,
                                            IBarU = lst[idx].IBarU,
                                            MenuPrice = lst[idx].MenuPrice,
                                            Name = lst[idx].Name,
                                            Qty = s.stock,
                                            ShopId = shopmodel.Id,
                                            ShopName = shopmodel.Name,
                                            DateEncoded = DateTime.Now,
                                            DateUpdated = DateTime.Now,
                                            Status = lst[idx].Status,
                                            AppliesOnline = lst[idx].AppliesOnline,
                                            Customisation = lst[idx].Customisation,
                                            DataEntry = lst[idx].DataEntry,
                                            DiscountCategoryId = lst[idx].DiscountCategoryId,
                                            DiscountCategoryName = lst[idx].DiscountCategoryName,
                                            MaxSelectionLimit = lst[idx].MaxSelectionLimit,
                                            MinSelectionLimit = lst[idx].MinSelectionLimit,
                                            Percentage = lst[idx].Percentage,
                                            PackingCharge = lst[idx].PackingCharge,
                                            PackingType = lst[idx].PackingType,
                                            EAN = lst[idx].EAN,
                                            GTIN = lst[idx].GTIN,
                                            GTIN14 = lst[idx].GTIN14,
                                            HoldOnStok = lst[idx].HoldOnStok,
                                            ISBN = lst[idx].ISBN,
                                            IsOnline = lst[idx].IsOnline,
                                            MasterProductId = lst[idx].MasterProductId,
                                            Price = lst[idx].Price,
                                            ProductTypeId = 3,
                                            ProductTypeName = "Medical",
                                            ShopCategoryId = shopmodel.ShopCategoryId,
                                            ShopCategoryName = shopmodel.ShopCategoryName,
                                            CreatedBy = "Admin",
                                            UpdatedBy = "Admin"
                                        });
                                    }
                                    else
                                    {
                                        createList.Add(new Models.Product
                                        {
                                            Id = 0,
                                            ItemId = s.itemid,
                                            IBarU = Convert.ToInt32(s.Iu),
                                            MenuPrice = s.Mrp,
                                            Name = s.Name,
                                            Qty = s.stock,
                                            ShopId = shopmodel.Id,
                                            ShopName = shopmodel.Name,
                                            DateEncoded = DateTime.Now,
                                            DateUpdated = DateTime.Now,
                                            Status = 0,
                                            AppliesOnline = 0,
                                            Customisation = false,
                                            DataEntry = 1,
                                            DiscountCategoryId = 0,
                                            DiscountCategoryName = null,
                                            MaxSelectionLimit = 0,
                                            MinSelectionLimit = 0,
                                            Percentage = 0,
                                            PackingCharge = 0,
                                            PackingType = 0,
                                            EAN = null,
                                            GTIN = null,
                                            GTIN14 = null,
                                            HoldOnStok = 0,
                                            ISBN = null,
                                            MasterProductId = 0,
                                            Price = 0,
                                            ProductTypeId = 3,
                                            ProductTypeName = "Medical",
                                            ShopCategoryId = shopmodel.ShopCategoryId,
                                            ShopCategoryName = shopmodel.ShopCategoryName,
                                            CreatedBy = "Admin",
                                            UpdatedBy = "Admin"
                                        });

                                    }
                                }
                                db.BulkInsert(createList);
                                if (updateList.Count > 0)
                                    db.BulkUpdate(updateList);
                                // db.SaveChanges();
                            }
                            //    var varMrp=newSortq.Where(i=>i.itemid == Convert.ToInt32(dr[0])).



                            //    stock.itemid = Convert.ToInt32(dr[0]);
                            //        stock.Name = dr[1].ToString();
                            //        stock.Code = "PRO" + dr[0].ToString();
                            //        stock.Mrp = Convert.ToDouble(dr[2].ToString());
                            //        stock.stock = Convert.ToInt32(dr[3]);
                            //        stock.IU = Convert.ToInt32(dr[4]);
                            //        lstStock.Add(stock);
                            //    }

                            //}

                        }
                        else
                        {
                            ViewBag.Error = "Please Upload File in .xls, .xlsx or .csv format";
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Please Upload Valid File";
                    }
                }

                return View("StockMaintenance");
            }
            catch (Exception ex)
            {
                return View("StockMaintenance");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCSUUD287")]
        public ActionResult UpdateDiscount(FormCollection formCollection, stockmodel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            try
            {
                var shopmodel = db.Shops.FirstOrDefault(a => a.Id == model.shopid);
                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (Request.Files[0].ContentLength > 0)
                    {
                        //int shopid = shopmodel[0].id;
                        string extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();
                        string Name = Request.Files[0].FileName.Replace(extension, Convert.ToString(shopmodel.Id));
                        Name = Name + extension;
                        string connString = "";
                        string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                        string path1 = string.Format("{0}{1}", Server.MapPath("~/Content/ExcelUpload/"), Name);
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Content/ExcelUpload/"));
                        }
                        if (validFileTypes.Contains(extension))
                        {
                            if (System.IO.File.Exists(path1))
                            {
                                System.IO.File.Delete(path1);
                            }
                            Request.Files[0].SaveAs(path1);
                            DataTable dt = new DataTable();
                            if (extension == ".csv")
                            {
                                dt = Utility.ConvertCSVtoDataTable(path1);
                            }
                            else if (extension.Trim() == ".xls")
                            {
                                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                                dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                            }
                            else if (extension.Trim() == ".xlsx")
                            {
                                connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                                dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                            }
                            var lstStock = new List<Models.Product>();
                            List<Models.Product> updateList = new List<Models.Product>();
                            List<Models.Product> createList = new List<Models.Product>();
                            //foreach (DataRow dr in dt.Rows)   
                            //{
                            //    var stock = new Stocks();
                            //if (!dr[1].ToString().EndsWith("*"))
                            //    {

                            var lst = db.Products.Where(m => m.ShopId == shopmodel.Id).ToList();

                            var newSortq = from row in dt.AsEnumerable()
                                           where Convert.ToString(row.ItemArray[1])[Convert.ToString(row.ItemArray[1]).Length - 1] != '*' && Convert.ToInt32(row.ItemArray[2]) != 0
                                           group row by Convert.ToInt32(row.ItemArray[0]) into g
                                           select new
                                           {
                                               itemid = Convert.ToInt32(g.Key),
                                               DiscountCategoryPercentage = g.Min(a => Convert.ToInt32(a.ItemArray[2]))


                                           };
                            if (newSortq.Count() > 0)
                            {
                                foreach (var s in newSortq)
                                {
                                    int idx = lst.FindIndex(a => a.ItemId == s.itemid);
                                    if (idx >= 0)
                                    {

                                        updateList.Add(new Models.Product
                                        {
                                            Id = lst[idx].Id,
                                            ItemId= s.itemid,
                                            IBarU = lst[idx].IBarU,
                                            MenuPrice = lst[idx].MenuPrice,
                                            Name = lst[idx].Name,
                                            Qty = lst[idx].Qty,
                                            ShopId = shopmodel.Id,
                                            ShopName = shopmodel.Name,
                                            DateEncoded = DateTime.Now,
                                            DateUpdated = DateTime.Now,
                                            Status = lst[idx].Status,
                                            AppliesOnline = lst[idx].AppliesOnline,
                                            Customisation = lst[idx].Customisation,
                                            DataEntry = lst[idx].DataEntry,
                                            DiscountCategoryId = lst[idx].DiscountCategoryId,
                                            MaxSelectionLimit = lst[idx].MaxSelectionLimit,
                                            MinSelectionLimit = lst[idx].MinSelectionLimit,
                                            Percentage = lst[idx].Percentage,
                                            PackingCharge = lst[idx].PackingCharge,
                                            PackingType = lst[idx].PackingType,
                                            DiscountCategoryName = lst[idx].DiscountCategoryName,
                                            EAN = lst[idx].EAN,
                                            GTIN = lst[idx].GTIN,
                                            GTIN14 = lst[idx].GTIN14,
                                            HoldOnStok = lst[idx].HoldOnStok,
                                            ISBN = lst[idx].ISBN,
                                            IsOnline = lst[idx].IsOnline,
                                            MasterProductId = lst[idx].MasterProductId,
                                            Price = lst[idx].MenuPrice - ((lst[idx].MenuPrice * s.DiscountCategoryPercentage) / 100),
                                            ProductTypeId = 3,
                                            ProductTypeName = "Medical",
                                            ShopCategoryId = shopmodel.ShopCategoryId,
                                            ShopCategoryName = shopmodel.ShopCategoryName,
                                            CreatedBy = "Admin",
                                            UpdatedBy = "Admin"
                                        });
                                    }
                                }

                                if (updateList.Count > 0)
                                    db.BulkUpdate(updateList);
                                // db.SaveChanges();
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Please Upload File in .xls, .xlsx or .csv format";

                        }

                    }
                    else
                    {
                        ViewBag.Error = "Please Upload Valid File";
                    }

                }

                return View("StockMaintenance");
            }
            catch (Exception ex)
            {
                return View("StockMaintenance");
            }
        }

        public async Task<JsonResult> GetActiveShopSelect(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status != 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

    }
}