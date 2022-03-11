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
        [AccessPolicy(PageCode = "SNCMPL135")]
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
        [AccessPolicy(PageCode = "SNCMPD136")]
        public JsonResult Delete(string Id)
        {
            var dCode = AdminHelpers.DCodeLong(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);
            if (master != null)
            {
                master.Status = 2;
                _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                //product delete
                var productList = _db.Products.Where(i => i.MasterProductId == master.Id).ToList();
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
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Dish Create
        [AccessPolicy(PageCode = "SNCMPDC137")]
        public ActionResult FoodCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Session["AddOns"] = new List<MasterAddOnsCreateViewModel>(); 
            Session["AddTagCategory"] = new List<TagCategorySessionList>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SNCMPDC137")]
        public ActionResult FoodCreate(MasterFoodCreateViewModel model)
        {
            using (sncEntities sncdb = new sncEntities())
            {
                var user = ((Helpers.Sessions.User)Session["USER"]);
                var isExist = sncdb.MasterProducts.Any(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 1 && i.CategoryId == model.CategoryId);
                if (isExist)
                {
                    ViewBag.ErrorMessage = model.Name + " Already Exist";
                    return View();
                }
                var master = _mapper.Map<MasterFoodCreateViewModel, MasterProduct>(model);
                if (master != null)

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
                sncdb.MasterProducts.Add(master);
                sncdb.SaveChanges();
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
                        productDishaddOn.CrustId = s.CrustId;
                        sncdb.ProductDishAddOns.Add(productDishaddOn);
                        sncdb.SaveChanges();
                    }
                }
                Session["AddOns"] = null;
                SaveKeywordData(master.Name);

                // Tag Category 
                SaveTagCategory(model.CategoryId, 0, 0, master.Id);
            };
            return View();
        }

        // Dish Update
        [AccessPolicy(PageCode = "SNCMPDE138")]
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
            if(model.CategoryId !=0)
            model.CategoryName = _db.Categories.FirstOrDefault(i => i.Id == master.CategoryId).Name;
            model.TagCategory = string.Join(",", _db.TagCategories.Where(i => i.MasterProductId == master.Id).Select(i => i.Id));
            model.TagCategoryName = string.Join(",", _db.TagCategories.Where(i => i.MasterProductId == master.Id).Select(i => i.CategoryName));

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
        [AccessPolicy(PageCode = "SNCMPDE138")]
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
                        productDishaddOn.CrustId = s.CrustId;
                        _db.ProductDishAddOns.Add(productDishaddOn);
                        _db.SaveChanges();
                    }
                }
            }
            Session["EditAddOns"] = null;
            SaveKeywordData(master.Name);

            return RedirectToAction("FoodEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        // Dish List
        [AccessPolicy(PageCode = "SNCMPDL139")]
        public ActionResult FoodList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name; 
            var List = _db.MasterProducts.Where(i => i.Status == 0 && i.ProductTypeId == 1)
                .Select(i => new MasterProductFoodListViewModel.MasterProductFoodList
                {
                    Id = i.Id,
                    Name = i.Name,
                    CategoryName = _db.Categories.FirstOrDefault(j=> j.Id == i.CategoryId).Name,
                    Price = i.Price,
                    ProductTypeName = i.ProductTypeName
                }).OrderBy(i => i.Name).ToList();
            return View(List);
        }

        // Upload FMCG MasterItems
        [AccessPolicy(PageCode = "SNCMPFU140")]
        public ActionResult FMCGUpload()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCMPFU140")]
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
                List<MasterProduct> masterList = new List<MasterProduct>();
                var master = _db.MasterProducts.Where(i => i.Status == 0).Select(i => new { Name = i.Name }).ToList();
                foreach (DataRow row in dt.Rows)
                {
                    if (row[model.Name].ToString().Trim() != string.Empty)
                    {
                        int idx = master.FindIndex(a => a.Name == row[model.Name].ToString().Trim());
                        if (idx <= 0)
                        {
                            var varCategoryId = CheckCategory(row[model.CategoryName].ToString().Trim(), 2, "FMCG");
                            var varSubCategoryId = CheckSubCategory(varCategoryId, row[model.CategoryName].ToString().Trim(), row[model.SubCategoryName].ToString().Trim(), 2, "FMCG");
                            var varNextSubCategoryId = CheckNextSubCategory(varSubCategoryId, row[model.SubCategoryName].ToString().Trim(), row[model.NextSubCategoryName].ToString().Trim(), 2, "FMCG");
                            masterList.Add(new MasterProduct
                            {
                                Name = row[model.Name].ToString().Trim(),
                                NickName = row[model.Name].ToString().Trim(),
                                CategoryId = varCategoryId,
                                SubCategoryId = varSubCategoryId,
                                NextSubCategoryId = varNextSubCategoryId,
                                BrandId = CheckBrand(row[model.BrandName].ToString()),
                                BrandName = row[model.BrandName].ToString(),
                                Weight = Convert.ToDouble(row[model.Weight]),
                                SizeLWH = Convert.ToDouble(row[model.SizeLBH]),
                                PackageId = CheckPackage(row[model.PackageName].ToString()),
                                PackageName = row[model.PackageName].ToString(),
                                MeasurementUnitId = CheckMeasurementUnit(row[model.MeasurementUnitName].ToString()),
                                MeasurementUnitName = row[model.MeasurementUnitName].ToString(),
                                ShortDescription = row[model.ShortDescription].ToString(),
                                LongDescription = row[model.LongDescription].ToString(),
                                Price = Convert.ToDouble(row[model.Price]),
                                GoogleTaxonomyCode = row[model.GoogleTaxonomyCode].ToString().Trim(),
                                ASIN = row[model.ASIN].ToString(),
                                ImagePath1 = row[model.ImagePath1].ToString().Trim(),
                                ImagePath2 = row[model.ImagePath2].ToString().Trim(),
                                ImagePath3 = row[model.ImagePath3].ToString().Trim(),
                                ImagePath4 = row[model.ImagePath4].ToString().Trim(),
                                ImagePath5 = row[model.ImagePath5].ToString().Trim(),
                                ProductTypeId = 2,
                                ProductTypeName = "FMCG",
                                Adscore = 1,
                                Status = 0,
                                CreatedBy = user.Name,
                                UpdatedBy = user.Name,
                                DateEncoded = DateTime.Now,
                                DateUpdated = DateTime.Now,
                            });
                        }
                    }
                }
                _db.BulkInsert(masterList);
            }
            return View();
        }

        // FMCG List
        [AccessPolicy(PageCode = "SNCMPFL141")]
        public ActionResult FMCGList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var List = _db.MasterProducts.Join(_db.Categories, mp=> mp.CategoryId, c=> c.Id, (mp,c)=> new { mp,c})
                       .OrderBy(i => i.mp.Name)
                       .Where(i => i.mp.Status == 0 && i.mp.ProductTypeId == 2)
                       .Select(i => new MasterProductFMCGListViewModel.MasterProductFMCGList
                       {
                           Id = i.mp.Id,
                           BrandName = i.mp.BrandName,
                           CategoryName = i.c.Name,
                           Name = i.mp.Name,
                           ProductTypeName = i.mp.ProductTypeName
                       }).ToList();
            return View(List);
        }

        // FMCG Create
        [AccessPolicy(PageCode = "SNCMPFC142")]
        public ActionResult FMCGCreate()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Session["AddTagCategory"] = null;
            Session["AddTagCategory"] = new List<TagCategorySessionList>();
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCMPFC142")]
        public ActionResult FMCGCreate(MasterFMCGCreateViewModel model)
        {
            using (sncEntities _dbb = new sncEntities())
            {
                var user = ((Helpers.Sessions.User)Session["USER"]);
                var master = _mapper.Map<MasterFMCGCreateViewModel, MasterProduct>(model);
                //MasterProduct master = new MasterProduct();
                var isExist = _dbb.MasterProducts.Any(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 2);
                if (isExist)
                {
                    ViewBag.ErrorMessage = model.Name + " Already Exist";
                    return View();
                }
                master.CreatedBy = user.Name;
                master.UpdatedBy = user.Name;
                master.ProductTypeId = 2;
                master.ProductTypeName = "FMCG";

                //  master.Name = model.Name;

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
                    _dbb.MasterProducts.Add(master);
                    _dbb.SaveChanges();
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
                SaveKeywordData(master.Name);
                // Tag Category 
                SaveTagCategory(model.CategoryId, model.SubCategoryId, model.NextSubCategoryId, master.Id);
            };
           

            return View();
        }

        // FMCG Update
        [AccessPolicy(PageCode = "SNCMPFE143")]
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
            var categoryName = _db.Categories.FirstOrDefault(i => i.Id == masterProduct.CategoryId);
            if (categoryName != null)
                model.CategoryName = categoryName.Name;
            else
                model.CategoryName = "N/A";
            var subcategoryName = _db.SubCategories.FirstOrDefault(i => i.Id == masterProduct.SubCategoryId);
            if (subcategoryName != null)
                model.SubCategoryName = subcategoryName.Name;
            else
                model.SubCategoryName = "N/A";
            var nextsubcategoryName = _db.NextSubCategories.FirstOrDefault(i => i.Id == masterProduct.NextSubCategoryId);
            if (nextsubcategoryName != null)
                model.NextSubCategoryName = nextsubcategoryName.Name;
            else
                model.NextSubCategoryName = "N/A";
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCMPFE143")]
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
            SaveKeywordData(master.Name);

            return RedirectToAction("FMCGEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        // Medical List
        [AccessPolicy(PageCode = "SNCMPML144")]
        public ActionResult MedicalList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var List = _db.MasterProducts.Join(_db.Categories, mp=> mp.CategoryId, c=> c.Id, (mp,c)=> new { mp,c})
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
        [AccessPolicy(PageCode = "SNCMPMC145")]
        public ActionResult MedicalCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Session["AddTagCategory"] = null;
            Session["AddTagCategory"] = new List<TagCategorySessionList>();
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SNCMPMC145")]
        public ActionResult MedicalCreate(MasterMedicalCreateViewModel model)
        {
            using (sncEntities snc = new sncEntities())
            {
                var isExist = snc.MasterProducts.Any(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 3);
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
                            var dcd = snc.DrugCompoundDetails.FirstOrDefault(i => i.Id == sid && i.Status == 0);
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
                    snc.MasterProducts.Add(master);
                    snc.SaveChanges();
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
                SaveKeywordDataWithCombination(master.Name, master.DrugCompoundDetailName);
                // Tag Category 
                SaveTagCategory(model.CategoryId, 0, 0, master.Id);
            };
            return View();
        }

        // Medical Update
        [AccessPolicy(PageCode = "SNCMPME146")]
        public ActionResult MedicalEdit(string id)
        {
            var dId = AdminHelpers.DCodeLong(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dId.ToString()))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<MasterProduct, MasterMedicalEditViewModel>(masterProduct);
            if (model.ImagePath1 !=null)
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
            if(masterProduct.CategoryId !=0)
            model.CategoryName = _db.Categories.FirstOrDefault(i => i.Id == masterProduct.CategoryId).Name;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SNCMPME146")]
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
                //if (model.DrugCompoundDetailIds != null)
                //{
                //    master.DrugCompoundDetailIds = String.Join(",", model.DrugCompoundDetailIds);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.DrugCompoundDetailIds)
                //    {
                //        var sid = Convert.ToInt32(s);
                //        var dcd = _db.DrugCompoundDetails.FirstOrDefault(i => i.Id == sid && i.Status == 0);
                //        sb.Append(dcd.AliasName);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.DrugCompoundDetailName = sb.ToString().Remove(sb.Length - 1);
                //        master.DrugCompoundDetailName = model.DrugCompoundDetailName;
                //    }
                //    else
                //    {
                //        model.DrugCompoundDetailName = sb.ToString();
                //        master.DrugCompoundDetailName = model.DrugCompoundDetailName;
                //    }
                //}
                //else
                //{
                //    master.DrugCompoundDetailIds = string.Empty;
                //    master.DrugCompoundDetailName = string.Empty;
                //}

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
            SaveKeywordDataWithCombination(master.Name,master.DrugCompoundDetailName);
            return RedirectToAction("MedicalEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        // Upload Medical MasterItems
        [AccessPolicy(PageCode = "SNCMPMU147")]
        public ActionResult MedicalUpload()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCMPMU147")]
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
                List<MasterProduct> masterList = new List<MasterProduct>();
                var master = _db.MasterProducts.Where(i => i.Status == 0).Select(i => new { Name = i.Name }).ToList();
                foreach (DataRow row in dt.Rows)
                {
                    if (row[model.Name].ToString().Trim() != string.Empty)
                    {
                        int idx = master.FindIndex(a => a.Name == row[model.Name].ToString().Trim());
                        var Name = row[model.Name].ToString().Trim();
                        if (idx <= 0)
                        {
                            masterList.Add(new MasterProduct
                            {
                                Name = Name,
                                NickName = Name,
                                BrandId = CheckBrand(row[model.BrandName].ToString()),
                                BrandName = row[model.BrandName].ToString(),
                                CategoryId = CheckCategory(row[model.CategoryName].ToString().Trim(), 3, "Medical"),
                                MeasurementUnitId = CheckMeasurementUnit(row[model.MeasurementUnitName].ToString()),
                                MeasurementUnitName = row[model.MeasurementUnitName].ToString(),
                                DrugCompoundDetailIds = row[model.DrugCompoundDetailIds].ToString(),
                                DrugCompoundDetailName = row[model.DrugCompoundDetailName].ToString(),
                                PriscriptionCategory = Convert.ToBoolean(row[model.PriscriptionCategory].ToString()),
                                OriginCountry = row[model.OriginCountry].ToString(),
                                Manufacturer = row[model.Manufacturer].ToString(),
                                IBarU = Convert.ToInt32(row[model.IBarU]),
                                Weight = Convert.ToDouble(row[model.Weight]),
                                SizeLWH = Convert.ToDouble(row[model.SizeLBH]),
                                Price = Convert.ToDouble(row[model.Price]),
                                PackageId = CheckPackage(row[model.PackageName].ToString()),
                                PackageName = row[model.PackageName].ToString(),
                                ImagePath1 = row[model.ImagePath1].ToString().Trim(),
                                ImagePath2 = row[model.ImagePath2].ToString().Trim(),
                                ImagePath3 = row[model.ImagePath3].ToString().Trim(),
                                ImagePath4 = row[model.ImagePath4].ToString().Trim(),
                                ImagePath5 = row[model.ImagePath5].ToString().Trim(),
                                Adscore = 1,
                                ProductTypeId = 3,
                                ProductTypeName = "Medical",
                                Status = 0,
                                CreatedBy = user.Name,
                                UpdatedBy = user.Name,
                                DateEncoded = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                ColorCode = "N/A",
                                LongDescription = "N/A",
                                ShortDescription = "N/A",
                            });
                        }
                    }
                }
                _db.BulkInsert(masterList);
            }
            return View();
        }
     
        // Electronic List
        [AccessPolicy(PageCode = "SNCMPEL148")]
        public ActionResult ElectronicList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = _db.MasterProducts.Where(i => i.Status == 0 && i.ProductTypeId == 4)
                .Select(i => new MasterElectronicListViewModel.MasterElectronicList
                {
                    Id = i.Id,
                    Name = i.Name,
                    BrandName = i.BrandName,
                    CategoryName = _db.Categories.FirstOrDefault(j=>j.Id == i.CategoryId).Name,
                    ProductTypeName = i.ProductTypeName

                }).OrderBy(i =>i.Name).ToList();
            return View(List);
        }

        // Electronic Create
        [AccessPolicy(PageCode = "SNCMPEC149")]
        public ActionResult ElectronicCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SNCMPEC149")]
        public ActionResult ElectronicCreate(MasterElectronicCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MasterElectronicCreateViewModel, MasterProduct>(model);
            var isExist = _db.MasterProducts.Any(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 4);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.ProductTypeId = 4;
            prod.ProductTypeName = "Electronic";
            if (model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            try
            {
                long ticks = DateTime.Now.Ticks;

                //ElectronicImage1
                if (model.ElectronicImage1 != null)
                {
                    uc.UploadFiles(model.ElectronicImage1.InputStream, ticks + "_" + model.ElectronicImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = ticks + "_" + model.ElectronicImage1.FileName.Replace(" ", "");
                }

                //ElectronicImage2
                if (model.ElectronicImage2 != null)
                {
                    uc.UploadFiles(model.ElectronicImage2.InputStream, ticks + "_" + model.ElectronicImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePath2 = ticks + "_" + model.ElectronicImage2.FileName.Replace(" ", "");
                }

                //ElectronicImage3
                if (model.ElectronicImage3 != null)
                {
                    uc.UploadFiles(model.ElectronicImage3.InputStream, ticks + "_" + model.ElectronicImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePath3 = ticks + "_" + model.ElectronicImage3.FileName.Replace(" ", "");
                }

                //ElectronicImage4
                if (model.ElectronicImage4 != null)
                {
                    uc.UploadFiles(model.ElectronicImage4.InputStream, ticks + "_" + model.ElectronicImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePath4 = ticks + "_" + model.ElectronicImage4.FileName.Replace(" ", "");
                }

                //ElectronicImage5
                if (model.ElectronicImage5 != null)
                {
                    uc.UploadFiles(model.ElectronicImage5.InputStream, ticks + "_" + model.ElectronicImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePath5 = ticks + "_" + model.ElectronicImage5.FileName.Replace(" ", "");
                }
                prod.DateEncoded = DateTime.Now;
                prod.DateUpdated = DateTime.Now;
                prod.Status = 0;
                _db.MasterProducts.Add(prod);
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
            SaveKeywordData(prod.Name);
            return View();
        }

        // Electronic Update
        [AccessPolicy(PageCode = "SNCMPEE150")]
        public ActionResult ElectronicEdit(string id)
        {
            var dId = AdminHelpers.DCodeLong(id);
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
            var model = _mapper.Map<MasterProduct, MasterElectronicEditViewModel>(masterProduct);
            var categoryName = _db.Categories.FirstOrDefault(i => i.Id == masterProduct.CategoryId);
            if (categoryName != null)
                model.CategoryName = categoryName.Name;
            else
                model.CategoryName = "N/A";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SNCMPEE150")]
        public ActionResult ElectronicEdit(MasterElectronicEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _db.MasterProducts.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            try
            {
                //ElectronicImage1
                if (model.ElectronicImage1 != null)
                {
                    // Delete Old File
                    if(model.ImagePath1 != null)
                    {
                        uc.DeleteFiles(model.ImagePath1,accesskey,secretkey);
                    }
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
            SaveKeywordData(prod.Name);
            return RedirectToAction("ElectronicEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        // Upload Electronic MasterItems
        [AccessPolicy(PageCode = "SNCMPEU151")]
        public ActionResult ElectronicUpload()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCMPEU151")]
        public ActionResult ElectronicUpload(HttpPostedFileBase upload, MasterElectronicUploadViewModel model)
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
                List<MasterProduct> masterList = new List<MasterProduct>();
                var master = _db.MasterProducts.Where(i => i.Status == 0).Select(i => new { Name = i.Name }).ToList();
                foreach (DataRow row in dt.Rows)
                {
                    if (row[model.Name].ToString().Trim() != string.Empty)
                    {
                        int idx = master.FindIndex(a => a.Name == row[model.Name].ToString().Trim());
                        if (idx <= 0)
                        {
                            var varCategoryId = CheckCategory(row[model.CategoryName].ToString().Trim(), 4, "Electronic");
                            var varSubCategoryId = CheckSubCategory(varCategoryId, row[model.CategoryName].ToString().Trim(), row[model.SubCategoryName].ToString().Trim(), 4, "Electronic");
                            var varNextSubCategoryId = CheckNextSubCategory(varSubCategoryId, row[model.SubCategoryName].ToString().Trim(), row[model.NextSubCategoryName].ToString().Trim(), 4, "Electronic");
                            masterList.Add(new MasterProduct
                            {
                                Name = row[model.Name].ToString().Trim(),
                                NickName = row[model.Name].ToString().Trim(),
                                CategoryId = varCategoryId,
                                SubCategoryId = varSubCategoryId,
                                NextSubCategoryId = varNextSubCategoryId,
                                BrandId = CheckBrand(row[model.BrandName].ToString()),
                                BrandName = row[model.BrandName].ToString(),
                                ProductTypeId = 4,
                                ProductTypeName = "Electronic",
                                Weight = Convert.ToDouble(row[Convert.ToInt32(model.Weight)]),
                                SizeLWH = Convert.ToDouble(row[Convert.ToInt32(model.SizeLBH)]),
                                PackageId = CheckPackage(row[model.PackageName].ToString()),
                                PackageName = row[model.PackageName].ToString(),
                                MeasurementUnitId = CheckMeasurementUnit(row[model.MeasurementUnitName].ToString()),
                                MeasurementUnitName = row[model.MeasurementUnitName].ToString(),
                                ShortDescription = row[model.ShortDescription].ToString(),
                                LongDescription = row[model.LongDescription].ToString(),
                                Price = Convert.ToDouble(row[Convert.ToInt32(model.Price)]),
                                GoogleTaxonomyCode = row[model.GoogleTaxonomyCode].ToString().Trim(),
                                ASIN = row[model.ASIN].ToString(),
                                ImagePath1 = row[model.ImagePath1].ToString().Trim(),
                                ImagePath2 = row[model.ImagePath2].ToString().Trim(),
                                ImagePath3 = row[model.ImagePath3].ToString().Trim(),
                                ImagePath4 = row[model.ImagePath4].ToString().Trim(),
                                ImagePath5 = row[model.ImagePath5].ToString().Trim(),
                                Adscore = 1,
                                Status = 0,
                                CreatedBy = user.Name,
                                UpdatedBy = user.Name,
                                DateEncoded = DateTime.Now,
                                DateUpdated = DateTime.Now,
                            });
                        }
                    }
                }
                _db.BulkInsert(masterList);
            }
            return View();
        }

        // Item Mapping
        [AccessPolicy(PageCode = "SNCMPIM152")]
        public ActionResult ItemMapping()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "SNCMPSM153")]
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
            model.PendingCount = _db.Products.Where(a => a.ShopId == shopId && a.Status == 0 && a.MasterProductId == 0).Count();
            model.MappedCount = _db.Products.Join(_db.MasterProducts, p => p.MasterProductId, mp => mp.Id, (p, mp) => new { p, mp })
                .Where(a => a.p.ShopId == shopId && a.p.Status == 0 && a.p.MasterProductId != 0).Count();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCMPPL154")]
        public ActionResult PendingList(int shopId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new MasterProductListViewModel();
            model.Lists = _db.Products.Where(a => a.ShopId == shopId && a.Status == 0 && a.MasterProductId == 0)
                     .OrderBy(i => i.Name).Select(i => new MasterProductListViewModel.PendingList
                     {
                         Id = i.Id,
                         Name = i.Name,
                         ItemId = i.ItemId,
                         ProductTypeName = i.ProductTypeName
                     }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCMPMAL155")]
        public ActionResult MappedList(int shopId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new MasterProductListViewModel();
            model.MappedLists = _db.Products.Join(_db.MasterProducts, p => p.MasterProductId, mp => mp.Id, (p, mp) => new { p, mp })
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

        [AccessPolicy(PageCode = "SNCMPIMU156")]
        public ActionResult ItemMappingUpdate(string id)
        {
            var dCode = AdminHelpers.DCodeLong(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = _db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, ItemMappingViewModel>(product);
            return View(model);
        }

        // Upload Master Items
        [AccessPolicy(PageCode = "SNCMPDU157")]
        public ActionResult FoodUpload()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCMPDU157")]
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
                List<MasterProduct> masterList = new List<MasterProduct>();
                var master = _db.MasterProducts.Where(i => i.Status == 0).Select(i => new { Name = i.Name }).ToList();

                foreach (DataRow row in dt.Rows)
                {
                    if (row[model.Name].ToString().Trim() != string.Empty)
                    {
                        int idx = master.FindIndex(a => a.Name == row[model.Name].ToString().Trim());
                        if (idx <= 0)
                        {
                            masterList.Add(new MasterProduct
                            {
                                Name = row[model.Name].ToString().Trim(),
                                NickName = row[model.Name].ToString().Trim(),
                                ProductTypeId = 1,
                                ProductTypeName = "Dish",
                                CategoryId = CheckCategory(row[model.CategoryName].ToString().Trim(), 1, "Dish"),
                                Customisation = Convert.ToBoolean(row[model.Customisation]),
                                ColorCode = row[model.ColorCode].ToString().Trim(),
                                Price = Convert.ToDouble(row[model.Price]),
                                GoogleTaxonomyCode = row[model.GoogleTaxonomyCode].ToString().Trim(),
                                Adscore = 1,
                                ImagePath1 = row[model.ImagePath1].ToString().Trim(),
                                Status = 0,
                                CreatedBy = user.Name,
                                UpdatedBy = user.Name,
                                DateEncoded = DateTime.Now,
                                DateUpdated = DateTime.Now,
                            });
                        }
                    }
                }
                _db.BulkInsert(masterList);
            }
            return View();
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
                    product.CategoryId = masterproduct.CategoryId;
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
                    product.MappedDate = DateTime.Now;
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

        public JsonResult UpdateMappingProduct(int masterproductId, int id, int shopId, bool isCheck)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var masterproduct = _db.MasterProducts.FirstOrDefault(i => i.Id == masterproductId);
            var product = _db.Products.FirstOrDefault(i => i.Id == id);
            if (isCheck == true)
            {
                product.MasterProductId = 0;
                product.CategoryId = 0;
                product.IBarU = 0;
                product.UpdatedBy = user.Name;
                product.DateUpdated = DateTime.Now;
                product.MappedDate = null;
                _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                if (product != null)
                {
                    product.MasterProductId = masterproductId;
                    product.CategoryId = masterproduct.CategoryId;
                    product.Customisation = masterproduct.Customisation;
                    product.Price = masterproduct.Price;
                    product.IBarU = Convert.ToInt32(masterproduct.IBarU);
                    product.ProductTypeId = masterproduct.ProductTypeId;
                    product.Price = masterproduct.Price;

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
                    product.MappedDate = DateTime.Now;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            bool results = true;
            return Json(new { results, shopId }, JsonRequestBehavior.AllowGet);
        }

        // Json Results
        public JsonResult GetMasterItemSelect2(string q = "")
        {
            if (q != "")
            {
                var model = _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId != 1)
                    .Join(_db.Categories, m => m.CategoryId, c => c.Id, (m, c) => new { m, c })
                    .Select(i => new
                    {
                        id = i.m.Id,
                        text = i.m.Name,
                        image = i.m.ImagePath1,
                        description = i.m.LongDescription,
                        brandname = i.m.BrandName,
                        categoryname = i.c.Name,
                        price = i.m.Price,
                        type = i.m.ProductTypeName
                    }).Take(50).ToList();
                return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

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
        public async Task<JsonResult> GetMedicalBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFMCGBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetElectronicBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Category Select2
        public async Task<JsonResult> GetDishCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetMedicalCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetElectronicCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 4).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Measurement Unit Select2
        public async Task<JsonResult> GetDrugUnitSelect2(string q = "")
        {
            var model = await _db.MeasurementUnits.Where(a => a.UnitName.Contains(q) && a.Status == 0).OrderBy(i => i.UnitName).Select(i => new
            {
                id = i.Id,
                text = i.UnitName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

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
        public async Task<JsonResult> GetMedicalPackageSelect2(string q = "")
        {
            var model = await _db.Packages.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

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

        // CrustSelect2
        public async Task<JsonResult> GetCrustSelect2(string q = "")
        {
            var model = await _db.Crusts.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
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

        public int CheckBrand(string BrandName)
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
                cat.OrderNo = 0;
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

        public async Task<JsonResult> GetAllSelect2(string q = "")
        {
            var model = await _db.MasterProducts.Where(a => a.Name.StartsWith(q) && a.Status == 0).Take(50).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UnMap(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var product = _db.Products.FirstOrDefault(i => i.Id == Id);
            if (product != null)
            {
                product.MasterProductId = 0;
                product.DateUpdated = DateTime.Now;
                product.UpdatedBy = user.Name;
                _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public void SaveKeywordData(string Name)
        {
            var nameArray = Name.Split(' ');
            foreach (var name in nameArray)
            {
                var checkExist = _db.KeywordDatas.Any(i => i.Name.Trim().ToLower() == name.Trim().ToLower());
                if (!checkExist)
                {
                    var keywordData = new KeywordData
                    {
                        Name = name
                    };
                    _db.KeywordDatas.Add(keywordData);
                    _db.SaveChanges();
                }
            }
        }

        public void SaveKeywordDataWithCombination(string Name,string Combination)
        {
            var nameArray = Name.Split(' ');
            foreach (var name in nameArray)
            {
                var checkExist = _db.KeywordDatas.Any(i => i.Name.Trim().ToLower() == name.Trim().ToLower());
                if (!checkExist)
                {
                    var keywordData = new KeywordData
                    {
                        Name = name
                    };
                    _db.KeywordDatas.Add(keywordData);
                    _db.SaveChanges();
                }
            }

            if (!string.IsNullOrEmpty(Combination))
            {
                var combinationArray = Combination.Split(' ');
                foreach (var name in combinationArray)
                {
                    foreach (var itemname in name.Split(','))
                    {
                        var checkExist = _db.KeywordDatas.Any(i => i.Name.Trim().ToLower() == itemname.Trim().ToLower());
                        if (!checkExist)
                        {
                            var keywordData = new KeywordData
                            {
                                Name = itemname
                            };
                            _db.KeywordDatas.Add(keywordData);
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        public async Task<JsonResult> GetDishTagCategorySelect2(string q = "")
        {
            var cat = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1)
                .Select(i => new
            {
                id = i.Id,
                text = i.Name,
                type =1
            }).ToListAsync();
            var catArray = cat.Select(i => i.id).ToArray();
            var subcat = await _db.SubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && catArray.Contains(a.CategoryId))
                .Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    type=2
                }).ToListAsync();

            var subcatArray = subcat.Select(i => i.id).ToArray();
            var nextSubCat = await _db.NextSubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && subcatArray.Contains(a.SubCategoryId))
               .Select(i => new
               {
                   id = i.Id,
                   text = i.Name,
                   type=3
               }).ToListAsync();

            var catSubList = cat.Union(subcat).ToList();
            var catSubNextList = catSubList.Union(nextSubCat);

            return Json(new { results = catSubNextList, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFMCGTagCategorySelect2(string q = "")
        {
            var cat = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2)
                .Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    type=1
                }).ToListAsync();
            var catArray = cat.Select(i => i.id).ToArray();
            var subcat = await _db.SubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && catArray.Contains(a.CategoryId))
                .Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    type=2
                }).ToListAsync();

            var subcatArray = subcat.Select(i => i.id).ToArray();
            var nextSubCat = await _db.NextSubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && subcatArray.Contains(a.SubCategoryId))
               .Select(i => new
               {
                   id = i.Id,
                   text = i.Name,
                   type=3
               }).ToListAsync();

            var catSubList = cat.Union(subcat).ToList();
            var catSubNextList = catSubList.Union(nextSubCat);

            return Json(new { results = catSubNextList, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetMedicalTagCategorySelect2(string q = "")
        {
            var cat = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3)
                .Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    type = 1
                }).ToListAsync();
            var catArray = cat.Select(i => i.id).ToArray();
            var subcat = await _db.SubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && catArray.Contains(a.CategoryId))
                .Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    type =2
                }).ToListAsync();

            var subcatArray = subcat.Select(i => i.id).ToArray();
            var nextSubCat = await _db.NextSubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && subcatArray.Contains(a.SubCategoryId))
               .Select(i => new
               {
                   id = i.Id,
                   text = i.Name,
                   type =3
               }).ToListAsync();

            var catSubList = cat.Union(subcat).ToList();
            var catSubNextList = catSubList.Union(nextSubCat);

            return Json(new { results = catSubNextList, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        //Tag Category Session
        public JsonResult AddFoodCreateTagCategory(int id, int type)
        {
            List<TagCategorySessionList> tagCategoryList = Session["AddTagCategory"] as List<TagCategorySessionList> ?? new List<TagCategorySessionList>();
            if (id != 0)
            {
                var tagCategory = new TagCategorySessionList
                {
                    Id = id,
                    Type = type
                };
                tagCategoryList.Add(tagCategory);
            }
            Session["AddTagCategory"] = tagCategoryList;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFoodCreateTagCategory(int id)
        {
            List<TagCategorySessionList> tagCategoryList = Session["AddTagCategory"] as List<TagCategorySessionList> ?? new List<TagCategorySessionList>();

            if (tagCategoryList.Remove(tagCategoryList.SingleOrDefault(i => i.Id == id)))
                Session["AddTagCategory"] = tagCategoryList;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddFoodUpdateTagCategory(int id, int type)
        {
            List<TagCategorySessionList> tagCategoryList = Session["UpdateTagCategory"] as List<TagCategorySessionList> ?? new List<TagCategorySessionList>();
            if (id != 0)
            {
                var tagCategory = new TagCategorySessionList
                {
                    Id = id,
                    Type = type
                };
                tagCategoryList.Add(tagCategory);
            }
            Session["UpdateTagCategory"] = tagCategoryList;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFoodUpdateTagCategory(int id)
        {
            List<TagCategorySessionList> tagCategoryList = Session["UpdateTagCategory"] as List<TagCategorySessionList> ?? new List<TagCategorySessionList>();

            if (tagCategoryList.Remove(tagCategoryList.SingleOrDefault(i => i.Id == id)))
                Session["UpdateTagCategory"] = tagCategoryList;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddFMCGCreateTagCategory(int id, int type)
        {
            List<TagCategorySessionList> tagCategoryList = Session["AddTagCategory"] as List<TagCategorySessionList> ?? new List<TagCategorySessionList>();
            if (id != 0)
            {
                var tagCategory = new TagCategorySessionList
                {
                    Id = id,
                    Type = type
                };
                tagCategoryList.Add(tagCategory);
            }
            Session["AddTagCategory"] = tagCategoryList;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFMCGCreateTagCategory(int id)
        {
            List<TagCategorySessionList> tagCategoryList = Session["AddTagCategory"] as List<TagCategorySessionList> ?? new List<TagCategorySessionList>();

            if (tagCategoryList.Remove(tagCategoryList.SingleOrDefault(i => i.Id == id)))
                Session["AddTagCategory"] = tagCategoryList;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddMedicalCreateTagCategory(int id, int type)
        {
            List<TagCategorySessionList> tagCategoryList = Session["AddTagCategory"] as List<TagCategorySessionList> ?? new List<TagCategorySessionList>();
            if (id != 0)
            {
                var tagCategory = new TagCategorySessionList
                {
                    Id = id,
                    Type = type
                };
                tagCategoryList.Add(tagCategory);
            }
            Session["AddTagCategory"] = tagCategoryList;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveMedicalCreateTagCategory(int id)
        {
            List<TagCategorySessionList> tagCategoryList = Session["AddTagCategory"] as List<TagCategorySessionList> ?? new List<TagCategorySessionList>();

            if (tagCategoryList.Remove(tagCategoryList.SingleOrDefault(i => i.Id == id)))
                Session["AddTagCategory"] = tagCategoryList;

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public void SaveTagCategory(int categoryId, int subCategoryId, int nextSubCategoryId, long masterId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            if (categoryId != 0)
            {
                var tagcategory = new TagCategory
                {
                    CategoryId = categoryId,
                    CategoryName = _db.Categories.FirstOrDefault(i => i.Id == categoryId)?.Name,
                    CreatedBy = user.Name,
                    UpdatedBy = user.Name,
                    DateUpdated = DateTime.Now,
                    DateEncoded = DateTime.Now,
                    MasterProductId = masterId,
                    Type = 1
                };
                _db.TagCategories.Add(tagcategory);
                _db.SaveChanges();
            }
            if (subCategoryId != 0)
            {
                var tagcategory = new TagCategory
                {
                    CategoryId = subCategoryId,
                    CategoryName = _db.SubCategories.FirstOrDefault(i => i.Id == subCategoryId)?.Name,
                    CreatedBy = user.Name,
                    UpdatedBy = user.Name,
                    DateUpdated = DateTime.Now,
                    DateEncoded = DateTime.Now,
                    MasterProductId = masterId,
                    Type = 2
                };
                _db.TagCategories.Add(tagcategory);
                _db.SaveChanges();
            }
            if (nextSubCategoryId != 0)
            {
                var tagcategory = new TagCategory
                {
                    CategoryId = nextSubCategoryId,
                    CategoryName = _db.NextSubCategories.FirstOrDefault(i => i.Id == nextSubCategoryId)?.Name,
                    CreatedBy = user.Name,
                    UpdatedBy = user.Name,
                    DateUpdated = DateTime.Now,
                    DateEncoded = DateTime.Now,
                    MasterProductId = masterId,
                    Type = 3
                };
                _db.TagCategories.Add(tagcategory);
                _db.SaveChanges();
            }

            List<TagCategorySessionList> tagCategoryList = Session["AddTagCategory"] as List<TagCategorySessionList>;
            if (tagCategoryList != null)
            {
                if (tagCategoryList.Count() > 0)
                {
                    foreach (var item in tagCategoryList)
                    {
                        var tagcategory = new TagCategory
                        {
                            CategoryId = item.Id,
                            CategoryName = item.Type == 1 ? _db.Categories.FirstOrDefault(i => i.Id == item.Id)?.Name : item.Type == 2 ? _db.SubCategories.FirstOrDefault(i => i.Id == item.Id)?.Name : _db.NextSubCategories.FirstOrDefault(i => i.Id == item.Id)?.Name,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name,
                            DateUpdated = DateTime.Now,
                            DateEncoded = DateTime.Now,
                            MasterProductId = masterId,
                            Type = item.Type
                        };
                        _db.TagCategories.Add(tagcategory);
                        _db.SaveChanges();
                    }
                }
            }
            Session["AddTagCategory"] = null;
        }
    }
}