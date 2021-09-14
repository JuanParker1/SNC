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
    public class ProductDishAddOnController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        // GET: ProductDishAddOn

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
        public ActionResult Index(HttpPostedFileBase upload, ProductDishAddOnViewModel model)
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

                List<ProductDishAddOn> ProductDishAddOnList = new List<ProductDishAddOn>();
                List<MasterProduct> MasterProductList = new List<MasterProduct>();
                var master = db.ProductDishAddOns.Where(i => i.Status == 0).Select(i => new { Name = i.AddOnItemName }).ToList();
                foreach (DataRow row in dt.Rows)
                {
                    var mpname = row[model.MasterProductName].ToString().Trim();
                    var masterproduct = db.MasterProducts.Where(i => i.Status == 0).Select(i => new { Name = mpname }).ToList();
                    if (masterproduct == null)
                    {
                        if (row[model.AddOnItemName].ToString().Trim() != string.Empty)
                        {
                            int idx = master.FindIndex(a => a.Name == row[model.AddOnItemName].ToString().Trim());
                            if (idx <= 0)
                            {
                                ProductDishAddOnList.Add(new ProductDishAddOn
                                {
                                    AddOnItemName = row[model.AddOnItemName].ToString(),
                                    MasterProductId = CheckMasterProduct(row[model.MasterProductName].ToString().Trim()),
                                    MasterProductName = row[model.MasterProductName].ToString().Trim(),
                                    AddOnCategoryId = CheckAddOnCategory(row[model.AddOnCategoryName].ToString()),
                                    AddOnCategoryName = row[model.AddOnCategoryName].ToString(),
                                    PortionId = CheckPortion(row[model.PortionName].ToString()),
                                    PortionName = row[model.PortionName].ToString(),
                                    PortionPrice = Convert.ToDouble(row[model.PortionPrice]),
                                    AddOnsPrice = Convert.ToDouble(row[model.AddOnsPrice]),
                                    CrustPrice = Convert.ToDouble(row[model.CrustPrice]),
                                    AddOnType = Convert.ToInt32(row[model.AddOnType]),
                                    CrustName = CheckCrust(row[model.CrustName].ToString()),
                                    MaxSelectionLimit = Convert.ToInt32(row[model.MaxSelectionLimit]),
                                    MinSelectionLimit = Convert.ToInt32(row[model.MinSelectionLimit]),
                                    Status = 0,
                                    DateEncoded = DateTime.Now,
                                    DateUpdated = DateTime.Now,
                                    CreatedBy = user.Name,
                                    UpdatedBy = user.Name
                                });
                            }
                        }
                    }
                }
                db.BulkInsert(ProductDishAddOnList);
            }
            return View();
        }

        public long CheckMasterProduct(string MasterProductName)
        {
            var master = db.MasterProducts.FirstOrDefault(i => i.Name == MasterProductName && i.Status == 0 && i.ProductTypeId == 1);
            if (master != null)
            {
                return master.Id;
            }
            else
            {
                MasterProduct mp = new MasterProduct();
                mp.Name = MasterProductName;
                mp.NickName = MasterProductName;
                mp.ProductTypeId = 1;
                mp.ProductTypeName = "Dish";
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                db.MasterProducts.Add(mp);
                db.SaveChanges();
                return mp.Id;
            }
        }

        public int CheckAddOnCategory(string AddOnCategoryName)
        {
            var master = db.AddOnCategories.FirstOrDefault(i => i.Name == AddOnCategoryName && i.Status == 0);
            if (master != null)
            {
                return master.Id;
            }
            else
            {
                AddOnCategory mp = new AddOnCategory();
                mp.Name = AddOnCategoryName;
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                db.AddOnCategories.Add(mp);
                db.SaveChanges();
                return mp.Id;
            }
        }

        public int CheckPortion(string PortionName)
        {
            var master = db.Portions.FirstOrDefault(i => i.Name == PortionName && i.Status == 0);
            if (master != null)
            {
                return master.Id;
            }
            else
            {
                Portion mp = new Portion();
                mp.Name = PortionName;
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                db.Portions.Add(mp);
                db.SaveChanges();
                return mp.Id;
            }
        }

        public string CheckCrust(string CrustName)
        {
            var master = db.Crusts.FirstOrDefault(i => i.Name == CrustName && i.Status == 0);
            if (master != null)
            {
                return master.Name;
            }
            else
            {
                Crust mp = new Crust();
                mp.Name = CrustName;
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                db.Crusts.Add(mp);
                db.SaveChanges();
                return mp.Name;
            }
        }
    }
}