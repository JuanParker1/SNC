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
    public class AddOnCategoryController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "CTA";
        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }
        public AddOnCategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<AddOnCategory, AddOnCategoryListViewModel.AddOnCategoryList>();

            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "SHNCTAL002")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            //var List = db.AddOnCategories.SqlQuery("Select Name from AddOnCategories order by Name").ToList();
            var model = new AddOnCategoryListViewModel();
            model.List = db.AddOnCategories.Where(i => i.Status == 0).Select(i => new AddOnCategoryListViewModel.AddOnCategoryList
            {
                Id = i.Id,
                Name = i.Name
            }).OrderBy(i => i.Name).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCTAS003")]
        public JsonResult Save(string name = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var addOnCategoryName = db.AddOnCategories.Where(i=> i.Name== name).FirstOrDefault();
            if (addOnCategoryName == null)
            {
                var addOnCategory = new AddOnCategory();
                addOnCategory.Name = name;
                addOnCategory.CreatedBy = user.Name;
                addOnCategory.UpdatedBy = user.Name;
                addOnCategory.DateEncoded = DateTime.Now;
                addOnCategory.DateUpdated = DateTime.Now;
                db.AddOnCategories.Add(addOnCategory);
                db.SaveChanges();

                IsAdded = addOnCategory.Id != 0 ? true : false;
                message = name + " Successfully Added";
            }
            else
            {
                message1 = name + " Already Exist!";
            }

            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCTAE004")]
        public JsonResult Edit(int code, string name)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            AddOnCategory addOnCategory = db.AddOnCategories.Where(i => i.Id == code).FirstOrDefault();
            if (addOnCategory != null)
            {
                addOnCategory.Name = name;
                addOnCategory.DateUpdated = DateTime.Now;
                addOnCategory.UpdatedBy = user.Name;
                addOnCategory.DateUpdated = DateTime.Now;
                db.Entry(addOnCategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               // bool result = AddOnCategory.Edit(addOnCategory, out int error);
                message = name + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCTAD005")]
        public JsonResult Delete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var addOnCategory = db.AddOnCategories.Where(i=>i.Id ==id).FirstOrDefault();
            if (addOnCategory != null)
            {
                addOnCategory.Status = 2;
                addOnCategory.DateUpdated = DateTime.Now;
                addOnCategory.UpdatedBy = user.Name;
                db.Entry(addOnCategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCTAI001")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNCTAI001")]
        public ActionResult Index(HttpPostedFileBase upload, AddOnCategoryMasterViewModel model)
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
                    var cat = db.AddOnCategories.Where(i => i.Name == row[model.Name].ToString());
                    if (cat == null)
                    {
                        db.AddOnCategories.Add(new AddOnCategory
                        {
                            Name = row[model.Name].ToString(),
                         //   Code = _generatedCode,
                            Status = 0,
                            DateEncoded = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name
                        });
                    }
                    db.SaveChanges();
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