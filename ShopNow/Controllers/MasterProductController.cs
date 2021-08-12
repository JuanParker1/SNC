using Amazon;
using Amazon.S3;
using AutoMapper;
using ExcelDataReader;
using ShopNow.Base;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class MasterProductController : Controller
    {
        private sncEntities _db = new sncEntities();
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
        
        public MasterProductController()
        {            
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<MasterElectronicCreateViewModel, MasterProduct>();
                config.CreateMap<MasterElectronicEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterElectronicEditViewModel>();
                config.CreateMap<MasterMedicalCreateViewModel, MasterProduct>();
                config.CreateMap<MasterMedicalEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterMedicalEditViewModel>();
                config.CreateMap<Product, ItemMappingViewModel>();
                config.CreateMap<MasterFMCGCreateViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterFMCGEditViewModel>();
                config.CreateMap<MasterFMCGEditViewModel, MasterProduct>();
                 config.CreateMap<MasterFoodCreateViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterFoodCreateViewModel>();
                config.CreateMap<MasterFoodEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterFoodEditViewModel>();
                config.CreateMap<MasterFoodEditViewModel.AddonList, MasterAddOnsCreateViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        // All MasterItem List
        [AccessPolicy(PageCode = "SHNMPRL001")]
        public ActionResult List()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0).ToList();
            return View(List);
        }

        // All MasterItem Delete
        [AccessPolicy(PageCode = "SHNMPRD004")]
        public JsonResult Delete(string Id)
        {
            var dCode = AdminHelpers.DCodeLong(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            //product delete
            var productList = _db.Products.Where(i => i.MasterProductId == master.Id);
            if (productList != null)
            {
                foreach (var item in productList)
                {
                    var product = _db.Products.FirstOrDefault(i => i.Id == item.Id);
                    product.Status = 2;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Dish Create
        [AccessPolicy(PageCode = "SHNMPRFC015")]
        public ActionResult FoodCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Session["AddOns"] = new List<MasterAddOnsCreateViewModel>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRFC015")]
        public ActionResult FoodCreate(MasterFoodCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var isExist = _db.MasterProducts.Any(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 1 && i.CategoryId == model.CategoryId);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }
            var master = _mapper.Map<MasterFoodCreateViewModel, MasterProduct>(model);
            master.CreatedBy = user.Name;
            master.UpdatedBy = user.Name;
            master.ProductTypeName = "Dish";
            master.ProductTypeId = 1;
            master.Status = 0;

            master.Name = model.Name;
            if (model.NickName == null)
                master.NickName = model.Name;

            master.DateEncoded = DateTime.Now;
            master.DateUpdated = DateTime.Now;
            master.Status = 0;
            try
            {
                long ticks = DateTime.Now.Ticks;
                if (model.DishImage != null)
                {
                    uc.UploadFiles(model.DishImage.InputStream, ticks + "_" + model.DishImage.FileName, accesskey, secretkey, "image");
                    master.ImagePath1 = ticks + "_" + model.DishImage.FileName.Replace(" ", "");
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            _db.MasterProducts.Add(master);
            _db.SaveChanges();
            ViewBag.Message = model.Name + " Saved Successfully!";

            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            var productDishaddOn = new ProductDishAddOn();
            if (addOns != null)
            {
                foreach (var s in addOns)
                {
                    productDishaddOn.AddOnItemName = s.AddOnItemName;
                    productDishaddOn.MasterProductId = master.Id;
                    productDishaddOn.MasterProductName = master.Name;
                    productDishaddOn.AddOnCategoryId = s.AddOnCategoryId;
                    productDishaddOn.AddOnCategoryName = s.AddOnCategoryName;
                    productDishaddOn.PortionId = s.PortionId;
                    productDishaddOn.PortionName = s.PortionName;
                    productDishaddOn.MinSelectionLimit = s.MinSelectionLimit;
                    productDishaddOn.MaxSelectionLimit = s.MaxSelectionLimit;
                    productDishaddOn.CrustName = s.CrustName;
                    productDishaddOn.AddOnsPrice = s.AddOnsPrice;
                    productDishaddOn.PortionPrice = s.PortionPrice;
                    productDishaddOn.CrustPrice = s.CrustPrice;
                    productDishaddOn.CreatedBy = user.Name;
                    productDishaddOn.UpdatedBy = user.Name;
                    productDishaddOn.DateEncoded = DateTime.Now;
                    productDishaddOn.DateUpdated = DateTime.Now;
                    productDishaddOn.Status = 0;
                    productDishaddOn.AddOnType = s.AddOnType;
                    productDishaddOn.MasterProductId = master.Id;
                    _db.ProductDishAddOns.Add(productDishaddOn);
                    _db.SaveChanges();
                }
            }
            Session["AddOns"] = null;
            return View();
        }

        // Dish Update
        [AccessPolicy(PageCode = "SHNMPRFE016")]
        public ActionResult FoodEdit(string Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dId = AdminHelpers.DCodeLong(Id);
            if (string.IsNullOrEmpty(dId.ToString()))
                return HttpNotFound();
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dId);
            Session["EditAddOns"] = new List<MasterAddOnsCreateViewModel>();
            var addOns = new List<MasterAddOnsCreateViewModel>();
            var model = _mapper.Map<MasterProduct, MasterFoodEditViewModel>(master);
            model.CategoryName = _db.Categories.FirstOrDefault(i => i.Id == master.CategoryId).Name;
            model.AddonLists = _db.ProductDishAddOns.Where(i => i.MasterProductId == master.Id && i.Status == 0).Select(i => new MasterFoodEditViewModel.AddonList
            {
                Id = i.Id,
                AddOnItemName = i.AddOnItemName,
                AddOnCategoryId = i.AddOnCategoryId,
                MasterProductName = i.MasterProductName,
                PortionId = i.PortionId,
                PortionName = i.PortionName,
                PortionPrice = i.PortionPrice,
                AddOnCategoryName = i.AddOnCategoryName,
                AddOnsPrice = i.AddOnsPrice,
                CrustName = i.CrustName,
                CrustPrice = i.CrustPrice,
                MinSelectionLimit = i.MinSelectionLimit,
                MaxSelectionLimit = i.MaxSelectionLimit,
                AddOnType = i.AddOnType,
                MasterProductId =i.MasterProductId.Value
            }).ToList();
            foreach(var s in model.AddonLists)
            {
                var addOn = _mapper.Map<MasterFoodEditViewModel.AddonList, MasterAddOnsCreateViewModel>(s);
                addOns.Add(addOn);
            }
            Session["EditAddOns"] = addOns;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRFE016")]
        public ActionResult FoodEdit(MasterFoodEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var master = _db.MasterProducts.FirstOrDefault(i=>i.Id == model.Id); 
            _mapper.Map(model, master);
            master.DateUpdated = DateTime.Now;
            master.UpdatedBy = user.Name;
            try
            {
                long ticks = DateTime.Now.Ticks;
                if (model.DishImage != null)
                {
                    uc.UploadFiles(model.DishImage.InputStream, ticks + "_" + model.DishImage.FileName, accesskey, secretkey, "image");
                    master.ImagePath1 = ticks + "_" + model.DishImage.FileName.Replace(" ", "");
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            List<MasterAddOnsCreateViewModel> addOns = Session["EditAddOns"] as List<MasterAddOnsCreateViewModel>;
            var productDishaddOn = new ProductDishAddOn();
            if (addOns != null)
            {
                foreach (var s in addOns)
                {
                    if (s.Id == 0)
                    {
                        productDishaddOn.AddOnItemName = s.AddOnItemName;
                        productDishaddOn.MasterProductId = master.Id;
                        productDishaddOn.MasterProductName = master.Name;
                        productDishaddOn.AddOnCategoryId = s.AddOnCategoryId;
                        productDishaddOn.AddOnCategoryName = s.AddOnCategoryName;
                        productDishaddOn.PortionId = s.PortionId;
                        productDishaddOn.PortionName = s.PortionName;
                        productDishaddOn.MinSelectionLimit = s.MinSelectionLimit;
                        productDishaddOn.MaxSelectionLimit = s.MaxSelectionLimit;
                        productDishaddOn.CrustName = s.CrustName;
                        productDishaddOn.AddOnsPrice = s.AddOnsPrice;
                        productDishaddOn.PortionPrice = s.PortionPrice;
                        productDishaddOn.CrustPrice = s.CrustPrice;
                        productDishaddOn.CreatedBy = user.Name;
                        productDishaddOn.UpdatedBy = user.Name;
                        productDishaddOn.DateEncoded = DateTime.Now;
                        productDishaddOn.DateUpdated = DateTime.Now;
                        productDishaddOn.Status = 0;
                        productDishaddOn.AddOnType = s.AddOnType;
                        productDishaddOn.MasterProductId = master.Id;
                        _db.ProductDishAddOns.Add(productDishaddOn);
                        _db.SaveChanges();
                    }
                }
            }
            Session["EditAddOns"] = null;
            return RedirectToAction("FoodEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        // Dish List
        [AccessPolicy(PageCode = "SHNMPRFL017")]
        public ActionResult FoodList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name; 
            var List = _db.MasterProducts.Join(_db.Categories, mp => mp.CategoryId, c => c.Id, (mp, c) => new { mp, c })
                .Where(i => i.mp.Status == 0 && i.mp.ProductTypeId == 1)
                .Select(i => new MasterProductFoodListViewModel.MasterProductFoodList
                {
                    Id = i.mp.Id,
                    Name = i.mp.Name,
                    CategoryName = i.c.Name,
                    Price = i.mp.Price,
                    ProductTypeName = i.mp.ProductTypeName
                }).OrderBy(i => i.Name).ToList();
            return View(List);
        }

        // Dish Delete
        [AccessPolicy(PageCode = "")]
        public JsonResult FoodDelete(string Id)
        {
            var dId = AdminHelpers.DCodeLong(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dId);
            if (master != null)
            {
                master.Status = 2;
                _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Upload FMCG MasterItems
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult FMCGUpload()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult FMCGUpload(HttpPostedFileBase upload, MasterFMCGUploadViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            DataTable dt = new DataTable();
            if (model.button == "upload")
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + upload.FileName);
                upload.SaveAs(path);
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        Stream stream = upload.InputStream;
                        IExcelDataReader reader = null;

                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View();
                        }
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                        reader.Close();
                        model.DataTable = result.Tables[0];
                        model.Filename = upload.FileName;
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }
                }
            }
            else
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + model.Filename);
                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";

                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                using (OleDbConnection connExcel = new OleDbConnection(excelConnectionString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                // Insert records to database table.
                // MainPageModel entities = new MainPageModel();
                foreach (DataRow row in dt.Rows)
                {
                    var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);
                    if (masterProduct == null)
                    {
                         model.CategoryId = CheckCategory(row[model.CategoryName].ToString(), model.ProductTypeId, model.ProductTypeName);
                         model.SubCategoryId = CheckSubCategory(model.CategoryId, row[model.CategoryName].ToString(), row[model.SubCategoryName].ToString(), model.ProductTypeId, model.ProductTypeName);
                        model.NextSubCategoryId = CheckNextSubCategory(model.SubCategoryId, row[model.SubCategoryName].ToString(), row[model.NextSubCategoryName].ToString(), model.ProductTypeId, model.ProductTypeName);
                        _db.MasterProducts.Add(new MasterProduct
                        {
                            Name = row[model.Name].ToString(),
                            BrandId = CheckBrand(row[model.BrandName].ToString(), model.ProductTypeId, model.ProductTypeName),
                            BrandName = row[model.BrandName].ToString(),
                            SizeLB = row[model.SizeLB].ToString(),
                            Weight = Convert.ToDouble(row[Convert.ToInt32(model.Weight)]),
                            GoogleTaxonomyCode = row[model.GoogleTaxonomyCode].ToString(),
                            ASIN = row[model.ASIN].ToString(),
                            CategoryId = model.CategoryId,
                            ShortDescription = row[model.ShortDescription].ToString(),
                            LongDescription = row[model.LongDescription].ToString(),
                            Price = Convert.ToDouble(row[Convert.ToInt32(model.Price)]),
                            ImagePath1 = row[model.ImagePath1].ToString(),
                            ImagePath2 = row[model.ImagePath2].ToString(),
                            ImagePath3 = row[model.ImagePath3].ToString(),
                            ImagePath4 = row[model.ImagePath4].ToString(),
                            ImagePath5 = row[model.ImagePath5].ToString(),
                            SubCategoryId = model.SubCategoryId,
                            NextSubCategoryId = model.NextSubCategoryId,
                            ProductTypeId = model.ProductTypeId,
                            ProductTypeName = model.ProductTypeName,
                            PackageId = CheckPackage(row[model.PackageName].ToString()),
                            PackageName = row[model.PackageName].ToString(),
                            MeasurementUnitId = CheckMeasurementUnit(row[model.MeasurementUnitName].ToString()),
                            MeasurementUnitName = row[model.MeasurementUnitName].ToString(),
                            Status = 0,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name,
                            DateEncoded = DateTime.Now,
                            DateUpdated = DateTime.Now,
                        });
                        _db.SaveChanges();
                    }
                }
            }
            return View();
        }

        // FMCG List
        [AccessPolicy(PageCode = "SHNMPRFL022")]
        public ActionResult FMCGList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var List = _db.MasterProducts
                       .OrderBy(mp => mp.Name)
                       .Where(mp => mp.Status == 0 && mp.ProductTypeId == 2)
                       .Select(i => new MasterProductFMCGListViewModel.MasterProductFMCGList
                       {
                           Id = i.Id,
                           BrandName = i.BrandName,
                           Name = i.Name,
                           ProductTypeName= i.ProductTypeName
                       })
                      .ToList();
            return View(List);
        }

        // FMCG Create
        [AccessPolicy(PageCode = "SHNMPRFC023")]
        public ActionResult FMCGCreate()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRFC023")]
        public ActionResult FMCGCreate(MasterFMCGCreateViewModel model)
        {
            var isExist = _db.MasterProducts.Any(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 2);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }

            var user = ((Helpers.Sessions.User)Session["USER"]);
            var master = _mapper.Map<MasterFMCGCreateViewModel, MasterProduct>(model);
            master.CreatedBy = user.Name;
            master.UpdatedBy = user.Name;
            master.ProductTypeId = 2;
            master.ProductTypeName = "FMCG";
            if (model.NickName == null)
            {
                master.NickName = model.Name;
            }
            try
            {
                long ticks = DateTime.Now.Ticks;

                //ProductImage1
                if (model.FMCGImage1 != null)
                {
                    uc.UploadFiles(model.FMCGImage1.InputStream, ticks + "_" + model.FMCGImage1.FileName, accesskey, secretkey, "image");
                    master.ImagePath1 = ticks + "_" + model.FMCGImage1.FileName.Replace(" ", "");

                }
                //ProductImage2
                if (model.FMCGImage2 != null)
                {
                    uc.UploadFiles(model.FMCGImage2.InputStream, ticks + "_" + model.FMCGImage2.FileName, accesskey, secretkey, "image");
                    master.ImagePath2 = ticks + "_" + model.FMCGImage2.FileName.Replace(" ", "");

                }
                //ProductImage3
                if (model.FMCGImage3 != null)
                {
                    uc.UploadFiles(model.FMCGImage3.InputStream, ticks + "_" + model.FMCGImage3.FileName, accesskey, secretkey, "image");
                    master.ImagePath3 = ticks + "_" + model.FMCGImage3.FileName.Replace(" ", "");

                }
                //ProductImage4
                if (model.FMCGImage4 != null)
                {
                    uc.UploadFiles(model.FMCGImage4.InputStream, ticks + "_" + model.FMCGImage4.FileName, accesskey, secretkey, "image");
                    master.ImagePath4 = ticks + "_" + model.FMCGImage4.FileName.Replace(" ", "");

                }

                //ProductImage5
                if (model.FMCGImage5 != null)
                {
                    uc.UploadFiles(model.FMCGImage5.InputStream, ticks + "_" + model.FMCGImage5.FileName, accesskey, secretkey, "image");
                    master.ImagePath5 = ticks + "_" + model.FMCGImage5.FileName.Replace(" ", "");
                }
                master.DateEncoded = DateTime.Now;
                master.DateUpdated = DateTime.Now;
                master.Status = 0;
                _db.MasterProducts.Add(master);
                _db.SaveChanges();
                ViewBag.Message = model.Name + " Saved Successfully!";

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            return View();
        }

        // FMCG Update
        [AccessPolicy(PageCode = "SHNMPRFE024")]
        public ActionResult FMCGEdit(string Id)
        {
            var dId = AdminHelpers.DCodeLong(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dId.ToString()))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Id == dId);
            if (masterProduct.ImagePath1 != null)
                masterProduct.ImagePath1 = masterProduct.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePath2 != null)
                masterProduct.ImagePath2 = masterProduct.ImagePath2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePath3 != null)
                masterProduct.ImagePath3 = masterProduct.ImagePath3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePath4 != null)
                masterProduct.ImagePath4 = masterProduct.ImagePath4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePath5 != null)
                masterProduct.ImagePath5 = masterProduct.ImagePath5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            var model = _mapper.Map<MasterProduct, MasterFMCGEditViewModel>(masterProduct);
            model.CategoryName = _db.Categories.FirstOrDefault(i => i.Id == masterProduct.CategoryId).Name;
            model.SubCategoryName = _db.SubCategories.FirstOrDefault(i => i.Id == masterProduct.SubCategoryId).Name;
            model.NextSubCategoryName = _db.NextSubCategories.FirstOrDefault(i => i.Id == masterProduct.NextSubCategoryId).Name;
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRFE024")]
        public ActionResult FMCGEdit(MasterFMCGEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, master);
            master.Name = model.Name;
            master.ProductTypeId = 2;

            master.UpdatedBy = user.Name;
            master.DateUpdated = DateTime.Now;
            if (model.NickName == null)
            {
                master.NickName = model.Name;
            }
            try
            {
                long ticks = DateTime.Now.Ticks;
                //ProductImage1
                if (model.FMCGImage1 != null)
                {
                    uc.UploadFiles(model.FMCGImage1.InputStream, ticks + "_" + model.FMCGImage1.FileName, accesskey, secretkey, "image");
                    master.ImagePath1 = ticks + "_" + model.FMCGImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.FMCGImage2 != null)
                {
                    uc.UploadFiles(model.FMCGImage2.InputStream, ticks + "_" + model.FMCGImage2.FileName, accesskey, secretkey, "image");
                    master.ImagePath2 = ticks + "_" + model.FMCGImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.FMCGImage3 != null)
                {
                    uc.UploadFiles(model.FMCGImage3.InputStream, ticks + "_" + model.FMCGImage3.FileName, accesskey, secretkey, "image");
                    master.ImagePath3 = ticks + "_" + model.FMCGImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.FMCGImage4 != null)
                {
                    uc.UploadFiles(model.FMCGImage4.InputStream, ticks + "_" + model.FMCGImage4.FileName, accesskey, secretkey, "image");
                    master.ImagePath4 = ticks + "_" + model.FMCGImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.FMCGImage5 != null)
                {
                    uc.UploadFiles(model.FMCGImage5.InputStream, ticks + "_" + model.FMCGImage5.FileName, accesskey, secretkey, "image");
                    master.ImagePath5 = ticks + "_" + model.FMCGImage5.FileName.Replace(" ", "");
                }
                master.DateUpdated = DateTime.Now;
                _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            //return RedirectToAction("FMCGList");
            return RedirectToAction("FMCGEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        // FMCG Delete
        [AccessPolicy(PageCode = "SHNMPRFD025")]
        public JsonResult FMCGDelete(string Id)
        {
            var dId = AdminHelpers.DCodeLong(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dId);
            if (master != null)
            {
                master.Status = 2;
                _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Medical List
        [AccessPolicy(PageCode = "SHNMPRML006")]
        public ActionResult MedicalList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var List = _db.MasterProducts.Join(_db.Categories, mp => mp.CategoryId, c => c.Id, (mp, c) => new { mp, c })
                        .Where(i => i.mp.Status == 0 && i.mp.ProductTypeId == 3)
                        .Select(i => new MasterProductMedicalListViewModel.MasterProductMedicalList
                        {
                            Id = i.mp.Id,
                            Name = i.mp.Name,
                            BrandName = i.mp.BrandName,
                            CategoryName = i.c.Name,
                            DrugCompoundDetailName = i.mp.DrugCompoundDetailName,
                            ProductTypeName = i.mp.ProductTypeName
                        }).OrderBy(i=> i.Name).ToList();
            return View(List);
        }

        // Medical Create
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public ActionResult MedicalCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public ActionResult MedicalCreate(MasterMedicalCreateViewModel model)
        {
            var isExist = _db.MasterProducts.Any(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 3);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var master = _mapper.Map<MasterMedicalCreateViewModel, MasterProduct>(model);
            master.CreatedBy = user.Name;
            master.UpdatedBy = user.Name;
            master.ProductTypeName = "Medical";
            master.ProductTypeId = 3;
            if (model.NickName == null)
            {
                master.NickName = model.Name;
            }
            try
            {
                long ticks = DateTime.Now.Ticks;
                if (model.DrugCompoundDetailIds != null)
                {
                    master.DrugCompoundDetailIds = String.Join(",", model.DrugCompoundDetailIds);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.DrugCompoundDetailIds)
                    {
                        var sid = Convert.ToInt32(s);
                        var dcd = _db.DrugCompoundDetails.FirstOrDefault(i => i.Id == sid && i.Status == 0);
                        sb.Append(dcd.AliasName);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.DrugCompoundDetailName = sb.ToString().Remove(sb.Length - 1);
                        master.DrugCompoundDetailName = model.DrugCompoundDetailName;
                    }
                    else
                    {
                        model.DrugCompoundDetailName = sb.ToString();
                        master.DrugCompoundDetailName = model.DrugCompoundDetailName;
                    }
                }

                //MedicalImage1
                if (model.MedicalImage1 != null)
                {
                    uc.UploadFiles(model.MedicalImage1.InputStream, ticks + "_" + model.MedicalImage1.FileName, accesskey, secretkey, "image");
                    master.ImagePath1 = ticks + "_" + model.MedicalImage1.FileName.Replace(" ", "");
                }

                //MedicalImage2
                if (model.MedicalImage2 != null)
                {
                    uc.UploadFiles(model.MedicalImage2.InputStream, ticks + "_" + model.MedicalImage2.FileName, accesskey, secretkey, "image");
                    master.ImagePath2 = ticks + "_" + model.MedicalImage2.FileName.Replace(" ", "");
                }

                //MedicalImage3
                if (model.MedicalImage3 != null)
                {
                    uc.UploadFiles(model.MedicalImage3.InputStream, ticks + "_" + model.MedicalImage3.FileName, accesskey, secretkey, "image");
                    master.ImagePath3 = ticks + "_" + model.MedicalImage3.FileName.Replace(" ", "");
                }

                //MedicalImage4
                if (model.MedicalImage4 != null)
                {
                    uc.UploadFiles(model.MedicalImage4.InputStream, ticks + "_" + model.MedicalImage4.FileName, accesskey, secretkey, "image");
                    master.ImagePath4 = ticks + "_" + model.MedicalImage4.FileName.Replace(" ", "");
                }

                //MedicalImage5
                if (model.MedicalImage5 != null)
                {
                    uc.UploadFiles(model.MedicalImage5.InputStream, ticks + "_" + model.MedicalImage5.FileName, accesskey, secretkey, "image");
                    master.ImagePath5 = ticks + "_" + model.MedicalImage5.FileName.Replace(" ", "");
                }
                master.DateEncoded = DateTime.Now;
                master.DateUpdated = DateTime.Now;
                master.Status = 0;
                _db.MasterProducts.Add(master);
                _db.SaveChanges();
                ViewBag.Message = model.Name + " Saved Successfully!";
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            return View();
        }

        // Medical Update
        [AccessPolicy(PageCode = "SHNMPRME008")]
        public ActionResult MedicalEdit(string id)
        {
            var dId = AdminHelpers.DCodeLong(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dId.ToString()))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<MasterProduct, MasterMedicalEditViewModel>(masterProduct);
            if(model.ImagePath1 !=null)
            model.ImagePath1 = model.ImagePath1.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePath2 != null)
                model.ImagePath2 = model.ImagePath2.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePath3 != null)
                model.ImagePath3 = model.ImagePath3.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePath4 != null)
                model.ImagePath4 = model.ImagePath4.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePath5 != null)
                model.ImagePath5 = model.ImagePath5.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            model.DrugCompoundDetailIds1 = masterProduct.DrugCompoundDetailIds;
            model.CategoryName = _db.Categories.FirstOrDefault(i => i.Id == masterProduct.CategoryId).Name;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRME008")]
        public ActionResult MedicalEdit(MasterMedicalEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, master);
            master.UpdatedBy = user.Name;
            master.DateUpdated = DateTime.Now;
            try
            {
                long ticks = DateTime.Now.Ticks;
                if (model.DrugCompoundDetailIds != null)
                {
                    master.DrugCompoundDetailIds = String.Join(",", model.DrugCompoundDetailIds);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.DrugCompoundDetailIds)
                    {
                        var sid = Convert.ToInt32(s);
                        var dcd = _db.DrugCompoundDetails.FirstOrDefault(i => i.Id == sid && i.Status == 0);
                        sb.Append(dcd.AliasName);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.DrugCompoundDetailName = sb.ToString().Remove(sb.Length - 1);
                        master.DrugCompoundDetailName = model.DrugCompoundDetailName;
                    }
                    else
                    {
                        model.DrugCompoundDetailName = sb.ToString();
                        master.DrugCompoundDetailName = model.DrugCompoundDetailName;
                    }
                }

                //MedicalImage1
                if (model.MedicalImage1 != null)
                {
                    uc.UploadFiles(model.MedicalImage1.InputStream, ticks + "_" + model.MedicalImage1.FileName, accesskey, secretkey, "image");
                    master.ImagePath1 = ticks + "_" + model.MedicalImage1.FileName.Replace(" ", "");
                }

                //MedicalImage2
                if (model.MedicalImage2 != null)
                {
                    uc.UploadFiles(model.MedicalImage2.InputStream, ticks + "_" + model.MedicalImage2.FileName, accesskey, secretkey, "image");
                    master.ImagePath2 = ticks + "_" + model.MedicalImage2.FileName.Replace(" ", "");
                }

                //MedicalImage3
                if (model.MedicalImage3 != null)
                {
                    uc.UploadFiles(model.MedicalImage3.InputStream, ticks + "_" + model.MedicalImage3.FileName, accesskey, secretkey, "image");
                    master.ImagePath3 = ticks + "_" + model.MedicalImage3.FileName.Replace(" ", "");
                }

                //MedicalImage4
                if (model.MedicalImage4 != null)
                {
                    uc.UploadFiles(model.MedicalImage4.InputStream, ticks + "_" + model.MedicalImage4.FileName, accesskey, secretkey, "image");
                    master.ImagePath4 = ticks + "_" + model.MedicalImage4.FileName.Replace(" ", "");
                }

                //MedicalImage5
                if (model.MedicalImage5 != null)
                {
                    uc.UploadFiles(model.MedicalImage5.InputStream, ticks + "_" + model.MedicalImage5.FileName, accesskey, secretkey, "image");
                    master.ImagePath5 = ticks + "_" + model.MedicalImage5.FileName.Replace(" ", "");
                }

                _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            // return RedirectToAction("MedicalList");
            return RedirectToAction("MedicalEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        // Medical Update
        [AccessPolicy(PageCode = "SHNMPRMD009")]
        public JsonResult MedicalDelete(string Id)
        {
            var dId = AdminHelpers.DCodeLong(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dId);
            if (master != null)
            {
                master.Status = 2;
                _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Upload Medical MasterItems
        [AccessPolicy(PageCode = "SHNMPRMI010")]
        public ActionResult MedicalUpload()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRMI010")]
        public ActionResult MedicalUpload(HttpPostedFileBase upload, MasterMedicalUploadViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            DataTable dt = new DataTable();

            if (model.button == "upload")
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + upload.FileName);
                upload.SaveAs(path);
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        Stream stream = upload.InputStream;
                        IExcelDataReader reader = null;

                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View();
                        }
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        // reader.IsFirstRowAsColumnNames = true;

                        reader.Close();
                        model.DataTable = result.Tables[0];
                        model.Filename = upload.FileName;
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }
                }
            }
            else
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + model.Filename);
                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";

                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                using (OleDbConnection connExcel = new OleDbConnection(excelConnectionString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                // Insert records to database table.
                // MainPageModel entities = new MainPageModel();
                foreach (DataRow row in dt.Rows)
                {
                    var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);
                    if (masterProduct == null)
                    {
                        _db.MasterProducts.Add(new MasterProduct
                        {
                            Name = row[model.Name].ToString(),
                            BrandId = MedicalCheckBrand(row[model.BrandName].ToString()),
                            BrandName = row[model.BrandName].ToString(),
                            CategoryId = MedicalCheckCategory(row[model.CategoryName].ToString(), model.ProductTypeId, model.ProductTypeName),
                            MeasurementUnitId = CheckMeasurementUnit(row[model.MeasurementUnitName].ToString()),
                            MeasurementUnitName = row[model.MeasurementUnitName].ToString(),
                            PriscriptionCategory = Convert.ToBoolean(row[Convert.ToInt32(model.PriscriptionCategory)]),
                            DrugCompoundDetailIds = row[model.DrugCompoundDetailIds].ToString(),
                            DrugCompoundDetailName = row[model.DrugCompoundDetailName].ToString(),
                            Price = Convert.ToDouble(row[Convert.ToInt32(model.Price)]),
                            ImagePath1 = row[model.ImagePath1].ToString(),
                            ImagePath2 = row[model.ImagePath2].ToString(),
                            ImagePath3 = row[model.ImagePath3].ToString(),
                            ImagePath4 = row[model.ImagePath4].ToString(),
                            ImagePath5 = row[model.ImagePath5].ToString(),
                            OriginCountry = row[model.OriginCountry].ToString(),
                            Manufacturer = row[model.Manufacturer].ToString(),
                            IBarU = Convert.ToInt32(row[model.IBarU ?? 0]),
                            SizeLB = row[model.SizeLB].ToString(),
                            Weight = Convert.ToDouble(row[Convert.ToInt32(model.Weight)]),
                            PackageId = CheckMedicalPackage(row[model.PackageName].ToString()),
                            PackageName = row[model.PackageName].ToString(),
                            ProductTypeId = model.ProductTypeId,
                            ProductTypeName = model.ProductTypeName,
                            ColorCode = "N/A",
                            LongDescription = "N/A",
                            ShortDescription = "N/A",
                            Status = 0,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name,
                            DateEncoded = DateTime.Now,
                            DateUpdated = DateTime.Now
                        });
                        _db.SaveChanges();
                    }
                }
            }
            return View();
        }

        public int MedicalCheckBrand(string BrandName)
        {
            var brand = _db.Brands.FirstOrDefault(i => i.Name == BrandName && i.Status == 0);
            if (brand != null)
            {
                return brand.Id;
            }
            else
            {
                Brand br = new Brand();
                br.Name = BrandName;
                br.Status = 0;
                br.DateEncoded = DateTime.Now;
                br.DateUpdated = DateTime.Now;
                _db.Brands.Add(br);
                _db.SaveChanges();
                return br.Id;
            }

        }

        public int MedicalCheckCategory(string CategoryName, int ProductTypeId, string ProductTypeName)
        {
            var category = _db.Categories.FirstOrDefault(i => i.Name == CategoryName && i.Status == 0 && i.ProductTypeId == ProductTypeId);
            if (category != null)
            {
                return category.Id;
            }
            else
            {
                Category cat = new Category();
                cat.Name = CategoryName;
                cat.ProductTypeId = ProductTypeId;
                cat.ProductTypeName = ProductTypeName;
                cat.OrderNo = 0;
                cat.Status = 0;
                cat.DateEncoded = DateTime.Now;
                cat.DateUpdated = DateTime.Now;
                _db.Categories.Add(cat);
                _db.SaveChanges();
                return cat.Id;
            }

        }

        public int CheckMedicalPackage(string PackageName)
        {
            var package = _db.Packages.FirstOrDefault(i => i.Name == PackageName && i.Status == 0);// Package.GetName(PackageName);
            if (package != null)
            {
                return package.Id;
            }
            else
            {
                Package mp = new Package();
                mp.Name = PackageName;
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                _db.Packages.Add(mp);
                _db.SaveChanges();
                return mp.Id;
            }

        }

        public int CheckPackage(string PackageName)
        {
            var package = _db.Packages.FirstOrDefault(i => i.Name == PackageName && i.Status == 0);
            if (package != null)
            {
                return package.Id;
            }
            else
            {
                Package mp = new Package();
                mp.Name = PackageName;
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                _db.Packages.Add(mp);
                _db.SaveChanges();
                return mp.Id;
            }
        }

        public int CheckMeasurementUnit(string MeasurementUnitName)
        {
            var mu = _db.MeasurementUnits.FirstOrDefault(i => i.UnitName == MeasurementUnitName && i.Status == 0);
            if (mu != null)
            {
                return mu.Id;
            }
            else
            {
                MeasurementUnit mp = new MeasurementUnit();
                mp.UnitName = MeasurementUnitName;
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                _db.MeasurementUnits.Add(mp);
                _db.SaveChanges();
                return mp.Id;
            }
        }

        // Electronic List
        [AccessPolicy(PageCode = "SHNMPREL018")]
        public ActionResult ElectronicList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = _db.MasterProducts.Join(_db.Categories, mp => mp.CategoryId, c => c.Id, (mp, c) => new { mp, c })
                .Where(i => i.mp.Status == 0 && i.mp.ProductTypeId == 4)
                .Select(i => new MasterElectronicListViewModel.MasterElectronicList
                {
                    Id = i.mp.Id,
                    Name = i.mp.Name,
                    BrandName = i.mp.BrandName,
                    CategoryName = i.c.Name,
                    ProductTypeName = i.mp.ProductTypeName

                }).OrderBy(i =>i.Name).ToList();
            return View(List);
        }

        // Electronic Create
        [AccessPolicy(PageCode = "SHNMPREC019")]
        public ActionResult ElectronicCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPREC019")]
        public ActionResult ElectronicCreate(MasterElectronicCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MasterElectronicCreateViewModel, MasterProduct>(model);
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.ProductTypeName = "Electronic";
            prod.ProductTypeId = 4;
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 1 && i.CategoryId == model.CategoryId);
            prod.Name = model.Name;
            if (model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            try
            {
                //if (model.CategoryCode != null)
                //{
                //    prod.CategoryCode = String.Join(",", model.CategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.CategoryCode)
                //    {
                //        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                //        sb.Append(cat.Name);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.CategoryName = model.CategoryName;
                //    }
                //    else
                //    {
                //        model.CategoryName = sb.ToString();
                //        prod.CategoryName = model.CategoryName;
                //    }
                //}
                //ElectronicImage1
                if (model.ElectronicImage1 != null)
                {
                    uc.UploadFiles(model.ElectronicImage1.InputStream, prod.Id + "_" + model.ElectronicImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = prod.Id + "_" + model.ElectronicImage1.FileName.Replace(" ", "");
                }

                //ElectronicImage2
                if (model.ElectronicImage2 != null)
                {
                    uc.UploadFiles(model.ElectronicImage2.InputStream, prod.Id + "_" + model.ElectronicImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePath2 = prod.Id + "_" + model.ElectronicImage2.FileName.Replace(" ", "");
                }

                //ElectronicImage3
                if (model.ElectronicImage3 != null)
                {
                    uc.UploadFiles(model.ElectronicImage3.InputStream, prod.Id + "_" + model.ElectronicImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePath3 = prod.Id + "_" + model.ElectronicImage3.FileName.Replace(" ", "");
                }

                //ElectronicImage4
                if (model.ElectronicImage4 != null)
                {
                    uc.UploadFiles(model.ElectronicImage4.InputStream, prod.Id + "_" + model.ElectronicImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePath4 = prod.Id + "_" + model.ElectronicImage4.FileName.Replace(" ", "");
                }

                //ElectronicImage5
                if (model.ElectronicImage5 != null)
                {
                    uc.UploadFiles(model.ElectronicImage5.InputStream, prod.Id + "_" + model.ElectronicImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePath5 = prod.Id + "_" + model.ElectronicImage5.FileName.Replace(" ", "");
                }
                if (name == null)
                {
                    prod.DateEncoded = DateTime.Now;
                    prod.DateUpdated = DateTime.Now;
                    prod.Status = 0;
                    _db.MasterProducts.Add(prod);
                    _db.SaveChanges();
                    ViewBag.Message = model.Name + " Saved Successfully!";
                }
                else
                {
                    ViewBag.ErrorMessage = model.Name + " Already Exist";
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            return View();
        }

        // Electronic Update
        [AccessPolicy(PageCode = "SHNMPREE020")]
        public ActionResult ElectronicEdit(string id)
        {
            var dId = AdminHelpers.DCodeLong(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dId.ToString()))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<MasterProduct, MasterElectronicEditViewModel>(masterProduct);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPREE020")]
        public ActionResult ElectronicEdit(MasterElectronicEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _db.MasterProducts.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            prod.Name = model.Name;
            prod.ProductTypeId = model.ProductTypeId;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            try
            {
                //ElectronicImage1
                if (model.ElectronicImage1 != null)
                {
                    uc.UploadFiles(model.ElectronicImage1.InputStream, prod.Id + "_" + model.ElectronicImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = prod.Id + "_" + model.ElectronicImage1.FileName.Replace(" ", "");
                }

                //ElectronicImage2
                if (model.ElectronicImage2 != null)
                {
                    uc.UploadFiles(model.ElectronicImage2.InputStream, prod.Id + "_" + model.ElectronicImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePath2 = prod.Id + "_" + model.ElectronicImage2.FileName.Replace(" ", "");
                }

                //ElectronicImage3
                if (model.ElectronicImage3 != null)
                {
                    uc.UploadFiles(model.ElectronicImage3.InputStream, prod.Id + "_" + model.ElectronicImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePath3 = prod.Id + "_" + model.ElectronicImage3.FileName.Replace(" ", "");
                }

                //ElectronicImage4
                if (model.ElectronicImage4 != null)
                {
                    uc.UploadFiles(model.ElectronicImage4.InputStream, prod.Id + "_" + model.ElectronicImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePath4 = prod.Id + "_" + model.ElectronicImage4.FileName.Replace(" ", "");
                }

                //ElectronicImage5
                if (model.ElectronicImage5 != null)
                {
                    uc.UploadFiles(model.ElectronicImage5.InputStream, prod.Id + "_" + model.ElectronicImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePath5 = prod.Id + "_" + model.ElectronicImage5.FileName.Replace(" ", "");
                }
                prod.DateUpdated = DateTime.Now;
                _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            return RedirectToAction("List");
        }

        // Electronic Delete
        [AccessPolicy(PageCode = "SHNMPRED021")]
        public JsonResult ElectronicDelete(string Id)
        {
            var dId = AdminHelpers.DCodeLong(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dId);
            if (master != null)
            {
                master.Status = 2;
                _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Item Mapping
        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public ActionResult ItemMapping()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public ActionResult SingleItemMapping(int shopId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ItemMappingViewModel();
            if (shopId != 0)
            {
                var shop = _db.Shops.FirstOrDefault(i => i.Id == shopId);
                model.ShopId = shopId;
                if (shop != null)
                {
                    model.ShopName = shop.Name;
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRPL013")]
        public ActionResult PendingList(int shopId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new MasterProductListViewModel();
           
            model.Lists = _db.Products
                 .Where(a => a.ShopId == shopId && a.Status == 0 && a.MasterProductId == 0)
                 .OrderBy(i => i.Name).Select(i => new MasterProductListViewModel.PendingList
                 {
                     Id = i.Id,
                     Name = i.Name,
                     ItemId = i.ItemId,
                     ProductTypeName = i.ProductTypeName
                 }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRML012")]
        public ActionResult MappedList(int shopId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new MasterProductListViewModel();
            model.MappedLists = _db.Products.Join(_db.MasterProducts, p=> p.MasterProductId, mp=> mp.Id, (p,mp)=>new {p,mp})
                .Where(a => a.p.ShopId == shopId && a.p.Status == 0 && a.p.MasterProductId != 0)
                .OrderBy(i => i.p.Name).Select(i => new MasterProductListViewModel.MappedList
                {
                    Id = i.p.Id,
                    Name = i.p.Name,
                    MasterProductName = i.mp.Name,
                    ProductTypeName = i.p.ProductTypeName
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRMU014")]
        public ActionResult ItemMappingUpdate(int id)
        {
            var dCode = AdminHelpers.DCodeLong(id.ToString());
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = _db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, ItemMappingViewModel>(product);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public JsonResult MappingProduct(int masterproductId, int itemId, int shopId)
        {
            try
            {
                var user = ((Helpers.Sessions.User)Session["USER"]);
                ViewBag.Name = user.Name;
                var masterproduct = _db.MasterProducts.FirstOrDefault(i => i.Id == masterproductId);

                var product = _db.Products.FirstOrDefault(i => i.Id == itemId);
                if (product != null)
                {
                    product.MasterProductId = masterproductId;
                    product.MasterProductId = masterproduct.Id;
                    product.Customisation = masterproduct.Customisation;
                    product.Price = masterproduct.Price;
                    product.IBarU = Convert.ToInt32(masterproduct.IBarU);
                    product.ProductTypeId = masterproduct.ProductTypeId;
                    if (shopId != 0)
                    {
                        var shop = _db.Shops.FirstOrDefault(i => i.Id == shopId);
                        if (shop != null)
                        {
                            product.ShopId = shop.Id;
                            product.ShopName = shop.Name;
                            product.ShopCategoryId = shop.ShopCategoryId;
                            product.ShopCategoryName = shop.ShopCategoryName;
                        }
                    }
                    product.UpdatedBy = user.Name;
                    product.DateUpdated = DateTime.Now;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                _db.Dispose();               
            }
        }

        [AccessPolicy(PageCode = "SHNMPRMU014")]
        public JsonResult UpdateMappingProduct(int masterproductId, int id, int shopId, bool isCheck)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var masterproduct = _db.MasterProducts.FirstOrDefault(i => i.Id == masterproductId);
            var product = _db.Products.FirstOrDefault(i => i.Id == id);
            if (isCheck == true)
            {
                product.MasterProductId = 0;
                product.IBarU = 0;
                product.UpdatedBy = user.Name;
                product.DateUpdated = DateTime.Now;
                _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                if (product != null && masterproduct != null)
                {
                    product.MasterProductId = masterproductId;
                    product.MasterProductId = masterproduct.Id;

                    if (product.Customisation == false && masterproduct.Customisation != false)
                    {
                        product.Customisation = masterproduct.Customisation;
                    }
                    if (product.Price == 0 && masterproduct.Price != 0)
                    {
                        product.Price = masterproduct.Price;
                    }
                    if (product.IBarU == 0 && masterproduct.IBarU != null)
                    {
                        product.IBarU = Convert.ToInt32(masterproduct.IBarU);
                    }
                    if (product.ProductTypeId == 0 && masterproduct.ProductTypeId != 0)
                    {
                        product.ProductTypeId = masterproduct.ProductTypeId;
                    }
                    if (masterproduct.Price != 0)
                    {
                        product.Price = masterproduct.Price;
                    }
                    if (masterproduct.IBarU != null)
                    {
                        product.IBarU = Convert.ToInt32(masterproduct.IBarU);
                    }
                    if (masterproduct.ProductTypeId != 0)
                    {
                        product.ProductTypeId = masterproduct.ProductTypeId;
                    }
                    if (shopId != 0)
                    {
                        var shop = _db.Shops.FirstOrDefault(i => i.Id == shopId);
                        if (shop != null)
                        {
                            product.ShopId = shop.Id;
                            product.ShopName = shop.Name;
                            product.ShopCategoryId = shop.ShopCategoryId;
                            product.ShopCategoryName = shop.ShopCategoryName;
                        }
                    }
                    product.UpdatedBy = user.Name;
                    product.DateUpdated = DateTime.Now;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            bool results = true;
            return Json(new { results, shopId }, JsonRequestBehavior.AllowGet);
        }

        // Json Results
        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public JsonResult GetMasterItemSelect2(string q = "")
        {
            if (q != "")
            {
                var model = _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId != 1).Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    image = i.ImagePath1,
                    description = i.LongDescription,
                    brandname = i.BrandName,
                    //categoryname = i.CategoryName,
                    price = i.Price,
                    type = i.ProductTypeName
                }).Take(50).ToList();
                return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public JsonResult GetMasterProductList(string itemName)
        {
            string result = itemName.Substring(0, 3);
            var model = new List<ItemMappingViewModel.ItemMappingList>();
            model = _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(result) && a.Status == 0).Select(i => new ItemMappingViewModel.ItemMappingList
            {
                Id = i.Id,
                Name = i.Name,
                ImagePath = i.ImagePath1,
                LongDescription = i.LongDescription
            }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public async Task<JsonResult> GetProductItemSelect2(int shopId, string q = "")
        {
            var model = await _db.Products
                .Where(a => a.Name.Contains(q) && a.ShopId == shopId && a.Status == 0 && a.MasterProductId == 0)
                .OrderBy(i => i.Name).Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    type = i.ProductTypeName
                }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public async Task<JsonResult> GetPendingProductItemSelect2(int shopId)
        {
            var model = await _db.Products
                .Join(_db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                .Where(a => a.p.ShopId == shopId && a.p.Status == 0 && a.p.MasterProductId == 0)
                .OrderBy(i => i.p.Name).Select(i => new
                {
                    Id = i.p.Id,
                    Name = i.m.Name,
                    BrandName = i.m.BrandName,
                   // CategoryName = i.m.CategoryName,
                    ProductType = i.p.ProductTypeName,
                    Price = i.p.Price
                }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDishMasterSelect2(string q = "")
        {
            var model = await _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                CategoryId = i.CategoryId,
                //CategoryName = i.CategoryName,
                BrandId = i.BrandId,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                Customisation = i.Customisation,
                ColorCode = i.ColorCode,
                ImagePath = i.ImagePath1,
                Price = i.Price,
                ProductType = i.ProductTypeName,
                ImagePath1 = i.ImagePath1
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Brand Select2
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetMedicalBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetFMCGBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Category Select2
        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetDishCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetMedicalCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Measurement Unit Select2
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetDrugUnitSelect2(string q = "")
        {
            var model = await _db.MeasurementUnits.Where(a => a.UnitName.Contains(q) && a.Status == 0).OrderBy(i => i.UnitName).Select(i => new
            {
                id = i.Id,
                text = i.UnitName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetFMCGMeasurementUnitSelect2(string q = "")
        {
            var model = await _db.MeasurementUnits.OrderBy(i => i.UnitName).Where(a => a.UnitName.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.UnitName
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Drug Compound Detail Select2
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetDrugCompoundDetailSelect2(string q = "")
        {
            var model = await _db.DrugCompoundDetails.Where(a => a.AliasName.Contains(q) && a.Status == 0).OrderBy(i => i.AliasName).Select(i => new
            {
                id = i.Id,
                text = i.AliasName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        //Package Select2
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetMedicalPackageSelect2(string q = "")
        {
            var model = await _db.Packages.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetFMCGPackageSelect2(string q = "")
        {
            var model = await _db.Packages.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Shop Select2
        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await _db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Sub Category Select2
        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetSubCategorySelect2(string q = "")
        {
            var model = await _db.SubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync(); 

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // NextSub Category Select2
        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetNextSubCategorySelect2(string q = "")
        {
            var model = await _db.NextSubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // AddonCategory Select2
        public async Task<JsonResult> GetAddonCategorySelect2(string q = "")
        {
            var model = await _db.AddOnCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // PortionSelect2
        public async Task<JsonResult> GetPortionSelect2(string q = "")
        {
            var model = await _db.Portions.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // ProductType Select2
        public async Task<JsonResult> GetProductTypeSelect2(string q = "")
        {
            var model = await _db.ProductTypes.Where(a => a.Name.Contains(q)).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // AddOns
        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult AddToAddOns(MasterAddOnsCreateViewModel model)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            if (addOns == null)
            {
                addOns = new List<MasterAddOnsCreateViewModel>();
            }
            //var id = addOns.Count() + 1;
            //model.Id = id;
            var count = addOns.Count() + 1;
            model.Index = count;
            addOns.Add(model);
            Session["AddOns"] = addOns;

            return Json(new
            {
                PortionId = model.PortionId,
                PortionName = model.PortionName,
                PortionPrice = model.PortionPrice,
                AddOnCategoryId = model.AddOnCategoryId,
                AddOnCategoryName = model.AddOnCategoryName,
                AddOnsPrice = model.AddOnsPrice,
                MinSelectionLimit = model.MinSelectionLimit,
                MaxSelectionLimit = model.MaxSelectionLimit,
                AddOnItemName = model.AddOnItemName,
                CrustName = model.CrustName,
                CrustPrice = model.CrustPrice,
                AddOnType = model.AddOnType,
                MasterProductId = model.MasterProductId,
                Id = model.Id,
                Index = model.Index
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult EditAddToAddOns(MasterAddOnsCreateViewModel model)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["EditAddOns"] as List<MasterAddOnsCreateViewModel>;
            if (addOns == null)
            {
                addOns = new List<MasterAddOnsCreateViewModel>();
            }
            var count = addOns.Count() + 1;
            model.Index = count;
            addOns.Add(model);
            Session["EditAddOns"] = addOns;

            return Json(new
            {
                PortionId = model.PortionId,
                PortionName = model.PortionName,
                PortionPrice = model.PortionPrice,
                AddOnCategoryId = model.AddOnCategoryId,
                AddOnCategoryName = model.AddOnCategoryName,
                AddOnsPrice = model.AddOnsPrice,
                MinSelectionLimit = model.MinSelectionLimit,
                MaxSelectionLimit = model.MaxSelectionLimit,
                AddOnItemName = model.AddOnItemName,
                CrustName = model.CrustName,
                CrustPrice = model.CrustPrice,
                AddOnType = model.AddOnType,
                MasterProductId = model.MasterProductId,
                Id = model.Id,
                Index = model.Index
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult RemoveFromAddOns(int id)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;

            if (addOns.Remove(addOns.SingleOrDefault(i => i.Index == id)))
            {
                this.Session["AddOns"] = addOns;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult EditRemoveAddOns(int id, int code)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["EditAddOns"] as List<MasterAddOnsCreateViewModel>;
            if (addOns == null)
            {
                addOns = new List<MasterAddOnsCreateViewModel>();
            }
            if (addOns.Remove(addOns.SingleOrDefault(i => i.Index == id)))
            {
                this.Session["EditAddOns"] = addOns;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            if (code != 0)
            {
                var addon = _db.ProductDishAddOns.FirstOrDefault(i => i.Id == code);
                if (addon != null)
                {
                    addon.Status = 2;
                    addon.DateUpdated = DateTime.Now;
                    _db.Entry(addon).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }

                if (addOns.Remove(addOns.SingleOrDefault(i => i.Index == id)))
                {
                    this.Session["EditAddOns"] = addOns;
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // Upload Master Items
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult FoodUpload()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult FoodUpload(HttpPostedFileBase upload, MasterDishUploadViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            DataTable dt = new DataTable();

            if (model.button == "upload")
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + upload.FileName);
                upload.SaveAs(path);
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        Stream stream = upload.InputStream;
                        IExcelDataReader reader = null;

                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View();
                        }
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                        reader.Close();
                        model.DataTable = result.Tables[0];
                        model.Filename = upload.FileName;
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }
                }
            }
            else
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + model.Filename);
                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";

                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                using (OleDbConnection connExcel = new OleDbConnection(excelConnectionString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                // Insert records to database table.
                // MainPageModel entities = new MainPageModel();
                foreach (DataRow row in dt.Rows)
                {
                    var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);
                    if (masterProduct == null)
                    {
                        _db.MasterProducts.Add(new MasterProduct
                        {
                            Name = row[model.Name].ToString(),
                            BrandId = CheckBrand(row[model.BrandName].ToString(), model.ProductTypeId, model.ProductTypeName),
                            BrandName = row[model.BrandName].ToString(),
                            CategoryId = CheckCategory(row[model.CategoryName].ToString(), model.ProductTypeId, model.ProductTypeName),
                            ShortDescription = row[model.ShortDescription].ToString(),
                            LongDescription = row[model.LongDescription].ToString(),
                            ProductTypeId = model.ProductTypeId,
                            ProductTypeName = model.ProductTypeName,
                            Customisation = Convert.ToBoolean(row[model.Customisation]),
                            ColorCode = row[model.ColorCode].ToString(),
                            Price = Convert.ToDouble(row[model.Price]),
                            ImagePath1 = row[model.ImagePath1].ToString(),
                            ImagePath2 = row[model.ImagePath2].ToString(),
                            ImagePath3 = row[model.ImagePath3].ToString(),
                            ImagePath4 = row[model.ImagePath4].ToString(),
                            ImagePath5 = row[model.ImagePath5].ToString(),
                            Status = 0,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name,
                            DateEncoded = DateTime.Now,
                            DateUpdated = DateTime.Now,
                        });
                        _db.SaveChanges();
                    }
                }
            }
            return View();
        }

        public int CheckBrand(string BrandName, int ProductTypeId, string ProductTypeName)
        {
            var brand = _db.Brands.FirstOrDefault(i => i.Name == BrandName && i.Status == 0);
            if (brand != null)
            {
                return brand.Id;
            }
            else
            {
                Brand br = new Brand();
                br.Name = BrandName;
                br.Status = 0;
                br.DateEncoded = DateTime.Now;
                br.DateUpdated = DateTime.Now;
                _db.Brands.Add(br);
                _db.SaveChanges();
                return br.Id;
            }
        }

        public int CheckCategory(string CategoryName, int ProductTypeId, string ProductTypeName)
        {
            var category = _db.Categories.FirstOrDefault(i => i.Name == CategoryName && i.Status == 0);
            if (category != null)
            {
                return category.Id;
            }
            else
            {
                Category cat = new Category();
                cat.Name = CategoryName;
                cat.ProductTypeId = ProductTypeId;
                cat.ProductTypeName = ProductTypeName;
                cat.Status = 0;
                cat.DateEncoded = DateTime.Now;
                cat.DateUpdated = DateTime.Now;
                _db.Categories.Add(cat);
                _db.SaveChanges();
                return cat.Id;
            }
        }

        public int CheckSubCategory(int CategoryId, string CategoryName, string SubCategoryName, int ProductTypeId, string ProductTypeName)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var subCategory = _db.SubCategories.FirstOrDefault(i => i.Name == SubCategoryName && i.Status == 0);
            if (subCategory != null)
            {
                return subCategory.Id;
            }
            else
            {
                SubCategory sub = new SubCategory();
                sub.Name = SubCategoryName;
                sub.CategoryId = CategoryId;
                sub.CategoryName = CategoryName;
                sub.ProductTypeId = ProductTypeId;
                sub.ProductTypeName = ProductTypeName;
                sub.CreatedBy = user.Name;
                sub.UpdatedBy = user.Name;
                sub.Status = 0;
                sub.DateEncoded = DateTime.Now;
                sub.DateUpdated = DateTime.Now;
                _db.SubCategories.Add(sub);
                _db.SaveChanges();
                return sub.Id;
            }
        }

        public int CheckNextSubCategory(int subCategoryId, string SubCategoryName, string NextSubCategoryName, int ProductTypeId, string ProductTypeName)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var nextSubCategory = _db.NextSubCategories.FirstOrDefault(i => i.Name == NextSubCategoryName && i.Status == 0);
            if (nextSubCategory != null)
            {
                return nextSubCategory.Id;
            }
            else
            {
                NextSubCategory sub = new NextSubCategory();
                sub.Name = NextSubCategoryName;
                sub.SubCategoryId = subCategoryId;
                sub.SubCategoryName = SubCategoryName;
                sub.ProductTypeId = ProductTypeId;
                sub.ProductTypeName = ProductTypeName;
                sub.CreatedBy = user.Name;
                sub.UpdatedBy = user.Name;
                sub.DateEncoded = DateTime.Now;
                sub.DateUpdated = DateTime.Now;
                sub.Status = 0;
                _db.NextSubCategories.Add(sub);
                _db.SaveChanges();
                return sub.Id;
            }
        }

        
    }
}