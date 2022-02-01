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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    //[Authorize]

    public class CategoryController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        UploadContent uc = new UploadContent();
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
        public CategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Category, CategoryListViewModel.CategoryList>();

            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "SNCCAL091")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.Categories
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);

        }

        [AccessPolicy(PageCode = "SNCCAS092")]
        public ActionResult Save(CategoryCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            //bool IsAdded = false;
            //string message = "";
            //string message1 = "";
            Category category = new Category();
            var categoryname = db.Categories.FirstOrDefault(i => i.Name == model.Name && i.ProductTypeId == model.ProductTypeId && i.Status == 0);
            if (categoryname == null)
            {
                category.CreatedBy = user.Name;
                category.UpdatedBy = user.Name;
                category.Status = 0;
                category.DateEncoded = DateTime.Now;
                category.DateUpdated = DateTime.Now;
                category.Name = model.Name;
                category.OrderNo = model.OrderNo;
                category.ProductTypeId = model.ProductTypeId;
                category.ProductTypeName = model.ProductTypeName;
                try
                {
                    if (model.CategoryImage != null)
                    {
                        uc.UploadFiles(model.CategoryImage.InputStream, model.CategoryImage.FileName, accesskey, secretkey, "image");
                        category.ImagePath = model.CategoryImage.FileName.Replace(" ", "");
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
                db.Categories.Add(category);
                db.SaveChanges();
                //IsAdded = category.Id != 0 ? true : false;
                //message = category.Name + " Successfully Added";
            }
            //else
            //{
            //    message1 = category.Name + " Already Exist!";
            //}
            // return View(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SNCCAE093")]
        public ActionResult Edit(CategoryEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Category category = db.Categories.FirstOrDefault(i => i.Id == model.Id);
            if (category != null)
            {
                category.Name = model.editName;
                category.OrderNo = model.editOrderNo;
                category.UpdatedBy = user.Name;
                category.DateUpdated = DateTime.Now;
                try
                {
                    if (model.editCategoryImage != null)
                    {
                        uc.UploadFiles(model.editCategoryImage.InputStream, model.editCategoryImage.FileName, accesskey, secretkey, "image");
                        category.ImagePath = model.editCategoryImage.FileName.Replace(" ", "");
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
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SNCCAD094")]
        public JsonResult Delete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var category = db.Categories.FirstOrDefault(i => i.Id == id);
            if (category != null)
            {
                category.Status = 2;
                category.DateUpdated = DateTime.Now;
                category.UpdatedBy = user.Name;
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // GET: Category
        [AccessPolicy(PageCode = "SNCCAI095")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCCAI095")]
        public ActionResult Index(HttpPostedFileBase upload, CategoryMasterViewModel model)
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

                List<Category> CategoryList = new List<Category>();
                var master = db.Categories.Where(i => i.Status == 0).Select(i => new { Name = i.Name }).ToList();
                foreach (DataRow row in dt.Rows)
                {
                    if (row[model.Name].ToString().Trim() != string.Empty)
                    {
                        int idx = master.FindIndex(a => a.Name == row[model.Name].ToString().Trim());
                        if (idx <= 0)
                        {
                            CategoryList.Add(new Category
                            {
                                Name = row[model.Name].ToString(),
                                ProductTypeId = model.ProductTypeId,
                                ProductTypeName = model.ProductTypeName,
                                OrderNo = 0,
                                Status = 0,
                                DateEncoded = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                CreatedBy = user.Name,
                                UpdatedBy = user.Name
                            });
                        }
                    }
                }
                db.BulkInsert(CategoryList);
            }
            return View();
        }

        public async Task<JsonResult> GetListSelect2(string q = "")
        {
            var model = await db.Categories.Where(a => a.Name.Contains(q)).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
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