using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExcelDataReader;
using ShopNow.Filters;
using ShopNow.Helpers;
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
    public class MeasurementUnitController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        

        public MeasurementUnitController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<MeasurementUnit, MeasurementUnitListViewModel.UnitList>();
                config.CreateMap<MeasurementUnitCreateEditViewModel, MeasurementUnit>();
                config.CreateMap<MeasurementUnit, MeasurementUnitCreateEditViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNDMUL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new MeasurementUnitListViewModel();

            model.List = db.MeasurementUnits.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<MeasurementUnitListViewModel.UnitList>(_mapperConfiguration).OrderBy(i => i.UnitName).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNDMUC002")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDMUC002")]
        public ActionResult Create(MeasurementUnitCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {
                var drug = _mapper.Map<MeasurementUnitCreateEditViewModel, MeasurementUnit>(model);
                drug.CreatedBy = user.Name;
                drug.UpdatedBy = user.Name;
                drug.Status = 0;
                drug.DateEncoded = DateTime.Now;
                drug.DateUpdated = DateTime.Now;
                db.MeasurementUnits.Add(drug);
                db.SaveChanges();
                // MeasurementUnit.Add(drug, out errorCode);
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SHNDMUE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            if (dCode==0)
                return HttpNotFound();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var drug = db.MeasurementUnits.FirstOrDefault(i => i.Id == dCode && i.Status == 0);// MeasurementUnit.Get(dCode);
            var model = _mapper.Map<MeasurementUnit, MeasurementUnitCreateEditViewModel>(drug);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDMUE003")]
        public ActionResult Edit(MeasurementUnitCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {
                var drug = db.MeasurementUnits.FirstOrDefault(i => i.Id == model.Id && i.Status == 0);// MeasurementUnit.Get(model.Code);
                drug.UnitName = model.UnitName;
                drug.UnitSymbol = model.UnitSymbol;
                drug.UnitType = model.UnitType;
                drug.ConversionFormula = model.ConversionFormula;
                drug.ConversionUnit = model.ConversionUnit;
                drug.Type = model.Type;
                drug.UpdatedBy = user.Name;
                drug.DateUpdated = DateTime.Now;
                db.Entry(drug).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                // MeasurementUnit.Edit(drug, out errorCode);

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SHNDMUD004")]
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var drug = db.MeasurementUnits.FirstOrDefault(i => i.Id == dCode && i.Status == 0);// MeasurementUnit.Get(dCode);
            drug.Status = 2;
            drug.UpdatedBy = user.Name;
            db.Entry(drug).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDMUI005")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDMUI005")]
        public ActionResult Index(HttpPostedFileBase upload, MeasurementUnitMasterViewModel model)
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
                    var drug = db.MeasurementUnits.FirstOrDefault(i => i.UnitName == row[model.UnitName].ToString() && i.Status == 0);// MeasurementUnit.GetName(row[model.UnitName].ToString());
                    if (drug == null)
                    {
                        db.MeasurementUnits.Add(new MeasurementUnit
                        {
                            UnitName = row[model.UnitName].ToString(),
                            Type = model.Type,
                            ConversionFormula = row[model.ConversionFormula].ToString(),
                            ConversionUnit = row[model.ConversionUnit].ToString(),
                            UnitSymbol = row[model.UnitSymbol].ToString(),
                            UnitType = row[model.UnitType].ToString(),
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

        [AccessPolicy(PageCode = "SHNDMUC002")]
        public JsonResult GetUnitName(string name)
        {
            var msu = db.MeasurementUnits.FirstOrDefault(i => i.UnitName == name.ToString() && i.Status == 0);// MeasurementUnit.GetName(name);
            if(msu != null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
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