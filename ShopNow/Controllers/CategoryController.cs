using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExcelDataReader;
using ShopNow.Filters;
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
    //[Authorize]

    public class CategoryController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "CAT";
        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }
        public CategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Category, CategoryListViewModel.CategoryList>();

            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "SHNCATL002")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.Categories
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);
            //var user = ((Helpers.Sessions.User)Session["USER"]);
            //ViewBag.Name = user.Name;
            //var model = new CategoryListViewModel();

            //model.List = Category.GetList().AsQueryable().ProjectTo<CategoryListViewModel.CategoryList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            //return View(model);
        }

        [AccessPolicy(PageCode = "SHNCATS003")]
        public JsonResult Save(string name = "", string type = "", int orderNo = 1)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var categoryname = db.Categories.FirstOrDefault(i => i.Name == name && i.ProductType == type && i.Status == 0);//Category.GetNameType(name, type);
            if (categoryname == null)
            {
                var category = new Category();
                category.Name = name;
                category.ProductType = type;
                category.OrderNo = orderNo;
                category.CreatedBy = user.Name;
                category.UpdatedBy = user.Name;
                //string code = Category.Add(category, out int error);
                category.Code = _generatedCode;
                category.Status = 0;
                category.DateEncoded = DateTime.Now;
                category.DateUpdated = DateTime.Now;
                db.Categories.Add(category);
                db.SaveChanges();
                IsAdded = category.Code != String.Empty ? true : false;
                message = name + " Successfully Added";
            }
            else
            {
                message1 = name + " Already Exist!";
            }
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCATE004")]
        public JsonResult Edit(string code, string name, string type, int orderNo)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            Category category = db.Categories.FirstOrDefault(i => i.Code == code); //Category.Get(code);
            if (category != null)
            {
                category.Name = name;
                category.ProductType = type;
                category.OrderNo = orderNo;
                category.DateUpdated = DateTime.Now;
                category.UpdatedBy = user.Name;
                category.DateUpdated = DateTime.Now;
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //bool result = Category.Edit(category, out int error);
                message = name + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCATD005")]
        public JsonResult Delete(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var category = db.Categories.FirstOrDefault(i => i.Code == code); //Category.Get(code);
            if (category != null)
            {
                category.Status = 2;
                category.DateUpdated = DateTime.Now;
                category.UpdatedBy = user.Name;
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCATL002")]
        public async Task<JsonResult> GetListSelect2(string q = "")
        {
            var model = await db.Categories.Where(a => a.Name.Contains(q)).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // GET: Category
        [AccessPolicy(PageCode = "SHNCATI001")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNCATI001")]
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
                foreach (DataRow row in dt.Rows)
                {
                    var cat = db.Categories.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0); //Category.GetName(row[model.Name].ToString());
                    if (cat == null)
                    {
                        db.Categories.Add(new Category
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