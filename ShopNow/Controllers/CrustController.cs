using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExcelDataReader;
using ShopNow.Filters;
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
    public class CrustController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public CrustController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Crust, CrustListViewModel.ListItem>();

            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CrustListViewModel();

            model.List = db.Crusts.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<CrustListViewModel.ListItem>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Save(string name = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var crustName = db.Crusts.FirstOrDefault(i => i.Name == name && i.Status == 0);
            if (crustName == null)
            {
                var crust = new Crust();
                crust.Name = name;
                crust.CreatedBy = user.Name;
                crust.UpdatedBy = user.Name;
                crust.Status = 0;
                crust.DateEncoded = DateTime.Now;
                crust.DateUpdated = DateTime.Now;
                db.Crusts.Add(crust);
                db.SaveChanges();

                IsAdded = crust.Id != 0 ? true : false;
                message = name + " Successfully Added";
            }
            else
            {
                message1 = name + " Already Exist!";
            }

            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Edit(int Id, string name)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            Crust crust = db.Crusts.FirstOrDefault(i => i.Id == Id);
            if (crust != null)
            {
                crust.Name = name;
                crust.DateUpdated = DateTime.Now;
                crust.UpdatedBy = user.Name;
                db.Entry(crust).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                message = name + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var crust = db.Crusts.FirstOrDefault(i => i.Id == Id);
            if (crust != null)
            {
                crust.Status = 2;
                crust.DateUpdated = DateTime.Now;
                crust.UpdatedBy = user.Name;
                db.Entry(crust).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult Index(HttpPostedFileBase upload, CrustMasterViewModel model)
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

                List<Crust> CrustList = new List<Crust>();
                var master = db.Crusts.Where(i => i.Status == 0).Select(i => new { Name = i.Name }).ToList();
                foreach (DataRow row in dt.Rows)
                {
                    if (row[model.Name].ToString().Trim() != string.Empty)
                    {
                        int idx = master.FindIndex(a => a.Name == row[model.Name].ToString().Trim());
                        if (idx <= 0)
                        {
                            CrustList.Add(new Crust
                            {
                                Name = row[model.Name].ToString(),
                                Status = 0,
                                DateEncoded = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                CreatedBy = user.Name,
                                UpdatedBy = user.Name
                            });
                        }
                    }
                }
                db.BulkInsert(CrustList);
            }
            return View();
        }
    }
}