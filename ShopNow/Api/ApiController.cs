using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Razorpay.Api;
using ShopNow.Base;
using ShopNow.Filters;
using ShopNow.MessageHandlers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ShopNow.Controllers
{
    [APIKeyHandler(ApiKey = "shopnow")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApiController : Controller
    {
        private sncEntities db = new sncEntities();

        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        //private string apipath= "https://admin.shopnowchat.in/";
        private string apipath = "http://117.221.69.52:85/";
        private const string _prefix = "";

        private static string _generateCode(string _prefix)
        {
            return ShopNow.Helpers.DRC.Generate(_prefix);
        }

        private static string _referenceCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }

        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.GenerateOTP();
            }
        }

        private static string _generatedDelivaryId
        {
            get
            {
                return ShopNow.Helpers.DRC.GenerateDelivaryBoy();
            }
        }
        public ApiController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<CustomerCreateViewModel, Models.Customer>();
                config.CreateMap<OtpViewModel, OtpVerification>();
                config.CreateMap<CustomerAddOnsAddressViewModel, CustomerAddress>();
                config.CreateMap<ShopCreateViewModel, Shop>();
                config.CreateMap<ShopUpdateViewModel, Shop>();
                config.CreateMap<Shop, ShopSingleUpdateViewModel>();
                config.CreateMap<ShopSingleUpdateViewModel, Shop>();
                config.CreateMap<Shop, ShopAllListViewModel.ShopList>();
                config.CreateMap<Shop, ShopDetails>();
                config.CreateMap<Product, ShopDetails.ProductList>();
                config.CreateMap<Product, ProductDetailsViewModel>();
                config.CreateMap<CustomerAddress, CustomerAddressListViewModel.CustomerList>();
                config.CreateMap<Models.Customer, CustomerProfileViewModel>();
                config.CreateMap<Shop, CustomerShopAllListViewModel.ShopList>();
                config.CreateMap<OtpVerification, CustomerShopAllListViewModel.VerifyList>();
                config.CreateMap<DeliveryBoyCreateViewModel, DeliveryBoy>();
                config.CreateMap<Models.Payment, CreditPaymentViewModel>();
                config.CreateMap<PaymentCreateApiViewModel, Models.Payment>();
                config.CreateMap<ShopSingleEditViewModel, Shop>();
                config.CreateMap<ShopReviewViewModel, CustomerReview>();

                config.CreateMap<LocationDetailsCreateViewModel, LocationDetail>();
                config.CreateMap<OrderCreateViewModel, Models.Order>();
                config.CreateMap<OrderCreateViewModel.ListItem, OrderItem>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        public JsonResult GetShop(string placeid)
        {
            using (WebClient myData = new WebClient())
            {
                string getDetails = myData.DownloadString("https://maps.googleapis.com/maps/api/place/details/json?place_id=" + placeid + "&key=AIzaSyCRsR3Wpkj_Vofy5FSU0otOx-6k-YFiNBk");
                var result = JsonConvert.DeserializeObject<Results>(getDetails);
                return Json(new { result, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCallVerify(string FromNumber, string to)
        {
            string result = ConnectCall.connectCustomerToAgent(FromNumber, to);
            return Json(new { result, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Register(CustomerCreateViewModel model)
        {
            var otpVerification = db.Customers.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber && i.Status == 0);
            if (otpVerification == null)
            {
                var user = _mapper.Map<CustomerCreateViewModel, Models.Customer>(model);
                user.Position = 0;
                if (user.Name == null || user.Name == "")
                {
                    user.Name = "Null";
                }
                user.Status = 0;
                user.DateEncoded = DateTime.Now;
                user.DateUpdated = DateTime.Now;
                db.Customers.Add(user);
                db.SaveChanges();
                if (user.Id != 0)
                {
                    var otpmodel = new OtpVerification();
                    otpmodel.CustomerId = user.Id;
                    otpmodel.CustomerName = user.Name;
                    otpmodel.PhoneNumber = model.PhoneNumber;
                    otpmodel.Otp = _generatedCode;
                    otpmodel.ReferenceCode = _referenceCode;
                    otpmodel.Verify = false;
                    otpmodel.CreatedBy = user.Name;
                    otpmodel.UpdatedBy = user.Name;
                    otpmodel.DateEncoded = DateTime.Now;
                    var dateAndTime = DateTime.Now;
                    var date = dateAndTime.ToString("d");
                    var time = dateAndTime.ToString("HH:mm");
                    string joyra = "04448134440";
                    string Msg = "Hi, " + otpmodel.Otp + " is the OTP for (Shop Now Chat) Verification at " + time + " with " + otpmodel.ReferenceCode + " reference - Joyra";
                    string result = SendSMS.execute(joyra, model.PhoneNumber, Msg);
                    otpmodel.Status = 0;
                    otpmodel.DateEncoded = DateTime.Now;
                    otpmodel.DateUpdated = DateTime.Now;
                    db.OtpVerifications.Add(otpmodel);
                    db.SaveChanges();
                    if (otpmodel != null)
                    {
                        return Json(new { message = "Successfully Registered and OTP send!", id = user.Id, user.Position });

                    }
                    else
                        return Json("Otp Failed to send!");
                }
                else
                    return Json("Registration Failed!");
            }
            else
            {
                var otpmodel = new OtpVerification();
                var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber);
                otpmodel.CustomerId = customer.Id;
                otpmodel.CustomerName = customer.Name;
                otpmodel.PhoneNumber = model.PhoneNumber;
                otpmodel.Otp = _generatedCode;
                otpmodel.ReferenceCode = _referenceCode;
                otpmodel.Verify = false;
                var dateAndTime = DateTime.Now;
                var date = dateAndTime.ToString("d");
                var time = dateAndTime.ToString("HH:mm");
                string joyra = "04448134440";
                string Msg = "Hi, " + otpmodel.Otp + " is the OTP for (Shop Now Chat) Verification at " + time + " with " + otpmodel.ReferenceCode + " reference - Joyra";
                string result = SendSMS.execute(joyra, model.PhoneNumber, Msg);
                otpmodel.Status = 0;
                otpmodel.DateEncoded = DateTime.Now;
                otpmodel.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(otpmodel);
                db.SaveChanges();
                if (otpmodel != null)
                {
                    return Json(new { message = "Already Customer and OTP send!", id = customer.Id, Position = customer.Position });
                }
                else
                    return Json("Otp Failed to send!");
            }
        }

        [HttpPost]
        public JsonResult CustomerUpdate(CustomerCreateViewModel model)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //db.Configuration.LazyLoadingEnabled = true;
            var customer = db.Customers.FirstOrDefault(i => i.Id == model.Id);
            if ((model.Name == null && model.Email == null) || (model.Name == "" && model.Email == ""))
            {
                customer.AadharName = model.AadharName;
                customer.AadharNumber = model.AadharNumber;
                customer.ImageAadharPath = model.ImageAadharPath;
                customer.AgeVerify = model.AgeVerify;
                customer.DOB = model.DOB;
                customer.ImagePath = model.ImagePath;
                customer.UpdatedBy = customer.Name;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully Updated Your Details!", Details = customer });
            }
            else
            {
                customer.Name = model.Name;
                customer.Email = model.Email;
                customer.UpdatedBy = customer.Name;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully Updated Your Details!", Details = customer });
            }
        }

        [HttpPost]
        public JsonResult CustomerPassword(CustomerPasswordViewModel model)
        {
            if (model.Password == null || model.Password == "")
            {
                return Json(new { message = "Not Give Empty!" });
            }
            else if (model.CustomerId != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);// Customer.Get(model.CustomerCode);
                customer.Password = model.Password;
                customer.Position = 1;
                customer.UpdatedBy = customer.Name;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully Your Password Created!" });
            }
            else if (model.StaffId != 0)
            {
                var staff = db.Staffs.FirstOrDefault(i => i.Id == model.StaffId);
                staff.Password = model.Password;
                staff.UpdatedBy = staff.Name;
                staff.DateUpdated = DateTime.Now;
                db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var customer = db.Customers.FirstOrDefault(i => i.Id == staff.CustomerId);
                customer.Position = 2;
                customer.UpdatedBy = customer.Name;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully Your Staff Password Created!" });
            }
            else
            {
                return Json(new { message = "Something went wrong!" });
            }
        }

        public JsonResult GetCustomerAddOnsDelete(int id)
        {
            var customerAddress = db.CustomerAddresses.FirstOrDefault(i => i.Id == id);
            customerAddress.Status = 2;
            db.Entry(customerAddress).State = System.Data.Entity.EntityState.Modified;
            db.SaveChangesAsync();
            if (id != 0)
                return Json(new { message = "Successfully Deleted Addons Address!", AddressType = customerAddress.AddressType }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { message = "Failed to Delete Addons Address!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CustomerAddOns(CustomerAddOnsAddressViewModel model)
        {
            var customerAddons = db.CustomerAddresses.Where(i => i.CustomerId == model.CustomerId && i.AddressType == model.AddressType && i.Status == 0).FirstOrDefault();
            if (customerAddons != null)
            {
                if (db.CustomerAddresses.Where(i => i.CustomerId == model.CustomerId && i.Status == 0 && i.Id == model.Id).Count() > 0)
                {
                    var customerAdd = db.CustomerAddresses.Where(i => i.CustomerId == model.CustomerId && i.AddressType == model.AddressType && i.Status == 0).FirstOrDefault();
                    customerAdd.Name = model.Name;
                    customerAdd.CountryName = model.CountryName;
                    customerAdd.AddressType = model.AddressType;
                    customerAdd.FlatNo = model.FlatNo;
                    customerAdd.LandMark = model.LandMark;
                    customerAdd.StateName = model.StateName;
                    customerAdd.DistrictName = model.DistrictName;
                    customerAdd.StreetName = model.StreetName;
                    customerAdd.Address = model.Address;
                    customerAdd.PinCode = model.PinCode;
                    customerAdd.Latitude = model.Latitude;
                    customerAdd.Longitude = model.Longitude;
                    customerAdd.UpdatedBy = customerAdd.UpdatedBy;
                    customerAdd.DateUpdated = DateTime.Now;
                    db.Entry(customerAdd).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    // CustomerAddress.Edit(customerAdd, out errorCode);
                    if (model.AddressType == 0)
                    {
                        var customer = db.Customers.Where(i => i.Id == model.CustomerId).FirstOrDefault();// Customer.Get(model.CustomerCode);
                        customer.AddressType = model.AddressType;
                        customer.LandMark = model.LandMark;
                        customer.FlatNo = model.FlatNo;
                        customer.CountryName = model.CountryName;
                        customer.StateName = model.StateName;
                        customer.DistrictName = model.DistrictName;
                        customer.StreetName = model.StreetName;
                        customer.Address = model.Address;
                        customer.PinCode = model.PinCode;
                        customer.Latitude = model.Latitude;
                        customer.Longitude = model.Longitude;
                        customer.UpdatedBy = customer.Name;
                        // Customer.Edit(customer, out errorCode);
                        customer.DateUpdated = DateTime.Now;
                        db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (model.CustomerId != 0)
                    {
                        return Json(new { message = "Successfully Added Addons Address!", Details = customerAdd }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = "Failed to Add Addons Address!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { message = "Already This Address Exist!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var user = _mapper.Map<CustomerAddOnsAddressViewModel, CustomerAddress>(model);
                user.Status = 0;
                user.DateEncoded = DateTime.Now;
                user.DateUpdated = DateTime.Now;
                db.CustomerAddresses.Add(user);
                db.SaveChanges();

                if (model.AddressType == 0)
                {
                    var customer = db.Customers.Where(i => i.Id == model.CustomerId).FirstOrDefault();
                    customer.AddressType = model.AddressType;
                    customer.LandMark = model.LandMark;
                    customer.FlatNo = model.FlatNo;
                    customer.CountryName = model.CountryName;
                    customer.StateName = model.StateName;
                    customer.DistrictName = model.DistrictName;
                    customer.StreetName = model.StreetName;
                    customer.Address = model.Address;
                    customer.PinCode = model.PinCode;
                    customer.Latitude = model.Latitude;
                    customer.Longitude = model.Longitude;
                    customer.UpdatedBy = customer.Name;
                    customer.DateUpdated = DateTime.Now;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (user != null)
                {
                    model.Id = user.Id;
                    return Json(new { message = "Successfully Added Addons Address!", Details = model });
                }
                else
                {
                    return Json(new { message = "Failed to Add Addons Address!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult CustomerAddressUpdate(CustomerAddressViewModel model)
        {
            db.Configuration.ProxyCreationEnabled = false;
            int checkCustomerAddTypeExist = db.CustomerAddresses.Where(i => i.Id != model.Id && i.CustomerId == model.CustomerId && i.AddressType == model.AddressType && i.Status == 0).Count();
            var customerAddress = db.CustomerAddresses.Where(i => i.Id == model.Id && i.Status == 0).FirstOrDefault();
            if (checkCustomerAddTypeExist > 0)
            {
                return Json(new { message = "This Addresstype Alreay Exist", Details = customerAddress });
            }
            else
            {
                customerAddress.Name = model.Name;
                customerAddress.CountryName = model.CountryName;
                customerAddress.AddressType = model.AddressType;
                customerAddress.FlatNo = model.FlatNo;
                customerAddress.LandMark = model.LandMark;
                customerAddress.StateName = model.StateName;
                customerAddress.DistrictName = model.DistrictName;
                customerAddress.StreetName = model.StreetName;
                customerAddress.Address = model.Address;
                customerAddress.PinCode = model.PinCode;
                customerAddress.Latitude = model.Latitude;
                customerAddress.Longitude = model.Longitude;
                customerAddress.UpdatedBy = customerAddress.Name;
                customerAddress.DateUpdated = DateTime.Now;
                db.Entry(customerAddress).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { message = "Successfully Updated Your Address!", Details = customerAddress }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopVerify(string FromNumber, string digits)
        {
            var otpVerification = db.OtpVerifications.Where(i => i.PhoneNumber == FromNumber && i.Otp == digits && i.Verify == false).OrderByDescending(i => i.DateEncoded).ToList();
            var otpVerificationCheck = db.OtpVerifications.Where(i => i.PhoneNumber == FromNumber && i.Otp == digits && i.Verify == true).OrderByDescending(i => i.DateEncoded).ToList();
            if (otpVerification.Count != 0)
            {
                DateTime currentTime = otpVerification.FirstOrDefault().DateEncoded.Date;
                if (currentTime == DateTime.Now.Date)
                {
                    return Json(new { message = "Still Verify Pending!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Otp Expired!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (otpVerificationCheck.Count != 0)
            {
                DateTime currentTime = otpVerificationCheck.FirstOrDefault().DateEncoded.Date;
                if (currentTime == DateTime.Now.Date)
                {
                    return Json(new { message = "Verified Successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Yesterday Otp Verified!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { message = "Your OTP Outdated!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVerify(string FromNumber, string digits)
        {
            var otpVerification = db.OtpVerifications.Where(i => i.PhoneNumber == FromNumber && i.Verify == false).OrderByDescending(i => i.DateEncoded).Take(1).ToList();

            if (otpVerification.Count != 0)
            {
                if (otpVerification.FirstOrDefault().Otp == digits)
                {
                    otpVerification[0].Verify = true;
                    db.Entry(otpVerification[0]).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { message = "Successfully Your Phone Number Verified!" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { message = "Failed to Verify Phone Number!" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { message = "Use Todays OTP. Please try!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeliveryBoyExist(DeliveryBoyExistViewModel model)
        {
            var deliveryBoyExist = db.DeliveryBoys.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber && (i.Status == 0 || i.Status == 1 || i.Status == 3));
            if (deliveryBoyExist == null)
            {
                return Json(new { message = "Not Available Delivery Boy!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (deliveryBoyExist.Status == 0)
                {
                    return Json(new { message = "Approved Delivery Boy!" }, JsonRequestBehavior.AllowGet);
                }
                else if (deliveryBoyExist.Status == 1)
                {
                    return Json(new { message = "Pending Delivery Boy!" }, JsonRequestBehavior.AllowGet);
                }
                else if (deliveryBoyExist.Status == 3)
                {
                    return Json(new { message = "Rejected Delivery Boy!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Unknown Status!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult GetPaymentCredit(int customerId)
        {
            var customerName = (from c in db.Customers
                                where c.Id == customerId && c.Position == 1
                                select c.Name).FirstOrDefault();
            var orderCount = (from s in db.Orders
                              join sh in db.Shops on s.ShopId equals sh.Id
                              join c in db.Customers on sh.CustomerId equals c.Id
                              where sh.CustomerId == customerId && (s.Status >= 2)
                              select s).Count();

            var platformcredits = (from ss in db.Payments
                                   where ss.CustomerId == customerId && ss.Status == 0 && ss.CreditType == 0
                                   select (Double?)ss.OriginalAmount).Sum() ?? 0;

            var platformorder = (Convert.ToInt32(orderCount) * (db.PlatFormCreditRates.FirstOrDefault().RatePerOrder));
            var varDelivery = (from ss in db.Payments
                               where ss.CustomerId == customerId && ss.Status == 0 && ss.CreditType == 1
                               select (Double?)ss.OriginalAmount).Sum() ?? 0;

            var varDeliveryCharges = (from ss in db.Orders
                                      join sh in db.Shops on ss.ShopId equals sh.Id
                                      where sh.CustomerId == customerId && ss.Status >= 2
                                      select (Double?)ss.DeliveryCharge).Sum() ?? 0;

            List<CreditPaymentViewModel> payment = new List<CreditPaymentViewModel>
            {
                new CreditPaymentViewModel{CustomerId=customerId,CustomerName=customerName,CreditType=0,Credits=Math.Floor(platformcredits - platformorder) },
                new CreditPaymentViewModel{CustomerId=customerId,CustomerName=customerName,CreditType=1,Credits=Math.Floor(varDelivery - varDeliveryCharges) }
            };
            return Json(payment, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProducts(int shopId, int page = 1, int pageSize = 5)
        {
            var source = (from p in db.Products
                          join m in db.MasterProducts on p.MasterProductId equals m.Id
                          where p.ShopId == shopId && (p.Status == 0 || p.Status == 1) && p.ShopId != 0
                          select new ActiveProductListViewModel.ProductList
                          {
                              Id = p.Id,
                              // Name = p.Name,
                              Name = m.Name,
                              ShopId = p.ShopId,
                              ShopName = p.ShopName,
                              Price = p.Price,
                              Qty = p.Qty,
                              Status = p.Status
                          }).ToList();
            int count = source.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetProducts?shopId=" + shopId + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetProducts?shopId=" + shopId + "&page=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopItemList(int shopId, string str = "", int page = 1, int pageSize = 20)
        {
            var shid = db.Shops.Where(s => s.Id == shopId).FirstOrDefault();
            var model = db.Products.Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })//, (c, p) => new { c, p })
                                                                                                                      //.AsEnumerable()
                            .Where(i => i.p.ShopId == shid.Id && (i.p.Status == 0 || i.p.Status == 1) && (str != "" ? i.m.Name.ToLower().StartsWith(str.ToLower()) : true))
                            .Select(i => new ActiveProductListViewModel.ProductList
                            {
                                Id = i.p.Id,
                                Name = i.m.Name,
                                ShopId = i.p.ShopId,
                                ShopName = i.p.ShopName,
                                Price = i.p.Price,
                                MenuPrice = i.p.MenuPrice,
                                Qty = i.p.Qty,
                                Status = i.p.Status,
                                ImagePath = i.m.ImagePath1
                            }).ToList();
            int count = model.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetShopItemList?shopId=" + shopId + "&str=" + str + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetShopItemList?shopId=" + shopId + "&str=" + str + "&page=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BillUpdate(BillUpdate model)
        {
            var bill = db.Bills.FirstOrDefault(i => i.Id == model.Id);
            var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
            if (bill != null && customer != null)
            {
                bill.DeliveryChargeCustomer = model.DeliveryChargeCustomer;
                bill.ItemType = model.ItemType;
                bill.ConvenientCharge = model.ConvenientChargeRange;
                bill.PackingCharge = model.PackingCharge;
                bill.UpdatedBy = customer.Name;
                bill.DateUpdated = DateTime.Now;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully Updated Bill!" });
            }
            else
            {
                return Json(new { message = "Fail to Update Bill!" });
            }
        }

        public JsonResult GetBillOrDelivary(int bill, int shopId)
        {
            var model = new BillApiListViewModel();
            model.List = db.Bills.Where(i => i.NameOfBill == bill && i.ShopId == shopId && i.Status == 0).Select(i => new BillApiListViewModel.BillList
            {
                Id = i.Id,
                ConvenientChargeRange = i.ConvenientCharge,
                ShopId = i.ShopId,
                ShopName = i.ShopName,
                ItemType = i.ItemType,
                ConvenientCharge = i.PlatformCreditRate,
                NameOfBill = i.NameOfBill,
                PackingCharge = i.PackingCharge,
                DeliveryRateSet = i.DeliveryRateSet,
                DeliveryChargeKM = i.DeliveryChargeKM,
                DeliveryChargeOneKM = i.DeliveryChargeOneKM,
                DeliveryChargeCustomer = i.DeliveryChargeCustomer,
                TotalAmount = i.TotalAmount,
                Distance = i.Distance
            }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveCustomerToken(int customerId, string token)
        {
            var customer = db.Customers.Where(c => c.Id == customerId).FirstOrDefault();
            try
            {
                customer.FcmTocken = token;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(token = customer.FcmTocken, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(token = "", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetInORActive(ProductActiveOrInViewModel model)
        {
            if (model.State == 0)
            {
                var product = db.Products.FirstOrDefault(i => i.Id == model.ProductId);
                product.Status = 0;
                if (model.CustomerId != 0)
                {
                    var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                    product.UpdatedBy = customer.Name;
                }
                product.DateUpdated = DateTime.Now;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully Activated the Product!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var product = db.Products.FirstOrDefault(i => i.Id == model.ProductId);
                product.Status = 1;
                if (model.CustomerId != 0)
                {
                    var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                    product.UpdatedBy = customer.Name;
                }
                product.DateUpdated = DateTime.Now;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully InActivated the Product!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeliveryBoyCreate(DeliveryBoyCreateViewModel model)
        {
            try
            {
                var deliveryBoyExist = db.DeliveryBoys.FirstOrDefault(i => i.Name == model.Name && i.PhoneNumber == model.PhoneNumber && (i.Status == 0 || i.Status == 1));
                if (deliveryBoyExist == null)
                {
                    var deliveryBoy = _mapper.Map<DeliveryBoyCreateViewModel, DeliveryBoy>(model);
                    var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                    if (customer != null)
                    {
                        deliveryBoy.Name = customer.Name;
                        deliveryBoy.Email = customer.Email;
                        deliveryBoy.CreatedBy = customer.Name;
                        deliveryBoy.UpdatedBy = customer.Name;
                        customer.Position = 3;
                        customer.UpdatedBy = customer.Name;
                        customer.DateUpdated = DateTime.Now;
                        db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    deliveryBoy.Status = 1;
                    deliveryBoy.DateEncoded = DateTime.Now;
                    deliveryBoy.DateUpdated = DateTime.Now;
                    db.DeliveryBoys.Add(deliveryBoy);
                    db.SaveChanges();
                    if (deliveryBoy.Id != 0)
                    {
                        return Json(new { message = "Successfully Created a Delivery Boy!", Position = customer.Position });
                    }
                    else
                        return Json(new { message = "Failed to Create a Delivery Boy!" });
                }
                else
                    return Json(new { message = "This Delivery Boy Already Exist!" });
            }
            catch
            {
                return Json(new { message = "Failed to Create a Delivery Boy!" });
            }
        }

        [HttpPost]
        public JsonResult GetStockCheck(ProductStockCheckViewModel model)
        {
            try
            {
                double stock = 0;
                StringBuilder sb = new StringBuilder();
                using (WebClient myData = new WebClient())
                {
                    myData.Headers["X-Auth-Token"] = "62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";
                    myData.Headers[HttpRequestHeader.Accept] = "application/json";
                    myData.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    foreach (var item in model.ListItems)
                    {
                        stock = Math.Floor(GetStockQty(item.ItemId.ToString()));
                        if (stock < item.Quantity)
                        {
                            if (stock != 0)
                                sb.Append($"{item.ProductName} has only {stock} available now. <br/>");
                            else
                                sb.Append($"{item.ProductName} has no stock available now. <br/>");
                        }
                    }
                }
                return Json(new { message = sb.ToString() });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AddPayment(PaymentCreateApiViewModel model)
        {
            var payment = _mapper.Map<PaymentCreateApiViewModel, Models.Payment>(model);
            var perOrderAmount = db.PlatFormCreditRates.Where(s => s.Status == 0).FirstOrDefault();
            if (model.CustomerId != 0)
            {
                //var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                //payment.CustomerName = model.CustomerName;
                payment.CreatedBy = model.CustomerName;
                payment.UpdatedBy = model.CustomerName;
                payment.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
                if (model.OrderId != 0)
                {
                    var order = db.Orders.FirstOrDefault(i => i.Id == model.OrderId);
                    order.Status = 2;
                    order.UpdatedBy = model.CustomerName;
                    order.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
                    order.DateUpdated = DateTime.Now;
                    //
                    order.NetDeliveryCharge = model.NetDeliveryCharge;
                    order.DeliveryCharge = model.GrossDeliveryCharge;
                    order.ShopDeliveryDiscount = model.ShopDeliveryDiscount;
                    order.Packingcharge = model.PackagingCharge;
                    order.Convinenientcharge = model.ConvenientCharge;
                    order.NetTotal = model.NetTotal;
                    db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //Reducing Platformcredits
                    var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                    var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
                    shopCredits.PlatformCredit -= payment.RatePerOrder.Value;
                    db.Entry(shopCredits).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    if (model.PaymentMode == "Online Payment")
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
                        string key = BaseClass.razorpaykey;// "rzp_live_PNoamKp52vzWvR";
                        string secret = BaseClass.razorpaySecretkey;//"yychwOUOsYLsSn3XoNYvD1HY";

                        RazorpayClient client = new RazorpayClient(key, secret);
                        Razorpay.Api.Payment varpayment = new Razorpay.Api.Payment();
                        var s = varpayment.Fetch(model.ReferenceCode);
                        PaymentsData pay = new PaymentsData();

                        pay.OrderNumber = Convert.ToInt32(model.OrderNumber);
                        pay.PaymentId = model.ReferenceCode;

                        pay.Invoice_Id = s["invoice_id"];
                        if (s["status"] == "created")
                            pay.Status = 0;
                        else if (s["status"] == "authorized")
                            pay.Status = 1;
                        else if (s["status"] == "captured")
                            pay.Status = 2;
                        else if (s["status"] == "refunded")
                            pay.Status = 3;
                        else if (s["status"] == "failed")
                            pay.Status = 4;
                        pay.Order_Id = s["order_id"];
                        if (s["fee"] != null && s["fee"] > 0)
                            pay.Fee = (decimal)s["fee"] / 100;
                        else
                            pay.Fee = s["fee"];
                        pay.Entity = s["entity"];
                        pay.Currency = s["currency"];
                        pay.Method = s["method"];
                        if (s["tax"] != null && s["tax"] > 0)
                            pay.Tax = (decimal)s["tax"] / 100;
                        else
                            pay.Tax = s["tax"];
                        if (s["amount"] != null && s["amount"] > 0)
                            pay.Amount = s["amount"] / 100;
                        else
                            pay.Amount = s["amount"];
                        pay.DateEncoded = DateTime.Now;
                        db.PaymentsDatas.Add(pay);
                        db.SaveChanges();
                    }


                    if (model.CreditType == 0 || model.CreditType == 1)
                    {
                        payment.PaymentCategoryType = 1;
                        payment.Credits = model.OriginalAmount.ToString();
                    }
                    else
                    {
                        payment.PaymentCategoryType = 0;
                        payment.Credits = "N/A";
                    }
                    payment.OrderNumber = order.OrderNumber;
                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    payment.RefundStatus = 1;
                    db.Payments.Add(payment);
                    db.SaveChanges();
                }
                return Json(new { message = "Successfully Added to Payment!", Details = model });
            }
            else
                return Json(new { message = "Failed to Add Payment !" });
        }

        [HttpPost]
        public JsonResult AddPaymentCreateOrder(razorpayOrderCreate model)
        {
            string Orderid = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                       SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;
            string key = BaseClass.razorpaykey;//"rzp_live_PNoamKp52vzWvR";
            string secret = BaseClass.razorpaySecretkey; //"yychwOUOsYLsSn3XoNYvD1HY";
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", model.Price);
            input.Add("currency", "INR");
            input.Add("receipt", "order_rcptid_11");
            RazorpayClient client = new RazorpayClient(key, secret);
            Razorpay.Api.Order order = client.Order.Create(input);
            Orderid = order["id"].ToString();
            return Json(new { message = "Success", Orderid = Orderid });
        }

        [HttpPost]
        public JsonResult UpdatedPayment(PaymentUpdatedApiViewModel model)
        {
            try
            {
                var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
                payment.UpdatedOriginalAmount = model.UpdatedOriginalAmount;
                payment.UpdatedAmount = model.UpdatedAmount;
                if (model.RefundAmount > 0)
                {
                    payment.RefundAmount = model.RefundAmount;
                    payment.RefundRemark = model.RefundRemark;
                }
                payment.UpdatedBy = model.CustomerName;
                payment.DateUpdated = DateTime.Now;
                db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully Updated Cart Payment!", Details = model });
            }
            catch
            {
                return Json(new { message = "Failed to Update Cart Payment !" });
            }
        }

        [HttpPost]
        public JsonResult AddOrder(OrderCreateViewModel model)
        {
            try
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
                if ((shopCredits.PlatformCredit < 26 || shopCredits.DeliveryCredit < 67))
                {
                    //Shop DeActivate
                    shop.Status = 6;
                    db.Entry(shop).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { message = "This shop is currently unservicable." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var order = _mapper.Map<OrderCreateViewModel, Models.Order>(model);
                    if (model.CustomerId != 0)
                    {
                        var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                        order.CustomerId = customer.Id;
                        order.CreatedBy = customer.Name;
                        order.UpdatedBy = customer.Name;
                        order.CustomerName = customer.Name;
                        order.CustomerPhoneNumber = customer.PhoneNumber;
                    }
                    order.OrderNumber = Convert.ToInt32(model.OrderNumber);
                    order.ShopId = shop.Id;
                    order.ShopName = shop.Name;
                    order.ShopPhoneNumber = shop.PhoneNumber ?? shop.ManualPhoneNumber;
                    order.ShopOwnerPhoneNumber = shop.OwnerPhoneNumber;
                    order.TotalPrice = model.ListItems.Sum(i => i.Price);
                    order.TotalProduct = model.ListItems.Count();
                    order.TotalQuantity = model.ListItems.Sum(i => Convert.ToInt32(i.Quantity));
                    order.DateEncoded = DateTime.Now;
                    order.DateUpdated = DateTime.Now;
                    order.Status = 0;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    foreach (var item in model.ListItems)
                    {
                        if (item.ItemId != 0)
                        {
                            var productMedicalStock = db.Products.FirstOrDefault(i => i.ItemId == item.ItemId && i.Status == 0);
                            productMedicalStock.HoldOnStok = Convert.ToInt32(item.Quantity);
                            productMedicalStock.Qty = productMedicalStock.Qty - Convert.ToInt32(item.Quantity);
                            db.Entry(productMedicalStock).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        var orderItem = _mapper.Map<OrderCreateViewModel.ListItem, OrderItem>(item);
                        orderItem.Status = 0;
                        orderItem.OrderId = order.Id;
                        orderItem.OrdeNumber = order.OrderNumber;
                        db.OrderItems.Add(orderItem);
                        db.SaveChanges();
                    }

                    if (order != null)
                    {
                        var fcmToken = (from c in db.Customers
                                        join s in db.Shops on c.Id equals s.CustomerId
                                        where s.Id == model.ShopId
                                        select c.FcmTocken ?? "").FirstOrDefault().ToString();
                        Helpers.PushNotification.SendbydeviceId("You have received new order.Accept Soon", "ShopNowChat", "a.mp3", fcmToken.ToString());

                        return Json(new { status = true, orderId = order.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAcceptOrder(int orderNo, int customerId, int status, int priority)
        {
            if (orderNo != 0 && customerId != 0 && status != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderNo);
                order.Status = status;
                order.UpdatedBy = customer.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var orderList = db.OrderItems.Where(i => i.OrderId == order.Id).ToList();
                foreach (var item in orderList)
                {
                    //Product Stock Update
                    var product = db.Products.FirstOrDefault(i => i.Id == item.ProductId && i.ProductTypeId == 3);
                    if (product != null)
                    {
                        product.HoldOnStok -= Convert.ToInt32(item.Quantity);
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var fcmToken = (from c in db.Customers
                                where c.Id == order.CustomerId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                //accept
                if (status == 3)
                {
                    Helpers.PushNotification.SendbydeviceId("Your order has been accepted by shop.", "ShopNowChat", "a.mp3", fcmToken.ToString());
                }

                //Refund
                if (status == 7)
                {
                    var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
                    if (payment.PaymentMode == "Online Payment")
                    {
                        payment.RefundAmount = payment.Amount;
                        payment.RefundRemark = "Your order has been cancelled by shop.";
                        payment.UpdatedBy = customer.Name;
                        payment.DateUpdated = DateTime.Now;
                        db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Helpers.PushNotification.SendbydeviceId($"Your refund of amount {payment.Amount} for order no {payment.OrderNumber} is for {payment.RefundRemark} initiated and you will get credited with in 7 working days.", "ShopNowChat", "a.mp3", fcmToken.ToString());
                    }
                }
                return Json(new { message = "Successfully Updated the Order!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to Update the Order!" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Have to check later
        public JsonResult GetShopDeliveredOrders(int shopId, int status, int page = 1, int pageSize = 5)
        {
            //var model = new CartAcceptListApiViewModel();
            //model.List = db.Orders.Where(j => j.Status == status)
            //    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
            //    .Join(db.Products, rz => rz.c.ProductCode, pr => pr.Id, (rz, pr) => new { rz, pr })
            //    .Join(db.Shops, py => py.rz.c.ShopCode, s => s.Code, (py, s) => new { py, s })
            //    .Join(db.ShopCharges, pay => pay.py.rz.c.OrderNo, sc => sc.OrderNo, (pay, sc)
            //    => new { pay, sc })
            //    .AsEnumerable()
            //   .Where(i => (i.pay.py.rz.p.PaymentResult == "success" || i.pay.py.rz.p.PaymentMode == "Cash On Hand") && i.pay.py.rz.p.ShopCode == shopCode)
            //   .GroupBy(j => j.pay.py.rz.c.OrderNo).Select(i => new CartAcceptListApiViewModel.CartList
            //   {
            //       Code = i.Any() ? i.FirstOrDefault().pay.py.rz.c.Code : "N/A",
            //       ProductCode = i.Any() ? i.FirstOrDefault().pay.py.rz.c.ProductCode : "N/A",
            //       PaymentMode = i.Any() ? i.FirstOrDefault().pay.py.rz.p.PaymentMode : "N/A",
            //       ShopCode = i.Any() ? i.FirstOrDefault().pay.py.rz.c.ShopCode : "N/A",
            //       ShopName = i.Any() ? i.FirstOrDefault().pay.py.rz.c.ShopName : "N/A",
            //       OrderNo = i.Any() ? i.FirstOrDefault().pay.py.rz.c.OrderNo : "N/A",
            //       CustomerName = i.Any() ? i.FirstOrDefault().pay.py.rz.c.CustomerName : "N/A",
            //       // ProductName = i.Any() ? i.FirstOrDefault().pay.py.pr.MasterProductName : "N/A",
            //       ProductName = i.Any() ? GetMasterProductName(i.FirstOrDefault().pay.py.pr.MasterProductCode) : "N/A",
            //       PhoneNumber = i.Any() ? i.FirstOrDefault().pay.py.rz.c.PhoneNumber : "N/A",
            //       DeliveryAddress = i.Any() ? i.FirstOrDefault().pay.py.rz.c.DeliveryAddress : "N/A",
            //       ShopLatitude = i.Any() ? i.FirstOrDefault().pay.s.Latitude : 0.0,
            //       ShopLongitude = i.Any() ? i.FirstOrDefault().pay.s.Longitude : 0.0,
            //       PackingCharge = i.Any() ? i.FirstOrDefault().sc.Packingcharge : 0.0,
            //       ConvinenientCharge = i.Any() ? i.FirstOrDefault().sc.Convinenientcharge : 0.0,
            //       SinglePrice = i.Any() ? i.FirstOrDefault().pay.py.rz.c.SinglePrice : 0.0,
            //       Price = i.FirstOrDefault().pay.py.rz.c.UpdatedPrice != 0 ? i.FirstOrDefault().pay.py.rz.c.Price : i.FirstOrDefault().pay.py.rz.c.UpdatedPrice,
            //       Amount = GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedAmount == 0 ? GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).Amount : GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedAmount,
            //       OriginalAmount = i.FirstOrDefault().pay.py.rz.p.UpdatedOriginalAmount == 0 ? i.FirstOrDefault().pay.py.rz.p.OriginalAmount : i.FirstOrDefault().pay.py.rz.p.UpdatedOriginalAmount,
            //       GrossDeliveryCharge = i.Any() ? i.FirstOrDefault().sc.GrossDeliveryCharge : 0.0,
            //       ShopDeliveryDiscount = i.Any() ? i.FirstOrDefault().sc.ShopDeliveryDiscount : 0.0,
            //       NetDeliveryCharge = i.Any() ? i.FirstOrDefault().sc.NetDeliveryCharge : 0.0,
            //       Qty = i.FirstOrDefault().pay.py.rz.c.UpdatedQty == "" ? i.FirstOrDefault().pay.py.rz.c.Qty : i.FirstOrDefault().pay.py.rz.c.UpdatedQty,
            //       Date = i.Any() ? i.FirstOrDefault().pay.py.rz.c.DateEncoded.ToString("dd-MMM-yyyy HH:mm") : "N/A",
            //       DateEncoded = i.Any() ? i.FirstOrDefault().pay.py.rz.c.DateEncoded : DateTime.Now,
            //       OrderList = GetOrderPendingList(i.FirstOrDefault().pay.py.rz.c.OrderNo, status), // Cart.GetOrderPendingList(i.FirstOrDefault().pay.py.rz.c.OrderNo, status),
            //       CartStatus = i.Any() ? i.FirstOrDefault().pay.py.rz.c.CartStatus : -5
            //   }).OrderByDescending(i => i.DateEncoded).ToList();


            db.Configuration.ProxyCreationEnabled = false;
            var model = new GetAllOrderListViewModel();
            model.OrderLists = db.Orders.Where(i => i.ShopId == shopId && i.Status==status)
                 .Join(db.Payments, o => o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
                 .GroupJoin(db.OrderItems, o => o.o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                 .Select(i => new GetAllOrderListViewModel.OrderList
                 {
                     Convinenientcharge = i.o.o.Convinenientcharge,
                     CustomerId = i.o.o.CustomerId,
                     CustomerName = i.o.o.CustomerName,
                     CustomerPhoneNumber = i.o.o.CustomerPhoneNumber,
                     //DateStr = i.o.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
                     DateEncoded = i.o.o.DateEncoded,
                     DeliveryAddress = i.o.o.DeliveryAddress,
                     DeliveryBoyId = i.o.o.DeliveryBoyId,
                     DeliveryBoyName = i.o.o.DeliveryBoyName,
                     DeliveryBoyPhoneNumber = i.o.o.DeliveryBoyPhoneNumber,
                     DeliveryCharge = i.o.o.DeliveryCharge,
                     Id = i.o.o.Id,
                     NetDeliveryCharge = i.o.o.NetDeliveryCharge,
                     OrderNumber = i.o.o.OrderNumber,
                     Packingcharge = i.o.o.Packingcharge,
                     PenaltyAmount = i.o.o.PenaltyAmount,
                     PenaltyRemark = i.o.o.PenaltyRemark,
                     ShopDeliveryDiscount = i.o.o.ShopDeliveryDiscount,
                     ShopId = i.o.o.ShopId,
                     ShopName = i.o.o.ShopName,
                     ShopOwnerPhoneNumber = i.o.o.ShopOwnerPhoneNumber,
                     ShopPhoneNumber = i.o.o.ShopPhoneNumber,
                     Status = i.o.o.Status,
                     TotalPrice = i.o.o.TotalPrice,
                     TotalProduct = i.o.o.TotalProduct,
                     TotalQuantity = i.o.o.TotalQuantity,
                     NetTotal = i.o.o.NetTotal,
                     WaitingCharge = i.o.o.WaitingCharge,
                     WaitingRemark = i.o.o.WaitingRemark,
                     RefundAmount = i.o.p.RefundAmount,
                     RefundRemark = i.o.p.RefundRemark,
                     PaymentMode = i.o.p.PaymentMode,
                     OrderItemList = i.oi.ToList(),
                 }).OrderByDescending(i => i.DateEncoded).ToList();


            int count = model.OrderLists.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.OrderLists.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetShopDeliveredOrders?shopId=" + shopId + "&status=" + status + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetShopDeliveredOrders?shopId=" + shopId + "&status=" + status + "&page=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelivaryTodayOrders(string phoneNumber)
        {
            //var model = new CartDelivaryListApiViewModel();
            //string dt = DateTime.Now.ToString("dd-MMM-yyyy");
            //model.ResturantList = db.Orders.Where(j => j.Status == 4 || j.Status == 5)
            //    .Join(db.Payments, c => c.ShopId, p => p.ShopId, (c, p) => new { c, p })
            //    .Join(db.Shops, ca => ca.c.ShopId, s => s.Id, (ca, s) => new { ca, s })
            //     .Join(db.DeliveryBoys, py => py.ca.c.DeliveryBoyId, d => d.Id, (py, d) => new { py, d })
            //   .AsEnumerable()
            //   .Where(i => i.py.ca.c.DeliveryBoyPhoneNumber == phoneNumber && i.py.s.ShopCategoryId == 1 && i.py.ca.c.DateEncoded.ToString("dd-MMM-yyyy") == dt)
            //   .Select(i => new CartDelivaryListApiViewModel.CartList
            //   {
            //       ShopName = i.py.ca.c.ShopName,
            //       CustomerName = i.py.ca.c.CustomerName,
            //       OrderNumber = i.py.ca.c.OrderNumber,
            //       ShopAddress = i.py.s.Address,
            //       ShopLatitude = i.py.s.Latitude,
            //       ShopLongitude = i.py.s.Longitude,
            //       ShopPhoneNumber = i.py.s.PhoneNumber,
            //       CartStatus = i.py.ca.c.Status,
            //       CustomerPhoneNumber = i.py.ca.c.CustomerPhoneNumber,
            //       CustomerLatitude = i.py.ca.c.Latitude,
            //       CustomerLongitude = i.py.ca.c.Longitude,
            //       DeliveryAddress = i.py.ca.c.DeliveryAddress,
            //       PaymentMode = GetPayment(i.py.ca.c.OrderNumber).PaymentMode,
            //       Amount = GetPayment(i.py.ca.c.OrderNumber).UpdatedAmount == 0 ? GetPayment(i.py.ca.c.OrderNumber).Amount : GetPayment(i.py.ca.c.OrderNumber).UpdatedAmount,
            //       OrderList = GetOrderList(i.py.ca.c.Id),
            //       OnWork = i.d.OnWork,
            //       Date = i.py.ca.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss"),
            //       RefundAmount = ((GetPayment(i.py.ca.c.OrderNumber).RefundAmount) ?? 0),
            //       RefundRemark = ((GetPayment(i.py.ca.c.OrderNumber).RefundRemark) ?? "N/A")
            //   }).ToList();
            //model.OtherList = db.Orders.Where(j => j.Status == 4 || j.Status == 5)
            //   .Join(db.Payments, c => c.ShopId, p => p.ShopId, (c, p) => new { c, p })
            //   .Join(db.Shops, ca => ca.c.ShopId, s => s.Id, (ca, s) => new { ca, s })
            //    .Join(db.DeliveryBoys, py => py.ca.c.DeliveryBoyId, d => d.Id, (py, d) => new { py, d })
            //  .AsEnumerable()
            //  .Where(i => i.py.ca.c.DeliveryBoyPhoneNumber == phoneNumber && i.py.s.ShopCategoryId != 1)
            //  .Select(i => new CartDelivaryListApiViewModel.CartList
            //  {
            //      ShopName = i.py.ca.c.ShopName,
            //      CustomerName = i.py.ca.c.CustomerName,
            //      OrderNumber = i.py.ca.c.OrderNumber,
            //      ShopAddress = i.py.s.Address,
            //      ShopLatitude = i.py.s.Latitude,
            //      ShopLongitude = i.py.s.Longitude,
            //      ShopPhoneNumber = i.py.s.PhoneNumber,
            //      CartStatus = i.py.ca.c.Status,
            //      CustomerPhoneNumber = i.py.ca.c.CustomerPhoneNumber,
            //      CustomerLatitude = i.py.ca.c.Latitude,
            //      CustomerLongitude = i.py.ca.c.Longitude,
            //      DeliveryAddress = i.py.ca.c.DeliveryAddress,
            //      PaymentMode = GetPayment(i.py.ca.c.OrderNumber).PaymentMode,
            //      Amount = GetPayment(i.py.ca.c.OrderNumber).UpdatedAmount == 0 ? GetPayment(i.py.ca.c.OrderNumber).Amount : GetPayment(i.py.ca.c.OrderNumber).UpdatedAmount,
            //      OrderList = GetOrderList(i.py.ca.c.Id),
            //      OnWork = i.d.OnWork,
            //      Date = i.py.ca.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss"),
            //      RefundAmount = ((GetPayment(i.py.ca.c.OrderNumber).RefundAmount) ?? 0),
            //      RefundRemark = ((GetPayment(i.py.ca.c.OrderNumber).RefundRemark) ?? "N/A")
            //  }).ToList();

            db.Configuration.ProxyCreationEnabled = false;
            var model = new TodayDeliveryListViewModel();
            model.ResturantList = db.Orders.Where(i => (i.Status == 4 || i.Status == 5) && i.DeliveryBoyPhoneNumber == phoneNumber)
                .Join(db.Shops.Where(i => i.ShopCategoryId == 1), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
                .Join(db.Payments, o => o.o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
                .Join(db.DeliveryBoys, o => o.o.o.DeliveryBoyId, d => d.Id, (o, d) => new { o, d })
                //.Join(db.Customers, o => o.o.o.o.CustomerId, c => c.Id, (o, c) => new { o, c })
                .GroupJoin(db.OrderItems, o => o.o.o.o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                .Select(i => new TodayDeliveryListViewModel.OrderList
                {
                    //Amount = i.o.o.p.Amount,
                    CustomerLatitude = 0,
                    CustomerLongitude = 0,
                    CustomerName = i.o.o.o.o.CustomerName,
                    CustomerPhoneNumber = i.o.o.o.o.CustomerPhoneNumber,
                    DateEncoded = i.o.o.o.o.DateEncoded,
                    DeliveryAddress = i.o.o.o.o.DeliveryAddress,
                    OnWork = i.o.d.OnWork,
                    OrderNumber = i.o.o.o.o.OrderNumber,
                    PaymentMode = i.o.o.p.PaymentMode,
                    RefundAmount = i.o.o.p.RefundAmount,
                    RefundRemark = i.o.o.p.RefundRemark,
                    ShopAddress = i.o.o.o.s.Address,
                    ShopLatitude = i.o.o.o.s.Latitude,
                    ShopLongitude = i.o.o.o.s.Longitude,
                    ShopName = i.o.o.o.s.Name,
                    ShopPhoneNumber = i.o.o.o.o.ShopPhoneNumber,
                    Status = i.o.o.o.o.Status,
                    Convinenientcharge = i.o.o.o.o.Convinenientcharge,
                    DeliveryCharge = i.o.o.o.o.DeliveryCharge,
                    NetDeliveryCharge = i.o.o.o.o.NetDeliveryCharge,
                    Packingcharge = i.o.o.o.o.Packingcharge,
                    ShopDeliveryDiscount = i.o.o.o.o.ShopDeliveryDiscount,
                    TotalPrice = i.o.o.o.o.TotalPrice,
                    TotalProduct = i.o.o.o.o.TotalProduct,
                    TotalQuantity = i.o.o.o.o.TotalQuantity,
                    NetTotal = i.o.o.o.o.NetTotal,
                    OrderItemList = i.oi.ToList()
                }).ToList();

            model.OtherList = db.Orders.Where(i => (i.Status == 4 || i.Status == 5) && i.DeliveryBoyPhoneNumber == phoneNumber)
               .Join(db.Shops.Where(i => i.ShopCategoryId != 1), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
               .Join(db.Payments, o => o.o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
               .Join(db.DeliveryBoys, o => o.o.o.DeliveryBoyId, d => d.Id, (o, d) => new { o, d })
               //.Join(db.Customers, o => o.o.o.o.CustomerId, c => c.Id, (o, c) => new { o, c })
               .GroupJoin(db.OrderItems, o => o.o.o.o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
               .Select(i => new TodayDeliveryListViewModel.OrderList
               {
                   //Amount = i.o.o.p.Amount,
                   CustomerLatitude = 0,
                   CustomerLongitude = 0,
                   CustomerName = i.o.o.o.o.CustomerName,
                   CustomerPhoneNumber = i.o.o.o.o.CustomerPhoneNumber,
                   DateEncoded = i.o.o.o.o.DateEncoded,
                   DeliveryAddress = i.o.o.o.o.DeliveryAddress,
                   OnWork = i.o.d.OnWork,
                   OrderNumber = i.o.o.o.o.OrderNumber,
                   PaymentMode = i.o.o.p.PaymentMode,
                   RefundAmount = i.o.o.p.RefundAmount,
                   RefundRemark = i.o.o.p.RefundRemark,
                   ShopAddress = i.o.o.o.s.Address,
                   ShopLatitude = i.o.o.o.s.Latitude,
                   ShopLongitude = i.o.o.o.s.Longitude,
                   ShopName = i.o.o.o.s.Name,
                   ShopPhoneNumber = i.o.o.o.o.ShopPhoneNumber,
                   Status = i.o.o.o.o.Status,
                   Convinenientcharge = i.o.o.o.o.Convinenientcharge,
                   DeliveryCharge = i.o.o.o.o.DeliveryCharge,
                   NetDeliveryCharge = i.o.o.o.o.NetDeliveryCharge,
                   Packingcharge = i.o.o.o.o.Packingcharge,
                   ShopDeliveryDiscount = i.o.o.o.o.ShopDeliveryDiscount,
                   TotalPrice = i.o.o.o.o.TotalPrice,
                   TotalProduct = i.o.o.o.o.TotalProduct,
                   TotalQuantity = i.o.o.o.o.TotalQuantity,
                   NetTotal = i.o.o.o.o.NetTotal,
                   OrderItemList = i.oi.ToList()
               }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelivaryBoyReject(int customerId, int orderNo)
        {
            if (orderNo != 0 && customerId != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderNo);
                order.Status = 3;
                order.DeliveryBoyId = customer.Id;
                order.DeliveryBoyName = customer.Name;
                order.DeliveryBoyPhoneNumber = customer.PhoneNumber;
                order.UpdatedBy = customer.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var delivery = db.DeliveryBoys.FirstOrDefault(i => i.CustomerId == customerId && i.Status == 0);
                delivery.isAssign = 0;
                delivery.DateUpdated = DateTime.Now;
                db.Entry(delivery).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully  Rejected Order by Delivery Boy!" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { message = "Failed to Reject Order!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopNotification(int shopId)
        {
            var customerTocken = (from c in db.Customers
                                  join s in db.Shops on c.Id equals s.CustomerId
                                  where s.Id == shopId
                                  select c.FcmTocken).ToString();
            if (shopId != 0)
            {
                var shop = db.Orders.OrderByDescending(q => q.Id).FirstOrDefault(i => i.ShopId == shopId && i.Status == 2 && i.Status == 0);
                if (shop != null)
                {
                    NotificationMessage("New Order Available! joyra", "Joyra", customerTocken);
                    // await  NotifyAsync(customerTocken, "Joyra", "New Order Available! joyraa");
                    return Json(new { message = "New Order Available!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    NotificationMessage("New Order Available! joyra", "Joyra", customerTocken);
                    return Json(new { message = "No Order Available!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { message = "Failed to Get Order!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPickUpNotification(int customerId)
        {
            if (customerId != 0)
            {
                var cart = db.Orders.OrderByDescending(q => q.Id).FirstOrDefault(i => i.CustomerId == customerId && i.Status == 5 && i.Status == 0);// Cart.GetCustomerPickUp(customerCode);
                if (cart != null)
                {
                    if (cart.DateUpdated.Date.ToString() == DateTime.Now.Date.ToString())
                    {
                        return Json(new { message = "Your Order on the way!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = "No Order on the way today!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { message = "No Order Available!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { message = "Failed to Pickup!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDelivaryBoyNotification(string phoneNo)
        {
            if (phoneNo != null || phoneNo != "")
            {
                var delivaryBoy = db.Orders.OrderByDescending(q => q.Id).FirstOrDefault(i => i.DeliveryBoyPhoneNumber == phoneNo && i.Status == 4 && i.Status == 0);// Cart.GetDelivaryPhoneNo(phoneNo);
                if (delivaryBoy != null)
                {
                    if (delivaryBoy.DateUpdated.Date.ToString() == DateTime.Now.Date.ToString())
                    {
                        return Json(new { message = "Today You have Task!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = "No Task Assigned!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { message = "No Cart Available!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { message = "Failed to DelivaryBoy Accept!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetOrderNoStatus(int orderNo)
        {
            var order = db.Orders.Where(i => i.OrderNumber == orderNo).FirstOrDefault();
            var shop = db.Shops.FirstOrDefault(i => i.Id == order.ShopId);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId);
            var model = new DeliveryBoyViewModel();
            model.ShopLatitude = shop.Latitude;
            model.ShopLongitude = shop.Longitude;
            model.CustomerLatitude = order.Latitude;
            model.CustomerLongitude = order.Longitude;
            if (deliveryBoy != null)
            {
                model.DeliveryBoyName = order.DeliveryBoyName;
                model.DeliveryBoyId = order.DeliveryBoyId;
                model.DeliveryBoyPhoneNumber = order.DeliveryBoyPhoneNumber;
            }
            else
            {
                model.DeliveryBoyName = "N/A";
                model.DeliveryBoyId = 0;
                model.DeliveryBoyPhoneNumber = "N/A";
            }
            model.CartStatus = order.Status;
            return Json(new { message = "Status of Cart!", model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPickUp(int orderNo, int customerId, double Amount, string PaymentMode)
        {
            if (orderNo != 0 && customerId != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId); //This is delivery boy
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderNo);
                order.Status = 5;
                order.UpdatedBy = customer.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var orderList = db.OrderItems.Where(i => i.OrderId == order.Id).ToList();
                foreach (var item in orderList)
                {
                    //Product Stock Update
                    var product = db.Products.FirstOrDefault(i => i.Id == item.ProductId && i.ProductTypeId==3);
                    if (product != null)
                    {
                        product.HoldOnStok -= Convert.ToInt32(item.Quantity);
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (PaymentMode == "Online Payment" && Amount > 1000)
                {
                    var models = new OtpVerification();
                    models.ShopId = order.ShopId;
                    models.CustomerId = order.CustomerId;
                    models.CustomerName = order.CustomerName;
                    models.PhoneNumber = order.DeliveryBoyPhoneNumber;
                    models.Otp = _generatedCode;
                    models.ReferenceCode = _referenceCode;
                    models.Verify = false;
                    models.OrderNo = orderNo;
                    models.CreatedBy = order.CustomerName;
                    models.UpdatedBy = order.CustomerName;
                    models.DateUpdated = DateTime.Now;
                    db.OtpVerifications.Add(models);
                    db.SaveChanges();
                }
                var fcmToken = (from c in db.Customers
                                where c.Id == order.CustomerId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                Helpers.PushNotification.SendbydeviceId("Your order is on the way.", "ShopNowChat", "a.mp3", fcmToken.ToString());
                return Json(new { message = "Successfully DelivaryBoy PickUp!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to DelivaryBoy PickUp!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDelivaryBoyAccept(int orderNo, int customerId)
        {
            if (orderNo != 0 && customerId != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.CustomerId == customerId && i.Status == 0);
                delivaryBoy.OnWork = 1;
                delivaryBoy.UpdatedBy = customer.Name;
                delivaryBoy.DateUpdated = DateTime.Now;
                db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //var shopCharge = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
                //var shop = db.Shops.FirstOrDefault(i => i.Id == shopCharge.ShopId);
                //var topup = db.TopUps.OrderByDescending(q => q.Id).FirstOrDefault(i => i.CustomerCode == shop.CustomerCode && i.CreditType == 1 && i.Status == 0);// TopUp.GetCustomerDelivary(shop.CustomerCode);
                //if (topup != null)
                //{
                //    topup.CreditAmount = topup.CreditAmount - shopCharge.GrossDeliveryCharge;
                //    topup.DateUpdated = DateTime.Now;
                //    db.Entry(topup).State = System.Data.Entity.EntityState.Modified;
                //    db.SaveChanges();
                //}
                return Json(new { message = "Successfully DelivaryBoy Accepted!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to DelivaryBoy Accept!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDelivaryBoyDelivered(int orderNo, int customerId, string otp)
        {
            var otpVerify = db.OtpVerifications.FirstOrDefault(i => i.OrderNo == orderNo);

            if (orderNo != 0 && customerId != 0 && otp != null || otp != "")
            {
                if (otp == "empty")
                {
                    goto Finish;
                }
                else if (otp == otpVerify.Otp)
                {
                    goto Finish;
                }
                else
                    return Json(new { message = "Type Correct OTP!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to DelivaryBoy Deliver!" }, JsonRequestBehavior.AllowGet);
            }
        Finish:
            var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.CustomerId == customerId && i.Status == 0);
            delivaryBoy.OnWork = 0;
            delivaryBoy.isAssign = 0;
            delivaryBoy.DateUpdated = DateTime.Now;
            db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            if (otp != "empty")
            {
                otpVerify.Verify = true;
                otpVerify.UpdatedBy = delivaryBoy.CustomerName;
                otpVerify.DateUpdated = DateTime.Now;
                db.Entry(otpVerify).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderNo);
            order.Status = 6;
            order.UpdatedBy = delivaryBoy.CustomerName;
            order.DateUpdated = DateTime.Now;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            //Reducing Platformcredits
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == orderNo);
            var shop = db.Shops.FirstOrDefault(i => i.Id == order.ShopId);
            var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
            shopCredits.DeliveryCredit -= payment.DeliveryCharge;
            db.Entry(shopCredits).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            var fcmToken = (from c in db.Customers
                            where c.Id == order.CustomerId
                            select c.FcmTocken ?? "").FirstOrDefault().ToString();
            Helpers.PushNotification.SendbydeviceId("Your order has been delivered.", "ShopNowChat", "a.mp3", fcmToken.ToString());
            return Json(new { message = "Successfully DelivaryBoy Delivered!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelivaryAssign(int orderNo, int deliveryBoyId, int customerId)
        {
            if (orderNo != 0 && deliveryBoyId != 0 && customerId != 0)
            {
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderNo);
                var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == deliveryBoyId);
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                order.DeliveryBoyId = deliveryBoy.Id;
                order.DeliveryBoyName = deliveryBoy.Name;
                order.DeliveryBoyPhoneNumber = deliveryBoy.PhoneNumber;
                order.Status = 4;
                order.UpdatedBy = customer.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                deliveryBoy.isAssign = 1;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //var detail = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
                //detail.CustomerId = deliveryBoy.CustomerId;
                //detail.CustomerName = deliveryBoy.CustomerName;
                //detail.DeliveryBoyId = deliveryBoy.Id;
                //detail.DeliveryBoyName = deliveryBoy.Name;
                //detail.Status = 4;
                //detail.UpdatedBy = customer.Name;
                //detail.DateUpdated = DateTime.Now;
                //db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();

                //delivery boy
                var fcmToken = (from c in db.Customers
                                where c.Id == deliveryBoyId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                Helpers.PushNotification.SendbydeviceId("You have a new Order. Accept Soon.", "ShopNowChat", "../../assets/b.mp3", fcmToken.ToString());

                //Customer
                var fcmTokenCustomer = (from c in db.Customers
                                        where c.Id == order.Id
                                        select c.FcmTocken ?? "").FirstOrDefault().ToString();
                Helpers.PushNotification.SendbydeviceId("Delivery Boy is Assigned for your Order.", "ShopNowChat", "../../assets/b.mp3", fcmToken.ToString());

                return Json(new { message = "Successfully DelivaryBoy Assign!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to DeliveryBoy Assign!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDelivaryHistory(string phoneNumber)
        {
            var model = new CartDelivaredListApiViewModel();
            model.List = db.Orders.Where(j => j.Status == 6)
               .Where(i => i.DeliveryBoyPhoneNumber == phoneNumber).Select(i => new CartDelivaredListApiViewModel.CartList
               {
                   Id = i.Id,
                   ShopName = i.ShopName,
                   OrderNumber = i.OrderNumber,
                   CartStatus = i.Status,
                   Date = i.DateUpdated.ToString("dd-MMM-yyyy"),
                   Price = i.TotalPrice,
                   DateUpdated = i.DateUpdated
               }).OrderByDescending(i => i.DateUpdated).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetAllOrders(int customerId, int page = 1, int pageSize = 5)
        //{
        //    var model = new CartListApiViewModel();
        //   // model.List = db.Orders
        //   //     .Join(db.OrderItems, o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
        //   //     .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
        //   // .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
        //   //   .Join(db.Shops, py => py.rz.c.o.ShopId, s => s.Id, (py, s) => new { py, s })
        //   //  .AsEnumerable()
        //   //.Where(i => i.py.rz.c.o.CustomerId == customerId && i.py.rz.p.CreditType == 2)
        //   //.Select(i => new CartListApiViewModel.CartList
        //   //{
        //   //    Id = i.py.rz.c.o.Id,
        //   //    ProductId = i.py.rz.c.oi.ProductId,
        //   //    ShopPhoneNumber = i.s.PhoneNumber,
        //   //    PaymentMode = i.py.rz.p.PaymentMode,
        //   //    ShopId = i.py.rz.c.o.ShopId,
        //   //    ShopName = i.py.rz.c.o.ShopName,
        //   //    CustomerName = i.py.rz.c.o.CustomerName,
        //   //    ProductName = GetMasterProductName(i.py.pr.MasterProductId),
        //   //    OrderNumber = i.py.rz.c.o.OrderNumber,
        //   //    Price = i.py.rz.c.o.TotalPrice,
        //   //    DeliveryBoyId = i.py.rz.c.o.DeliveryBoyId,
        //   //    DeliveryBoyName = i.py.rz.c.o.DeliveryBoyName,
        //   //    DeliveryBoyPhoneNumber = i.py.rz.c.o.DeliveryBoyPhoneNumber,
        //   //    PhoneNumber = i.py.rz.c.o.ShopPhoneNumber,
        //   //    Otp = GetOtp(i.py.rz.c.o.OrderNumber),
        //   //    DeliveryAddress = i.py.rz.c.o.DeliveryAddress,
        //   //    PackingCharge = i.py.rz.c.o.Packingcharge,
        //   //    ConvinenientCharge = i.py.rz.c.o.Convinenientcharge,
        //   //    Amount = GetPayment(i.py.rz.c.o.OrderNumber).Amount,
        //   //    GrossDeliveryCharge = i.py.rz.c.o.DeliveryCharge,
        //   //    ShopDeliveryDiscount = i.py.rz.c.o.ShopDeliveryDiscount,
        //   //    NetDeliveryCharge = i.py.rz.c.o.NetDeliveryCharge,
        //   //    Qty = i.py.rz.c.o.TotalQuantity,
        //   //    OrderList = GetOrderList(i.py.rz.c.o.OrderNumber),
        //   //    Date = i.py.rz.c.o.DateEncoded.ToString("dd/MMM/yyyy HH:mm"),
        //   //    DateEncoded = i.py.rz.c.o.DateEncoded,
        //   //    CartStatus = i.py.rz.c.o.Status,
        //   //    RfAmount = i.py.rz.p.RefundAmount,
        //   //    RefundRemark = i.py.rz.p.RefundRemark
        //   //}).OrderBy(j => j.CartStatus).OrderByDescending(i => i.DateEncoded).ToList();
        //    int count = model.List.Count();
        //    int CurrentPage = page;
        //    int PageSize = pageSize;
        //    int TotalCount = count;
        //    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
        //    var items = model.List.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        //    var previous = CurrentPage - 1;
        //    var previousurl = apipath + "/Api/GetAllOrders?customerId=" + customerId + "&page=" + previous;
        //    var previousPage = CurrentPage > 1 ? previousurl : "No";
        //    var current = CurrentPage + 1;
        //    var nexturl = apipath + "/Api/GetAllOrders?customerId=" + customerId + "&page=" + current;
        //    var nextPage = CurrentPage < TotalPages ? nexturl : "No";
        //    var paginationMetadata = new
        //    {
        //        totalCount = TotalCount,
        //        pageSize = PageSize,
        //        currentPage = CurrentPage,
        //        totalPages = TotalPages,
        //        previousPage,
        //        nextPage
        //    };
        //    return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetShopReAssignOrders(int shopId, int page = 1, int pageSize = 5)
        {
            var model = new CartAcceptListApiViewModel();
            model.List = db.Orders.Where(j => j.Status == 3)
                 .Join(db.OrderItems, o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
                .Join(db.Shops, py => py.rz.c.o.ShopId, s => s.Id, (py, s) => new { py, s })
                   // .Join(db.ShopCharges, pay => pay.py.rz.c.o.OrderNumber, sc => sc.OrderNo, (pay, sc)=> new { pay, sc })
                   .AsEnumerable()
               .Where(i => (i.py.rz.p.PaymentResult == "success" || i.py.rz.p.PaymentMode == "Cash On Hand") && i.py.rz.p.ShopId == shopId)
               .Select(i => new CartAcceptListApiViewModel.CartList
               {
                   Id = i.py.rz.c.o.Id,
                   ProductId = i.py.rz.c.oi.ProductId,
                   ShopId = i.py.rz.c.o.ShopId,
                   ShopName = i.py.rz.c.o.ShopName,
                   OrderNumber = i.py.rz.c.o.OrderNumber,
                   PaymentMode = i.py.rz.p.PaymentMode,
                   CustomerName = i.py.rz.c.o.CustomerName,
                   ProductName = GetMasterProductName(i.py.pr.MasterProductId),
                   PhoneNumber = i.py.rz.c.o.ShopPhoneNumber,
                   DeliveryAddress = i.py.rz.c.o.DeliveryAddress,
                   ShopLatitude = i.s.Latitude,
                   ShopLongitude = i.s.Longitude,
                   PackingCharge = i.py.rz.c.o.Packingcharge,
                   ConvinenientCharge = i.py.rz.c.o.Convinenientcharge,
                   Amount = GetPayment(i.py.rz.c.o.OrderNumber).Amount,
                   //OriginalAmount = GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount == 0 ? GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).OriginalAmount : GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount,
                   GrossDeliveryCharge = i.py.rz.c.o.DeliveryCharge,
                   ShopDeliveryDiscount = i.py.rz.c.o.ShopDeliveryDiscount,
                   NetDeliveryCharge = i.py.rz.c.o.NetDeliveryCharge,
                   Qty = i.py.rz.c.o.TotalQuantity,
                   Date = i.py.rz.c.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
                   DateEncoded = i.py.rz.c.o.DateEncoded,
                   OrderList = GetOrderPendingList(i.py.rz.c.o.OrderNumber),
                   CartStatus = i.py.rz.c.o.Status
               }).OrderByDescending(i => i.DateEncoded).ToList();
            int count = model.List.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.List.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetShopAcceptOrders?shopId=" + shopId + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetShopAcceptOrders?shopId=" + shopId + "&page=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopAcceptOrders(int shopId, int mode = 0, int page = 1, int pageSize = 5)
        {
            //var model = new CartAcceptListApiViewModel();
            //if (mode == 0)
            //{
            //    model.List = db.Orders.Where(j => j.Status == 2)
            //     .Join(db.OrderItems, o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
            //    .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
            //    .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
            //    .Join(db.Shops, py => py.rz.c.o.ShopId, s => s.Id, (py, s) => new { py, s })
            //       .AsEnumerable()
            //       .Where(i => (i.py.rz.p.PaymentResult == "success" || i.py.rz.p.PaymentMode == "Cash On Hand" || i.py.rz.p.PaymentMode == "Online Payment" || i.py.rz.p.PaymentMode == "pending") && i.py.rz.p.ShopId == shopId)
            //       .Select(i => new CartAcceptListApiViewModel.CartList
            //       {
            //           Id = i.py.rz.c.o.Id,
            //           ProductId = i.py.rz.c.oi.ProductId,
            //           ShopId = i.py.rz.c.o.ShopId,
            //           ShopName = i.py.rz.c.o.ShopName,
            //           OrderNumber = i.py.rz.c.o.OrderNumber,
            //           PaymentMode = i.py.rz.p.PaymentMode,
            //           CustomerName = i.py.rz.c.o.CustomerName,
            //           ProductName = GetMasterProductName(i.py.pr.MasterProductId),
            //           PhoneNumber = i.py.rz.c.o.ShopPhoneNumber,
            //           DeliveryAddress = i.py.rz.c.o.DeliveryAddress,
            //           ShopLatitude = i.s.Latitude,
            //           ShopLongitude = i.s.Longitude,
            //           PackingCharge = i.py.rz.c.o.Packingcharge,
            //           ConvinenientCharge = i.py.rz.c.o.Convinenientcharge,
            //           Amount = GetPayment(i.py.rz.c.o.OrderNumber).Amount,
            //           GrossDeliveryCharge = i.py.rz.c.o.DeliveryCharge,
            //           ShopDeliveryDiscount = i.py.rz.c.o.ShopDeliveryDiscount,
            //           NetDeliveryCharge = i.py.rz.c.o.NetDeliveryCharge,
            //           Qty = i.py.rz.c.o.TotalQuantity,
            //           Date = i.py.rz.c.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
            //           DateEncoded = i.py.rz.c.o.DateEncoded,
            //           OrderList = GetOrderPendingList(i.py.rz.c.o.OrderNumber),
            //           CartStatus = i.py.rz.c.o.Status
            //       }).OrderByDescending(i => i.DateEncoded).ToList();
            //}
            //else
            //{
            //    model.List = db.Orders.Where(j => j.Status == 3 || j.Status == 4)
            //     .Join(db.OrderItems, o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
            //    .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
            //    .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
            //    .Join(db.Shops, py => py.rz.c.o.ShopId, s => s.Id, (py, s) => new { py, s })
            //       .AsEnumerable()
            //      .Where(i => (i.py.rz.p.PaymentResult == "success" || i.py.rz.p.PaymentMode == "Cash On Hand" || i.py.rz.p.PaymentMode == "Online Payment" || i.py.rz.p.PaymentMode == "pending") && i.py.rz.p.ShopId == shopId)
            //      .Select(i => new CartAcceptListApiViewModel.CartList
            //      {
            //          Id = i.py.rz.c.o.Id,
            //          ProductId = i.py.rz.c.oi.ProductId,
            //          ShopId = i.py.rz.c.o.ShopId,
            //          ShopName = i.py.rz.c.o.ShopName,
            //          OrderNumber = i.py.rz.c.o.OrderNumber,
            //          PaymentMode = i.py.rz.p.PaymentMode,
            //          CustomerName = i.py.rz.c.o.CustomerName,
            //          ProductName = GetMasterProductName(i.py.pr.MasterProductId),
            //          PhoneNumber = i.py.rz.c.o.ShopPhoneNumber,
            //          DeliveryAddress = i.py.rz.c.o.DeliveryAddress,
            //          ShopLatitude = i.s.Latitude,
            //          ShopLongitude = i.s.Longitude,
            //          PackingCharge = i.py.rz.c.o.Packingcharge,
            //          ConvinenientCharge = i.py.rz.c.o.Convinenientcharge,
            //          Amount = GetPayment(i.py.rz.c.o.OrderNumber).Amount,
            //          GrossDeliveryCharge = i.py.rz.c.o.DeliveryCharge,
            //          ShopDeliveryDiscount = i.py.rz.c.o.ShopDeliveryDiscount,
            //          NetDeliveryCharge = i.py.rz.c.o.NetDeliveryCharge,
            //          Qty = i.py.rz.c.o.TotalQuantity,
            //          Date = i.py.rz.c.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
            //          DateEncoded = i.py.rz.c.o.DateEncoded,
            //          OrderList = GetOrderPendingList(i.py.rz.c.o.OrderNumber),
            //          CartStatus = i.py.rz.c.o.Status
            //      }).OrderByDescending(i => i.DateEncoded).ToList();
            //}
            db.Configuration.ProxyCreationEnabled = false;
            var model = new GetAllOrderListViewModel();
            model.OrderLists = db.Orders.Where(i => i.ShopId == shopId && (mode == 0 ? i.Status == 2 : (i.Status == 3 || i.Status == 4)))
                 .Join(db.Payments, o => o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
                 .GroupJoin(db.OrderItems, o => o.o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                 .Select(i => new GetAllOrderListViewModel.OrderList
                 {
                     Convinenientcharge = i.o.o.Convinenientcharge,
                     CustomerId = i.o.o.CustomerId,
                     CustomerName = i.o.o.CustomerName,
                     CustomerPhoneNumber = i.o.o.CustomerPhoneNumber,
                     //DateStr = i.o.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
                     DateEncoded = i.o.o.DateEncoded,
                     DeliveryAddress = i.o.o.DeliveryAddress,
                     DeliveryBoyId = i.o.o.DeliveryBoyId,
                     DeliveryBoyName = i.o.o.DeliveryBoyName,
                     DeliveryBoyPhoneNumber = i.o.o.DeliveryBoyPhoneNumber,
                     DeliveryCharge = i.o.o.DeliveryCharge,
                     Id = i.o.o.Id,
                     NetDeliveryCharge = i.o.o.NetDeliveryCharge,
                     OrderNumber = i.o.o.OrderNumber,
                     Packingcharge = i.o.o.Packingcharge,
                     PenaltyAmount = i.o.o.PenaltyAmount,
                     PenaltyRemark = i.o.o.PenaltyRemark,
                     ShopDeliveryDiscount = i.o.o.ShopDeliveryDiscount,
                     ShopId = i.o.o.ShopId,
                     ShopName = i.o.o.ShopName,
                     ShopOwnerPhoneNumber = i.o.o.ShopOwnerPhoneNumber,
                     ShopPhoneNumber = i.o.o.ShopPhoneNumber,
                     Status = i.o.o.Status,
                     TotalPrice = i.o.o.TotalPrice,
                     TotalProduct = i.o.o.TotalProduct,
                     TotalQuantity = i.o.o.TotalQuantity,
                     NetTotal = i.o.o.NetTotal,
                     WaitingCharge = i.o.o.WaitingCharge,
                     WaitingRemark = i.o.o.WaitingRemark,
                     RefundAmount = i.o.p.RefundAmount,
                     RefundRemark = i.o.p.RefundRemark,
                     OrderItemList = i.oi.ToList(),
                     PaymentMode = i.o.p.PaymentMode
                 }).ToList();

            int count = model.OrderLists.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.OrderLists.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetShopAcceptOrders?shopId=" + shopId + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetShopAcceptOrders?shopId=" + shopId + "&page=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopAllOrders(int shopId, int page = 1, int pageSize = 5)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //var model = new CartListApiViewModel();
            //model.List = db.Orders.Where(j => (j.Status == 4 || j.Status == 3))
            //    .Join(db.OrderItems, o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
            //    .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
            //    .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
            //    .Join(db.Shops, py => py.rz.c.o.ShopId, s => s.Id, (py, s) => new { py, s })
            //        //.Join(db.ShopCharges, pay => pay.py.rz.c.o.OrderNumber, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
            //        //.Join(db.DeliveryBoys, ca => ca.pay.py.rz.c.DeliveryBoyCode, db => db.Code, (ca, db) => new { ca, db })
            //        .AsEnumerable()
            //   .Where(i => (i.py.rz.p.PaymentResult == "success" || i.py.rz.p.PaymentResult == "pending" || i.py.rz.p.PaymentMode == "Cash On Hand" || i.py.rz.p.PaymentMode == "Online Payment") && i.py.rz.c.o.ShopId == shopId)
            //   .Select(i => new CartListApiViewModel.CartList
            //   {
            //       Id = i.py.rz.c.o.Id,
            //       ProductId = i.py.rz.c.oi.ProductId,
            //       ShopId = i.py.rz.c.o.ShopId,
            //       ShopName = i.py.rz.c.o.ShopName,
            //       OrderNumber = i.py.rz.c.o.OrderNumber,
            //       PaymentMode = i.py.rz.p.PaymentMode,
            //       CustomerName = i.py.rz.c.o.CustomerName,
            //       ProductName = GetMasterProductName(i.py.pr.MasterProductId),
            //       PhoneNumber = i.py.rz.c.o.ShopPhoneNumber,
            //       DeliveryAddress = i.py.rz.c.o.DeliveryAddress,
            //       ShopLatitude = i.s.Latitude,
            //       ShopLongitude = i.s.Longitude,
            //       PackingCharge = i.py.rz.c.o.Packingcharge,
            //       ConvinenientCharge = i.py.rz.c.o.Convinenientcharge,
            //       Amount = GetPayment(i.py.rz.c.o.OrderNumber).Amount,
            //       //OriginalAmount = GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount == 0 ? GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).OriginalAmount : GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount,
            //       GrossDeliveryCharge = i.py.rz.c.o.DeliveryCharge,
            //       ShopDeliveryDiscount = i.py.rz.c.o.ShopDeliveryDiscount,
            //       NetDeliveryCharge = i.py.rz.c.o.NetDeliveryCharge,
            //       Qty = i.py.rz.c.o.TotalQuantity,
            //       Date = i.py.rz.c.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
            //       DateEncoded = i.py.rz.c.o.DateEncoded,
            //       OrderList = GetOrderPendingList(i.py.rz.c.o.OrderNumber),
            //       CartStatus = i.py.rz.c.o.Status,
            //       RfAmount = i.py.rz.p.RefundAmount,
            //       RefundRemark = i.py.rz.p.RefundRemark
            //   }).OrderByDescending(i => i.DateEncoded).ToList();


            var model = new GetAllOrderListViewModel();
            model.OrderLists = db.Orders.Where(i => i.ShopId == shopId && (i.Status == 3 || i.Status == 4 || i.Status == 5))
                 .Join(db.Payments, o => o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
                 .GroupJoin(db.DeliveryBoys, o => o.o.DeliveryBoyId, d => d.Id, (o, d) => new { o, d })
                 .GroupJoin(db.OrderItems, o => o.o.o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                 .Select(i => new GetAllOrderListViewModel.OrderList
                 {
                     Convinenientcharge = i.o.o.o.Convinenientcharge,
                     CustomerId = i.o.o.o.CustomerId,
                     CustomerName = i.o.o.o.CustomerName,
                     CustomerPhoneNumber = i.o.o.o.CustomerPhoneNumber,
                     DateEncoded = i.o.o.o.DateEncoded,
                     DeliveryAddress = i.o.o.o.DeliveryAddress,
                     DeliveryBoyId = i.o.o.o.DeliveryBoyId,
                     DeliveryBoyName = i.o.o.o.DeliveryBoyName,
                     DeliveryBoyPhoneNumber = i.o.o.o.DeliveryBoyPhoneNumber,
                     DeliveryCharge = i.o.o.o.DeliveryCharge,
                     Id = i.o.o.o.Id,
                     NetDeliveryCharge = i.o.o.o.NetDeliveryCharge,
                     OrderNumber = i.o.o.o.OrderNumber,
                     Packingcharge = i.o.o.o.Packingcharge,
                     PenaltyAmount = i.o.o.o.PenaltyAmount,
                     PenaltyRemark = i.o.o.o.PenaltyRemark,
                     ShopDeliveryDiscount = i.o.o.o.ShopDeliveryDiscount,
                     ShopId = i.o.o.o.ShopId,
                     ShopName = i.o.o.o.ShopName,
                     ShopOwnerPhoneNumber = i.o.o.o.ShopOwnerPhoneNumber,
                     ShopPhoneNumber = i.o.o.o.ShopPhoneNumber,
                     Status = i.o.o.o.Status,
                     TotalPrice = i.o.o.o.TotalPrice,
                     TotalProduct = i.o.o.o.TotalProduct,
                     TotalQuantity = i.o.o.o.TotalQuantity,
                     NetTotal = i.o.o.o.NetTotal,
                     WaitingCharge = i.o.o.o.WaitingCharge,
                     WaitingRemark = i.o.o.o.WaitingRemark,
                     RefundAmount = i.o.o.p.RefundAmount,
                     RefundRemark = i.o.o.p.RefundRemark,
                     OrderItemList = i.oi.ToList(),
                     PaymentMode = i.o.o.p.PaymentMode,
                     Onwork = i.o.d.Any() ? i.o.d.FirstOrDefault().OnWork : 0
                 }).ToList();

            int count = model.OrderLists.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.OrderLists.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetShopAllOrders?shopId=" + shopId + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetShopAllOrders?shopId=" + shopId + "&str=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ShopRegister(ShopCreateViewModel model)
        {
            //var shopExist = db.Shops.Where(m => m.OwnerPhoneNumber == model.OwnerPhoneNumber && m.Latitude == model.Latitude && m.Longitude == model.Longitude && (m.Status == 0 || m.Status == 6));
            bool isExist = db.Shops.Any(i => i.Name == model.Name && (i.Status == 1 || i.Status == 0));
            if (!isExist)
            {
                var shop = _mapper.Map<ShopCreateViewModel, Shop>(model);
                shop.CreatedBy = model.CustomerName;
                shop.UpdatedBy = model.CustomerName;
                var number = model.PhoneNumber.Trim();
                var txt = number.Replace(" ", "");
                if (txt.Length == 13) { shop.PhoneNumber = txt.Substring(3); }
                else if (txt.Length == 12) { shop.PhoneNumber = txt.Substring(2); }
                else if (txt.Length == 11) { shop.PhoneNumber = txt.Substring(1); }
                else
                {
                    shop.PhoneNumber = txt;
                }
                shop.DateEncoded = DateTime.Now;
                shop.DateUpdated = DateTime.Now;
                shop.Status = 1;
                db.Shops.Add(shop);
                db.SaveChanges();
                if (model.AuthorisedBrandName != null)
                {
                    var bran = db.Brands.FirstOrDefault(i => i.Name == model.AuthorisedBrandName);
                    if (bran != null)
                    {
                        shop.AuthorisedBrandId = bran.Id;
                    }
                    else
                    {
                        var brand = new Brand();
                        brand.Name = model.AuthorisedBrandName;
                        brand.Status = -1;
                        brand.DateEncoded = DateTime.Now;
                        brand.DateUpdated = DateTime.Now;
                        db.Brands.Add(brand);
                        db.SaveChanges();
                        shop.AuthorisedBrandId = brand.Id;
                    }
                }
                var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                customer.Position = 1;
                customer.UpdatedBy = customer.Name;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var otpmodel = new OtpVerification();
                otpmodel.ShopId = shop.Id;
                otpmodel.CustomerId = customer.Id;
                otpmodel.CustomerName = customer.Name;
                otpmodel.PhoneNumber = model.PhoneNumber;
                otpmodel.Otp = _generatedCode;
                otpmodel.ReferenceCode = _referenceCode;
                otpmodel.Verify = false;
                otpmodel.CreatedBy = customer.Name;
                otpmodel.UpdatedBy = customer.Name;
                otpmodel.Status = 0;
                otpmodel.DateEncoded = DateTime.Now;
                otpmodel.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(otpmodel);
                db.SaveChanges();
                if (shop != null)
                {
                    return Json(new { message = "Successfully Registered Your Shop!", Details = shop, Otp = otpmodel.Otp, Position = customer.Position });
                }
                else
                    return Json(new { message = "Your Shop Registration Failed!" });
            }
            else
                return Json(new { message = "This Shop Already Exist!" });
        }

        [HttpPost]
        public JsonResult ShopUpdate(ShopUpdateViewModel model)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, shop);
            if (model.AuthorisedBrandName != null)
            {
                var bran = db.Brands.FirstOrDefault(i => i.Name == model.AuthorisedBrandName);
                if (bran != null)
                {
                    shop.AuthorisedBrandId = bran.Id;
                }
                else
                {
                    var brand = new Brand();
                    brand.Name = model.AuthorisedBrandName;
                    brand.Status = -1;
                    brand.DateEncoded = DateTime.Now;
                    brand.DateUpdated = DateTime.Now;
                    db.Brands.Add(brand);
                    db.SaveChanges();
                    shop.AuthorisedBrandId = brand.Id;
                }
            }
            shop.UpdatedBy = shop.CustomerName;
            shop.DateUpdated = DateTime.Now;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            if (shop.Id != 0)
            {
                return Json(new { message = "Successfully Updated Your Shop!", Details = shop });
            }
            else
                return Json(new { message = "Your Shop Updation Failed!" });
        }

        public JsonResult GetNextVerify(string OwnerPhoneNumber, string PhoneNumber, int shopId)
        {
            var otpmodel = new OtpVerification();
            var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == OwnerPhoneNumber);
            otpmodel.CustomerId = customer.Id;
            otpmodel.CustomerName = customer.Name;
            otpmodel.ShopId = shopId;
            otpmodel.PhoneNumber = PhoneNumber;
            otpmodel.Otp = _generatedCode;
            otpmodel.ReferenceCode = _referenceCode;
            otpmodel.Verify = false;
            otpmodel.CreatedBy = customer.Name;
            otpmodel.UpdatedBy = customer.Name;
            otpmodel.Status = 0;
            otpmodel.DateEncoded = DateTime.Now;
            otpmodel.DateUpdated = DateTime.Now;
            db.OtpVerifications.Add(otpmodel);
            db.SaveChanges();
            return Json(new { message = "Your Todays OTP is: " + otpmodel.Otp }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopDetailsNew(int shopId = 0, int categoryId = 0, string str = "")
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == shopId);
            //shop.Code = ss[0].Code;
            //shop.Address = ss[0].Address;
            //shop.CustomerReview = ss[0].CustomerReview;
            //shop.Rating = ss[0].Rating;
            //shop.PhoneNumber = ss[0].PhoneNumber;
            //shop.Name = ss[0].Name;
            //shop.ShopCategoryCode = ss[0].ShopCategoryCode;
            ShopDetails model = _mapper.Map<Shop, ShopDetails>(shop);
            var rate = db.CustomerReviews.Where(j => j.ShopId == shop.Id).ToList();
            var reviewCount = db.CustomerReviews.Where(j => j.ShopId == shop.Id).Count();
            if (reviewCount > 0)
                model.Rating = rate.Sum(l => l.Rating) / reviewCount;
            else
                reviewCount = 0;
            model.CustomerReview = reviewCount;
            model.CategoryLists = db.Database.SqlQuery<ShopDetails.CategoryList>($"select distinct CategoryId as Id, c.Name as Name from MasterProducts m join Categories c on c.Id = m.CategoryId join Products p on p.MasterProductId = m.id where p.ShopId = {shopId}  and c.Status = 0 and CategoryId !=0 and c.Name is not null group by CategoryId,c.Name order by Name").ToList<ShopDetails.CategoryList>();
            if (shop.ShopCategoryId == 1)
            {
                model.ProductLists = (from pl in db.Products
                                      join m in db.MasterProducts on pl.MasterProductId equals m.Id
                                      join c in db.Categories on m.CategoryId equals c.Id
                                      //into cat
                                      //from c in cat.DefaultIfEmpty()
                                      where pl.ShopId == shopId && pl.Status == 0 && (categoryId != 0 ? m.CategoryId == categoryId : true)
                                      select new ShopDetails.ProductList
                                      {
                                          Id = pl.Id,
                                          Name = m.Name,
                                          ShopId = pl.ShopId,
                                          ShopName = pl.ShopName,
                                          CategoryId = c.Id,
                                          CategoryName = c.Name,
                                          ColorCode = m.ColorCode,
                                          Price = pl.Price,
                                          ImagePath = ((!string.IsNullOrEmpty(m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                                          Status = pl.Status,
                                          Customisation = pl.Customisation
                                      }).Where(i => str != "" ? i.Name.ToLower().Contains(str) : true).ToList();
            }
            else if (shop.ShopCategoryId == 2)
            {
                model.ProductLists = (from pl in db.Products
                                      join m in db.MasterProducts on pl.MasterProductId equals m.Id
                                      join nsc in db.NextSubCategories on m.NextSubCategoryId equals nsc.Id into cat
                                      from nsc in cat.DefaultIfEmpty()
                                      where pl.ShopId == shopId && pl.Status == 0 && m.Name.ToLower().Contains(str) && (categoryId != 0 ? m.CategoryId == categoryId : true)
                                      select new ShopDetails.ProductList
                                      {
                                          Id = pl.Id,
                                          Name = m.Name,
                                          ShopId = pl.ShopId,
                                          ShopName = pl.ShopName,
                                          CategoryId = nsc.Id,
                                          CategoryName = nsc.Name,
                                          ColorCode = m.ColorCode,
                                          Price = pl.Price,
                                          ImagePath = ((!string.IsNullOrEmpty(m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                                          Status = pl.Status,
                                          Customisation = pl.Customisation
                                      }).ToList();
            }
            return new JsonResult()
            {
                ContentType = "application/json",
                Data = model,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        public JsonResult GetShopDetails(int id, int categoryId, string str = "")
        {
            Shop shop = new Shop();
            var ss = (from p in db.Shops
                      where p.Id == id
                      select new
                      {
                          Address = p.Address,
                          id = p.Id,
                          Rating = p.Rating,
                          CustomerReview = p.CustomerReview,
                          Name = p.Name,
                          PhoneNumber = p.PhoneNumber,
                          ShopCategoryId = p.ShopCategoryId
                      }).ToList();
            shop.Id = ss[0].id;
            shop.Address = ss[0].Address;
            shop.CustomerReview = ss[0].CustomerReview;
            shop.Rating = ss[0].Rating;
            shop.PhoneNumber = ss[0].PhoneNumber;
            shop.Name = ss[0].Name;
            shop.ShopCategoryId = ss[0].ShopCategoryId;
            ShopDetails model = _mapper.Map<Shop, ShopDetails>(shop);
            model.CategoryLists = db.Database.SqlQuery<ShopDetails.CategoryList>($"select distinct CategoryCode as Code, CategoryName as Name from Products p join Categories c on c.Code = p.CategoryCode where shopid ={id}  and c.Status = 0 and CategoryCode is not null and CategoryName is not null group by CategoryCode,CategoryName order by Name").ToList<ShopDetails.CategoryList>();
            if (shop.ShopCategoryId == 0)
            {
                //model.ProductLists = db.Products.Where(i => i.ShopCode == code && i.Status == 0).ToList().Where(i => str != "" ? i.Name.ToLower().StartsWith(str.ToLower()) : true && categoryCode != "" ? i.CategoryCode == categoryCode : true).AsQueryable().ProjectTo<ShopDetails.ProductList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();
                model.ProductLists = (from pl in db.Products
                                      join m in db.MasterProducts on pl.MasterProductId equals m.Id
                                      where pl.ShopId == id && pl.Status == 0 && (categoryId != 0 ? pl.ShopCategoryId == categoryId : true)
                                      select new ShopDetails.ProductList
                                      {
                                          Id = pl.Id,
                                          Name = m.Name,
                                          ShopId = pl.ShopId,
                                          ShopName = pl.ShopName,
                                          CategoryId = m.CategoryId,
                                          //CategoryName = m.CategoryName,
                                          ColorCode = m.ColorCode,
                                          Price = pl.Price,
                                          ImagePath = m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23"),
                                          Status = pl.Status

                                      }).Where(i => str != "" ? i.Name.ToLower().Contains(str) : true).ToList();
            }
            else if (shop.ShopCategoryId == 1)
            {
                model.ProductLists = (from pl in db.Products
                                      join m in db.MasterProducts on pl.MasterProductId equals m.Id
                                      where pl.ShopId == id && pl.Status == 0 && m.Name.ToLower().Contains(str) && (categoryId != 0 ? pl.ShopCategoryId == categoryId : true)
                                      select new ShopDetails.ProductList
                                      {
                                          Id = pl.Id,
                                          Name = m.Name,
                                          ShopId = pl.ShopId,
                                          ShopName = pl.ShopName,
                                          CategoryId = m.CategoryId,
                                          // CategoryName = m.CategoryName,
                                          //ColorCode = pl.ColorCode,
                                          Price = pl.Price,
                                          ImagePath = m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23"),
                                          Status = pl.Status
                                      }).ToList();
            }
            return new JsonResult()
            {
                ContentType = "application/json",
                Data = model,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
            // return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerRefered(int CustomerId)
        {
            var referealCount = db.Customers.Where(c => c.ReferralNumber != null).Count();
            if (referealCount <= 0)
                return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductList(double latitude, double longitude, string str = "", int page = 1, int pageSize = 10)
        {
            var model = new ProductSearchViewModel();
            double? varlongitude = longitude;
            double? varlatitude = latitude;
            int? varpage = page;
            int? varPagesize = pageSize;
            var s = db.GetProductList(varlongitude, varlatitude, str, varpage, varPagesize).ToList();
            string queryOtherList = "SELECT  * " +
             " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8  and Status = 0 and Latitude != 0 and Longitude != 0" +
             "and Name like '%'+@str+'%' order by Rating";
            model.ShopList = db.Shops.SqlQuery(queryOtherList,
                 new SqlParameter("Latitude", latitude),
                 new SqlParameter("Longitude", longitude), new SqlParameter("str", str)).Select(i => new ProductSearchViewModel.ShopLists
                 {
                     ImagePath = i.ImagePath,
                     ShopId = i.Id,
                     ShopName = i.Name,
                     Latitude = i.Latitude,
                     Longitude = i.Longitude,
                     DistrictName = i.DistrictName,
                     Rating = i.Rating,
                     ShopCategoryId = i.ShopCategoryId,
                     ShopOnline = i.IsOnline,
                     ShopStatus = i.Status
                 }).ToList();
            var productrCount = db.GetProductListCount(varlongitude, varlatitude, str).ToList();
            int count = 0;
            if (productrCount.Count > 0)
                count = Convert.ToInt32(productrCount[0]);
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = s;
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetProductList?latitude=" + latitude + "&longitude=" + longitude + "&str=" + str + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetProductList?latitude=" + latitude + "&longitude=" + longitude + "&str=" + str + "&page=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            //int count1 = model.ShopList.Count();
            //int CurrentPage1 = page;
            //int PageSize1 = pageSize;
            //int TotalCount1 = count1;
            //int TotalPages1 = (int)Math.Ceiling(count1 / (double)PageSize1);
            //var items1 = model.ShopList.Skip((CurrentPage1 - 1) * PageSize1).Take(PageSize1).ToList();
            //var previous1 = CurrentPage1 - 1;
            //var previousurl1 = apipath + "/Api/GetProductList?latitude=" + latitude + "&longitude=" + longitude + "&str=" + str + "&page=" + previous;
            //var previousPage1 = CurrentPage1 > 1 ? previousurl1 : "No";
            //var current1 = CurrentPage1 + 1;
            //var nexturl1 = apipath + "/ Api/GetProductList?latitude=" + latitude + "&longitude=" + longitude + "&str=" + str + "&page=" + current;
            //var nextPage1 = CurrentPage1 < TotalPages1 ? nexturl1 : "No";
            //var paginationMetadata1 = new
            //{
            //    totalCount = TotalCount1,
            //    pageSize = PageSize1,
            //    currentPage = CurrentPage1,
            //    totalPages = TotalPages1,
            //    previousPage1,
            //    nextPage1
            //};
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetP(string str = "")
        {
            var apiSettings = db.ApiSettings.Where(m => m.Status == 0).ToList();

            if (apiSettings.Count > 0) {
                foreach (var api in apiSettings)
                {
                    string s = "";
                    string Url =api.Url+ "items?q=status==R,outletId=="+api.OutletId+"&limit=100000";
                    using (WebClient client = new WebClient())
                    {
                        client.Headers["X-Auth-Token"] = api.AuthKey; //"62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";

                        s = client.DownloadString(Url);
                    }

                    var lst = db.Products.Where(m => m.ShopId == api.ShopId).Select(si => si.ItemId).ToList();
                    List<Product> updateList = new List<Product>();
                    List<Product> createList = new List<Product>();
                    Product varProduct = new Product();
                    var lstDiscount = (dynamic)null;
                    goto GetDiscoutCatecories;
                GetDiscoutCatecories:
                    lstDiscount = db.DiscountCategories.Where(m => m.ShopId == api.ShopId).Select(si => si.Percentage).ToList();

                    dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(s, new ExpandoObjectConverter());

                    foreach (var pro in ((IEnumerable<dynamic>)config.items).Where(t => t.status == "R"))
                    {
                        varProduct.Id = Convert.ToString(pro.itemId);
                        varProduct.Name = pro.itemName;
                        varProduct.IBarU = Convert.ToInt32(pro.iBarU);
                        varProduct.DateUpdated = DateTime.Now;
                        varProduct.ShopCategoryId = 4;
                        varProduct.ShopId = api.ShopId;

                        foreach (var med in pro.stock)
                        {
                            varProduct.Qty = Convert.ToInt32(Math.Floor(Convert.ToDecimal(med.stock)));
                            varProduct.MenuPrice = Convert.ToDouble(med.mrp);
                            varProduct.Price = Convert.ToDouble(med.salePrice);
                            varProduct.TaxPercentage = Convert.ToDouble(med.taxPercentage);
                            varProduct.ItemTimeStamp = Convert.ToString(med.itemTimeStamp);
                            varProduct.AppliesOnline = Convert.ToInt32(pro.appliesOnline);
                            varProduct.OutletId = Convert.ToInt32(pro.outletId);
                            varProduct.DiscountCategoryName = Convert.ToString(med.cat + api.Category);
                        }
                        int idx = lst.IndexOf(pro.itemId);
                        if (idx >= 0)
                        {
                            //update
                        }
                        else
                        {
                            //Add
                        }

                    }
                }
            }

            //// DownloadString (encoding specified)
            //using (WebClient client = new WebClient())
            //{
            //    client.Headers["X-Auth-Token"] = "62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";
            //    // specify encoding
            //    client.Encoding = System.Text.UTF8Encoding.UTF8;

            //    // output
            //    Console.WriteLine(client.DownloadString(Url));
            //}

            //// DownloadData (encoding specified)
            //using (WebClient client = new WebClient())
            //{
            //    client.Headers["X-Auth-Token"] = "62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";
            //    Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(client.DownloadData(Url)));
            //}
            return Json(new { Page =""}, JsonRequestBehavior.AllowGet);
        }

        public static double GetStockQty(string code)
        {
         
            using (WebClient myData = new WebClient())
            {
                myData.Headers["X-Auth-Token"] = "62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";
                myData.Headers[HttpRequestHeader.Accept] = "application/json";
                myData.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string getList = myData.DownloadString("http://joyrahq.gofrugal.com/RayMedi_HQ/api/v1/items?q=status==R,outletId==2,itemId==" + code);
                var result = JsonConvert.DeserializeObject<RootObject>(getList);
                foreach (var pro in result.items)
                {
                    foreach (var med in pro.stock)
                    {
                        return Convert.ToDouble(med.stock);
                    }
                }
            }
            return 0;
        }

        public JsonResult GetShopCategoryList(int shopId = 0, int CategoryId = 0, string str = "", int page = 1, int pageSize = 20)
        {
            //  var shid = db.Shops.Where(s => s.Id == shopId).FirstOrDefault();
            int ? count = 0;
            var total = db.GetShopCategoryProductCount(shopId, CategoryId, str).ToList();
            if (total.Count > 0)
             count = total[0]; 
            var skip = page - 1;
            var model = db.GetShopCategoryProducts(shopId, CategoryId, str, skip, pageSize).ToList();
            int CurrentPage = page;
            int PageSize = pageSize;
            int ? TotalCount = count;
            int  TotalPages = (int)Math.Ceiling(count.Value / (double)PageSize);
            var items = model;
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetShopCategoryList?shopId=" + shopId + "&categoryId=" + CategoryId + "&str=" + str + "&page=" + previous;
             var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetShopCategoryList?shopId=" + shopId + "&categoryId=" + CategoryId + "&str=" + str + "&page=" + current;
             var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }
        string GetMasterProductName(long id)

        {
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == id);
            var name = "";
            if (masterProduct != null)
            {
                name = masterProduct.Name != null ? masterProduct.Name : "N/A";
            }
            return name;
        }

        public JsonResult GetSingleShopDetails(int id)
        {
            var shid = db.Shops.Where(s => s.Id == id).FirstOrDefault();
            var shop = db.Shops.FirstOrDefault(i => i.Id == shid.Id);
            ShopSingleUpdateViewModel model = _mapper.Map<Shop, ShopSingleUpdateViewModel>(shop);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopBalanceNotification(int customerId)
        {
            var varShopOwner = db.Customers.Where(s => s.Id == customerId && s.Position == 1).FirstOrDefault();
            if (varShopOwner != null)
            {
                var varCustomer = db.Customers.Where(s => s.Id == customerId && s.Position == 1).FirstOrDefault();
                var customerName = (from c in db.Customers
                                    where c.Id == varCustomer.Id && c.Position == 1
                                    select c.Name).FirstOrDefault();

                var orderCount = (from s in db.Orders
                                  join sh in db.Shops on s.ShopId equals sh.Id
                                  join c in db.Customers on sh.CustomerId equals c.Id
                                  where sh.CustomerId == customerId && (s.Status >= 2)
                                  select s).Count();

                var platformcredits = (from ss in db.Payments
                                       where ss.CustomerId == customerId && ss.Status == 0 && ss.CreditType == 0
                                       select (Double?)ss.OriginalAmount).Sum() ?? 0;

                var platformorder = (Convert.ToInt32(orderCount) * (db.PlatFormCreditRates.FirstOrDefault().RatePerOrder));
                var varDelivery = (from ss in db.Payments
                                   where ss.CustomerId == customerId && ss.Status == 0 && ss.CreditType == 1
                                   select (Double?)ss.OriginalAmount).Sum() ?? 0;

                var varDeliveryCharges = (from ss in db.Orders
                                          join sh in db.Shops on ss.ShopId equals sh.Id
                                          where sh.CustomerId == customerId && ss.Status >= 2
                                          select (Double?)ss.DeliveryCharge).Sum() ?? 0;

                if (platformcredits - platformorder <= 100)
                {
                    return Json(new { message = "Recharge Immediately" }, JsonRequestBehavior.AllowGet);
                }
                else if (varDelivery - varDeliveryCharges <= 150)
                {
                    return Json(new { message = "Recharge Immediately" }, JsonRequestBehavior.AllowGet);
                }
                else if (platformcredits - platformorder <= 200 && platformcredits - platformorder > 100)
                {
                    return Json(new { message = "Your Credit are Low !" }, JsonRequestBehavior.AllowGet);
                }
                else if (varDelivery - varDeliveryCharges >= 150 && varDelivery - varDeliveryCharges <= 250)
                {
                    return Json(new { message = "Your Credits are Low !" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { message = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddReview(ShopReviewViewModel model)
        {
            var review = _mapper.Map<ShopReviewViewModel, CustomerReview>(model);
            //ClassCustomerReview.Add(review, out errorCode);
            review.DateEncoded = DateTime.Now;
            review.DateUpdated = DateTime.Now;
            review.CreatedBy = model.CustomerName;
            review.UpdatedBy = model.CustomerName;
            review.Status = 0;
            db.CustomerReviews.Add(review);
            db.SaveChanges();

            if (review != null)
            {
                return Json(new { message = "Successfully Added to Rating!", Details = model });
            }
            else
            {
                return Json(new { message = "Failure Added to Rating!", Details = model });
            }
        }

        [HttpPost]
        public JsonResult UpdateReview(ShopReviewUpdateViewModel model)
        {
            var review = db.CustomerReviews.FirstOrDefault(i => i.Id == model.Id);
            review.CustomerRemark = model.CustomerRemark;
            review.Rating = model.Rating;
            db.Entry(review).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { message = "Successfully Updated to Rating!", Details = model });

        }

        public JsonResult GetAllReview(int shopId, int customerId, int page = 1, int pageSize = 9)
        {
            var model = new ReviewListViewModel();
            model.CustomerList = db.CustomerReviews
                             .Where(i => i.Status == 0 && i.ShopId == shopId && i.CustomerId == customerId)
                         .Select(i => new ReviewListViewModel.ReviewlList
                         {
                             Id = i.Id,
                             ShopName = i.ShopName,
                             CustomerName = i.CustomerName,
                             CustomerRemark = i.CustomerRemark,
                             Rating = i.Rating
                         }).ToList();
            model.ReviewlLists = db.CustomerReviews
                             .Where(i => i.Status == 0 && i.ShopId == shopId && i.CustomerId != customerId)
                         .Select(i => new ReviewListViewModel.ReviewlList
                         {
                             Id = i.Id,
                             ShopName = i.ShopName,
                             CustomerName = i.CustomerName,
                             CustomerRemark = i.CustomerRemark,
                             Rating = i.Rating
                         }).ToList();
            int count = model.ReviewlLists.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.ReviewlLists.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetAllReview?shopId=" + shopId + "&customerId=" + customerId + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetAllReview?shopId=" + shopId + "&customerId=" + customerId + "&page=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items, model.CustomerList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ShopSingleUpdate(ShopSingleEditViewModel model)
        {
            // int errorCode = 0;
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.Id);
            shop.UpdatedBy = shop.CustomerName;
            shop.DateUpdated = DateTime.Now;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            if (shop.Id != 0)
            {
                return Json(new { message = "Successfully Updated Your Shop!", Details = shop });
            }
            else
                return Json(new { message = "Your Shop Updation Failed!" });
        }

        [HttpPost]
        public JsonResult SingleShopImgeUpdate(SingleShopImgeUpdateViewModel model)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.Id);
            shop.ImagePath = model.ImagePath;
            shop.UpdatedBy = shop.CustomerName;
            shop.DateUpdated = DateTime.Now;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { message = "Successfully Updated Your Shop Image!", Details = shop });
        }

        public JsonResult GetProductDetails(int id)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == id);
            ProductDetailsViewModel model = _mapper.Map<Product, ProductDetailsViewModel>(product);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductQuickUpdate(ProductQuickUpdateViewModel model)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == model.Id);
            product.Price = model.Price;
            product.MenuPrice = model.MenuPrice;
            product.Qty = model.Qty;
            if (model.CustomerId != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);

                product.UpdatedBy = customer.Name;
            }
            product.DateUpdated = DateTime.Now;
            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { message = "Successfully Updated Your Shop Image!", Details = product });
        }

        public JsonResult GetCustomerProfile(int customerId)
        {
            var customer = db.Customers.Where(i => i.Id == customerId && i.Status == 0).FirstOrDefault();
            CustomerProfileViewModel model = _mapper.Map<Models.Customer, CustomerProfileViewModel>(customer);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllShops(int customerId)
        {
            var model = new CustomerShopAllListViewModel();
            model.List = db.Shops.Where(i => i.CustomerId == customerId)
                 .GroupJoin(db.Customers, s => s.CustomerId, c => c.Id, (s, c) => new { s, c })
                .GroupJoin(db.OtpVerifications, ss => ss.s.Id, o => o.ShopId, (ss, o) => new { ss, o })
                             .AsEnumerable()
                            .Select(i => new CustomerShopAllListViewModel.ShopList
                            {
                                Id = i.ss.s.Id,
                                Name = i.ss.s.Name,
                                PhoneNumber = i.ss.s.PhoneNumber,
                                ShopCategoryId = i.ss.s.ShopCategoryId,
                                ShopCategoryName = i.ss.s.ShopCategoryName,
                                Otp = i.o.Any() ? i.o.LastOrDefault().Otp : "N/A",
                                Password = i.ss.c.FirstOrDefault().Password != null ? "Password Generated" : "Not Password Generated",
                                Status = i.ss.s.Status,
                                isOnline = i.ss.s.IsOnline,
                                Rating = i.ss.s.Rating,
                                ImagePath = i.ss.s.ImagePath,
                                DistrictName = i.ss.s.DistrictName,
                                Verify = i.ss.s.Verify,
                                OtpVerify = i.o.Any() ? i.o.LastOrDefault().Verify : false,
                                CustomerId = i.ss.s.CustomerId,
                                DateEncoded = i.o.Any() ? i.o.LastOrDefault().DateEncoded.ToString("dd/MMM/yyyy") : "N/A"
                            }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopInORActive(ShopActiveOrInViewModel model)
        {
            if (model.State == 0)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                shop.IsOnline = true;
                if (model.CustomerId != 0)
                {
                    var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                    shop.UpdatedBy = customer.Name;
                }
                shop.DateUpdated = DateTime.Now;
                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully Activated the Shop!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                shop.IsOnline = false;
                if (model.CustomerId != 0)
                {
                    var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                    shop.UpdatedBy = customer.Name;
                }
                shop.DateUpdated = DateTime.Now;
                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully InActivated the Shop!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllAddress(int customerId)
        {
            var model = new CustomerAddressListViewModel();
            model.List = db.CustomerAddresses.Where(i => i.CustomerId == customerId && i.Status == 0).ToList().AsQueryable().ProjectTo<CustomerAddressListViewModel.CustomerList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelivaryBoyStatus(int customerId)
        {
            var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.CustomerId == customerId && i.Status == 0);
            if (delivaryBoy.Active == 1)
            {
                return Json("You are Active", JsonRequestBehavior.AllowGet);
            }
            else
                return Json("You are InActive", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelivaryBoyActive(int customerId, int state)
        {
            if (state == 1 && (customerId != 0))
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.CustomerId == customerId && i.Status == 0);
                delivaryBoy.Active = 1;
                delivaryBoy.UpdatedBy = customer.Name;
                db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("You are Active", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.CustomerId == customerId && i.Status == 0);
                delivaryBoy.Active = 0;
                delivaryBoy.UpdatedBy = customer.Name;
                db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("You are InActive", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetNearDelivaryBoy(double Latitude, double Longitude)
        {
            var model = new DeliveryBoyApiListViewModel();
            string queryOtherList = "SELECT  * " +
            " FROM DeliveryBoys where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and OnWork = 0 and isAssign = 0 and Active = 1 and status=0 and Latitude != 0 and Longitude != 0";
            model.Lists = db.DeliveryBoys.SqlQuery(queryOtherList,
            new SqlParameter("Latitude", Latitude),
            new SqlParameter("Longitude", Longitude)).Select(i => new DeliveryBoyApiListViewModel.DeliveryBoyViewModel
            {
                Id = i.Id,
                Name = "D" + _generatedDelivaryId,
                Address = i.Address,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                DelivaryCustomerId = i.CustomerId,
                DeilvaryCustomerName = i.CustomerName,
                DeilvaryName = i.Name,
                DeilvaryPhoneNumber = i.PhoneNumber,
                Status = i.Status
            }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNearShops(double Latitude, double Longitude)
        {
            var model = new NearShopImages();
            string query = "SELECT * " +
                               " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and Status = 0 and Latitude != 0 and Longitude != 0" +
                               " order by IsOnline desc,Adscore desc,Rating desc";
            model.NearShops = db.Shops.SqlQuery(query,
             new SqlParameter("Latitude", Latitude),
             new SqlParameter("Longitude", Longitude)).Select(i => new NearShopImages.shops
             {
                 id = i.Id,
                 image = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",

             }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNearPlaces(double Latitude, double Longitude, string a)
        {
            var model = new PlacesListView();
            string query = "SELECT * " +
                               " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId = 1 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
                               " order by IsOnline desc,Adscore desc,Rating desc";
            string querySuperMarketList = "SELECT * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId = 3 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by IsOnline desc,Adscore desc,Rating desc";
            string queryGroceriesList = "SELECT * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId = 2 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by IsOnline desc,Adscore desc,Rating desc";
            string queryHealthList = "SELECT * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId = 4 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by IsOnline desc,Adscore desc,Rating desc";
            string queryElectronicsList = "SELECT * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId = 5 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by IsOnline desc,Adscore desc,Rating desc";
            string qServicesList = "SELECT * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId = 6 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by IsOnline desc,Adscore desc,Rating desc";
            if (a == "-1")
            {
                string queryOtherList = "SELECT * " +
                " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId = 7 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
                " order by IsOnline desc,Adscore desc,Rating desc";
                model.ResturantList = db.Shops.SqlQuery(query,
                 new SqlParameter("Latitude", Latitude),
                 new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                 {
                     Id = i.Id,
                     Name = i.Name,
                     DistrictName = i.StreetName,
                     //  Rating = RatingCalculation(i.Id),
                     ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                     ShopCategoryId = i.ShopCategoryId,
                     ShopCategoryName = i.ShopCategoryName,
                     List = GetBannerImageList(i.Id),
                     Latitude = i.Latitude,
                     Longitude = i.Longitude,
                     Status = i.Status,
                     isOnline = i.IsOnline,
                     ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                     Address = i.Address
                 }).ToList();
                model.SuperMarketList = db.Shops.SqlQuery(querySuperMarketList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                model.GroceriesList = db.Shops.SqlQuery(queryGroceriesList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    //Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                model.HealthList = db.Shops.SqlQuery(queryHealthList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();

                model.ElectronicsList = db.Shops.SqlQuery(queryElectronicsList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    //Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                model.ServicesList = db.Shops.SqlQuery(qServicesList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    //Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                model.OtherList = db.Shops.SqlQuery(queryOtherList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    List = GetBannerImageList(i.Id),
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else if (a == "0")
            {
                model.OtherList = db.Shops.SqlQuery(query,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    // Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else if (a == "1")
            {
                model.OtherList = db.Shops.SqlQuery(queryGroceriesList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    //Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),// db.Banners.Where(j => j.Status == 0 && j.ShopCode == i.Code).ToList(),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else if (a == "2")
            {
                model.OtherList = db.Shops.SqlQuery(querySuperMarketList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    //Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else if (a == "3")
            {
                model.OtherList = db.Shops.SqlQuery(queryHealthList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    // Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else if (a == "4")
            {
                model.OtherList = db.Shops.SqlQuery(queryElectronicsList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    //Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else if (a == "5")
            {
                model.OtherList = db.Shops.SqlQuery(qServicesList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    //Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline
                }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string queryOtherList = "SELECT top(6) * " +
              " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId = 7 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0 " +
              " order by Rating";
                model.OtherList = db.Shops.SqlQuery(queryOtherList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    //Rating = RatingCalculation(i.Code),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address
                }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        #region Reports
        public JsonResult GetShopOrderReport(int shopId, string dt, string from, string to, int page = 1, int pageSize = 5)
        {
            var model = new ShopOrderAmountApiViewModel();
            if (dt == "1")
            {
                dt = DateTime.Now.ToString("dd-MMM-yyyy");
                model.List = db.Orders.Where(i => i.Status == 6)
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                     //.Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.c.ShopId == shopId && i.c.DateEncoded.ToString("dd-MMM-yyyy") == dt)
                   .Select(i => new ShopOrderAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       ShopPaymentStatus = i.c.ShopPaymentStatus,
                       Amount = i.p.OriginalAmount.ToString(),
                       Date = i.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).ToList();
                if (model.List.Count() != 0)
                {
                    model.TotalAmount = model.List.Sum(i => Convert.ToDouble(i.Amount));
                    model.ShopPaymentStatus = model.List.Any() ? model.List.FirstOrDefault().ShopPaymentStatus : -5;
                }
            }
            else if (dt == "2")
            {
                DateTime from1 = DateTime.Parse(from);
                DateTime to1 = DateTime.Parse(to);
                model.List = db.Orders.Where(i => i.Status == 6)
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                     //.Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.c.ShopId == shopId && i.c.DateEncoded >= from1 && i.c.DateEncoded <= to1)
                   .Select(i => new ShopOrderAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       ShopPaymentStatus = i.c.ShopPaymentStatus,
                       Amount = i.p.OriginalAmount.ToString(),
                       Date = i.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).ToList();
                if (model.List.Count() != 0)
                {
                    model.TotalAmount = model.List.Sum(i => Convert.ToDouble(i.Amount));
                    model.ShopPaymentStatus = model.List.FirstOrDefault().ShopPaymentStatus;
                }
            }
            else
            {
                model.List = db.Orders.Where(i => i.Status == 6)
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                     // .Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.c.ShopId == shopId && i.c.DateEncoded.ToString("dd-MMM-yyyy") == dt)
                   .Select(i => new ShopOrderAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       ShopPaymentStatus = i.c.ShopPaymentStatus,
                       Amount = i.p.OriginalAmount.ToString(),
                       Date = i.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).ToList();
                if (model.List.Count() != 0)
                {
                    model.TotalAmount = model.List.Sum(i => Convert.ToDouble(i.Amount));
                    model.ShopPaymentStatus = model.List.FirstOrDefault().ShopPaymentStatus;
                }
            }
            int count = model.List.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.List.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = "https://admin.shopnowchat.in/Api/GetShopOrderReport?shopId=" + shopId + "&dt=" + dt + "&from=" + from + "&to=" + to + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = "https://admin.shopnowchat.in/Api/GetShopOrderReport?shopId=" + shopId + "&dt=" + dt + "&from=" + from + "&to=" + to + "&page=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelivaryBoyFullReport(string phoneNumber)
        {
            var model = new DelivaryCreditAmountApiViewModel();
            model.List = db.Orders.Where(i => i.Status == 6)
                 .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                  .AsEnumerable()
                .Where(i => i.c.DeliveryBoyPhoneNumber == phoneNumber && i.p.PaymentMode != "Online Payment" && i.c.DeliveryOrderPaymentStatus == 0)
                .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                {
                    Amount = (i.p.Amount - (i.p.RefundAmount ?? 0)).ToString()

                }).ToList();
            if (model.List.Count() != 0)
            {
                model.TotalAmount = model.List.Sum(i => Convert.ToDouble(i.Amount));
                model.TargetAmount = 1500.00;
            }
            return Json(new { model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelivaryBoyReport(string phoneNumber, string dt, string from, string to, int page = 1, int pageSize = 5)
        {
            var model = new DelivaryBoyReportViewModel();
            if (dt == "1")
            {
                dt = DateTime.Now.ToString("dd-MMM-yyyy");
                model.List = db.Orders.Where(i => i.Status == 6)
                            .Join(db.Shops, scc => scc.ShopId, s => s.Id, (scc, s) => new { scc, s })
                     .AsEnumerable()
                   .Where(i => i.scc.DeliveryBoyPhoneNumber == phoneNumber && i.scc.DateEncoded.ToString("dd-MMM-yyyy") == dt)
                   .Select(i => new DelivaryBoyReportViewModel.CartList
                   {
                       OrderNumber = i.scc.OrderNumber,
                       CartStatus = i.scc.Status,
                       GrossDeliveryCharge = i.scc.DeliveryCharge,
                       CustomerLatitude = i.scc.Latitude,
                       CustomerLongitude = i.scc.Longitude,
                       ShopLatitude = i.s.Latitude,
                       ShopLongitude = i.s.Longitude,
                       DateEncoded = i.scc.DateEncoded,
                       Date = i.scc.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).OrderByDescending(j => j.DateEncoded).ToList();
                if (model.List.Count() != 0)
                {
                    model.EarningOfToday = model.List.Sum(i => i.GrossDeliveryCharge);
                }
            }
            else if (dt == "2")
            {
                DateTime from1 = DateTime.Parse(from);
                DateTime to1 = DateTime.Parse(to);
                model.List = db.Orders.Where(i => i.Status == 6)
                            // .Join(db.ShopCharges, c => c.OrderNumber, sc => sc.OrderNo, (c, sc) => new { c, sc })
                            .Join(db.Shops, scc => scc.ShopId, s => s.Id, (scc, s) => new { scc, s })
                          .Where(i => ((DbFunctions.TruncateTime(i.scc.DateEncoded) >= DbFunctions.TruncateTime(from1)) &&
            (DbFunctions.TruncateTime(i.scc.DateEncoded) <= DbFunctions.TruncateTime(to1))))
                          .AsEnumerable()
                   .Select(i => new DelivaryBoyReportViewModel.CartList
                   {
                       OrderNumber = i.scc.OrderNumber,
                       CartStatus = i.scc.Status,
                       GrossDeliveryCharge = i.scc.DeliveryCharge,
                       CustomerLatitude = i.scc.Latitude,
                       CustomerLongitude = i.scc.Longitude,
                       ShopLatitude = i.s.Latitude,
                       ShopLongitude = i.s.Longitude,
                       DateEncoded = i.scc.DateEncoded,
                       Date = i.scc.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).OrderByDescending(j => j.DateEncoded).ToList();
                if (model.List.Count() != 0)
                {
                    model.EarningOfToday = model.List.Sum(i => i.GrossDeliveryCharge);
                }
            }
            else
            {
                model.List = db.Orders.Where(i => i.Status == 6)
                            // .Join(db.ShopCharges, c => c.OrderNumber, sc => sc.OrderNo, (c, sc) => new { c, sc })
                            .Join(db.Shops, scc => scc.ShopId, s => s.Id, (scc, s) => new { scc, s })
                     .AsEnumerable()
                   .Where(i => i.scc.DeliveryBoyPhoneNumber == phoneNumber && i.scc.DateEncoded.ToString("dd-MMM-yyyy") == dt)
                   .Select(i => new DelivaryBoyReportViewModel.CartList
                   {
                       OrderNumber = i.scc.OrderNumber,
                       CartStatus = i.scc.Status,
                       GrossDeliveryCharge = i.scc.DeliveryCharge,
                       CustomerLatitude = i.scc.Latitude,
                       CustomerLongitude = i.scc.Longitude,
                       ShopLatitude = i.s.Latitude,
                       ShopLongitude = i.s.Longitude,
                       DateEncoded = i.scc.DateEncoded,
                       Date = i.scc.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).OrderByDescending(j => j.DateEncoded).ToList();
                if (model.List.Count() != 0)
                {
                    model.EarningOfToday = model.List.Sum(i => i.GrossDeliveryCharge);
                }
            }
            int count = model.List.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.List.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = "https://admin.shopnowchat.in/Api/GetDelivaryBoyReport?phoneNumber=" + phoneNumber + "&dt=" + dt + "&from=" + from + "&to=" + to + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = "https://admin.shopnowchat.in/Api/GetDelivaryBoyReport?phoneNumber=" + phoneNumber + "&dt=" + dt + "&from=" + from + "&to=" + to + "&page=" + current;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelivaryCreditReport(string phoneNumber, string dt, string from, string to)
        {
            var model = new DelivaryCreditAmountApiViewModel();
            if (dt == "1")
            {
                dt = DateTime.Now.ToString("dd-MMM-yyyy");
                model.List = db.Orders.Where(i => i.Status == 6)
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                     //.Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.c.DeliveryBoyPhoneNumber == phoneNumber && i.c.DateEncoded.ToString("dd-MMM-yyyy") == dt && i.p.PaymentMode != "Online Payment" && i.c.DeliveryOrderPaymentStatus == 0)
                   .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       GrossDeliveryCharge = i.c.DeliveryCharge,
                       DeliveryBoyPaymentStatus = i.c.DeliveryBoyPaymentStatus,
                       Amount = i.p.Amount.ToString(),
                       Date = i.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).ToList();
                if (model.List.Count() != 0)
                {
                    model.TotalAmount = model.List.Sum(i => Convert.ToDouble(i.Amount));
                    model.EarningOfToday = model.List.Sum(i => i.GrossDeliveryCharge);
                    model.TargetAmount = 1500.00;
                    model.DeliveryPaymentStatus = model.List.Any() ? model.List.FirstOrDefault().DeliveryBoyPaymentStatus : -5;
                }
                else
                {
                    model.TotalAmount = 0.0;
                    model.EarningOfToday = 0.0;
                    model.TargetAmount = 1500.00;
                    model.DeliveryPaymentStatus = 1;
                }
            }
            else if (dt == "2")
            {
                DateTime from1 = DateTime.Parse(from);
                DateTime to1 = DateTime.Parse(to);
                model.List = db.Orders.Where(i => i.Status == 6)
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                     //.Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.c.DeliveryBoyPhoneNumber == phoneNumber && i.p.PaymentMode != "Online Payment" && i.c.DateEncoded >= from1 && i.c.DateEncoded <= to1 && i.c.DeliveryOrderPaymentStatus == 0)
                   .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       GrossDeliveryCharge = i.c.DeliveryCharge,
                       DeliveryBoyPaymentStatus = i.c.DeliveryBoyPaymentStatus,
                       Amount = i.p.Amount.ToString(),
                       Date = i.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).ToList();
                if (model.List.Count() != 0)
                {
                    model.TotalAmount = model.List.Sum(i => Convert.ToDouble(i.Amount));
                    model.EarningOfToday = model.List.Sum(i => i.GrossDeliveryCharge);
                    model.TargetAmount = 1500.00;
                    model.DeliveryPaymentStatus = model.List.FirstOrDefault().DeliveryBoyPaymentStatus;
                }
                else
                {
                    model.TotalAmount = 0.0;
                    model.EarningOfToday = 0.0;
                    model.TargetAmount = 1500.00;
                    model.DeliveryPaymentStatus = 1;
                }
            }
            else
            {
                model.List = db.Orders.Where(i => i.Status == 6)
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                    //.Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                    .AsEnumerable()
                   .Where(i => i.c.DeliveryBoyPhoneNumber == phoneNumber && i.c.DateEncoded.ToString("dd-MMM-yyyy") == dt && i.p.PaymentMode != "Online Payment" && i.c.DeliveryOrderPaymentStatus == 0)
                   .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       GrossDeliveryCharge = i.c.DeliveryCharge,
                       DeliveryBoyPaymentStatus = i.c.DeliveryBoyPaymentStatus,
                       Amount = i.p.Amount.ToString(),
                       Date = i.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).ToList();
                if (model.List.Count() != 0)
                {
                    model.TotalAmount = model.List.Sum(i => Convert.ToDouble(i.Amount));
                    model.EarningOfToday = model.List.Sum(i => i.GrossDeliveryCharge);
                    model.TargetAmount = 1500.00;
                    model.DeliveryPaymentStatus = model.List.FirstOrDefault().DeliveryBoyPaymentStatus;
                }
                else
                {
                    model.TotalAmount = 0.0;
                    model.EarningOfToday = 0.0;
                    model.TargetAmount = 1500.00;
                    model.DeliveryPaymentStatus = 1;
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region helpers
        double GetMeters(Double Latitudes, Double Longitudes, Double Latitude, Double Longitude)
        {
            return (((Math.Acos(Math.Sin((Latitude * Math.PI / 180)) * Math.Sin((Latitudes * Math.PI / 180)) + Math.Cos((Latitude * Math.PI / 180)) * Math.Cos((Latitudes * Math.PI / 180))
                    * Math.Cos(((Longitude - Longitudes) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344);
        }

        public class Message
        {
            public string[] registration_ids { get; set; }
            public Notification notification { get; set; }
            public object data { get; set; }
        }

        public class Notification
        {
            public string title { get; set; }
            public string text { get; set; }
        }

        public void SendNotification(string tocken)
        {
            try
            {
                dynamic data = new
                {
                    to = tocken, // Uncoment this if you want to test for single device
                                 // registration_ids = singlebatch, // this is for multiple user 
                    notification = new
                    {
                        title = "--title--",     // Notification title
                        body = "--message--",    // Notification body data
                        link = "shopnowpay.com"       // When click on notification user redirect to this link
                    }
                };
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
                string SERVER_API_KEY = "AAAASx4c4GY:APA91bEYysUEFT9F1XhO44epVtF0Mxq2SNbqIZUSQ3Xroov65JF9TzH7v9TghwG4JiWVa8HgqJVJnfklHIqhFuCQfW9T8b8TzrOOMYJd9eh2H1HcJFg06Vnjqz0aJk1tCSSuUL9BeUrsD";
                string SENDER_ID = "322627756134";
                WebRequest tRequest;
                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                String sResponseFromServer = tReader.ReadToEnd();
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void NotificationMessage(string msg, string tag, string token)
        {
            // public string[] registration_ids { get; set; }
            try
            {
                var applicationID = "key=AAAASx4c4GY:APA91bEYysUEFT9F1XhO44epVtF0Mxq2SNbqIZUSQ3Xroov65JF9TzH7v9TghwG4JiWVa8HgqJVJnfklHIqhFuCQfW9T8b8TzrOOMYJd9eh2H1HcJFg06Vnjqz0aJk1tCSSuUL9BeUrsD";

                var senderId = "322627756134";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";

                var data = new

                {
                    to = token,
                    //registration_ids = token,
                    notification = new
                    {
                        body = msg,
                        title = tag,
                        icon = "myicon"
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }

        string GetOtp(int id)
        {
            try
            {
                var otp = db.OtpVerifications.FirstOrDefault(i => i.OrderNo == id);
                if (otp != null)
                {
                    return otp.Otp;
                }
                else
                    return "N/A";
            }
            catch (Exception e)
            {
                return "Somthing went wrong";
            }
        }

        List<OrderItem> GetOrderList(long orderId)
        {
            try
            {
                return db.OrderItems.Where(i => i.OrderId == orderId && i.Status == 0).ToList();
            }
            catch
            {
                return new List<OrderItem>();
            }
        }

        List<Banner> GetBannerList(int shopId)
        {
            try
            {
                return db.Banners.Where(j => j.Status == 0 && j.ShopId == shopId).ToList();
            }
            catch
            {
                return new List<Banner>();
            }
        }

        List<BannerImages> GetBannerImageList(int id)
        {
            try
            {
                var d = DateTime.Now.Date.Date;
                var teenStudentsName = (from s in db.Banners
                                        where (s.Status == 0 || s.Status == 6) && s.ShopId == id && (DbFunctions.TruncateTime(s.FromDate) <= DbFunctions.TruncateTime(DateTime.Now) && DbFunctions.TruncateTime(s.Todate) >= DbFunctions.TruncateTime(DateTime.Now))
                                        select new BannerImages { Bannerpath = (s.BannerPath != null) ? s.BannerPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "", ShopId = s.ShopId, ProductId = s.MasterProductId }).ToList();

                return teenStudentsName;
            }
            catch (Exception ex)
            {
                return new List<BannerImages>();
            }
        }

        List<OrderItem> GetOrderPendingList(int orderNo)
        {
            try
            {
                return db.OrderItems.Where(i => i.OrdeNumber == orderNo && i.Status == 0).ToList();
            }
            catch
            {
                return new List<OrderItem>();
            }
        }

        //List<Cart> GetShopOrderList(string orderNo)
        //{
        //    try
        //    {
        //        return db.Carts.Where(i => i.OrderNo == orderNo && i.CartStatus == 4 || i.CartStatus == 5 && i.Status == 0).ToList();
        //    }
        //    catch
        //    {
        //        return new List<Cart>();
        //    }
        //}
        //List<Cart> GetPendingList(string orderNo)
        //{
        //    try
        //    {
        //        return db.Carts.Where(i => i.OrderNo == orderNo && (i.CartStatus == 4 || i.CartStatus == 3) && i.Status == 0).ToList();
        //    }
        //    catch
        //    {
        //        return new List<Cart>();
        //    }
        //}

        Double RatingCalculation(int id)
        {
            Double rating = 0;
            var ratingCount = db.CustomerReviews.Where(i => i.ShopId == id).Count();
            var ratingSum = (from ss in db.CustomerReviews
                             where ss.ShopId == id
                             select (Double?)ss.Rating).Sum() ?? 0;
            if (ratingCount == 0)
                ratingCount = 1;
            rating = (ratingSum * 5) / (ratingCount * 5);
            return Math.Round(rating, 1);
        }

        Models.Payment GetPayment(int code)
        {
            try
            {
                return db.Payments.FirstOrDefault(i => i.OrderNumber == code);
            }
            catch
            {
                return (Models.Payment)null;
            }
        }
        #endregion

        public JsonResult GetDeliveryBoyPayout(DateTime startDate, DateTime endDate, string phoneNo, int page = 1, int pageSize = 5)
        {
            //DelivaryBoyPayoutReportViewModel
            var model = new DelivaryBoyPayoutReportViewModel();
            model.List = db.Orders.Where(i => i.Status == 6 && ((DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(startDate)) && (DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(endDate))))
            .Join(db.DeliveryBoys.Where(i => i.PhoneNumber == phoneNo), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
            .GroupBy(i => DbFunctions.TruncateTime(i.c.DateEncoded))
            .AsEnumerable()
            .Select(i => new DelivaryBoyPayoutReportViewModel.PayoutOut
            {
                Date = i.Any() ? i.FirstOrDefault().c.DateEncoded.ToString("dd-MMM-yyyy HH:ss") : "",
                date = i.FirstOrDefault().c.DateEncoded,
                totalamount = i.Sum(a => a.c.DeliveryCharge),
                paidamount = GetPaidAmount(i.Key.Value, phoneNo),
            }).OrderByDescending(j => j.date).ToList();
            int count = model.List.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.List;
            //var items = model.List.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            //var previous = CurrentPage - 1;
            //var previousurl = "https://admin.shopnowchat.in/Api/GetDelivaryBoyReport?startDate=" + startDate + "&endDate=" + endDate + "&phoneNo=" + phoneNo + "&page=" + previous;
            //var previousPage = CurrentPage > 1 ? previousurl : "No";
            //var current = CurrentPage + 1;
            //var nexturl = "https://admin.shopnowchat.in/Api/GetDelivaryBoyReport?startDate=" + startDate + "&endDate=" + endDate + "&phoneNo=" + phoneNo + "&page=" + current;
            //var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            //var paginationMetadata = new
            //{
            //    totalCount = TotalCount,
            //    pageSize = PageSize,
            //    currentPage = CurrentPage,
            //    totalPages = TotalPages,
            //    previousPage,
            //    nextPage
            //};
            return Json(new { items }, JsonRequestBehavior.AllowGet);
            // return Json(list, JsonRequestBehavior.AllowGet);
        }

        public double GetPaidAmount(DateTime dateEncoded, string phoneNo)
        {
            var list = db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPaymentStatus == 1 && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(dateEncoded))
           .Join(db.DeliveryBoys.Where(i => i.PhoneNumber == phoneNo), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
           .GroupBy(i => DbFunctions.TruncateTime(dateEncoded))
           .Select(i => new
           {
               amount = i.Any() ? i.Sum(a => a.c.DeliveryCharge) : 0
           }).FirstOrDefault();
            if (list != null)
                return Convert.ToDouble(list.amount);
            else
                return 0;
        }

        public JsonResult GetShopReports(DateTime startDate, DateTime endDate, int shopId, int type)
        {
            var model = new ShopApiReportsViewModel();
            if (type == 1) //Earnings & Settlements
            {
                model.ListItems = db.Payments
                     .Where(i => ((DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(startDate)) &&
                 (DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(endDate))))
                 .Join(db.Shops.Where(i => i.Id == shopId), p => p.ShopId, s => s.Id, (p, s) => new { p, s })
                 .Join(db.Orders.Where(i => i.Status == 6), p => p.p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                 .GroupBy(i => DbFunctions.TruncateTime(i.p.p.DateEncoded))
                 .AsEnumerable()
                 .Select(i => new ShopApiReportsViewModel.EarningListItem
                 {
                     Date = i.Any() ? i.FirstOrDefault().p.p.DateEncoded.ToString("dd-MMM-yyyy HH:ss") : "",
                     DateEncoded = i.FirstOrDefault().p.p.DateEncoded,
                     Earning = i.Sum(a => a.p.p.Amount),
                     Paid = GetShopPaidAmount(i.Key.Value, shopId),
                 }).OrderByDescending(j => j.DateEncoded).ToList();
                return Json(model.ListItems, JsonRequestBehavior.AllowGet);
            }
            else if (type == 2) //Refund & Delivery
            {
                model.RefundLists = db.Payments
                    .Where(i => ((DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(startDate)) &&
                (DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(endDate))))
                .Join(db.Shops.Where(i => i.Id == shopId), p => p.ShopId, s => s.Id, (p, s) => new { p, s })
                .Join(db.Orders.Where(i => i.Status == 6), p => p.p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                // .Join(db.ShopCharges.Where(i => i.Status == 0), p => p.p.p.OrderNo, sc => sc.OrderNo, (p, sc) => new { p, sc })
                .AsEnumerable()
                .Select(i => new ShopApiReportsViewModel.RefundListItem
                {
                    Date = i.p.p.DateEncoded.ToString("dd-MMM-yyyy HH:ss"),
                    DateEncoded = i.p.p.DateEncoded,
                    Earning = i.p.p.Amount,
                    Refund = i.p.p.RefundAmount ?? 0,
                    DeliveryCredits = i.c.DeliveryCharge,
                    OrderNo = i.p.p.OrderNumber
                }).OrderByDescending(i => i.DateEncoded).ToList();
                return Json(model.RefundLists, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public double GetShopPaidAmount(DateTime dateEncoded, int shopId)
        {
            var list = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(dateEncoded))
                .Join(db.Shops.Where(i => i.Id == shopId), p => p.ShopId, s => s.Id, (p, s) => new { p, s })
            .Join(db.Orders.Where(i => i.Status == 6 && i.ShopPaymentStatus == 1), p => p.p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
           .GroupBy(i => DbFunctions.TruncateTime(dateEncoded))
           .Select(i => new
           {
               amount = i.Any() ? i.Sum(a => a.p.p.Amount) : 0
           }).FirstOrDefault();
            if (list != null)
                return Convert.ToDouble(list.amount);
            else
                return 0;
        }

        public JsonResult GetAddonList(int productId)
        {
            var list = db.ShopDishAddOns.Where(i => i.ProductId == productId && i.IsActive == true).ToList();
            if (list.Count() > 0)
                return Json(new { list = list, type = list.FirstOrDefault().AddOnType }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationDetails(double sourceLatitude, double sourceLongitude, double destinationLatitude, double destinationLongitude)
        {
            bool isAvailable = db.LocationDetails.Any(i => i.SourceLatitude == sourceLatitude && i.SourceLongitude == sourceLongitude && i.DestinationLatitude == destinationLatitude && i.DestinationLongitude == destinationLongitude);
            if (isAvailable)
            {
                var location = db.LocationDetails.FirstOrDefault(i => i.SourceLatitude == sourceLatitude && i.SourceLongitude == sourceLongitude && i.DestinationLatitude == destinationLatitude && i.DestinationLongitude == destinationLongitude);
                return Json(location, JsonRequestBehavior.AllowGet);
            }
            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveLocationDetails(LocationDetailsCreateViewModel model)
        {
            var locationDetails = _mapper.Map<LocationDetailsCreateViewModel, LocationDetail>(model);
            db.LocationDetails.Add(locationDetails);
            db.SaveChanges();
            return Json(new { message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllOrders(int customerId, int page = 1, int pageSize = 5,int type=0) //1-Live,2-Past
        {
            db.Configuration.ProxyCreationEnabled = false;
            var model = new GetAllOrderListViewModel();
            model.OrderLists = db.Orders.Where(i => i.CustomerId == customerId && (type == 1 ? (i.Status >= 2 && i.Status <= 5) : (i.Status == 6 || i.Status == 7)))
                 .Join(db.Payments, o => o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
                 .GroupJoin(db.OrderItems, o => o.o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                 .Select(i => new GetAllOrderListViewModel.OrderList
                 {
                     Convinenientcharge = i.o.o.Convinenientcharge,
                     CustomerId = i.o.o.CustomerId,
                     CustomerName = i.o.o.CustomerName,
                     CustomerPhoneNumber = i.o.o.CustomerPhoneNumber,
                     DateEncoded = i.o.o.DateEncoded,
                     DeliveryAddress = i.o.o.DeliveryAddress,
                     DeliveryBoyId = i.o.o.DeliveryBoyId,
                     DeliveryBoyName = i.o.o.DeliveryBoyName,
                     DeliveryBoyPhoneNumber = i.o.o.DeliveryBoyPhoneNumber,
                     DeliveryCharge = i.o.o.DeliveryCharge,
                     Id = i.o.o.Id,
                     NetDeliveryCharge = i.o.o.NetDeliveryCharge,
                     OrderNumber = i.o.o.OrderNumber,
                     Packingcharge = i.o.o.Packingcharge,
                     PenaltyAmount = i.o.o.PenaltyAmount,
                     PenaltyRemark = i.o.o.PenaltyRemark,
                     ShopDeliveryDiscount = i.o.o.ShopDeliveryDiscount,
                     ShopId = i.o.o.ShopId,
                     ShopName = i.o.o.ShopName,
                     ShopOwnerPhoneNumber = i.o.o.ShopOwnerPhoneNumber,
                     ShopPhoneNumber = i.o.o.ShopPhoneNumber,
                     Status = i.o.o.Status,
                     TotalPrice = i.o.o.TotalPrice,
                     TotalProduct = i.o.o.TotalProduct,
                     TotalQuantity = i.o.o.TotalQuantity,
                     NetTotal = i.o.o.NetTotal,
                     WaitingCharge = i.o.o.WaitingCharge,
                     WaitingRemark = i.o.o.WaitingRemark,
                     RefundAmount=i.o.p.RefundAmount,
                     RefundRemark = i.o.p.RefundRemark,
                     PaymentMode = i.o.p.PaymentMode,
                     OrderItemList = i.oi.ToList()
                 }).ToList();

            int count = model.OrderLists.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.OrderLists.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetAllOrders?customerId=" + customerId + "&page=" + previous + "&type=" + type;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetAllOrders?customerId=" + customerId + "&page=" + current + "&type=" + type;
            var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);

        }

        public void UpdateAchievements(int customerId)
        {
            var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
            if (customer != null)
            {
                var orderList = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6).ToList();

                //count wise
                switch (orderList.Count())
                {
                    case 1:
                        customer.WalletAmount += 10;
                        break;
                    case 50:
                        customer.WalletAmount += 250;
                        break;
                    case 150:
                        customer.WalletAmount += 500;
                        break;
                    case 450:
                        customer.WalletAmount += 1500;
                        break;
                }

                //shop wise
                if (orderList.GroupBy(i=>i.ShopId).Count() == 5)
                    customer.WalletAmount += 50;

                
                var orderListItem = db.Orders.Where(i => i.CustomerId == customer.Id)
                    .Join(db.OrderItems, o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                    .ToList();
                //Category wise
                if (orderListItem.GroupBy(i=>i.oi.CategoryId).Count() == 3)
                    customer.WalletAmount += 50;
                else if (orderListItem.GroupBy(i => i.oi.CategoryId).Count() == 6)
                    customer.WalletAmount += 50;
            }
        }
           
    }
}