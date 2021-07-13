﻿using AutoMapper;
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
    public class ProductSpecificationController : Controller
    {
        private ShopnowchatEntities _db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "PSF";
        private static string _generatedCode(string _prefix)
        {
    
                return ShopNow.Helpers.DRC.Generate(_prefix);
            
        }

        public ProductSpecificationController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductSpecification, ProductSpecificationListViewModel.ProductSpecificationList>();
                config.CreateMap<ProductSpecificationCreateEditViewModel, ProductSpecification>();
                config.CreateMap<ProductSpecification, ProductSpecificationCreateEditViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNPSFL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ProductSpecificationListViewModel();

            model.List = _db.ProductSpecifications.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<ProductSpecificationListViewModel.ProductSpecificationList>(_mapperConfiguration).OrderBy(i => i.MasterProductName).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPSFC002")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNPSFC002")]
        public ActionResult Create(ProductSpecificationCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            int errorCode = 0;
            try
            {
                var ps = _mapper.Map<ProductSpecificationCreateEditViewModel, ProductSpecification>(model);
                ps.CreatedBy = user.Name;
                ps.UpdatedBy = user.Name;
                // ps.Code = ProductSpecification.Add(ps, out errorCode);
                ps.Code = _generatedCode("PSF");
                ps.Status = 0;
                ps.DateEncoded = DateTime.Now;
                ps.DateUpdated = DateTime.Now;
                _db.ProductSpecifications.Add(ps);
                _db.SaveChanges();
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SHNPSFE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var ps = _db.ProductSpecifications.FirstOrDefault(i => i.Code == dCode && i.Status == 0);// ProductSpecification.Get(dCode);
            var model = _mapper.Map<ProductSpecification, ProductSpecificationCreateEditViewModel>(ps);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNPSFE003")]
        public ActionResult Edit(ProductSpecificationCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            int errorCode = 0;
            try
            {
                ProductSpecification ps = _db.ProductSpecifications.FirstOrDefault(i => i.Code == model.Code && i.Status == 0);// ProductSpecification.Get(model.Code);
                _mapper.Map(model, ps);
                ps.UpdatedBy = user.Name;
                ps.DateUpdated = DateTime.Now;
                ps.DateUpdated = DateTime.Now;
                _db.Entry(ps).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                //ProductSpecification.Edit(ps, out errorCode);

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SHNPSFD004")]
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var ps = _db.ProductSpecifications.FirstOrDefault(i => i.Code == dCode && i.Status == 0);// ProductSpecification.Get(dCode);
            ps.Status = 2;
            _db.Entry(ps).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNPSFI005")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNPSFI005")]
        public ActionResult Index(HttpPostedFileBase upload, ProductSpecificationViewModel model)
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
                    var mp = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.MasterProductName].ToString() && i.Status == 0);// MasterProduct.GetName(row[model.MasterProductName].ToString());
                    var sp = _db.Specifications.FirstOrDefault(i => i.Name == row[model.SpecificationName].ToString() && i.Status == 0);// Specification.GetName(row[model.SpecificationName].ToString());
                    if (mp != null && sp != null)
                    {
                        _db.ProductSpecifications.Add(new ProductSpecification
                        {
                            Code = _generatedCode("PSF"),
                            MasterProductName = row[model.MasterProductName].ToString(),
                            MasterProductCode = CheckProduct(row[model.MasterProductName].ToString()),
                            SpecificationCode = CheckSpecification(row[model.SpecificationName].ToString()),
                            SpecificationName = row[model.SpecificationName].ToString(),
                            Value = row[model.Value].ToString(),
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

        public string CheckProduct(string ProductName)
        {
            var master = _db.MasterProducts.FirstOrDefault(i => i.Name == ProductName && i.Status == 0); ;// MasterProduct.GetName(ProductName);
            return master.Code;
        }
        public string CheckSpecification(string SpecificationName)
        {
            var specification = _db.Specifications.FirstOrDefault(i => i.Name == SpecificationName && i.Status == 0); ;// Specification.GetName(SpecificationName);
            if (specification != null)
            {
                return specification.Code;
            }
            else
            {
                Specification sp = new Specification();
                sp.Name = SpecificationName;
                sp.Code = _generatedCode("SPF");
                sp.Status = 0;
                sp.DateEncoded = DateTime.Now;
                sp.DateUpdated = DateTime.Now;
                _db.Specifications.Add(sp);
                _db.SaveChanges();
                return sp.Code;
            }
        }

        [AccessPolicy(PageCode = "SHNPSFC002")]
        public async Task<JsonResult> GetMasterProductSelect2(string q = "")
        {
            var model = await _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Product").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPSFC002")]
        public async Task<JsonResult> GetSpecificationSelect2(string q = "")
        {
            var model = await _db.Specifications.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

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