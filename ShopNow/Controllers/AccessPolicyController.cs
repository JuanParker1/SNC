using AutoMapper;
using ExcelDataReader;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
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
    public class AccessPolicyController : Controller
    {

        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "APG";
        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }
        public AccessPolicyController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Page, AccessPolicyListViewModel.AccessPolicy>();
                config.CreateMap<AccessPolicy, AccessPolicyListViewModel.AccessManage>();
                config.CreateMap<AccessPolicyCreateEditViewModel, AccessPolicy>();
                config.CreateMap<AccessPolicy, AccessPolicyCreateEditViewModel>();
                config.CreateMap<AccessPolicy, AccessPolicyItemListViewModel.AccessPolicy>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNAPGL001")]
        public ActionResult List(string customercode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Session["AccessList"] = null;
            var model = new AccessPolicyListViewModel();
            if (customercode != "")
            {
                model.List = db.Pages.Where(i => i.Status == 0)
                .GroupJoin(db.AccessPolicies.Where(i => i.CustomerCode == customercode && i.isAccess == true), p => p.Code, a => a.PageCode, (p, a) => new { p, a })
                            .Select(i => new AccessPolicyListViewModel.AccessPolicy
                            {
                                PageCode = i.p.Code,
                                PageName = i.p.Name,
                                IsAccess = i.a.Any() ? i.a.FirstOrDefault().isAccess : false,
                                Code = i.a.FirstOrDefault().Code,
                                CustomerCode = i.a.FirstOrDefault().CustomerCode,
                                CustomerName = i.a.FirstOrDefault().CustomerName,
                                Status = i.p.Status,
                            }).OrderBy(i => i.PageName).ToList();
                if (model.CustomerCode == null)
                {
                    model.CustomerCode = customercode;
                    model.CustomerName = db.Customers.FirstOrDefault(m => m.Code == customercode).Name;
                }
                List<AccessPolicyViewModel> itemList = Session["AccessList"] as List<AccessPolicyViewModel>;
                foreach (var ap in model.List)
                {
                    if (itemList == null)
                    {
                        itemList = new List<AccessPolicyViewModel>();
                    }
                    AccessPolicyViewModel item = new AccessPolicyViewModel();
                    item.Code = ap.Code;
                    item.PageCode = ap.PageCode;
                    item.PageName = ap.PageName;
                    item.ShopCode = ap.ShopCode;
                    item.ShopName = ap.ShopName;
                    item.StaffCode = ap.StaffCode;
                    item.StaffName = ap.StaffName;
                    item.CustomerCode = ap.CustomerCode;
                    item.CustomerName = ap.CustomerName;
                    item.IsAccess = ap.IsAccess;
                    item.Position = ap.Position;
                    item.Status = ap.Status;
                    itemList.Add(item);
                }
                Session["AccessList"] = itemList;
            }
            else
            {
                model.List = db.Pages.Where(i => i.Status == 0).Select(i => new AccessPolicyListViewModel.AccessPolicy
                {
                    Code = i.Code,
                    PageCode = i.Code,
                    PageName = i.Name,
                    Status = i.Status
                }).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNAPGL001")]
        public ActionResult List(AccessPolicyListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shiftsessions = this.Session["AccessList"] as List<AccessPolicyViewModel>;

            foreach (var s in shiftsessions)
            {
                if (s.Code == null && s.IsAccess == true)
                {
                    var access = new AccessPolicy();
                    access.PageCode = s.PageCode;
                    access.PageName = s.PageName;
                    access.ShopCode = model.ShopCode;
                    access.ShopName = model.ShopName;
                    access.StaffCode = model.StaffCode;
                    access.StaffName = model.StaffName;
                    access.CustomerCode = model.CustomerCode;
                    access.CustomerName = model.CustomerName;
                    access.isAccess = true;
                    access.CreatedBy = user.Name;
                    access.UpdatedBy = user.Name;
                    var cust = db.Customers.Where(m => m.Code == model.CustomerCode).Select(m => m.Position).FirstOrDefault();
                    if (cust != 0)
                    {
                        access.Position = cust;
                    }
                    // string shiftsessioncode = AccessPolicy.Add(access, out int errorCode);
                    access.Code = Helpers.DRC.Generate("APG");
                    access.Status = 0;
                    access.DateEncoded = DateTime.Now;
                    access.DateUpdated = DateTime.Now;
                    db.AccessPolicies.Add(access);
                    db.SaveChanges();
                }
            }
            model.List = db.Pages.Where(i => i.Status == 0)
               .GroupJoin(db.AccessPolicies.Where(i => i.CustomerCode == model.CustomerCode && i.isAccess == true), p => p.Code, a => a.PageCode, (p, a) => new { p, a })
                           .Select(i => new AccessPolicyListViewModel.AccessPolicy
                           {
                               PageCode = i.p.Code,
                               PageName = i.p.Name,
                               IsAccess = i.a.Any() ? i.a.FirstOrDefault().isAccess : false
                           }).OrderBy(i=>i.PageName).ToList();
            //model.List = Page.GetList().AsQueryable().ProjectTo<AccessPolicyListViewModel.AccessPolicy>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            return View(model);
        }

        [AccessPolicy(PageCode = "SHNAPGM002")]
        public ActionResult Manage(string shopcode="", string staffcode="")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Session["ManageList"] = new List<AccessPolicyViewModel>();
            var model = new AccessPolicyListViewModel();

            if (shopcode != "" && staffcode != "")
            {
                model.ManageList = db.Pages.Where(i => i.Status == 0)
                .GroupJoin(db.AccessPolicies.Where(i => i.ShopCode == shopcode && i.StaffCode == staffcode && i.isAccess == true), p => p.Code, a => a.PageCode, (p, a) => new { p, a })
                            .Select(i => new AccessPolicyListViewModel.AccessManage
                            {
                                PageCode = i.p.Code,
                                PageName = i.p.Name,
                                IsAccess = i.a.Any() ? i.a.FirstOrDefault().isAccess : false,
                                Code = i.a.FirstOrDefault().Code,
                                ShopCode = i.a.FirstOrDefault().ShopCode,
                                ShopName = i.a.FirstOrDefault().ShopName,
                                StaffCode = i.a.FirstOrDefault().StaffCode,
                                StaffName = i.a.FirstOrDefault().StaffName,
                                CustomerCode = i.a.FirstOrDefault().CustomerCode,
                                CustomerName = i.a.FirstOrDefault().CustomerName,
                                Status = i.p.Status,
                            }).OrderBy(i=> i.PageName).ToList();
                if (model.ShopCode == null)
                {
                    model.ShopCode = shopcode;
                    //model.ShopName = Shop.Get(shopcode).Name;
                    model.ShopName = db.Shops.Where(s => s.Code == shopcode).Select(s => s.Name).ToString();
                    model.StaffCode = staffcode;
                    model.StaffName = db.Staffs.Where(s => s.Code == shopcode).Select(s => s.Name).ToString();//Staff.Get(staffcode).Name;
                }
                List<AccessPolicyViewModel> itemList = Session["ManageList"] as List<AccessPolicyViewModel>;
                foreach (var ap in model.ManageList)
                {
                    if (itemList == null)
                    {
                        itemList = new List<AccessPolicyViewModel>();
                    }
                    AccessPolicyViewModel item = new AccessPolicyViewModel();
                    item.Code = ap.Code;
                    item.PageCode = ap.PageCode;
                    item.PageName = ap.PageName;
                    item.ShopCode = ap.ShopCode;
                    item.ShopName = ap.ShopName;
                    item.StaffCode = ap.StaffCode;
                    item.StaffName = ap.StaffName;
                    item.CustomerCode = ap.CustomerCode;
                    item.CustomerName = ap.CustomerName;
                    item.IsAccess = ap.IsAccess;
                    item.Position = ap.Position;
                    item.Status = ap.Status;
                    itemList.Add(item);
                }
                Session["ManageList"] = itemList;
            }
            else
            {
                model.ManageList = db.AccessPolicies.Where(i => i.ShopCode == shopcode && i.StaffCode == staffcode).Select(i => new AccessPolicyListViewModel.AccessManage
                {
                    Code = i.Code,
                    PageCode = i.PageCode,
                    PageName = i.PageName,
                    IsAccess = i.isAccess
                }).ToList();
            }
            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNAPGM002")]
        public ActionResult Manage(AccessPolicyListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shiftsessions = this.Session["ManageList"] as List<AccessPolicyViewModel>;

            foreach (var s in shiftsessions)
            {
                if (s.Code == null && s.IsAccess == true)
                {
                    var access = new AccessPolicy();

                    access.PageCode = s.PageCode;
                    access.PageName = s.PageName;
                    access.ShopCode = s.ShopCode;
                    access.ShopName = s.ShopName;
                    access.StaffCode = s.StaffCode;
                    access.StaffName = s.StaffName;
                    var shop = db.Shops.Where(m => m.Code == s.ShopCode).FirstOrDefault();  //Shop.Get(s.ShopCode);
                    var customer = db.Customers.Where(m => m.Code == shop.CustomerCode).FirstOrDefault(); //Customer.Get(shop.CustomerCode);
                    if (customer != null)
                    {
                        access.CustomerCode = customer.Code;
                        access.CustomerName = customer.Name;
                        access.Position = customer.Position;
                    }
                    access.isAccess = true;
                    access.CreatedBy = user.Name;
                    access.UpdatedBy = user.Name;
                    access.Status = s.Status;
                    //string shiftsessioncode = AccessPolicy.Add(access, out int errorCode);
                    access.Code = Helpers.DRC.Generate("APG");
                    access.Status = 0;
                    access.DateEncoded = DateTime.Now;
                    access.DateUpdated = DateTime.Now;
                    db.AccessPolicies.Add(access);
                    db.SaveChanges();
                }
            }
            model.ManageList = db.Pages.Where(i => i.Status == 3)
               .GroupJoin(db.AccessPolicies.Where(i => i.ShopCode == model.ShopCode && i.StaffCode == model.StaffCode && i.isAccess == true), p => p.Code, a => a.PageCode, (p, a) => new { p, a })
                           .Select(i => new AccessPolicyListViewModel.AccessManage
                           {
                               PageCode = i.p.Code,
                               PageName = i.p.Name,
                               IsAccess = i.a.Any() ? i.a.FirstOrDefault().isAccess : false
                           }).ToList();

            return RedirectToAction("Manage", "AccessPolicy");
        }

        [AccessPolicy(PageCode = "SHNAPGL001")]
        public JsonResult AddSession(AccessPolicyViewModel model)
        {
            List<AccessPolicyViewModel> itemList = Session["ManageList"] as List<AccessPolicyViewModel>;
            
            if (itemList == null)
            {
                itemList = new List<AccessPolicyViewModel>();
            }
            var isExisting = itemList.Any(i => i.PageCode == model.PageCode && i.PageName == model.PageName);
            if (isExisting)
            {
                itemList.RemoveAll(i => i.PageCode == model.PageCode && i.PageName == model.PageName);
                itemList.Add(model);
                Session["ManageList"] = itemList;
            }
            return Json(new { list = itemList }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNAPGL001")]
        public JsonResult RemoveSession(string pageCode, string pageName)
        {
            List<AccessPolicyViewModel> itemList = Session["ManageList"] as List<AccessPolicyViewModel>;
            if (itemList == null)
            {
                itemList = new List<AccessPolicyViewModel>();
            }
            var isExisting = itemList.Any(i => i.PageCode == pageCode && i.PageName == pageName);
            if (isExisting)
            {
                var code = itemList.FirstOrDefault(i => i.PageCode == pageCode && i.PageName == pageName).Code;
                if (code != null)
                {
                    var ap = db.AccessPolicies.Where(m => m.Code == code).FirstOrDefault();//AccessPolicy.Get(code);
                    ap.isAccess = false;
                    ap.Status = 2;
                    //AccessPolicy.Edit(ap, out int error);
                    ap.DateUpdated = DateTime.Now;
                    db.Entry(ap).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (itemList.Remove(itemList.FirstOrDefault(i => i.PageCode == pageCode && i.PageName == pageName)))
            {
                this.Session["ManageList"] = itemList;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNAPGM002")]
        public JsonResult AddManageSession(AccessPolicyViewModel model)
        {
            List<AccessPolicyViewModel> itemList = Session["AccessList"] as List<AccessPolicyViewModel>;

            if (itemList == null)
            {
                itemList = new List<AccessPolicyViewModel>();
            }
            var isExisting = itemList.Any(i => i.PageCode == model.PageCode && i.PageName == model.PageName);
            if (isExisting)
            {
                itemList.RemoveAll(i => i.PageCode == model.PageCode && i.PageName == model.PageName);
                itemList.Add(model);
                Session["AccessList"] = itemList;
            }
            return Json(new { list = itemList }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNAPGM002")]
        public JsonResult RemoveManageSession(string pageCode, string pageName)
        {
            List<AccessPolicyViewModel> itemList = Session["AccessList"] as List<AccessPolicyViewModel>;
            if (itemList == null)
            {
                itemList = new List<AccessPolicyViewModel>();
            }
            var isExisting = itemList.Any(i => i.PageCode == pageCode && i.PageName == pageName);
            if (isExisting)
            {
                var code = itemList.FirstOrDefault(i => i.PageCode == pageCode && i.PageName == pageName).Code;
                if (code != null)
                {
                    //var ap = AccessPolicy.Get(code);
                    //ap.IsAccess = false;
                    //ap.Status = 2;
                    //AccessPolicy.Edit(ap, out int error);
                    var ap = db.AccessPolicies.Where(m => m.Code == code).FirstOrDefault();//AccessPolicy.Get(code);
                    ap.isAccess = false;
                    ap.Status = 2;
                    //AccessPolicy.Edit(ap, out int error);
                    ap.DateUpdated = DateTime.Now;
                    db.Entry(ap).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (itemList.Remove(itemList.FirstOrDefault(i => i.PageCode == pageCode && i.PageName == pageName)))
            {
                this.Session["AccessList"] = itemList;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNAPGL001")]
        public async Task<JsonResult> GetStaffSelect2(string shopcode)
        {
            var model = await db.Staffs.OrderBy(i => i.Name).Where(a => a.ShopCode == shopcode).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNAPGL001")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNAPGL001")]
        public async Task<JsonResult> GetCustomerSelect2(string q = "")
        {
            var model = await db.Customers.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Position == 4).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNAPGC005")]
        public async Task<JsonResult> GetPageSelect2(string q = "")
        {
            var model = await db.Pages.Where(i=>i.Status == 0 || i.Status == 3).OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNAPGI003")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNAPGI003")]
        public ActionResult Index(HttpPostedFileBase upload, AccessPolicyMasterViewModel model)
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
                foreach (DataRow row in dt.Rows)
                {
                    var pagename = row[model.PageName].ToString();
                    var pagecode = row[model.PageCode].ToString();
                    if (pagename != null && pagecode != null)
                    {
                        db.AccessPolicies.Add(new AccessPolicy
                        {
                            Code = _generatedCode,
                            PageName = row[model.PageName].ToString(),
                            PageCode = row[model.PageCode].ToString(),
                            //Status = Convert.ToInt32(row[model.Status]),
                            //Position = Convert.ToInt32(row[model.Position]),
                            //Status = 0,
                            //Position = 4,
                            Status = 3,
                            Position = 1,
                            CustomerCode = user.Code,
                            CustomerName = user.Name,
                            DateEncoded = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name,
                            isAccess = true
                        });
                        db.SaveChanges();
                    }
                }
            }
            return View();
        }

        [AccessPolicy(PageCode = "SHNAPGIL004")]
        public ActionResult ItemList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.AccessPolicies
                        select s).OrderBy(s => s.PageName).Where(i => i.Status == 0 || i.Status==3).ToList();
            return View(List);
            //var user = ((Helpers.Sessions.User)Session["USER"]);
            //ViewBag.Name = user.Name;
            //var model = new AccessPolicyItemListViewModel();
            //model.List = db.AccessPolicyGroups.Where(i=> i.Status == 0 || i.Status == 3).Select(i=> new AccessPolicyItemListViewModel.AccessPolicy
            //{
            //    Code = i.Code,
            //    PageCode = i.PageCode,
            //    PageName = i.PageName,
            //    ShopCode = i.ShopCode,
            //    ShopName = i.ShopName,
            //    StaffCode = i.StaffCode,
            //    StaffName = i.StaffName,
            //    Position = i.Position,
            //    Status = i.Status
            //}).OrderBy(i => i.PageName).ToList();

            //return View(model);
        }

        [AccessPolicy(PageCode = "SHNAPGC005")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNAPGC005")]
        public ActionResult Create(AccessPolicyCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {
                var access = _mapper.Map<AccessPolicyCreateEditViewModel, AccessPolicy>(model);
                access.CreatedBy = user.Name;
                access.UpdatedBy = user.Name;
                var shop = db.Shops.Where(m => m.Code == model.ShopCode).FirstOrDefault(); //Shop.Get(model.ShopCode);
                var customer = db.Customers.Where(c => c.Code == shop.CustomerCode).FirstOrDefault(); //Customer.Get(shop.CustomerCode);
                if (customer != null)
                {
                    access.CustomerCode = customer.Code;
                    access.CustomerName = customer.Name;
                }
                access.isAccess = true;
                // access.Code = AccessPolicy.Add(access, out errorCode);
                access.Code = Helpers.DRC.Generate("APG");
                access.Status = 0;
                access.DateEncoded = DateTime.Now;
                access.DateUpdated = DateTime.Now;
                db.AccessPolicies.Add(access);
                db.SaveChanges();
                return RedirectToAction("ItemList");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SHNAPGE006")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var access = db.AccessPolicies.Where(m => m.Code == dCode).FirstOrDefault();//AccessPolicy.Get(dCode);
            var model = _mapper.Map<AccessPolicy, AccessPolicyCreateEditViewModel>(access);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNAPGE006")]
        public ActionResult Edit(AccessPolicyCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int errorCode = 0;
            try
            {
                AccessPolicy access = db.AccessPolicies.Where(m => m.Code == model.Code).FirstOrDefault();//AccessPolicy.Get(model.Code);
                _mapper.Map(model, access);
                access.DateUpdated = DateTime.Now;
                access.UpdatedBy = user.Name;
                access.isAccess = true;
                var shop = db.Shops.Where(s => s.Code == model.ShopCode).FirstOrDefault(); //Shop.Get(model.ShopCode);
                var customer = db.Customers.Where(c => c.Code == shop.CustomerCode).FirstOrDefault(); //Customer.Get(shop.CustomerCode);
                if (customer != null)
                {
                    access.CustomerCode = customer.Code;
                    access.CustomerName = customer.Name;
                }
                // bool success = AccessPolicy.Edit(access, out errorCode);
                access.DateUpdated = DateTime.Now;
                db.Entry(access).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ItemList");
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error Code: " + errorCode);
            }
        }

        [AccessPolicy(PageCode = "SHNAPGD007")]
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var access = db.AccessPolicies.Where(m => m.Code == dCode).FirstOrDefault(); //AccessPolicy.Get(dCode);
            access.Status = 2;
            access.DateUpdated = DateTime.Now;
            access.UpdatedBy = user.Name;
            db.Entry(access).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ItemList");
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
