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
    public class ShopCategoryController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public ShopCategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<ShopCategory, ShopCategoryListViewModel.ShopCategoryList>();

            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "SHNSCTI001")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSCTI001")]
        public ActionResult Index(HttpPostedFileBase upload, ShopCategoryMasterViewModel model)
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
                    var shopCategory = db.ShopCategories.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);// ShopCategory.GetName(row[model.Name].ToString());
                    if (shopCategory == null)
                    {
                        db.ShopCategories.Add(new ShopCategory
                        {
                            Name = row[model.Name].ToString(),
                            Id = Convert.ToInt32(row[model.Id]),
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

        [AccessPolicy(PageCode = "SHNSCTL004")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ShopCategoryListViewModel();
            model.List = db.ShopCategories.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<ShopCategoryListViewModel.ShopCategoryList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNSCTS002")]
        public JsonResult Save(string name = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            int count = 0;
            var shopCategoryName = db.ShopCategories.FirstOrDefault(i => i.Name == name && i.Status == 0);// ShopCategory.GetName(name);
            count = db.ShopCategories.Count();
            if (shopCategoryName == null)
            {
                var shopCategory = new ShopCategory();
                shopCategory.Name = name;
                shopCategory.Id = count;
                shopCategory.CreatedBy = user.Name;
                shopCategory.UpdatedBy = user.Name;
                //string code = ShopCategory.Add(shopCategory, out int error);
                shopCategory.DateEncoded = DateTime.Now;
                shopCategory.DateUpdated = DateTime.Now;
                db.ShopCategories.Add(shopCategory);
                db.SaveChanges();
                IsAdded = shopCategory.Id != 0 ? true : false;
                message = name + " Successfully Added";
            }
            else
            {
                message1 = name + " Already Exist!";
            }
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSCTE003")]
        public JsonResult Edit(int code, string name)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            ShopCategory shopCategory = db.ShopCategories.FirstOrDefault(i => i.Id == code);// ShopCategory.Get(code);
            if (shopCategory != null)
            {
                shopCategory.Name = name;
                shopCategory.DateUpdated = DateTime.Now;
                shopCategory.UpdatedBy = user.Name;
                // bool result = ShopCategory.Edit(shopCategory, out int error);
                shopCategory.DateUpdated = DateTime.Now;
                db.Entry(shopCategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                message = name + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSCTD005")]
        public JsonResult Delete(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shopCategory = db.ShopCategories.FirstOrDefault(i => i.Id == code);// ShopCategory.Get(code);
            if (shopCategory != null)
            {
                shopCategory.Status = 2;
                shopCategory.DateUpdated = DateTime.Now;
                shopCategory.UpdatedBy = user.Name;
                db.Entry(shopCategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
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