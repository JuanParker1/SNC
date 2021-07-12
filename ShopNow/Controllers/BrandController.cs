using AutoMapper;
using ExcelDataReader;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    //[Authorize]

    public class BrandController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "BRA";
        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }
        public BrandController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Brand, BrandListViewModel.BrandList>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNBRAL004")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.Brands
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);
            //var user = ((Helpers.Sessions.User)Session["USER"]);
            //ViewBag.Name = user.Name;
            //var model = new BrandListViewModel();
            //model.List = Brand.GetList().AsQueryable().ProjectTo<BrandListViewModel.BrandList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            //return View(model);
        }

        [AccessPolicy(PageCode = "SHNBRAC001")]
        public JsonResult Save(string name = "", string type = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var brandname = db.Brands.FirstOrDefault(i => i.Name == name && i.ProductType == type && i.Status == 0); //Brand.GetNameType(name, type);
            if (brandname == null)
            {
                var brand = new Brand();
                brand.Name = name;
                brand.ProductType = type;
                brand.CreatedBy = user.Name;
                brand.UpdatedBy = user.Name;
                brand.Code = _generatedCode;
                brand.Status = 0;
                brand.DateEncoded = DateTime.Now;
                brand.DateUpdated = DateTime.Now;
                db.Brands.Add(brand);
                db.SaveChanges();
                string code = brand.Code;// Brand.Add(brand, out int error);
                IsAdded = code != String.Empty ? true : false;
                message = name + " Successfully Added";
            }
            else
            {
                message1 = name + " Already Exist!";
            }

            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNBRAE002")]
        public JsonResult Edit(string code, string name, string type)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            Brand brand = db.Brands.Where(b => b.Code == code).FirstOrDefault(); //Brand.Get(code);
            if (brand != null)
            {
                brand.Name = name;
                brand.ProductType = type;
                brand.DateUpdated = DateTime.Now;
                brand.UpdatedBy = user.Name;
                brand.DateUpdated = DateTime.Now;
                db.Entry(brand).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               // bool result = Brand.Edit(brand, out int error);
                message = name + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNBRAR005")]
        public JsonResult Delete(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var brand = db.Brands.Where(b => b.Code == code).FirstOrDefault(); //Brand.Get(code);
            if (brand != null)
            {
                brand.Status = 2;
                brand.DateUpdated = DateTime.Now;
                brand.UpdatedBy = user.Name;
                db.Entry(brand).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        // GET: Brand
        [AccessPolicy(PageCode = "SHNBRAI003")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNBRAI003")]
        public ActionResult Index(HttpPostedFileBase upload, BrandMasterViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
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
                    var brand = db.Brands.Where(b => b.Name == row[model.Name].ToString() && b.Status == 0).Select(b => b.Name).ToString(); //Brand.GetName(row[model.Name].ToString());
                    if (brand == null)
                    {
                        db.Brands.Add(new Brand
                        {
                            Name = row[model.Name].ToString(),
                            Code = _generatedCode,
                            ProductType = model.ProductType,
                            Status = 0,
                            DateEncoded = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name
                        });
                        db.SaveChanges();
                    }
                }
                
            }
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
