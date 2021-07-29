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
                    config.CreateMap<CustomerAddress, CustomerDetailsViewModel.AddressList>();
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
            var List = (from s in db.Customers
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);
        }

        [AccessPolicy(PageCode = "SHNCUSL001")]
        public ActionResult AadharPending()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CustomerListViewModel();
            model.List = db.Customers.Where(i => i.Status == 0 && i.ImageAadharPath != null && i.ImageAadharPath != "Rejected" && i.AadharVerify == false).Select(i => new CustomerListViewModel.CustomerList
            {
                Id = i.Id,
                Name = i.Name,
                Address = i.Address,
                CountryName = i.CountryName,
                DistrictName = i.DistrictName,
                Email = i.Email,
                ImagePath = i.ImagePath,
                PhoneNumber = i.PhoneNumber,
                StateName = i.StateName
            }).OrderBy(i=> i.Name).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCUSD002")]
        public ActionResult Details(string id)
        {
            var dCode = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode==0)
                return HttpNotFound();
            var customer = db.Customers.Where(m => m.Id == dCode).FirstOrDefault();
            var model = _mapper.Map<Customer, CustomerDetailsViewModel>(customer);
            model.List = db.CustomerAddresses.Where(i => i.Status == 0 && i.CustomerId == dCode).ToList().AsQueryable().ProjectTo<CustomerDetailsViewModel.AddressList>(_mapperConfiguration).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCUSE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode==0)
                return HttpNotFound();
            var customer = db.Customers.Where(m => m.Id == dCode).FirstOrDefault();//Customer.Get(dCode);
            var model = _mapper.Map<Customer, CustomerEditViewModel>(customer);
            model.DOB = customer.DOB != null ? customer.DOB.Value.ToString("dd-MM-yyyy") : "N/A";
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCUSIA005")]
        public ActionResult InActive(int code)
        {
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault(); //Customer.Get(code);
            customer.Status = 1;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Customer");
        }

        [AccessPolicy(PageCode = "SHNCUSA006")]
        public ActionResult Active(int code)
        {
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();// Customer.Get(code);
            customer.Status = 0;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Customer");
        }

        [AccessPolicy(PageCode = "SHNCUSR004")]
        public ActionResult Delete(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var dCode = AdminHelpers.DCodeInt(code);
            var customer = db.Customers.FirstOrDefault(i => i.Id == dCode);// db.Customers.Where(m => m.Code == code).FirstOrDefault();// Customer.Get(code);
            customer.Status = 2;
            customer.UpdatedBy = user.Name;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Customer");
        }

        [AccessPolicy(PageCode = "SHNCUSVAI007")]
        public JsonResult VerifyAadharImage(int code)
        {
           
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();//Customer.Get(code);
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
                //Customer.Edit(customer, out int error);
            }
            
            return Json(new { IsAdded = IsAdded, message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSVA008")]
        public JsonResult VerifyAge(int code)
        {
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();//Customer.Get(code);
            customer.AgeVerify = true;
            customer.DateUpdated = DateTime.Now;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            // Customer.Edit(customer, out int error);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSRAI009")]
        public JsonResult RejectAadharImage(int code)
        {
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();//Customer.Get(code);
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
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();//Customer.Get(code);
            if (customer.AadharVerify == true)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else if(customer.AadharVerify == false && customer.ImageAadharPath == "Rejected")
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
                StateName = i.StateName,
                ImagePath = i.ImagePath
            }).OrderBy(i=>i.Name).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCUSSA012")]
        public JsonResult Save(int code = 0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";

            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();//Customer.Get(code);
            if (customer != null && customer.Position != 4)
            {
                customer.Position = 4;
                customer.UpdatedBy = user.Name;
                customer.DateUpdated = DateTime.Now;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //Customer.Edit(customer, out int error);
                IsAdded = true;
                message = customer.Name + " Added Successfully";
            }
            else if(customer != null && customer.Position == 4)
            {
                message1 = customer.Name + " Already Exist";
            }
            
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCUSSA012")]
        public JsonResult Remove(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            var customer = db.Customers.Where(m => m.Id == code).FirstOrDefault();//Customer.Get(code);
            if (customer != null)
            {
                customer.Position = 0;
                customer.DateUpdated = DateTime.Now;
                customer.UpdatedBy = user.Name;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                IsAdded = true;
            }
            return Json(new { IsAdded = IsAdded }, JsonRequestBehavior.AllowGet);
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
