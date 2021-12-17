using AutoMapper;
using ExcelDataReader;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class PageController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "PGE";
        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }

        public PageController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Page, PageListViewModel.PageList>();
                config.CreateMap<PageCreateEditViewModel, Page>();
                config.CreateMap<Page, PageCreateEditViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCPGL177")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.Pages
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);
       
        }

        [AccessPolicy(PageCode = "SNCPGC178")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCPGC178")]
        public ActionResult Create(PageCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {
                var page = _mapper.Map<PageCreateEditViewModel, Page>(model);
                page.Code = model.Code.ToUpper();
                page.Name = model.Name.ToUpper();
                page.CreatedBy = user.Name;
                page.UpdatedBy = user.Name;
                page.DateEncoded = DateTime.Now;
                page.DateUpdated = DateTime.Now;
                db.Pages.Add(page);
                db.SaveChanges();
                // page.Code = Page.Add(page, out errorCode);
                return RedirectToAction("List","Page");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SNCPGE179")]
        public ActionResult Edit(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var page = db.Pages.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Page, PageCreateEditViewModel>(page);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCPGE179")]
        public ActionResult Edit(PageCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {
                var page = db.Pages.FirstOrDefault(i => i.Code == model.Code);
                page.Name = model.Name.ToUpper();
                page.Status = model.Status;
                page.DateUpdated = DateTime.Now;
                page.UpdatedBy = user.Name;
                page.DateUpdated = DateTime.Now;
                db.Entry(page).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List","Page");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SNCPGD180")]
        public JsonResult Delete(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var page = db.Pages.FirstOrDefault(i => i.Id == dCode);
            if (page != null)
            {
                page.Status = 2;
                page.DateUpdated = DateTime.Now;
                page.UpdatedBy = user.Name;
                db.Entry(page).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCPGI181")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCPGI181")]
        public ActionResult Index(HttpPostedFileBase upload, PageMasterViewModel model)
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
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();
                          
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                var master = db.Pages.Where(i => i.Status == 0).Select(i => new { Name = i.Name }).ToList();
                foreach (DataRow row in dt.Rows)
                {
                    if (row[model.Name].ToString().Trim() != string.Empty)
                    {
                        int idx = master.FindIndex(a => a.Name == row[model.Name].ToString().Trim());
                        if (idx <= 0)
                        {
                            db.Pages.Add(new Page
                            {
                                Name = row[model.Name].ToString().ToUpper(),
                                Code = row[model.Code].ToString().ToUpper(),
                                Status = model.Status,
                                DateEncoded = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                CreatedBy = user.Name,
                                UpdatedBy = user.Name
                            });
                            db.SaveChanges();
                        }
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