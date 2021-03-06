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

        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
     
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

        [AccessPolicy(PageCode = "SNCAPL001")]
        public ActionResult List(int customerid=0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Session["AccessList"] = null;
            var model = new AccessPolicyListViewModel();
            if (customerid != 0)
            {
                model.List = db.Pages.Where(i => i.Status == 0)
                .GroupJoin(db.AccessPolicies.Where(i => i.CustomerId == customerid && i.isAccess == true && i.Status == 0), p => p.Code, a => a.PageCode, (p, a) => new { p, a })
                            .Select(i => new AccessPolicyListViewModel.AccessPolicy
                            {
                                PageCode = i.p.Code,
                                PageName = i.p.Name,
                                IsAccess = i.a.Any() ? i.a.FirstOrDefault().isAccess : false,
                                Id = i.a.Any() ? i.a.FirstOrDefault().Id : 0,
                                CustomerId = i.a.Any() ? i.a.FirstOrDefault().CustomerId : 0,
                                CustomerName = i.a.Any() ? i.a.FirstOrDefault().CustomerName : "",
                                Status = i.p.Status,
                            }).OrderBy(i => i.PageName).ToList();
                
                    model.CustomerId = customerid;
                    model.CustomerName = db.Customers.FirstOrDefault(m => m.Id == customerid).Name;
                    int counter = 1;
                    model.List.ForEach(x => x.No = counter++);

                List<AccessPolicyViewModel> itemList = Session["AccessList"] as List<AccessPolicyViewModel>;
                foreach (var ap in model.List)
                {
                    if (itemList == null)
                    {
                        itemList = new List<AccessPolicyViewModel>();
                    }
                    AccessPolicyViewModel item = new AccessPolicyViewModel();
                    item.Id = ap.Id;
                    item.PageCode = ap.PageCode;
                    item.PageName = ap.PageName;
                    item.CustomerId = ap.CustomerId;
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
                    Id = i.Id,
                    PageCode = i.Code,
                    PageName = i.Name,
                    Status = i.Status
                }).ToList();
                int counter = 1;
                model.List.ForEach(x => x.No = counter++);
            }

            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCAPL001")]
        public ActionResult List(AccessPolicyListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shiftsessions = this.Session["AccessList"] as List<AccessPolicyViewModel>;

            foreach (var s in shiftsessions)
            {
                if (s.Id == 0 && s.IsAccess == true)
                {
                    var access = new AccessPolicy();
                    access.PageCode = s.PageCode;
                    access.PageName = s.PageName;
                    //access.StaffId = model.StaffId;
                    //access.StaffName = model.StaffName;
                    access.CustomerId = model.CustomerId;
                    access.CustomerName = model.CustomerName;
                    access.isAccess = true;
                    access.CreatedBy = user.Name;
                    access.UpdatedBy = user.Name;
                    var cust = db.Customers.Where(m => m.Id == model.CustomerId).Select(m => m.Position).FirstOrDefault();
                    if (cust != 0)
                    {
                        access.Position = cust;
                    }
                    access.Status = 0;
                    access.DateEncoded = DateTime.Now;
                    access.DateUpdated = DateTime.Now;
                    db.AccessPolicies.Add(access);
                    db.SaveChanges();
                }
            }
            model.List = db.Pages.Where(i => i.Status == 0)
               .GroupJoin(db.AccessPolicies.Where(i => i.CustomerId == model.CustomerId && i.isAccess == true), p => p.Code, a => a.PageCode, (p, a) => new { p, a })
                           .Select(i => new AccessPolicyListViewModel.AccessPolicy
                           {
                               PageCode = i.p.Code,
                               PageName = i.p.Name,
                               IsAccess = i.a.Any() ? i.a.FirstOrDefault().isAccess : false
                           }).OrderBy(i=>i.PageName).ToList();
            Session["AccessList"] = null;
            if(model.CustomerId != 0) {
                return RedirectToAction("List", new{customerid=model.CustomerId});
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCAPM002")]
        public ActionResult Manage(int StaffId=0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Session["ManageList"] = new List<AccessPolicyViewModel>();
            var model = new AccessPolicyListViewModel();
            if (StaffId == 0)
            {
                model.ManageList = db.Pages.Where(i => i.Status == 3).Select(i => new AccessPolicyListViewModel.AccessManage
                {
                    PageCode = i.Code,
                    PageName = i.Name
                }).OrderBy(i => i.PageName).ToList();
                model.StaffId = 0;
                model.StaffName = null;
            }
            else
            {
                model.ManageList = db.Pages.Where(i=> i.Status == 3)
                    .GroupJoin(db.AccessPolicies.Where(i => i.StaffId == StaffId && i.isAccess == true), p => p.Code, a => a.PageCode, (p, a) => new { p, a })
                .Select(i => new AccessPolicyListViewModel.AccessManage
                {
                    PageCode = i.p.Code,
                    PageName = i.p.Name,
                    IsAccess = i.a.Any() ? i.a.FirstOrDefault().isAccess : false,
                    Id = i.a.Any() ? i.a.FirstOrDefault().Id : 0,
                    CustomerId = i.a.Any() ? i.a.FirstOrDefault().CustomerId:0,
                    CustomerName = i.a.Any() ? i.a.FirstOrDefault().CustomerName:"",
                    Position = i.a.Any() ? i.a.FirstOrDefault().Position:0,
                    StaffId = i.a.Any() ? i.a.FirstOrDefault().StaffId:0,
                    StaffName = i.a.Any() ? i.a.FirstOrDefault().StaffName:"",
                    Status = i.a.Any() ? i.a.FirstOrDefault().Status:0
                }).ToList();

                model.StaffId = StaffId;
                model.StaffName = db.Staffs.FirstOrDefault(i => i.Id == StaffId).Name;

                List<AccessPolicyViewModel> itemList = Session["ManageList"] as List<AccessPolicyViewModel>;
                foreach (var ap in model.ManageList)
                {
                    if (ap.IsAccess == true)
                    {
                        if (itemList == null)
                        {
                            itemList = new List<AccessPolicyViewModel>();
                        }
                        AccessPolicyViewModel item = new AccessPolicyViewModel();
                        item.Id = ap.Id;
                        item.PageCode = ap.PageCode;
                        item.PageName = ap.PageName;
                        item.StaffId = ap.StaffId;
                        item.StaffName = ap.StaffName;
                        item.CustomerId = ap.CustomerId;
                        item.CustomerName = ap.CustomerName;
                        item.IsAccess = ap.IsAccess;
                        item.Position = ap.Position;
                        item.Status = ap.Status;
                        itemList.Add(item);
                    }
                }
                Session["ManageList"] = itemList;
            }
            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCAPM002")]
        public ActionResult Manage(AccessPolicyListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shiftsessions = this.Session["ManageList"] as List<AccessPolicyViewModel>;

            foreach (var s in shiftsessions)
            {
                if (s.Id == 0 && s.IsAccess == true)
                {
                    var access = new AccessPolicy();

                    access.PageCode = s.PageCode;
                    access.PageName = s.PageName;
                    access.StaffId = s.StaffId;
                    access.StaffName = s.StaffName;
                    access.CustomerId = s.StaffId;
                    access.CustomerName = s.StaffName;
                    access.isAccess = true;
                    access.CreatedBy = user.Name;
                    access.UpdatedBy = user.Name;
                    access.DateEncoded = DateTime.Now;
                    access.DateUpdated = DateTime.Now;
                    access.Position = 1;
                    access.Status = 3;
                    db.AccessPolicies.Add(access);
                    db.SaveChanges();
                }
            }
            model.ManageList = db.Pages.Where(i => i.Status == 3)
               .GroupJoin(db.AccessPolicies.Where(i => i.StaffId == model.StaffId && i.isAccess == true), p => p.Code, a => a.PageCode, (p, a) => new { p, a })
                           .Select(i => new AccessPolicyListViewModel.AccessManage
                           {
                               PageCode = i.p.Code,
                               PageName = i.p.Name,
                               IsAccess = i.a.Any() ? i.a.FirstOrDefault().isAccess : false
                           }).ToList();

            return View(model);
        }

        [AccessPolicy(PageCode = "SNCAPI003")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCAPI003")]
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
                            PageName = row[model.PageName].ToString(),
                            PageCode = row[model.PageCode].ToString(),
                            Status = model.Status,
                            Position = model.Position,
                            CustomerId = user.Id,
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

        [AccessPolicy(PageCode = "SNCAPIL004")]
        public ActionResult ItemList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.AccessPolicies
                        select s).OrderBy(s => s.PageName).Where(i => i.Status == 0 || i.Status == 3).ToList();
            return View(List);
        }

        [AccessPolicy(PageCode = "SNCAPC005")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCAPC005")]
        public ActionResult Create(AccessPolicyCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var access = _mapper.Map<AccessPolicyCreateEditViewModel, AccessPolicy>(model);
            access.CreatedBy = user.Name;
            access.UpdatedBy = user.Name;
            access.CustomerId = user.Id;
            access.CustomerName = user.Name;
            access.isAccess = true;
            access.Status = 0;
            access.DateEncoded = DateTime.Now;
            access.DateUpdated = DateTime.Now;
            db.AccessPolicies.Add(access);
            db.SaveChanges();
            return RedirectToAction("ItemList");
        }

        [AccessPolicy(PageCode = "SNCAPE006")]
        public ActionResult Edit(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var access = db.AccessPolicies.Where(m => m.Id == dCode).FirstOrDefault();
            var model = _mapper.Map<AccessPolicy, AccessPolicyCreateEditViewModel>(access);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCAPE006")]
        public ActionResult Edit(AccessPolicyCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            AccessPolicy access = db.AccessPolicies.Where(m => m.Id == model.Id).FirstOrDefault();
            _mapper.Map(model, access);
            access.DateUpdated = DateTime.Now;
            access.UpdatedBy = user.Name;
            access.isAccess = true;
            access.CustomerId = user.Id;
            access.CustomerName = user.Name;
            access.DateUpdated = DateTime.Now;
            db.Entry(access).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ItemList");
        }

        [AccessPolicy(PageCode = "SNCAPD007")]
        public JsonResult Delete(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var access = db.AccessPolicies.Where(m => m.Id == Id).FirstOrDefault();
            if (access != null)
            {
                access.Status = 2;
                access.DateUpdated = DateTime.Now;
                access.UpdatedBy = user.Name;
                db.Entry(access).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //JSON 
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
            }
            itemList.Add(model);
            Session["ManageList"] = itemList;
            return Json(new { list = itemList }, JsonRequestBehavior.AllowGet);
        }

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
                var code = itemList.FirstOrDefault(i => i.PageCode == pageCode && i.PageName == pageName).Id;
                if (code != 0)
                {
                    var ap = db.AccessPolicies.Where(m => m.Id == code).FirstOrDefault();
                    ap.isAccess = false;
                    ap.Status = 2;
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
                var code = itemList.FirstOrDefault(i => i.PageCode == pageCode && i.PageName == pageName).Id;
                if (code != 0)
                {
                    var ap = db.AccessPolicies.Where(m => m.Id == code).FirstOrDefault();
                    ap.isAccess = false;
                    ap.Status = 2;
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

        public JsonResult AddSelectAllSession(int CustomerId, string CustomerName, bool IsAccess)
        {
            List<AccessPolicyViewModel> itemList = Session["AccessList"] as List<AccessPolicyViewModel>;

            if (itemList == null)
            {
                itemList = new List<AccessPolicyViewModel>();
            }
            if (IsAccess)
            {
                var List = db.Pages.Where(i => i.Status == 0).Select(i => new AccessPolicyViewModel
                {
                    PageCode = i.Code,
                    PageName = i.Name,
                    Status = i.Status,
                    CustomerId = CustomerId,
                    CustomerName = CustomerName,
                    IsAccess = true
                }).ToList();

                //foreach (var ap in List)
                //{
                //    AccessPolicyViewModel item = new AccessPolicyViewModel();
                //    //item.Id = ap.Id;
                //    item.PageCode = ap.PageCode;
                //    item.PageName = ap.PageName;
                //    item.CustomerId = CustomerId;
                //    item.CustomerName = CustomerName;
                //    item.IsAccess = true;
                //    itemList.Add(item);
                //}
                Session["AccessList"] = List;
            }
            return Json(new { list = itemList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveSelectAllSession(AccessPolicyViewModel model)
        {
            List<AccessPolicyViewModel> itemList = Session["AccessList"] as List<AccessPolicyViewModel>;
            if (itemList == null)
            {
                itemList = new List<AccessPolicyViewModel>();
            }
            itemList.Remove(model);
            Session["AccessList"] = itemList.Remove(model);

            return Json(new { list = itemList }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetStaffSelect2(string q="")
        {
            var model = await db.Staffs.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> GetShopSelect2(string q = "")
        //{
        //    var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
        //    {
        //        id = i.Id,
        //        text = i.Name
        //    }).ToListAsync();

        //    return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        //}

        public async Task<JsonResult> GetCustomerSelect2(string q = "")
        {
            var model = await db.Customers.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Position == 4).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetPageSelect2(string q = "")
        {
            var model = await db.Pages.Where(i=>i.Status == 0 || i.Status == 3).OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

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
