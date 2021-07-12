using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExcelDataReader;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class DishAddOnController : Controller
    {
       
        private ShopnowchatEntities _db = new ShopnowchatEntities();
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
       
        private static string _generatedCode(string _prefix)
        {
           
                return ShopNow.Helpers.DRC.Generate(_prefix);
           
        }

        public DishAddOnController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<DishAddOn, DishAddOnListViewModel.DishAddOnList>();
                config.CreateMap<DishAddOnCreateEditViewModel, DishAddOn>();
                config.CreateMap<DishAddOn, DishAddOnCreateEditViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNDANL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DishAddOnListViewModel();

            model.List = _db.DishAddOns.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<DishAddOnListViewModel.DishAddOnList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNDANC002")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNDANC002")]
        public ActionResult Create(DishAddOnCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var dishAddOn = _mapper.Map<DishAddOnCreateEditViewModel, DishAddOn>(model);
            dishAddOn.CreatedBy= user.Name;
            dishAddOn.Code = _generatedCode("DAN");
            dishAddOn.DateEncoded = DateTime.Now;
            dishAddOn.DateUpdated = DateTime.Now;
            _db.DishAddOns.Add(dishAddOn);
            _db.SaveChanges();
            // DishAddOn.Add(dishAddOn, out int errorCode);

            return RedirectToAction("List","DishAddOn");
        }

        [AccessPolicy(PageCode = "SHNDANE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var dishAddOn = _db.DishAddOns.FirstOrDefault(i => i.Code == dCode);// DishAddOn.Get(dCode);
            var model = _mapper.Map<DishAddOn, DishAddOnCreateEditViewModel>(dishAddOn);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNDANE003")]
        public ActionResult Edit(DishAddOnCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {
                DishAddOn da = _db.DishAddOns.FirstOrDefault(i => i.Code == model.Code);//DishAddOn.Get(model.Code);
                _mapper.Map(model, da);
                da.UpdatedBy = user.Name;
                da.DateUpdated = DateTime.Now;
                da.DateUpdated = DateTime.Now;
                _db.Entry(da).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                // DishAddOn.Edit(da, out errorCode);

                return RedirectToAction("List", "DishAddOn");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SHNDAND004")]
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var dishAddOn = _db.DishAddOns.FirstOrDefault(i => i.Code == dCode);// DishAddOn.Get(dCode);
            dishAddOn.Status = 2;
            _db.Entry(dishAddOn).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("List", "DishAddOn");
        }

        [AccessPolicy(PageCode = "SHNDANI005")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDANI005")]
        public ActionResult Index(HttpPostedFileBase upload, DishAddOnViewModel model)
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
               
                foreach (DataRow row in dt.Rows)
                {
                    var mp = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.MasterProductName].ToString() && i.Status == 0); //MasterProduct.GetName(row[model.MasterProductName].ToString());
                    //var ad = AddOnCategory.GetName(row[model.AddOnCategoryName].ToString());
                    if (mp != null)
                    {
                        _db.DishAddOns.Add(new DishAddOn
                        {
                            Code = _generatedCode("DAN"),
                            Name = row[model.Name].ToString(),
                            MasterProductName = row[model.MasterProductName].ToString(),
                            MasterProductCode = CheckProduct(row[model.MasterProductName].ToString()),
                            AddOnCategoryCode = CheckAddOnCategory(row[model.AddOnCategoryName].ToString()),
                            AddOnCategoryName = row[model.AddOnCategoryName].ToString(),
                            PortionCode = CheckPortion(row[model.PortionName].ToString()),
                            PortionName = row[model.PortionName].ToString(),
                            CrustName = row[model.CrustName].ToString(),
                            Qty = 1,
                            Price = Convert.ToDouble(row[model.Price]),
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

        public string CheckProduct(string ProductName)
        {
            var master = _db.MasterProducts.FirstOrDefault(i => i.Name == ProductName && i.Status == 0);//MasterProduct.GetName(ProductName);
            return master.Code;
        }
        public string CheckAddOnCategory(string AddOnCategoryName)
        {
            var addon = _db.AddOnCategories.FirstOrDefault(i => i.Name == AddOnCategoryName && i.Status == 0);// AddOnCategory.GetName(AddOnCategoryName);
            if (addon != null)
            {
                return addon.Code;
            }
            else
            {
                AddOnCategory ad = new AddOnCategory();
                ad.Name = AddOnCategoryName;
                ad.Code = _generatedCode("CTA");
                ad.DateEncoded = DateTime.Now;
                ad.DateUpdated = DateTime.Now;
                _db.AddOnCategories.Add(ad);
                _db.SaveChanges();
               //  ad.Code = ad.Code// AddOnCategory.Add(ad, out int error);
                return ad.Code;
            }
        }
        public string CheckPortion(string PortionName)
        {
            var cust = _db.Portions.FirstOrDefault(i => i.Name == PortionName && i.Status == 0);// Portion.GetName(PortionName);
            if (cust != null)
            {
                return cust.Code;
            }
            else
            {
                Portion ct = new Portion();
                ct.Name = PortionName;
                ct.Code = _generatedCode("CST");
                ct.DateEncoded = DateTime.Now;
                ct.DateUpdated = DateTime.Now;
                _db.Portions.Add(ct);
                _db.SaveChanges();
                // ct.Code = Portion.Add(ct, out int error);
                return ct.Code;
            }
        }

        [AccessPolicy(PageCode = "SHNDANC002")]
        public async Task<JsonResult> GetCustomiseSelect2(string q = "")
        {
            var model = await _db.Portions.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDANC002")]
        public async Task<JsonResult> GetAddonCategorySelect2(string q = "")
        {
            var model = await _db.AddOnCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}