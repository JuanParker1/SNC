using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class CustomerController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        UploadContent uc = new UploadContent();

        public CustomerController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Customer, CustomerListViewModel.CustomerList>();
                config.CreateMap<Customer, CustomerDetailsViewModel>();
                config.CreateMap<CustomerEditViewModel, Customer>();
                config.CreateMap<Customer, CustomerEditViewModel>();
                config.CreateMap<Customer, CustomerDetailsViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNCUSL001")]
        public ActionResult List()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CustomerListViewModel();
            model.List = db.Customers.Where(i => i.Status == 0).AsEnumerable().Select((i, index) => new CustomerListViewModel.CustomerList
            {
                No = index + 1,
                Id = i.Id,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                Address = i.Address,
                DistrictName = i.DistrictName,
                StateName = i.StateName,
                DateEncoded = i.DateEncoded
            }).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCUSL001")]
        public ActionResult AadharPending()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CustomerListViewModel();
            model.List = db.Customers.Where(i => i.Status == 0 && i.ImageAadharPath != null && i.ImageAadharPath != "NULL" && i.ImageAadharPath != "Rejected" && i.AadharVerify == false).Select(i => new CustomerListViewModel.CustomerList
            {
                Id = i.Id,
                Name = i.Name,
                Address = i.Address,
                DistrictName = i.DistrictName,
                PhoneNumber = i.PhoneNumber,
                StateName = i.StateName
            }).OrderBy(i => i.Name).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCUSD002")]
        public ActionResult Details(string id)
        {
            var dId = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var customer = db.Customers.Where(m => m.Id == dId).FirstOrDefault();
            var model = _mapper.Map<Customer, CustomerDetailsViewModel>(customer);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCUSE003")]
        public ActionResult Edit(string id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dId = AdminHelpers.DCodeInt(id);
            if (dId == 0)
                return HttpNotFound();
            var customer = db.Customers.Where(m => m.Id == dId).FirstOrDefault();
            var model = _mapper.Map<Customer, CustomerEditViewModel>(customer);
            model.DOB = customer.DOB != null ? customer.DOB.Value.ToString("dd-MM-yyyy") : "N/A";
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCUSIA005")]
        public ActionResult InActive(int code)
        {
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();
            customer.Status = 1;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Customer");
        }

        [AccessPolicy(PageCode = "SHNCUSA006")]
        public ActionResult Active(int id)
        {
            var customer = db.Customers.Where(m => m.Id == id).FirstOrDefault();
            customer.Status = 0;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Customer");
        }

        [AccessPolicy(PageCode = "SHNCUSR004")]
        public JsonResult Delete(string id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var dId = AdminHelpers.DCodeInt(id);
            var customer = db.Customers.FirstOrDefault(i => i.Id == dId && i.Status == 0);
            if (customer != null)
            {
                customer.Status = 2;
                customer.UpdatedBy = user.Name;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSVAI007")]
        public JsonResult VerifyAadharImage(int code)
        {
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();
            bool IsAdded = false;
            string message = "";
            if (customer != null)
            {
                if (customer.AadharName != null && customer.AadharNumber != null && customer.DOB != null)
                {
                    IsAdded = true;
                }
                else
                {
                    if (customer.AadharName == null && customer.AadharNumber == null && customer.DOB == null)
                    {
                        message = "Aadhar Name, Aadhar Number and Date Of Birth are Empty!";
                    }
                    else if (customer.AadharName == null && customer.AadharNumber == null)
                    {
                        message = "Aadhar Name and Aadhar Number are Empty!";
                    }
                    else if (customer.AadharNumber == null && customer.DOB == null)
                    {
                        message = "Aadhar Number and Date Of Birth are Empty!";
                    }
                    else if (customer.AadharName == null && customer.DOB == null)
                    {
                        message = "Aadhar Name and Date Of Birth are Empty!";
                    }
                    else if (customer.AadharName == null)
                    {
                        message = "Aadhar Name is Empty!";
                    }
                    else if (customer.AadharNumber == null)
                    {
                        message = "Aadhar Number is Empty!";
                    }
                    else if (customer.DOB == null)
                    {
                        message = "Date Of Birth is Empty!";
                    }
                }
                customer.AadharVerify = IsAdded;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { IsAdded = IsAdded, message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSVA008")]
        public JsonResult VerifyAge(int code)
        {
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();
            customer.AgeVerify = true;
            customer.DateUpdated = DateTime.Now;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSRAI009")]
        public JsonResult RejectAadharImage(int code)
        {
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();
            customer.AadharVerify = false;
            customer.AadharName = "Rejected";
            customer.AadharNumber = "Rejected";
            customer.ImageAadharPath = "Rejected";
            customer.DateUpdated = DateTime.Now;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            // Customer.Edit(customer, out int error);
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSCAI010")]
        public JsonResult CheckAadharImage(int code)
        {
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();
            if (customer.AadharVerify == true)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else if (customer.AadharVerify == false && customer.ImageAadharPath == "Rejected")
            {
                return Json(new { data = customer.ImageAadharPath, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSAD011")]
        public ActionResult Admin()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CustomerListViewModel();

            model.List = db.Customers.Where(i => i.Position == 4 && i.Status == 0).Select(i => new CustomerListViewModel.CustomerList
            {
                Id = i.Id,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                Address = i.Address,
                DistrictName = i.DistrictName,
                StateName = i.StateName
            }).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCUSSA012")]
        public JsonResult Save(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";

            var customer = db.Customers.Where(m => m.Id == Id).FirstOrDefault();
            if (customer != null && customer.Position != 4)
            {
                customer.Position = 4;
                customer.UpdatedBy = user.Name;
                customer.DateUpdated = DateTime.Now;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                IsAdded = true;
                message = customer.Name + " Added Successfully";
            }
            else if (customer != null && customer.Position == 4)
            {
                message1 = customer.Name + " Already Exist";
            }

            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSSA012")]
        public JsonResult Remove(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var customer = db.Customers.Where(m => m.Id == Id).FirstOrDefault();
            if (customer != null)
            {
                customer.Position = 0;
                customer.DateUpdated = DateTime.Now;
                customer.UpdatedBy = user.Name;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSSA012")]
        public async Task<JsonResult> GetCustomerSelect2(string q = "")
        {
            var model = await db.Customers.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.Position != 4).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetDistrictSelect2(string q = "")
        {
            var model = await db.Customers
                .Where(a => a.DistrictName.Contains(q) && a.Status == 0 && a.Position != 4)
                .GroupBy(i => i.DistrictName)
                .Select(i => new
                {
                    id = i.Key,
                    text = i.Key
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
