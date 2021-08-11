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
    public class DrugCompoundDetailController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
       
        public DrugCompoundDetailController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<DrugCompoundDetail, DrugCompoundDetailListViewModel.DrugCompoundList>();
                config.CreateMap<DrugCompoundDetailCreateEditViewModel, DrugCompoundDetail>();
                config.CreateMap<DrugCompoundDetail, DrugCompoundDetailCreateEditViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNDCDL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DrugCompoundDetailListViewModel();

            model.List = db.DrugCompoundDetails.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<DrugCompoundDetailListViewModel.DrugCompoundList>(_mapperConfiguration).OrderBy(i => i.AliasName).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNDCDC002")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDCDC002")]
        public ActionResult Create(DrugCompoundDetailCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            try
            {
                var drug = _mapper.Map<DrugCompoundDetailCreateEditViewModel, DrugCompoundDetail>(model);
                drug.CreatedBy = user.Name;
                drug.UpdatedBy = user.Name;
                drug.Status = 0;
                drug.DateEncoded = DateTime.Now;
                drug.DateUpdated = DateTime.Now;
                db.DrugCompoundDetails.Add(drug);
                db.SaveChanges();
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error: " + ex);
            }
        }

        [AccessPolicy(PageCode = "SHNDCDE003")]
        public ActionResult Edit(string Id)
        {
            var dId = AdminHelpers.DCodeInt(Id);
            if (dId==0)
                return HttpNotFound();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var drug = db.DrugCompoundDetails.FirstOrDefault(i => i.Id == dId && i.Status == 0);
            var model = _mapper.Map<DrugCompoundDetail, DrugCompoundDetailCreateEditViewModel>(drug);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDCDE003")]
        public ActionResult Edit(DrugCompoundDetailCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            try
            {
                DrugCompoundDetail drug = db.DrugCompoundDetails.FirstOrDefault(i => i.Id == model.Id && i.Status == 0);
                _mapper.Map(model, drug);
                drug.UpdatedBy = user.Name;
                drug.DateUpdated = DateTime.Now;
                
                db.Entry(drug).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error: " + ex);
            }
        }

        [AccessPolicy(PageCode = "SHNDCDD004")]
        public ActionResult Delete(string Id)
        {
            var dId = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var drug = db.DrugCompoundDetails.FirstOrDefault(i => i.Id == dId && i.Status == 0);
            drug.Status = 2;
            drug.UpdatedBy = user.Name;
            db.Entry(drug).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDCDI005")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDCDI005")]
        public ActionResult Index(HttpPostedFileBase upload, DrugCompoundDetailMasterViewModel model)
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
                    var drug = db.DrugCompoundDetails.FirstOrDefault(i => i.AliasName == row[model.AliasName].ToString() && i.Status == 0);// DrugCompoundDetail.GetName(row[model.AliasName].ToString());
                    if (drug == null)
                    {
                        db.DrugCompoundDetails.Add(new DrugCompoundDetail
                        {
                            AliasName = row[model.AliasName].ToString(),                     
                            IndicationTreatmentAgeGroup = row[model.IndicationTreatmentAgeGroup].ToString(),
                            IntakeContraindication = row[model.IntakeContraindication].ToString(),
                            IntakeIndications = row[model.IntakeIndications].ToString(),
                            InteractingCompounds = row[model.InteractingCompounds].ToString(),
                            Alcohol = Convert.ToBoolean(row[model.Alcohol]),
                            Breastfeeding = Convert.ToBoolean(row[model.Breastfeeding]),
                            Driving = Convert.ToBoolean(row[model.Driving]),
                            Pregnancy = Convert.ToBoolean(row[model.Pregnancy]),
                            CompoundUsageReasons = row[model.CompoundUsageReasons].ToString(),
                            MechanismOfAction = row[model.MechanismOfAction].ToString(),
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