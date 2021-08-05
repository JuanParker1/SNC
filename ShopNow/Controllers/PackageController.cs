using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class PackageController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public PackageController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Package, PackageListViewModel.PackageList>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNMPKL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PackageListViewModel();
            model.List = db.Packages.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<PackageListViewModel.PackageList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNMPKS002")]
        public JsonResult Save(string name)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var package = db.Packages.FirstOrDefault(i => i.Name == name && i.Status == 0);
            if (package == null)
            {
                var medicalPackage = new Package();
                medicalPackage.Name = name;
                medicalPackage.CreatedBy = user.Name;
                medicalPackage.UpdatedBy = user.Name;

                medicalPackage.Status = 0;
                medicalPackage.DateEncoded = DateTime.Now;
                medicalPackage.DateUpdated = DateTime.Now;
                db.Packages.Add(medicalPackage);
                db.SaveChanges();
                IsAdded = medicalPackage.Id != 0 ? true : false;
                message = name + " Successfully Added";
            }
            else
            {
                message1 = name + " Already Exist!";
            }
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPKE003")]
        public JsonResult Edit(int id, string name)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            Package medicalPackage = db.Packages.FirstOrDefault(i => i.Id == id && i.Status == 0);
            {
                medicalPackage.Name = name;
                medicalPackage.DateUpdated = DateTime.Now;
                medicalPackage.UpdatedBy = user.Name;
                medicalPackage.DateUpdated = DateTime.Now;
                db.Entry(medicalPackage).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                message = name + " Updated Successfully";
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPKD004")]
        public JsonResult Delete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var medicalPackage = db.Packages.FirstOrDefault(i => i.Id == id && i.Status == 0);
            if (medicalPackage != null)
            {
                medicalPackage.Status = 2;
                medicalPackage.DateUpdated = DateTime.Now;
                medicalPackage.UpdatedBy = user.Name;
                db.Entry(medicalPackage).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPKI005")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPKI005")]
        public ActionResult Index(HttpPostedFileBase upload, PackageMasterViewModel model)
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
                    var medicalPackage = db.Packages.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);
                    if (medicalPackage == null)
                    {
                        db.Packages.Add(new Package
                        {
                            Name = row[model.Name].ToString(),
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