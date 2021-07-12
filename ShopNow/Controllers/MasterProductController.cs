using Amazon;
using Amazon.S3;
using AutoMapper;
using ExcelDataReader;
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
        private ShopnowchatEntities _db = new ShopnowchatEntities();
        
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "MPR";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static string _generatedCode(string _prefix)
        {
            
                return ShopNow.Helpers.DRC.Generate(_prefix);
           
        }
        public MasterProductController()
        {
            _db.Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<MasterProduct, MasterProductListViewModel.MasterProductList>();
                config.CreateMap<MasterProductCreateEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterProductCreateEditViewModel>();
                config.CreateMap<MasterProduct, MedicalDrugListViewModel.MedicalDrugList>();
                config.CreateMap<MedicalDrugCreateEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MedicalDrugCreateEditViewModel>();
                config.CreateMap<Product, ItemMappingViewModel>();
                config.CreateMap<MasterFMCGCreateEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterFMCGCreateEditViewModel>();
                config.CreateMap<MasterFoodCreateViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterFoodCreateViewModel>();
                config.CreateMap<MasterFoodEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterFoodEditViewModel>();
                config.CreateMap<MasterFoodEditViewModel.AddonList, MasterAddOnsCreateViewModel>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

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
        
        [AccessPolicy(PageCode = "SHNMPRFC015")]
        public ActionResult FoodCreate()
        {
            Session["AddOns"] = new List<MasterAddOnsCreateViewModel>();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRFC015")]
        public ActionResult FoodCreate(MasterFoodCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MasterFoodCreateViewModel, MasterProduct>(model);
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.ProductType = "Dish";
            prod.Status = 0;
            prod.Code = _generatedCode("MPR");
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductType == "Dish" && i.CategoryCode == model.CategoryCode);
            prod.Name = model.Name;
            if(model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            if (name == null)
            {
                prod.DateEncoded = DateTime.Now;
                prod.DateUpdated = DateTime.Now;
                prod.Status = 0;
                try
                {
                    if (model.ProductImage1 != null)
                    {
                        uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                        prod.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
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
                _db.MasterProducts.Add(prod);
                _db.SaveChanges();
                ViewBag.Message = model.Name + " Saved Successfully!";
            }
            else
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
            }

            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            var productDishaddOn = new ProductDishAddOn();
            foreach (var s in addOns)
            {
                productDishaddOn.Code = _generatedCode("PDA");
                productDishaddOn.AddOnItemName = s.AddOnItemName;
                productDishaddOn.MasterProductCode = prod.Code;
                productDishaddOn.MasterProductName = prod.Name;
                productDishaddOn.AddOnCategoryCode = s.AddOnCategoryCode;
                productDishaddOn.AddOnCategoryName = s.AddOnCategoryName;
                productDishaddOn.PortionCode = s.PortionCode;
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
                productDishaddOn.MasterProductId = prod.Id;
                _db.ProductDishAddOns.Add(productDishaddOn);
                _db.SaveChanges();
            }

            return View();
        }

        [AccessPolicy(PageCode = "SHNMPRFE016")]
        public ActionResult FoodEdit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            Session["AddOns"] = new List<MasterAddOnsCreateViewModel>();
            var addOns = new List<MasterAddOnsCreateViewModel>();
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = _db.MasterProducts.FirstOrDefault(i => i.Code == dCode); 
            var model = _mapper.Map<MasterProduct, MasterFoodEditViewModel>(product);
            model.AddonLists = _db.ProductDishAddOns.Where(i => i.MasterProductCode == product.Code && i.Status == 0).Select(i => new MasterFoodEditViewModel.AddonList
            {
                Code = i.Code,
                AddOnItemName = i.AddOnItemName,
                MasterProductCode = i.MasterProductCode,
                MasterProductName = i.MasterProductName,
                PortionCode = i.PortionCode,
                PortionName = i.PortionName,
                PortionPrice = i.PortionPrice,
                AddOnCategoryCode = i.AddOnCategoryCode,
                AddOnCategoryName = i.AddOnCategoryName,
                AddOnsPrice = i.AddOnsPrice,
                CrustName = i.CrustName,
                CrustPrice = i.CrustPrice,
                MinSelectionLimit = i.MinSelectionLimit,
                MaxSelectionLimit = i.MaxSelectionLimit,
                AddOnType = i.AddOnType,
                MasterProductId =i.MasterProductId
            }).ToList();
            foreach(var s in model.AddonLists)
            {
                var addOn = _mapper.Map<MasterFoodEditViewModel.AddonList, MasterAddOnsCreateViewModel>(s);
                addOns.Add(addOn);
            }
            Session["AddOns"] = addOns;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRFE016")]
        public ActionResult FoodEdit(MasterFoodEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _db.MasterProducts.FirstOrDefault(i=>i.Code == model.Code); 
            _mapper.Map(model, prod);
            prod.DateUpdated = DateTime.Now;
            prod.UpdatedBy = user.Name;
            try
            {
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
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
            _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            var productDishaddOn = new ProductDishAddOn();
            foreach (var s in addOns)
            {
                if (s.Code == null)
                {
                    productDishaddOn.Code = _generatedCode("PDA");
                    productDishaddOn.AddOnItemName = s.AddOnItemName;
                    productDishaddOn.MasterProductCode = prod.Code;
                    productDishaddOn.MasterProductName = prod.Name;
                    productDishaddOn.AddOnCategoryCode = s.AddOnCategoryCode;
                    productDishaddOn.AddOnCategoryName = s.AddOnCategoryName;
                    productDishaddOn.PortionCode = s.PortionCode;
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
                    productDishaddOn.MasterProductId = prod.Id;
                    _db.ProductDishAddOns.Add(productDishaddOn);
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("FoodList", "MasterProduct");
        }

        [AccessPolicy(PageCode = "SHNMPRFL017")]
        public ActionResult FoodList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0 && mp.ProductType == "Dish").ToList();
            return View(List);
        }

        [AccessPolicy(PageCode = "SHNMPRC002")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRC002")]
        public ActionResult Create(MasterProductCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MasterProductCreateEditViewModel, MasterProduct>(model);
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.Code = _generatedCode("MPR");
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductType == "Dish");
            try
            {
                if (model.CategoryCode != null)
                {
                    prod.CategoryCode = String.Join(",", model.CategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.CategoryCode)
                    {
                        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);//Category.Get(s);
                        sb.Append(cat.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                         model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.CategoryName = model.CategoryName;
                    }
                    else
                    {
                        model.CategoryName = sb.ToString();
                        prod.CategoryName = model.CategoryName;
                    }
                }
                //var productImage = MasterProduct.Get(prod.Code);
                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                //if (model.ProductImage2 != null)
                //{
                //    uc.UploadFiles(model.ProductImage2.InputStream, prod.Code + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                //    prod.ImagePathLarge2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                //}

                //ProductImage3
                //if (model.ProductImage3 != null)
                //{
                //    uc.UploadFiles(model.ProductImage3.InputStream, prod.Code + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                //    prod.ImagePathLarge3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                //}

                //ProductImage4
                //if (model.ProductImage4 != null)
                //{
                //    uc.UploadFiles(model.ProductImage4.InputStream, prod.Code + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                //    prod.ImagePathLarge4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                //}

                //ProductImage5
                //if (model.ProductImage5 != null)
                //{
                //    uc.UploadFiles(model.ProductImage5.InputStream, prod.Code + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                //    prod.ImagePathLarge5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
                //}
                if (name == null)
                {
                    // MasterProduct.Add(prod);
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


        [AccessPolicy(PageCode = "SHNMPRE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Code == dCode);// MasterProduct.Get(dCode);
            var model = _mapper.Map<MasterProduct, MasterProductCreateEditViewModel>(masterProduct);
            if (masterProduct != null)
            {
                model.CategoryCode1 = masterProduct.CategoryCode;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRE003")]
        public ActionResult Edit(MasterProductCreateEditViewModel model)
        {
            int errorCode = 0;
            var user = ((Helpers.Sessions.User)Session["USER"]);
            try
            {
                var prod = _db.MasterProducts.FirstOrDefault(i => i.Code == model.Code);//MasterProduct.Get(model.Code);
                _mapper.Map(model, prod);
                prod.Name = model.Name;
                prod.ProductType = model.ProductType;
                prod.UpdatedBy = user.Name;
                prod.DateUpdated = DateTime.Now;
                if (model.CategoryCode != null)
                {
                    prod.CategoryCode = String.Join(",", model.CategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.CategoryCode)
                    {
                        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                        if (cat != null)
                        {
                            sb.Append(cat.Name);
                            sb.Append(",");
                        }
                    }
                    if (sb.Length >= 1)
                    {
                        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.CategoryName = model.CategoryName;
                    }
                    else
                    {
                        model.CategoryName = sb.ToString();
                        prod.CategoryName = model.CategoryName;
                    }
                }
                prod.DateUpdated = DateTime.Now;
                _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                //MasterProduct.Edit(prod, out errorCode);
                try
                {
                    var productImage = _db.MasterProducts.FirstOrDefault(i => i.Code == prod.Code);// MasterProduct.Get(prod.Code);
                    //ProductImage1
                    if (model.ProductImage1 != null)
                    {
                        uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                        productImage.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                    }

                    //ProductImage2
                    if (model.ProductImage2 != null)
                    {
                        uc.UploadFiles(model.ProductImage2.InputStream, prod.Code + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                        productImage.ImagePathLarge2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                    }

                    //ProductImage3
                    if (model.ProductImage3 != null)
                    {
                        uc.UploadFiles(model.ProductImage3.InputStream, prod.Code + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                        productImage.ImagePathLarge3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                    }

                    //ProductImage4
                    if (model.ProductImage4 != null)
                    {
                        uc.UploadFiles(model.ProductImage4.InputStream, prod.Code + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                        productImage.ImagePathLarge4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                    }

                    //ProductImage5
                    if (model.ProductImage5 != null)
                    {
                        uc.UploadFiles(model.ProductImage5.InputStream, prod.Code + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                        productImage.ImagePathLarge5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
                    }
                    if (model.ProductImage1 != null || model.ProductImage2 != null || model.ProductImage3 != null || model.ProductImage4 != null || model.ProductImage5 != null)
                    {
                        productImage.DateUpdated = DateTime.Now;
                        _db.Entry(productImage).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        //MasterProduct.Edit(productImage, out int error);
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
                return RedirectToAction("DishList");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SHNMPRD004")]
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Code == dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult Index(HttpPostedFileBase upload, MainPageModel model)
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
                    var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);// MasterProduct.GetName(row[model.Name].ToString());
                    if (masterProduct == null)
                    {
                        _db.MasterProducts.Add(new MasterProduct
                        {
                            Name = row[model.Name].ToString(),
                            Code = _generatedCode("MPR"),
                            BrandCode = CheckBrand(row[model.BrandName].ToString(), model.ProductType),
                            BrandName = row[model.BrandName].ToString(),
                            CategoryCode = CheckCategory(row[model.CategoryName].ToString(), model.ProductType),
                            CategoryName = row[model.CategoryName].ToString(),
                            ShortDescription = row[model.ShortDescription].ToString(),
                            LongDescription = row[model.LongDescription].ToString(),
                            Customisation = Convert.ToBoolean(row[model.Customisation]),
                            ColorCode = row[model.ColorCode].ToString(),
                            Price = Convert.ToDouble(row[model.Price]),
                            ImagePathLarge1 = row[model.ImagePathLarge1].ToString(),
                            ImagePathLarge2 = row[model.ImagePathLarge2].ToString(),
                            ImagePathLarge3 = row[model.ImagePathLarge3].ToString(),
                            ImagePathLarge4 = row[model.ImagePathLarge4].ToString(),
                            ImagePathLarge5 = row[model.ImagePathLarge5].ToString(),
                            ProductType = model.ProductType,
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

        public string CheckBrand(string BrandName, string ProductType)
        {
            var brand = _db.Brands.FirstOrDefault(i => i.Name == BrandName && i.Status == 0);// Brand.GetName(BrandName);
            if (brand != null)
            {
                return brand.Code;
            }
            else
            {
                Brand br = new Brand();
                br.Name = BrandName;
                br.ProductType = ProductType;
                br.Code = _generatedCode("BRA");
                br.Status = 0;
                br.DateEncoded = DateTime.Now;
                br.DateUpdated = DateTime.Now;
                _db.Brands.Add(br);
                _db.SaveChanges();
                //br.Code = Brand.Add(br, out int error);
                return br.Code;
            }

        }


        public string CheckSubCategory(string CategoryCode, string CategoryName, string SubCategoryName1, string ProductType)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var subCategory = _db.SubCategories.FirstOrDefault(i => i.Name == SubCategoryName1 && i.Status == 0);// SubCategory.GetName(SubCategoryName1);
            if (subCategory != null)
            {

                return subCategory.Code;
            }
            else
            {
                SubCategory sub = new SubCategory();
                sub.Name = SubCategoryName1;
                sub.CategoryCode = CategoryCode;
                sub.CategoryName = CategoryName;
                sub.ProductType = ProductType;
                sub.CreatedBy = user.Name;
                sub.UpdatedBy = user.Name;
                sub.Code = _generatedCode("SCT");
                sub.Status = 0;
                sub.DateEncoded = DateTime.Now;
                sub.DateUpdated = DateTime.Now;
                _db.SubCategories.Add(sub);
                _db.SaveChanges();
                //sub.Code = SubCategory.Add(sub, out int error);
                return sub.Code;
            }

        }


        public string CheckNextSubCategory(string subCategoryCode1, string SubCategoryName1, string SubCategoryName2, string ProductType)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);

            var nextSubCategory = _db.NextSubCategories.FirstOrDefault(i => i.Name == SubCategoryName2 && i.Status == 0);// NextSubCategory.GetName(SubCategoryName2);
            if (nextSubCategory != null)
            {

                return nextSubCategory.Code;
            }
            else
            {
                NextSubCategory sub = new NextSubCategory();
                sub.Name = SubCategoryName2;
                sub.SubCategoryCode = subCategoryCode1;
                sub.SubCategoryName = SubCategoryName1;
                sub.ProductType = ProductType;
                sub.CreatedBy = user.Name;
                sub.UpdatedBy = user.Name;
                sub.Code = _generatedCode("NSC");
                sub.DateEncoded = DateTime.Now;
                sub.DateUpdated = DateTime.Now;
                sub.Status = 0;
                _db.NextSubCategories.Add(sub);
                _db.SaveChanges();
                //sub.Code = NextSubCategory.Add(sub, out int error);
                return sub.Code;
            }

        }

        public string CheckCategory(string CategoryName, string ProductType)
        {
            var category = _db.Categories.FirstOrDefault(i => i.Name == CategoryName && i.Status == 0);// Category.GetName(CategoryName);
            if (category != null)
            {

                return category.Code;
            }
            else
            {
                Category cat = new Category();
                cat.Name = CategoryName;
                cat.ProductType = ProductType;
                cat.Code = _generatedCode("CAT");
                cat.Status = 0;
                cat.DateEncoded = DateTime.Now;
                cat.DateUpdated = DateTime.Now;
                _db.Categories.Add(cat);
                _db.SaveChanges();
                //cat.Code = Category.Add(cat, out int error);
                return cat.Code;
            }

        }

        [AccessPolicy(PageCode = "SHNMPREL018")]
        public ActionResult ElectronicList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0 && mp.ProductType == "Product").ToList();
            return View(List);
        }

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
        public ActionResult ElectronicCreate(MasterProductCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MasterProductCreateEditViewModel, MasterProduct>(model);
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.Code = _generatedCode("MPR");
            if (model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductType == "Product");// MasterProduct.GetElectronicName(model.Name);
            try
            {
                if (model.CategoryCode != null)
                {
                    prod.CategoryCode = String.Join(",", model.CategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.CategoryCode)
                    {
                        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                        sb.Append(cat.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.CategoryName = model.CategoryName;
                    }
                    else
                    {
                        model.CategoryName = sb.ToString();
                        prod.CategoryName = model.CategoryName;
                    }
                }
                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.ProductImage2 != null)
                {
                    uc.UploadFiles(model.ProductImage2.InputStream, prod.Code + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.ProductImage3 != null)
                {
                    uc.UploadFiles(model.ProductImage3.InputStream, prod.Code + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.ProductImage4 != null)
                {
                    uc.UploadFiles(model.ProductImage4.InputStream, prod.Code + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.ProductImage5 != null)
                {
                    uc.UploadFiles(model.ProductImage5.InputStream, prod.Code + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
                }
                if (name == null)
                {
                    prod.DateEncoded = DateTime.Now;
                    prod.DateUpdated = DateTime.Now;
                    prod.Status = 0;
                    _db.MasterProducts.Add(prod);
                    _db.SaveChanges();
                    //  MasterProduct.Add(prod);
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

        [AccessPolicy(PageCode = "SHNMPREE020")]
        public ActionResult ElectronicEdit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Code == dCode);// MasterProduct.Get(dCode);
            var model = _mapper.Map<MasterProduct, MasterProductCreateEditViewModel>(masterProduct);
            model.CategoryCode1 = masterProduct.CategoryCode;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPREE020")]
        public ActionResult ElectronicEdit(MasterProductCreateEditViewModel model)
        {
            int errorCode = 0;
            var user = ((Helpers.Sessions.User)Session["USER"]);
            try
            {
                var prod = _db.MasterProducts.FirstOrDefault(i => i.Code == model.Code);// MasterProduct.Get(model.Code);
                _mapper.Map(model, prod);
                prod.Name = model.Name;
                prod.ProductType = model.ProductType;
                prod.UpdatedBy = user.Name;
                prod.DateUpdated = DateTime.Now;
                try
                {
                    if (model.CategoryCode != null)
                    {
                        prod.CategoryCode = String.Join(",", model.CategoryCode);
                        StringBuilder sb = new StringBuilder();
                        foreach (var s in model.CategoryCode)
                        {
                            var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                            if (cat != null)
                            {
                                sb.Append(cat.Name);
                                sb.Append(",");
                            }
                        }
                        if (sb.Length >= 1)
                        {
                            model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                            prod.CategoryName = model.CategoryName;
                        }
                        else
                        {
                            model.CategoryName = sb.ToString();
                            prod.CategoryName = model.CategoryName;
                        }
                    }
                
                    //ProductImage1
                    if (model.ProductImage1 != null)
                    {
                        uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                        prod.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                    }

                    //ProductImage2
                    if (model.ProductImage2 != null)
                    {
                        uc.UploadFiles(model.ProductImage2.InputStream, prod.Code + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                        prod.ImagePathLarge2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                    }

                    //ProductImage3
                    if (model.ProductImage3 != null)
                    {
                        uc.UploadFiles(model.ProductImage3.InputStream, prod.Code + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                        prod.ImagePathLarge3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                    }

                    //ProductImage4
                    if (model.ProductImage4 != null)
                    {
                        uc.UploadFiles(model.ProductImage4.InputStream, prod.Code + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                        prod.ImagePathLarge4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                    }

                    //ProductImage5
                    if (model.ProductImage5 != null)
                    {
                        uc.UploadFiles(model.ProductImage5.InputStream, prod.Code + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                        prod.ImagePathLarge5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
                    }
                    prod.DateUpdated = DateTime.Now;
                    _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    //  MasterProduct.Edit(prod, out errorCode);
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
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SHNMPRED021")]
        public ActionResult ElectronicDelete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Code == dCode);// MasterProduct.Get(dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNMPRML006")]
        public ActionResult MedicalList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0 && mp.ProductType == "Medical").ToList();
            return View(List);
        }

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
        public ActionResult MedicalCreate(MedicalDrugCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MedicalDrugCreateEditViewModel, MasterProduct>(model);
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.ProductType = "Medical";
            prod.Code = _generatedCode("MPR");
            if (model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductType == "Medical");
            try
            {
                if (model.CategoryCode != null)
                {
                    prod.CategoryCode = String.Join(",", model.CategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.CategoryCode)
                    {
                        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                        sb.Append(cat.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.CategoryName = model.CategoryName;
                    }
                    else
                    {
                        model.CategoryName = sb.ToString();
                        prod.CategoryName = model.CategoryName;
                    }
                }
                if (model.DrugCompoundDetailCode != null)
                {
                    prod.DrugCompoundDetailCode = String.Join(",", model.DrugCompoundDetailCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.DrugCompoundDetailCode)
                    {
                        var dcd = _db.DrugCompoundDetails.FirstOrDefault(i => i.Code == s && i.Status == 0); // DrugCompoundDetail.Get(s);
                        sb.Append(dcd.AliasName);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.CombinationDrugCompound = sb.ToString().Remove(sb.Length - 1);
                        prod.CombinationDrugCompound = model.CombinationDrugCompound;
                    }
                    else
                    {
                        model.CombinationDrugCompound = sb.ToString();
                        prod.CombinationDrugCompound = model.CombinationDrugCompound;
                    }
                }
               
                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.ProductImage2 != null)
                {
                    uc.UploadFiles(model.ProductImage2.InputStream, prod.Code + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.ProductImage3 != null)
                {
                    uc.UploadFiles(model.ProductImage3.InputStream, prod.Code + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.ProductImage4 != null)
                {
                    uc.UploadFiles(model.ProductImage4.InputStream, prod.Code + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.ProductImage5 != null)
                {
                    uc.UploadFiles(model.ProductImage5.InputStream, prod.Code + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
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

        [AccessPolicy(PageCode = "SHNMPRME008")]
        public ActionResult MedicalEdit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Code == dCode);
            var model = _mapper.Map<MasterProduct, MedicalDrugCreateEditViewModel>(masterProduct);
            if(model.ImagePathLarge1 !=null)
            model.ImagePathLarge1 = model.ImagePathLarge1.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePathLarge2 != null)
                model.ImagePathLarge2 = model.ImagePathLarge2.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePathLarge3 != null)
                model.ImagePathLarge3 = model.ImagePathLarge3.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePathLarge4 != null)
                model.ImagePathLarge4 = model.ImagePathLarge4.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePathLarge5 != null)
                model.ImagePathLarge5 = model.ImagePathLarge5.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            model.DrugCompoundDetailCode1 = masterProduct.DrugCompoundDetailCode;
            model.CategoryCode1 = masterProduct.CategoryCode;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRME008")]
        public ActionResult MedicalEdit(MedicalDrugCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _db.MasterProducts.FirstOrDefault(i => i.Code == model.Code);
            _mapper.Map(model, prod);
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            try
            {
                if (model.CategoryCode != null)
                {
                    prod.CategoryCode = String.Join(",", model.CategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.CategoryCode)
                    {
                        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                        if (cat != null)
                        {
                            sb.Append(cat.Name);
                            sb.Append(",");
                        }
                    }
                    if (sb.Length >= 1)
                    {
                        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.CategoryName = model.CategoryName;
                    }
                    else
                    {
                        model.CategoryName = sb.ToString();
                        prod.CategoryName = model.CategoryName;
                    }
                }

                if (model.DrugCompoundDetailCode != null)
                {
                    prod.DrugCompoundDetailCode = String.Join(",", model.DrugCompoundDetailCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.DrugCompoundDetailCode)
                    {
                        var dcd = _db.DrugCompoundDetails.FirstOrDefault(i => i.Code == s.Replace(" ","") && i.Status == 0);// DrugCompoundDetail.Get(s);
                        if (dcd != null)
                        {
                            sb.Append(dcd.AliasName);
                            sb.Append(",");
                        }
                    }
                    if (sb.Length >= 1)
                    {
                        model.CombinationDrugCompound = sb.ToString().Remove(sb.Length - 1);
                        prod.CombinationDrugCompound = model.CombinationDrugCompound;
                    }
                    else
                    {
                        model.CombinationDrugCompound = sb.ToString();
                        prod.CombinationDrugCompound = model.CombinationDrugCompound;
                    }
                }

                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                    //if (product != null)
                    //product.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.ProductImage2 != null)
                {
                    uc.UploadFiles(model.ProductImage2.InputStream, prod.Code + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                    //if (product != null)
                    //    product.ImagePathLarge2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.ProductImage3 != null)
                {
                    uc.UploadFiles(model.ProductImage3.InputStream, prod.Code + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                    //if (product != null)
                    //    product.ImagePathLarge3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.ProductImage4 != null)
                {
                    uc.UploadFiles(model.ProductImage4.InputStream, prod.Code + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                    //if (product != null)
                    //    product.ImagePathLarge4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.ProductImage5 != null)
                {
                    uc.UploadFiles(model.ProductImage5.InputStream, prod.Code + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
                    //if (product != null)
                    //    product.ImagePathLarge5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
                }
                _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                //  MasterProduct.Edit(prod, out int error);
                //if (product != null)
                //{
                //    product.MasterProductCode = prod.Code;
                //    product.MasterProductCode = prod.Name;
                //    _db.Entry(product).State = EntityState.Modified;
                //    _db.SaveChanges();
                //}
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
            return RedirectToAction("MedicalList");

        }

        [AccessPolicy(PageCode = "SHNMPRMD009")]
        public ActionResult MedicalDelete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Code == dCode);// MasterProduct.Get(dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("MedicalList");
        }

        [AccessPolicy(PageCode = "SHNMPRMI010")]
        public ActionResult MedicalIndex()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRMI010")]
        public ActionResult MedicalIndex(HttpPostedFileBase upload, MedicalDrugViewModel model)
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
                    var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);// MasterProduct.GetName(row[model.Name].ToString());
                    if (masterProduct == null)
                    {
                        _db.MasterProducts.Add(new MasterProduct
                        {
                            Name = row[model.Name].ToString(),
                            Code = _generatedCode("MPR"),
                            BrandCode = MedicalCheckBrand(row[model.BrandName].ToString(), model.ProductType),
                            BrandName = row[model.BrandName].ToString(),
                            CategoryCode = MedicalCheckCategory(row[model.CategoryName].ToString(), model.ProductType),
                            CategoryName = row[model.CategoryName].ToString(),
                            MeasurementUnitCode = row[model.DrugMeasurementUnitCode].ToString(),
                            MeasurementUnitName = row[model.DrugMeasurementUnitName].ToString(),
                            PriscriptionCategory = Convert.ToBoolean(row[model.PriscriptionCategory]),
                            DrugCompoundDetailCode = row[model.DrugCompoundDetailCode].ToString(),
                            CombinationDrugCompound = row[model.CombinationDrugCompound].ToString(),
                            Price = Convert.ToDouble(row[model.Price]),
                            ImagePathLarge1 = row[model.ImagePathLarge1].ToString(),
                            ImagePathLarge2 = row[model.ImagePathLarge2].ToString(),
                            ImagePathLarge3 = row[model.ImagePathLarge3].ToString(),
                            ImagePathLarge4 = row[model.ImagePathLarge4].ToString(),
                            ImagePathLarge5 = row[model.ImagePathLarge5].ToString(),
                            OriginCountry = row[model.OriginCountry].ToString(),
                            Manufacturer = row[model.Manufacturer].ToString(),
                            iBarU = row[model.iBarU].ToString(),
                            SizeLB = row[model.SizeLB].ToString(),
                            weight = Convert.ToDouble(row[model.weight]),
                            PackageCode = CheckMedicalPackage(row[model.PackageName].ToString()),
                            PackageName = row[model.PackageName].ToString(),
                            ProductType = model.ProductType,
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

        public string MedicalCheckBrand(string BrandName, string ProductType)
        {
            var brand = _db.Brands.FirstOrDefault(i => i.Name == BrandName && i.Status == 0);// Brand.GetName(BrandName);
            if (brand != null)
            {
                return brand.Code;
            }
            else
            {
                Brand br = new Brand();
                br.Name = BrandName;
                br.ProductType = ProductType;
                br.Code = _generatedCode("BRA");
                br.Status = 0;
                br.DateEncoded = DateTime.Now;
                br.DateUpdated = DateTime.Now;
                _db.Brands.Add(br);
                _db.SaveChanges();
              //  br.Code = Brand.Add(br, out int error);
                return br.Code;
            }

        }

        public string MedicalCheckCategory(string CategoryName, string ProductType)
        {
            var category = _db.Categories.FirstOrDefault(i => i.Name == CategoryName && i.Status == 0);// Category.GetName(CategoryName);
            if (category != null)
            {

                return category.Code;
            }
            else
            {
                Category cat = new Category();
                cat.Name = CategoryName;
                cat.ProductType = ProductType;
                cat.Code = _generatedCode("CAT");
                cat.Status = 0;
                cat.DateEncoded = DateTime.Now;
                cat.DateUpdated = DateTime.Now;
                _db.Categories.Add(cat);
                _db.SaveChanges();
                //cat.Code = Category.Add(cat, out int error);
                return cat.Code;
            }

        }

        public string CheckMedicalPackage(string PackageName)
        {
            var package = _db.Packages.FirstOrDefault(i => i.Name == PackageName && i.Status == 0);// Package.GetName(PackageName);
            if (package != null)
            {
                return package.Code;
            }
            else
            {
                Package mp = new Package();
                mp.Name = PackageName;
                mp.Type = 1;
                mp.Code = _generatedCode("PKG");
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                _db.Packages.Add(mp);
                _db.SaveChanges();
                // mp.Code = Package.Add(mp, out int error);
                return mp.Code;
            }

        }

        public string CheckPackage(string PackageName)
        {
            var package = _db.Packages.FirstOrDefault(i => i.Name == PackageName && i.Status == 0);// Package.GetName(PackageName);
            if (package != null)
            {
                return package.Code;
            }
            else
            {
                Package mp = new Package();
                mp.Name = PackageName;
                mp.Type = 2;
                mp.Code = _generatedCode("PKG");
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                _db.Packages.Add(mp);
                _db.SaveChanges();
                // mp.Code = Package.Add(mp, out int error);
                return mp.Code;
            }

        }

        public string CheckFMCGMeasurementUnit(string MeasurementUnitName)
        {
            var mu = _db.MeasurementUnits.FirstOrDefault(i => i.UnitName == MeasurementUnitName && i.Status == 0);// MeasurementUnit.GetName(MeasurementUnitName);
            if (mu != null)
            {
                return mu.Code;
            }
            else
            {
                MeasurementUnit mp = new MeasurementUnit();
                mp.UnitName = MeasurementUnitName;
                mp.Type = 2;
                mp.Code = _generatedCode("MSU");
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                _db.MeasurementUnits.Add(mp);
                _db.SaveChanges();
                // mp.Code = MeasurementUnit.Add(mp, out int error);
                return mp.Code;
            }

        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public ActionResult ItemMapping()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public ActionResult SingleItemMapping(string shopcode)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ItemMappingViewModel();
            if (shopcode != null)
            {
                var shop = _db.Shops.FirstOrDefault(i => i.Code == shopcode);// Shop.Get(shopcode);
                model.ShopCode = shopcode;
                if (shop != null)
                {
                    model.ShopName = shop.Name;
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRPL013")]
        public ActionResult PendingList(string shopcode)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new MasterProductListViewModel();
            //var List = (from p in _db.Products
            //            select p).OrderBy(p => p.Name).Where(p => p.ShopCode == shopcode && p.Status == 0 && p.MasterProductCode == null && p.MasterProductName == null).ToList();
            //return View(List);
            var shid = _db.Shops.Where(s => s.Code == shopcode).FirstOrDefault();
            model.Lists = _db.ProductMedicalStocks
                 .Join(_db.Products, ms => ms.productid, p => p.Id, (ms, p) => new { ms, p })
                 .Where(a => a.ms.SupplierName != "NA" && a.p.shopid == shid.Id && a.p.Status == 0 
                 && a.p.MasterProductCode == null && a.p.MasterProductName == null)
                 .OrderBy(i => i.p.Name).Select(i => new MasterProductListViewModel.PendingList
                 {
                     Id = i.p.Id,
                     Code = i.p.Code,
                     Name = i.p.Name,
                     ItemId = i.p.ItemId,
                     ProductType = i.p.ProductType
                 }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRML012")]
        public ActionResult MappedList(string shopcode)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            //var model = new MasterProductListViewModel();
            //model.MappedLists = _db.ProductMedicalStocks
            //     .Join(_db.Products, ms => ms.productid, p => p.Id, (ms, p) => new { ms, p })
            //     .Where(a => a.ms.SupplierName != "NA" && a.p.ShopCode == shopcode && a.p.Status == 0 && a.p.MasterProductCode != null && a.p.MasterProductName != null)
            //     .OrderBy(i => i.p.Name).Select(i => new MasterProductListViewModel.MappedList
            //     {
            //         Id = i.p.Id,
            //         Code = i.p.Code,
            //         Name = i.p.Name,
            //         MasterProductCode = i.p.MasterProductCode,
            //         MasterProductName = i.p.MasterProductName,
            //         ProductType = i.p.ProductType
            //     }).ToList();
            var List = (from pms in _db.ProductMedicalStocks
                        join p in _db.Products on pms.productid equals p.Id
                        where (pms.SupplierName != "N/A" && p.ShopCode == shopcode && p.Status == 0 && p.MasterProductCode != null && p.MasterProductName != null)
                        select p).ToList();
            return View(List);
        }

        [AccessPolicy(PageCode = "SHNMPRMU014")]
        public ActionResult ItemMappingUpdate(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = _db.Products.FirstOrDefault(i => i.Code == dCode);// Product.Get(dCode);
            var model = _mapper.Map<Product, ItemMappingViewModel>(product);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public JsonResult MappingProduct(string masterproductcode, string itemcode, string shopcode)
        {
            try
            {
                var user = ((Helpers.Sessions.User)Session["USER"]);
                ViewBag.Name = user.Name;
                var masterproduct = _db.MasterProducts.FirstOrDefault(i => i.Code == masterproductcode);// MasterProduct.Get(masterproductcode);

                var product = _db.Products.FirstOrDefault(i => i.Code == itemcode);// Product.Get(itemcode);
                if (product != null)
                {
                    product.MasterProductCode = masterproductcode;
                    product.MasterProductName = masterproduct.Name;
                    product.MasterProductId = masterproduct.Id;
                    if (product.BrandCode == null && masterproduct.BrandCode != null)
                    {
                        product.BrandCode = masterproduct.BrandCode;
                        product.BrandName = masterproduct.BrandName;
                    }
                    if (product.CategoryCode == null && masterproduct.CategoryCode != null)
                    {
                        product.CategoryCode = masterproduct.CategoryCode;
                        product.CategoryName = masterproduct.CategoryName;
                    }
                    if (product.MeasurementUnitCode == null && masterproduct.MeasurementUnitCode != null)
                    {
                        product.MeasurementUnitCode = masterproduct.MeasurementUnitCode;
                        product.MeasurementUnitName = masterproduct.MeasurementUnitName;
                    }
                    if (product.DrugCompoundDetailCode == null && masterproduct.DrugCompoundDetailCode != null)
                    {
                        product.DrugCompoundDetailCode = masterproduct.DrugCompoundDetailCode;
                        product.CombinationDrugCompound = masterproduct.CombinationDrugCompound;
                    }
                    if (product.GoogleTaxonomyCode == null && masterproduct.GoogleTaxonomyCode != null)
                    {
                        product.GoogleTaxonomyCode = masterproduct.GoogleTaxonomyCode;
                    }
                    if (product.ShortDescription == null && masterproduct.ShortDescription != null)
                    {
                        product.ShortDescription = masterproduct.ShortDescription;
                    }
                    if (product.LongDescription == null && masterproduct.LongDescription != null)
                    {
                        product.LongDescription = masterproduct.LongDescription;
                    }
                    if (product.Customisation == false && masterproduct.Customisation != false)
                    {
                        product.Customisation = masterproduct.Customisation;
                    }
                    if (product.ColorCode == null && masterproduct.ColorCode != null)
                    {
                        product.ColorCode = masterproduct.ColorCode;
                    }
                    if (product.ImagePathLarge1 == null && masterproduct.ImagePathLarge1 != null)
                    {
                        product.ImagePathLarge1 = masterproduct.ImagePathLarge1;
                    }
                    if (product.ImagePathLarge2 == null && masterproduct.ImagePathLarge2 != null)
                    {
                        product.ImagePathLarge2 = masterproduct.ImagePathLarge2;
                    }
                    if (product.ImagePathLarge3 == null && masterproduct.ImagePathLarge3 != null)
                    {
                        product.ImagePathLarge3 = masterproduct.ImagePathLarge3;
                    }
                    if (product.ImagePathLarge4 == null && masterproduct.ImagePathLarge4 != null)
                    {
                        product.ImagePathLarge4 = masterproduct.ImagePathLarge4;
                    }
                    if (product.ImagePathLarge5 == null && masterproduct.ImagePathLarge5 != null)
                    {
                        product.ImagePathLarge5 = masterproduct.ImagePathLarge5;
                    }
                    if (product.Price == 0 && masterproduct.Price != 0)
                    {
                        product.Price = masterproduct.Price;
                    }
                    if (product.PriscriptionCategory == false && masterproduct.PriscriptionCategory != false)
                    {
                        product.PriscriptionCategory = masterproduct.PriscriptionCategory;
                    }
                    if (product.iBarU == null && masterproduct.iBarU != null)
                    {
                        product.iBarU = masterproduct.iBarU;
                    }
                    if (product.OriginCountry == null && masterproduct.OriginCountry != null)
                    {
                        product.OriginCountry = masterproduct.OriginCountry;
                    }
                    if (product.Manufacturer == null && masterproduct.Manufacturer != null)
                    {
                        product.Manufacturer = masterproduct.Manufacturer;
                    }
                    if (product.weight == 0 && masterproduct.weight != 0)
                    {
                        product.weight = masterproduct.weight;
                    }
                    if (product.SizeLB == null && masterproduct.SizeLB != null)
                    {
                        product.SizeLB = masterproduct.SizeLB;
                    }
                    if (product.ProductType == null && masterproduct.ProductType != null)
                    {
                        product.ProductType = masterproduct.ProductType;
                    }
                    //if (shopcode == null)
                    //{
                    //    product.ShopCode = "Admin";
                    //    product.ShopName = "Admin";
                    //}
                    if (shopcode != null)
                    {
                        var shop = _db.Shops.FirstOrDefault(i => i.Code == shopcode);// Shop.Get(shopcode);
                        if (shop != null)
                        {
                            product.ShopCode = shop.Code;
                            product.ShopName = shop.Name;
                            product.ShopCategoryCode = shop.ShopCategoryCode;
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
        public JsonResult UpdateMappingProduct(string masterproductcode, string code, string shopcode, bool isCheck)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var masterproduct = _db.MasterProducts.FirstOrDefault(i => i.Code == masterproductcode);// MasterProduct.Get(masterproductcode);
            var product = _db.Products.FirstOrDefault(i => i.Code == code);// Product.Get(code);
            if (isCheck == true)
            {
                product.MasterProductCode = null;
                product.MasterProductName = null;
                product.BrandCode = null;
                product.BrandName = null;
                product.CategoryCode = null;
                product.CategoryName = null;
                product.MeasurementUnitCode = null;
                product.MeasurementUnitName = null;
                product.DrugCompoundDetailCode = null;
                product.CombinationDrugCompound = null;
                product.GoogleTaxonomyCode = null;
                product.ShortDescription = null;
                product.LongDescription = null;
                product.ColorCode = null;
                product.ImagePath = null;
                product.iBarU = null;
                product.OriginCountry = null;
                product.Manufacturer = null;
                product.UpdatedBy = user.Name;
                product.DateUpdated = DateTime.Now;
                _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                //Product.Edit(product, out int errorCode);
            }
            else
            {
                if (product != null && masterproduct != null)
                {
                    product.MasterProductCode = masterproductcode;
                    product.MasterProductName = masterproduct.Name;
                    product.MasterProductId = masterproduct.Id;
                    if (masterproduct.BrandCode != null)
                    {
                        product.BrandCode = masterproduct.BrandCode;
                        product.BrandName = masterproduct.BrandName;
                    }
                    if (masterproduct.CategoryCode != null)
                    {
                        product.CategoryCode = masterproduct.CategoryCode;
                        product.CategoryName = masterproduct.CategoryName;
                    }
                    if (masterproduct.MeasurementUnitCode != null)
                    {
                        product.MeasurementUnitCode = masterproduct.MeasurementUnitCode;
                        product.MeasurementUnitName = masterproduct.MeasurementUnitName;
                    }
                    if (masterproduct.DrugCompoundDetailCode != null)
                    {
                        product.DrugCompoundDetailCode = masterproduct.DrugCompoundDetailCode;
                        product.CombinationDrugCompound = masterproduct.CombinationDrugCompound;
                    }
                    if (masterproduct.GoogleTaxonomyCode != null)
                    {
                        product.GoogleTaxonomyCode = masterproduct.GoogleTaxonomyCode;
                    }
                    if (masterproduct.ShortDescription != null)
                    {
                        product.ShortDescription = masterproduct.ShortDescription;
                    }
                    if (masterproduct.LongDescription != null)
                    {
                        product.LongDescription = masterproduct.LongDescription;
                    }
                    if (masterproduct.Customisation != false)
                    {
                        product.Customisation = masterproduct.Customisation;
                    }
                    if (masterproduct.ColorCode != null)
                    {
                        product.ColorCode = masterproduct.ColorCode;
                    }
                    if (product.ImagePathLarge1 == null && masterproduct.ImagePathLarge1 != null)
                    {
                        product.ImagePathLarge1 = masterproduct.ImagePathLarge1;
                    }
                    if (product.ImagePathLarge2 == null && masterproduct.ImagePathLarge2 != null)
                    {
                        product.ImagePathLarge2 = masterproduct.ImagePathLarge2;
                    }
                    if (product.ImagePathLarge3 == null && masterproduct.ImagePathLarge3 != null)
                    {
                        product.ImagePathLarge3 = masterproduct.ImagePathLarge3;
                    }
                    if (product.ImagePathLarge4 == null && masterproduct.ImagePathLarge4 != null)
                    {
                        product.ImagePathLarge4 = masterproduct.ImagePathLarge4;
                    }
                    if (product.ImagePathLarge5 == null && masterproduct.ImagePathLarge5 != null)
                    {
                        product.ImagePathLarge5 = masterproduct.ImagePathLarge5;
                    }
                    if (masterproduct.Price != 0)
                    {
                        product.Price = masterproduct.Price;
                    }
                    if (masterproduct.PriscriptionCategory != false)
                    {
                        product.PriscriptionCategory = masterproduct.PriscriptionCategory;
                    }
                    if (masterproduct.iBarU != null)
                    {
                        product.iBarU = masterproduct.iBarU;
                    }
                    if (masterproduct.OriginCountry != null)
                    {
                        product.OriginCountry = masterproduct.OriginCountry;
                    }
                    if (masterproduct.Manufacturer != null)
                    {
                        product.Manufacturer = masterproduct.Manufacturer;
                    }
                    if (masterproduct.weight != 0)
                    {
                        product.weight = masterproduct.weight;
                    }
                    if (masterproduct.SizeLB != null)
                    {
                        product.SizeLB = masterproduct.SizeLB;
                    }
                    if (masterproduct.ProductType != null)
                    {
                        product.ProductType = masterproduct.ProductType;
                    }
                    if (shopcode == null)
                    {
                        product.ShopCode = "Admin";
                        product.ShopName = "Admin";
                    }
                    if (shopcode != null)
                    {
                        var shop = _db.Shops.FirstOrDefault(i => i.Code == shopcode);// Shop.Get(shopcode);
                        if (shop != null)
                        {
                            product.ShopCode = shop.Code;
                            product.ShopName = shop.Name;
                            product.ShopCategoryCode = shop.ShopCategoryCode;
                            product.ShopCategoryName = shop.ShopCategoryName;
                        }
                    }
                    product.UpdatedBy = user.Name;
                    product.DateUpdated = DateTime.Now;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    //Product.Edit(product, out int errorCode);
                }
            }
            bool results = true;
            return Json(new { results, shopcode }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult GroceryEntry()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult GroceryEntry(HttpPostedFileBase upload, GroceryPageModel model)
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
                    var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);// MasterProduct.GetName(row[model.Name].ToString());
                    if (masterProduct == null)
                    {
                        model.CategoryCode = CheckCategory(row[model.CategoryName].ToString(), model.ProductType);
                        model.SubCategoryCode1 = CheckSubCategory(model.CategoryCode, row[model.CategoryName].ToString(), row[model.SubCategoryName1].ToString(), model.ProductType);
                        _db.MasterProducts.Add(new MasterProduct
                        {
                            Name = row[model.Name].ToString(),
                            Code = _generatedCode("MPR"),
                            BrandCode = CheckBrand(row[model.BrandName].ToString(), model.ProductType),
                            BrandName = row[model.BrandName].ToString(),
                            SizeLB = row[model.SizeLB].ToString(),
                            weight = Convert.ToDouble(row[model.weight]),
                            GoogleTaxonomyCode = row[model.GoogleTaxonomyCode].ToString(),
                            ASIN = row[model.ASIN].ToString(),
                            CategoryCode = model.CategoryCode,
                            CategoryName = row[model.CategoryName].ToString(),
                            ShortDescription = row[model.ShortDescription].ToString(),
                            LongDescription = row[model.LongDescription].ToString(),
                            Price = Convert.ToDouble(row[model.Price]),
                            ImagePathLarge1 = row[model.ImagePathLarge1].ToString(),
                            ImagePathLarge2 = row[model.ImagePathLarge2].ToString(),
                            ImagePathLarge3 = row[model.ImagePathLarge3].ToString(),
                            ImagePathLarge4 = row[model.ImagePathLarge4].ToString(),
                            ImagePathLarge5 = row[model.ImagePathLarge5].ToString(),
                            SubCategoryCode = model.SubCategoryCode1,
                            SubCategoryName = row[model.SubCategoryName1].ToString(),
                            NextSubCategoryCode = CheckNextSubCategory(model.SubCategoryCode1, row[model.SubCategoryName1].ToString(), row[model.SubCategoryName2].ToString(), model.ProductType),
                            NextSubCategoryName = row[model.SubCategoryName2].ToString(),
                            ProductType = model.ProductType,
                            PackageCode = CheckPackage(row[model.PackageName].ToString()),
                            PackageName = row[model.PackageName].ToString(),
                            MeasurementUnitCode = CheckFMCGMeasurementUnit(row[model.MeasurementUnitName].ToString()),
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

        [AccessPolicy(PageCode = "SHNMPRFL022")]
        public ActionResult FMCGList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0 && mp.ProductType == "FMCG").ToList();
            return View(List);
        }

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
        public ActionResult FMCGCreate(MasterFMCGCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MasterFMCGCreateEditViewModel, MasterProduct>(model);
            var product = _db.Products.Where(s => s.MasterProductCode == model.Code).FirstOrDefault();
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.ProductType = "FMCG";
            prod.Code = _generatedCode("MPR");
            if(model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductType == "FMCG");// MasterProduct.GetFMCGName(model.Name);
            try
            {
                if (model.CategoryCode != null)
                {
                    prod.CategoryCode = String.Join(",", model.CategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.CategoryCode)
                    {
                        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                        sb.Append(cat.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.CategoryName = model.CategoryName;
                    }
                    else
                    {
                        model.CategoryName = sb.ToString();
                        prod.CategoryName = model.CategoryName;
                    }
                }
                if (model.SubCategoryCode != null)
                {
                    prod.SubCategoryCode = String.Join(",", model.SubCategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.SubCategoryCode)
                    {
                        var scat = _db.SubCategories.FirstOrDefault(i => i.Code == s);// SubCategory.Get(s);
                        sb.Append(scat.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.SubCategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.SubCategoryName = model.SubCategoryName;
                    }
                    else
                    {
                        model.SubCategoryName = sb.ToString();
                        prod.SubCategoryName = model.SubCategoryName;
                    }
                }
                if (model.NextSubCategoryCode != null)
                {
                    prod.NextSubCategoryCode = String.Join(",", model.NextSubCategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.NextSubCategoryCode)
                    {
                        var nscat = _db.NextSubCategories.FirstOrDefault(i => i.Code == s);// NextSubCategory.Get(s);
                        sb.Append(nscat.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.NextSubCategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.NextSubCategoryName = model.NextSubCategoryName;
                    }
                    else
                    {
                        model.NextSubCategoryName = sb.ToString();
                        prod.NextSubCategoryName = model.NextSubCategoryName;
                    }
                }
                //ProductImage1
                if (model.FMCGLargeImage1 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage1.InputStream, prod.Code + "_" + model.FMCGLargeImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge1 = prod.Code + "_" + model.FMCGLargeImage1.FileName.Replace(" ","");
                    product.ImagePathLarge1 = prod.Code + "_" + model.FMCGLargeImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.FMCGLargeImage2 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage2.InputStream, prod.Code + "_" + model.FMCGLargeImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge2 = prod.Code + "_" + model.FMCGLargeImage2.FileName.Replace(" ", "");
                    product.ImagePathLarge2 = prod.Code + "_" + model.FMCGLargeImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.FMCGLargeImage3 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage3.InputStream, prod.Code + "_" + model.FMCGLargeImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge3 = prod.Code + "_" + model.FMCGLargeImage3.FileName.Replace(" ", "");
                    product.ImagePathLarge3 = prod.Code + "_" + model.FMCGLargeImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.FMCGLargeImage4 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage4.InputStream, prod.Code + "_" + model.FMCGLargeImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge4 = prod.Code + "_" + model.FMCGLargeImage4.FileName.Replace(" ", "");
                    product.ImagePathLarge4 = prod.Code + "_" + model.FMCGLargeImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.FMCGLargeImage5 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage5.InputStream, prod.Code + "_" + model.FMCGLargeImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge5 = prod.Code + "_" + model.FMCGLargeImage5.FileName.Replace(" ", "");
                    product.ImagePathLarge5 = prod.Code + "_" + model.FMCGLargeImage5.FileName.Replace(" ", "");
                }
                if (name == null)
                {
                    prod.DateEncoded = DateTime.Now;
                    prod.DateUpdated = DateTime.Now;
                    prod.Status = 0;
                    _db.MasterProducts.Add(prod);
                    _db.SaveChanges();
                    product.DateUpdated = DateTime.Now;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    // MasterProduct.Add(prod);
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
   
        [AccessPolicy(PageCode = "SHNMPRFE024")]
        public ActionResult FMCGEdit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Code == dCode);// MasterProduct.Get(dCode);
            if (masterProduct.ImagePathLarge1 != null)
                masterProduct.ImagePathLarge1 = masterProduct.ImagePathLarge1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePathLarge2 != null)
                masterProduct.ImagePathLarge2 = masterProduct.ImagePathLarge2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePathLarge3 != null)
                masterProduct.ImagePathLarge3 = masterProduct.ImagePathLarge3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePathLarge4 != null)
                masterProduct.ImagePathLarge4 = masterProduct.ImagePathLarge4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePathLarge5 != null)
                masterProduct.ImagePathLarge5 = masterProduct.ImagePathLarge5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            var model = _mapper.Map<MasterProduct, MasterFMCGCreateEditViewModel>(masterProduct);
            model.CategoryCode1 = masterProduct.CategoryCode;
            model.SubCategoryCode1 = masterProduct.SubCategoryCode;
            model.NextSubCategoryCode1 = masterProduct.NextSubCategoryCode;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRFE024")]
        public ActionResult FMCGEdit(MasterFMCGCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _db.MasterProducts.FirstOrDefault(i => i.Code == model.Code);// MasterProduct.Get(model.Code);
            var product = _db.Products.Where(s => s.MasterProductCode == model.Code).FirstOrDefault();
            _mapper.Map(model, prod);
            prod.Name = model.Name;
            prod.ProductType = model.ProductType;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            if (model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            try
            {
                if (model.CategoryCode != null)
                {
                    prod.CategoryCode = String.Join(",", model.CategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.CategoryCode)
                    {
                        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                        sb.Append(cat.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.CategoryName = model.CategoryName;
                    }
                    else
                    {
                        model.CategoryName = sb.ToString();
                        prod.CategoryName = model.CategoryName;
                    }
                }
                if (model.SubCategoryCode != null)
                {
                    prod.SubCategoryCode = String.Join(",", model.SubCategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.SubCategoryCode)
                    {
                        var scat = _db.SubCategories.FirstOrDefault(i => i.Code == s);// SubCategory.Get(s);
                        sb.Append(scat.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.SubCategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.SubCategoryName = model.SubCategoryName;
                    }
                    else
                    {
                        model.SubCategoryName = sb.ToString();
                        prod.SubCategoryName = model.SubCategoryName;
                    }
                }
                if (model.NextSubCategoryCode != null)
                {
                    prod.NextSubCategoryCode = String.Join(",", model.NextSubCategoryCode);
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.NextSubCategoryCode)
                    {
                        var nscat = _db.NextSubCategories.FirstOrDefault(i => i.Code == s);// NextSubCategory.Get(s);
                        sb.Append(nscat.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.NextSubCategoryName = sb.ToString().Remove(sb.Length - 1);
                        prod.NextSubCategoryName = model.NextSubCategoryName;
                    }
                    else
                    {
                        model.NextSubCategoryName = sb.ToString();
                        prod.NextSubCategoryName = model.NextSubCategoryName;
                    }
                }
                //ProductImage1
                if (model.FMCGLargeImage1 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage1.InputStream, prod.Code + "_" + model.FMCGLargeImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge1 = prod.Code + "_" + model.FMCGLargeImage1.FileName.Replace(" ", "");
                    if (product != null)
                    {
                        product.ImagePathLarge1 = prod.Code + "_" + model.FMCGLargeImage1.FileName.Replace(" ", "");
                    }
                }

                //ProductImage2
                if (model.FMCGLargeImage2 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage2.InputStream, prod.Code + "_" + model.FMCGLargeImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge2 = prod.Code + "_" + model.FMCGLargeImage2.FileName.Replace(" ", "");
                    if (product != null)
                    {
                        product.ImagePathLarge2 = prod.Code + "_" + model.FMCGLargeImage2.FileName.Replace(" ", "");
                    }
                }

                //ProductImage3
                if (model.FMCGLargeImage3 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage3.InputStream, prod.Code + "_" + model.FMCGLargeImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge3 = prod.Code + "_" + model.FMCGLargeImage3.FileName.Replace(" ", "");
                    if (product != null)
                    {
                        product.ImagePathLarge3 = prod.Code + "_" + model.FMCGLargeImage3.FileName.Replace(" ", "");
                    }
                }

                //ProductImage4
                if (model.FMCGLargeImage4 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage4.InputStream, prod.Code + "_" + model.FMCGLargeImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge4 = prod.Code + "_" + model.FMCGLargeImage4.FileName.Replace(" ", "");
                    if (product != null)
                    {
                        product.ImagePathLarge4 = prod.Code + "_" + model.FMCGLargeImage4.FileName.Replace(" ", "");
                    }
                }

                //ProductImage5
                if (model.FMCGLargeImage5 != null)
                {
                    uc.UploadFiles(model.FMCGLargeImage5.InputStream, prod.Code + "_" + model.FMCGLargeImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge5 = prod.Code + "_" + model.FMCGLargeImage5.FileName.Replace(" ", "");
                    if (product != null)
                    {
                        product.ImagePathLarge5 = prod.Code + "_" + model.FMCGLargeImage5.FileName.Replace(" ", "");
                    }
                }
                prod.DateUpdated = DateTime.Now;
                _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                if (product != null)
                {
                    product.MasterProductCode = prod.Code;
                    product.MasterProductName = prod.Name;
                    product.DateUpdated = DateTime.Now;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }

                //  MasterProduct.Edit(prod, out int error);
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

            return RedirectToAction("FMCGList");
        }

        [AccessPolicy(PageCode = "SHNMPRFD025")]
        public ActionResult FMCGDelete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Code == dCode);// MasterProduct.Get(dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("FMCGList");
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public JsonResult GetMasterProductList(string itemName)
        {
            string result = itemName.Substring(0, 3);
            var model = new List<ItemMappingViewModel.ItemMappingList>();
            model = _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(result) && a.Status == 0).Select(i => new ItemMappingViewModel.ItemMappingList
            {
                Code = i.Code,
                Name = i.Name,
                ImagePath = i.ImagePath,
                LongDescription = i.LongDescription
            }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public async Task<JsonResult> GetProductItemSelect2(string shopcode, string q = "")
        {
            var model = await _db.ProductMedicalStocks
                .Join(_db.Products , ms => ms.productid, p => p.Id, (ms, p) => new { ms, p })
                .Where(a => a.ms.SupplierName != "NA" && a.p.Name.Contains(q) && a.p.ShopCode== shopcode && a.p.Status == 0 && a.p.MasterProductCode == null && a.p.MasterProductName == null)
                .OrderBy(i => i.p.Name).Select(i => new
            {
                id = i.p.Code,
                text = i.p.Name,
                type =i.p.ProductType
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public async Task<JsonResult> GetPendingProductItemSelect2(string shopcode)
        {
            var model = await _db.ProductMedicalStocks
                .Join(_db.Products, ms => ms.productid, p => p.Id, (ms, p) => new { ms, p })
                .Where(a => a.ms.SupplierName != "NA" && a.p.ShopCode == shopcode && a.p.Status == 0 && a.p.MasterProductCode == null && a.p.MasterProductName == null)
                .OrderBy(i => i.p.Name).Select(i => new
                {
                    Code = i.p.Code,
                    Name = i.p.Name,
                    BrandName = i.p.BrandName,
                    CategoryName = i.p.CategoryName,
                    ProductType = i.p.ProductType,
                    Price = i.p.Price
                }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetMedicalBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Medical").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetMedicalCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Medical").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetDrugUnitSelect2(string q = "")
        {
            var model = await _db.MeasurementUnits.Where(a => a.UnitName.Contains(q) && a.Status == 0 && a.Type == 1).OrderBy(i => i.UnitName).Select(i => new
            {
                id = i.Code,
                text = i.UnitName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetDrugCompoundDetailSelect2(string q = "")
        {
            var model = await _db.DrugCompoundDetails.Where(a => a.AliasName.Contains(q) && a.Status == 0).OrderBy(i => i.AliasName).Select(i => new
            {
                id = i.Code,
                text = i.AliasName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetMedicalPackageSelect2(string q = "")
        {
            var model = await _db.Packages.Where(a => a.Name.Contains(q) && a.Status == 0 && a.Type == 1).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await _db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public JsonResult GetMasterItemSelect2(string q = "")
         {
            if (q != "")
            {
                var model = _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType !="Dish" ).Select(i => new
                {
                    id = i.Code,
                    text = i.Name,
                    image = i.ImagePathLarge1,
                    description = i.LongDescription,
                    brandname = i.BrandName,
                    categoryname = i.CategoryName,
                    price = i.Price,
                    type = i.ProductType
                }).Take(50).ToList();
                return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
          
           
            
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetFMCGBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType=="FMCG").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetDishCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Dish").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetFMCGPackageSelect2(string q = "")
        {
            var model = await _db.Packages.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.Type == 2).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetFMCGMeasurementUnitSelect2(string q = "")
        {
            var model = await _db.MeasurementUnits.OrderBy(i => i.UnitName).Where(a => a.UnitName.Contains(q) && a.Status == 0 && a.Type == 2).Select(i => new
            {
                id = i.Code,
                text = i.UnitName
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "FMCG").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetSubCategorySelect2(string q = "")
        {
            var model = await _db.SubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "FMCG").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync(); 

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetNextSubCategorySelect2(string q = "")
        {
            var model = await _db.NextSubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "FMCG").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
        ///// <summary>
        ///// page disposing objects
        ///// </summary>
        ///// <param name="disposing"></param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //public JsonResult AddOns(string code)
        //{
        //    var model = new MasterAddOnsCreateViewModel();
        //    model.DishLists = _db.ProductDishAddOns.Where(i => i.MasterProductCode == code && i.Status == 0).Select(i => new MasterAddOnsCreateViewModel.DishList
        //    {
        //        AddOnItemName = i.AddOnItemName,
        //        Code = i.Code,
        //        MasterProductCode = i.MasterProductCode,
        //        MasterProductName = i.MasterProductName,
        //        AddOnCategoryCode = i.AddOnCategoryCode,
        //        AddOnCategoryName = i.AddOnCategoryName,
        //        PortionCode = i.PortionCode,
        //        PortionName = i.PortionName,
        //        CrustName = i.CrustName,
        //        Price = i.PortionPrice
        //    }).ToList();
        //    return Json(model.DishLists, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult AddToAddOns(MasterAddOnsCreateViewModel model)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            if (addOns == null)
            {
                addOns = new List<MasterAddOnsCreateViewModel>();
            }
            var id = addOns.Count() + 1;
            model.Id = id;
            addOns.Add(model);
            Session["AddOns"] = addOns;

            return Json(new
            {
                PortionCode = model.PortionCode,
                PortionName = model.PortionName,
                PortionPrice = model.PortionPrice,
                AddOnCategoryCode = model.AddOnCategoryCode,
                AddOnCategoryName = model.AddOnCategoryName,
                AddOnsPrice = model.AddOnsPrice,
                MinSelectionLimit = model.MinSelectionLimit,
                MaxSelectionLimit = model.MaxSelectionLimit,
                AddOnItemName = model.AddOnItemName,
                Code = model.Code,
                CrustName = model.CrustName,
                CrustPrice = model.CrustPrice,
                AddOnType = model.AddOnType,
                MasterProductId = model.MasterProductId,
                Id = model.Id
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult RemoveFromAddOns(int id)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;

            if (addOns.Remove(addOns.SingleOrDefault(i => i.Id == id)))
            {
                this.Session["AddOns"] = addOns;
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult EditRemoveAddOns(int id, string code)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            if (addOns == null)
            {
                addOns = new List<MasterAddOnsCreateViewModel>();
            }
            if (addOns.Remove(addOns.SingleOrDefault(i => i.Id == id)))
            {
                this.Session["AddOns"] = addOns;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            if (code != "")
            {
                var addon = _db.ProductDishAddOns.FirstOrDefault(i => i.Code == code);
                addon.Status = 2;
                addon.DateUpdated = DateTime.Now;
                _db.Entry(addon).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                if (addOns.Remove(addOns.SingleOrDefault(i => i.Code == code)))
                {
                    this.Session["AddOns"] = addOns;
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDishMasterSelect2(string q = "")
        {
            var model = await _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Dish").Select(i => new
            {
                id = i.Code,
                text = i.Name,
                CategoryCode = i.CategoryCode,
                CategoryName = i.CategoryName,
                BrandCode = i.BrandCode,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                Customisation = i.Customisation,
                ColorCode = i.ColorCode,
                ImagePath = i.ImagePath,
                Price = i.Price,
                ProductType = i.ProductType,
                ImagePathLarge1 = i.ImagePathLarge1
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAddonCategorySelect2(string q = "")
        {
            var model = await _db.AddOnCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetPortionSelect2(string q = "")
        {
            var model = await _db.Portions.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }


    }
}