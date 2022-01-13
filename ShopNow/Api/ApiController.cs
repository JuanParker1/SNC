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
using System.Data.Entity.Migrations;
//using N.EntityFramework.Extensions;
using Z.EntityFramework.Extensions;
using System.Data.Entity.SqlServer;
using System.Configuration;

namespace ShopNow.Controllers
{
    [APIKeyHandler(ApiKey = "shopnow")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApiController : Controller
    {
        private sncEntities db = new sncEntities();

        readonly string _connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private string apipath= "http://admin.shopnowchat.in/";
        //private string apipath = "http://117.221.69.52:91/";
        //private string apipath = "http://103.78.159.20:91/";
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
                config.CreateMap<Models.Order, OrderDetailsApiViewModel>();
                config.CreateMap<SavePrescriptionViewModel, CustomerPrescription>();
                config.CreateMap<OrderCreateViewModel.ListItem.AddOnListItem, OrderItemAddon>();

                config.CreateMap<Product, ProductDetailsApiViewModel>();
                config.CreateMap<Product, MedicalProductDetailsApiViewModel>();

            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        public JsonResult GetShop(string placeid)
        {
            using (WebClient myData = new WebClient())
            {
                myData.Headers["X-ApiKey"] = "Tx9ANC5RqngpTOM9VJ0JP2+1LbZvo1LI";
                myData.Headers[HttpRequestHeader.Accept] = "application/json";
                myData.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string getDetails = myData.DownloadString("https://maps.googleapis.com/maps/api/place/details/json?place_id=" + placeid + "&key=AIzaSyCl_9yf43Z6hWLvVvwd68p7WXwq_oYcS_0");  //AIzaSyCRsR3Wpkj_Vofy5FSU0otOx-6k-YFiNBk
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
                    if (model.PhoneNumber == "1234567890")
                        otpmodel.Otp = "123789";
                    else
                        otpmodel.Otp = _generatedCode;
                    otpmodel.ReferenceCode = _referenceCode;
                    otpmodel.Verify = false;
                    otpmodel.CreatedBy = user.Name;
                    otpmodel.UpdatedBy = user.Name;
                    otpmodel.DateEncoded = DateTime.Now;
                    //var dateAndTime = DateTime.Now;
                    //var date = dateAndTime.ToString("d");
                    //var time = dateAndTime.ToString("HH:mm");
                    string joyra = "04448134440";
                    string Msg = "Hi, " + otpmodel.Otp + " is the OTP for (Shop Now Chat) Verification at " + DateTime.Now.ToString("HH:mm") + " with " + otpmodel.ReferenceCode + " reference - Joyra";
                    string result = SendSMS.execute(joyra, model.PhoneNumber, Msg);
                    otpmodel.Status = 0;
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
                if (model.PhoneNumber == "1234567890")
                    otpmodel.Otp = "123789";
                else
                    otpmodel.Otp = _generatedCode;
                otpmodel.ReferenceCode = _referenceCode;
                otpmodel.Verify = false;
                var dateAndTime = DateTime.Now;
                var date = dateAndTime.ToString("d");
                var time = dateAndTime.ToString("HH:mm");
                string joyra = "04448134440";
                string Msg = "Hi, " + otpmodel.Otp + " is the OTP for (Shop Now Chat) Verification at " + time + " with " + otpmodel.ReferenceCode + " reference - Joyra";
                string result = "";
                if (model.PhoneNumber != "1234567890")
                 result = SendSMS.execute(joyra, model.PhoneNumber, Msg);
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
                if (!string.IsNullOrEmpty(model.AlternateNumber))
                    customer.AlternateNumber = model.AlternateNumber;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = "Successfully Updated Your Details!", Details = customer });
            }
            else
            {
                customer.Name = model.Name;
                customer.Email = model.Email;
                customer.AlternateNumber = model.AlternateNumber;
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
                //var customer = db.Customers.FirstOrDefault(i => i.Id == staff.CustomerId);
                //customer.Position = 2;
                //customer.UpdatedBy = customer.Name;
                //customer.DateUpdated = DateTime.Now;
                //db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();
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
                              Status = p.Status,
                              IsOnline = p.IsOnline,
                              NextOnTime = p.NextOnTime
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
            string defaultImagePath = "../../assets/images/noimageres.svg";
            if (shid.ShopCategoryId == 4)
                defaultImagePath = "../../assets/images/1.5-cm-X-1.5-cm.png";
            var model = db.Products.Where(i => (i.Price != 0 || i.MenuPrice != 0) && i.ShopId == shid.Id && (i.Status == 0 || i.Status == 1))
                .Join(db.MasterProducts.Where(i => (str != "" ? i.Name.ToLower().StartsWith(str.ToLower()) : true)), p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
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
                            ImagePath = ((!string.IsNullOrEmpty(i.m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : defaultImagePath),
                            IsOnline = i.p.IsOnline,
                            NextOnTime = i.p.NextOnTime,
                            Size = i.m.SizeLWH,
                            Weight = i.m.Weight,
                            IsPreorder = i.p.IsPreorder,
                            PreorderHour = i.p.PreorderHour
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
            //var bill = db.Bills.FirstOrDefault(i => i.Id == model.Id);
            //var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
            //if (bill != null && customer != null)
            //{
            //    bill.DeliveryChargeCustomer = model.DeliveryChargeCustomer;
            //    bill.ItemType = model.ItemType;
            //    bill.ConvenientCharge = model.ConvenientChargeRange;
            //    bill.PackingCharge = model.PackingCharge;
            //    bill.UpdatedBy = customer.Name;
            //    bill.DateUpdated = DateTime.Now;
            //    db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
            //    db.SaveChanges();
            //    return Json(new { message = "Successfully Updated Bill!" });
            //}
            //else
            //{
            //    return Json(new { message = "Fail to Update Bill!" });
            //}

            var bill = db.BillingCharges.FirstOrDefault(i => i.Id == model.Id);
            var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
            if (bill != null && customer != null)
            {
                bill.DeliveryDiscountPercentage = model.DeliveryChargeCustomer;
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

        public JsonResult GetBillAndDeliveryCharge(int shopId, double totalSize = 0, double totalWeight = 0)
        {
            var model = new BillingDeliveryChargeViewModel();
            var shop = db.Shops.FirstOrDefault(i => i.Id == shopId);
            int mode = 1; //1-bike,2-carrier bike,3-Auto
            double liters = totalSize / 1000;

            if (totalWeight <= 20 || liters <= 60)
                mode = 1;
            if ((totalWeight > 20 && totalWeight <= 40) || (liters > 60 && liters <= 120))
                mode = 2;
            if (totalWeight > 40 || liters > 120)
                mode = 3;
            var deliveryCharge = db.DeliveryCharges.FirstOrDefault(i => i.Type == shop.DeliveryType && i.TireType == shop.DeliveryTierType && i.VehicleType == mode && i.Status == 0);
            if (deliveryCharge != null)
            {
                model.DeliveryChargeKM = deliveryCharge.ChargeUpto5Km;
                model.DeliveryChargeOneKM = deliveryCharge.ChargePerKm;
                model.DeliveryMode = deliveryCharge.VehicleType;
                model.Distance = 5;
                model.Remark = db.PincodeRates.FirstOrDefault(i => i.Id == shop.PincodeRateId && i.Status == 0)?.Remarks;
            }

            var billingCharge = db.BillingCharges.FirstOrDefault(i => i.ShopId == shopId && i.Status == 0);
            if (billingCharge != null)
            {
                model.BillingId = billingCharge.Id;
                model.ConvenientChargeRange = billingCharge.ConvenientCharge;
                model.PackingCharge = billingCharge.PackingCharge;
                model.DeliveryDiscountPercentage = billingCharge.DeliveryDiscountPercentage;
                model.ItemType = billingCharge.ItemType;

                var platformCreditRate = db.PlatFormCreditRates.FirstOrDefault(i => i.Status == 0);
                if (platformCreditRate != null)
                    model.ConvenientCharge = platformCreditRate.RatePerOrder;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveCustomerToken(SaveFCMTokenViewModel model)
        {
            var customer = db.Customers.FirstOrDefault(c => c.Id == model.CustomerId);
            try
            {
                customer.FcmTocken = model.Token;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { token = customer.FcmTocken }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { token = "" }, JsonRequestBehavior.AllowGet);
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
                var isExist = db.DeliveryBoys.Any(i => i.PhoneNumber == model.PhoneNumber);
                var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                if (!isExist)
                {
                    var deliveryBoy = _mapper.Map<DeliveryBoyCreateViewModel, DeliveryBoy>(model);
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
                    if (deliveryBoy != null)
                    {
                        return Json(new { message = "Successfully Created a Delivery Boy!", Position = customer.Position });
                    }
                    else
                        return Json(new { message = "Failed to Create a Delivery Boy!" });
                }
                else
                {
                    var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber);
                    _mapper.Map(model, deliveryBoy);
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
                    db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    if (deliveryBoy != null)
                    {
                        return Json(new { message = "Successfully Created a Delivery Boy!", Position = customer.Position });
                    }
                    else
                        return Json(new { message = "Failed to Create a Delivery Boy!" });

                }
                //return Json(new { message = "This Delivery Boy Already Exist!" });
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
                if (model.ListItems.FirstOrDefault().OutletId > 0)
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

                            // stock =Math.Floor(GetStockQty(item.ItemId.ToString(),item.ouletid));
                            stock = Math.Floor(GetStockQty(item.ItemId.ToString(), item.OutletId));
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
                else
                    return Json(new { message = "" });
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
            var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);

            if (customer != null)
            {
                //payment.CustomerName = model.CustomerName;
                payment.CreatedBy = model.CustomerName;
                payment.UpdatedBy = model.CustomerName;
                payment.DeliveryCharge = model.GrossDeliveryCharge;
                payment.PackingCharge = model.PackagingCharge;
                payment.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
                payment.PaymentModeType = model.PaymentMode == "Online Payment" ? 1 : 2;

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
                    order.WalletAmount = model.WalletAmount;
                    order.OfferId = model.OfferId;
                    order.OfferAmount = model.OfferAmount;
                    order.TipsAmount = model.TipsAmount;
                    db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    //Reducing Platformcredits
                    var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                    var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
                    shopCredits.PlatformCredit -= payment.RatePerOrder.Value;
                    db.Entry(shopCredits).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    payment.OrderNumber = order.OrderNumber;
                }
               // Helpers.LogFile.WriteToFile(model.PaymentMode);
                if (model.PaymentMode == "Online Payment")
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                               SecurityProtocolType.Tls11 |
                                               SecurityProtocolType.Tls12;
                    string key = BaseClass.razorpaykey;// "rzp_live_PNoamKp52vzWvR";
                    string secret = BaseClass.razorpaySecretkey;//"yychwOUOsYLsSn3XoNYvD1HY";
                   // Helpers.LogFile.WriteToFile(key);
                    //Helpers.LogFile.WriteToFile("reference code" + " " + model.ReferenceCode);
                    RazorpayClient client = new RazorpayClient(key, secret);
                    Razorpay.Api.Payment varpayment = new Razorpay.Api.Payment();
                    var s = varpayment.Fetch(model.ReferenceCode);
                  // Helpers.LogFile.WriteToFile("s" + " " + s["invoice_id"]);
                    PaymentsData pay = new PaymentsData();

                    pay.OrderNumber = model.OrderNumber;
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

                    pay.Fee -= pay.Tax; //Total fee minu tax
                    pay.DateEncoded = DateTime.Now;
                    db.PaymentsDatas.Add(pay);
                    db.SaveChanges();
                }

                if (model.CreditType == 0 || model.CreditType == 1)
                {
                    payment.PaymentCategoryType = 1;
                    payment.Credits = model.CreditType == 0 ? "Platform Credits" : "Delivery Credits";
                    //payment.CreditType = model.CreditType;
                }
                else
                {
                    payment.PaymentCategoryType = 0;
                    payment.Credits = "N/A";
                }

                payment.DateEncoded = DateTime.Now;
                payment.DateUpdated = DateTime.Now;
                payment.Status = 0;
                payment.RefundStatus = 1;
                db.Payments.Add(payment);
                db.SaveChanges();

                if (payment.Id != 0)
                {
                    if (model.WalletAmount != 0)
                    {
                        customer.WalletAmount -= model.WalletAmount;
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //For Credits adding
                    if (model.CreditType == 0 || model.CreditType == 1)
                    {
                        //var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                        var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == model.CustomerId);
                        if (shopCredits != null)
                        {
                            shopCredits.PlatformCredit += model.PlatformCreditAmount;
                            shopCredits.DeliveryCredit += model.DeliveryCreditAmount;
                            shopCredits.DateUpdated = DateTime.Now;
                            db.Entry(shopCredits).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else {
                            var shopcredit = new ShopCredit
                            {
                                CustomerId = model.CustomerId,
                                DateUpdated = DateTime.Now,
                                DeliveryCredit = model.DeliveryCreditAmount,
                                PlatformCredit = model.PlatformCreditAmount
                            };
                            db.ShopCredits.Add(shopcredit);
                            db.SaveChanges();
                        }
                    }

                    return Json(new { message = "Successfully Added to Payment!", Details = model });
                }
                else
                {
                    return Json(new { message = "Failed to Add Payment !" });
                }
            }
            else
                return Json(new { message = "Failed to Add Payment !" });
        }

        [HttpPost]
        public JsonResult AddOnlinePayment(SaveOnlinePaymentViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.ReferenceCode) || model.OrderNumber != 0)
                {
                    var order = db.Orders.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
                    order.PaymentMode = "Online Payment";
                    order.PaymentModeType = 1;
                    order.TipsAmount = model.TipsAmount;
                    order.NetTotal += model.TipsAmount;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();

                    var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
                    payment.PaymentMode = "Online Payment";
                    payment.PaymentModeType = 1;
                    payment.Key = "Razor";
                    payment.ReferenceCode = model.ReferenceCode;
                    db.Entry(payment).State = EntityState.Modified;
                    db.SaveChanges();

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                      SecurityProtocolType.Tls11 |
                                                      SecurityProtocolType.Tls12;
                    string key = BaseClass.razorpaykey;// "rzp_live_PNoamKp52vzWvR";
                    string secret = BaseClass.razorpaySecretkey;//"yychwOUOsYLsSn3XoNYvD1HY";

                    RazorpayClient client = new RazorpayClient(key, secret);
                    Razorpay.Api.Payment varpayment = new Razorpay.Api.Payment();
                    var s = varpayment.Fetch(model.ReferenceCode);
                    PaymentsData pay = new PaymentsData();

                    pay.OrderNumber = model.OrderNumber;
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
                    return Json(new { status = false, message = "Successfully Added to Payment!" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false, message = "Failed to Add Payment!" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = false, message = "Failed to Add Payment!" }, JsonRequestBehavior.AllowGet);
            }
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
                if ((shopCredits.PlatformCredit < 26 && shopCredits.DeliveryCredit < 67) && shop.IsTrail == false)
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
                    //var customer = (dynamic)null;
                    if (model.CustomerId != 0)
                    {
                        var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                        order.CustomerId = customer.Id;
                        order.CreatedBy = customer.Name;
                        order.UpdatedBy = customer.Name;
                        order.CustomerName = customer.Name;
                        order.CustomerPhoneNumber = customer.PhoneNumber;

                        //Store Referral Number
                        customer.ReferralNumber = model.ReferralNumber;
                        db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    //if (model.ReferralNumber == string.Empty)
                    // {
                    //    customer.IsReferred = true;

                    //}
                    //  else
                    // {
                    // customer.IsReferred = false;

                    // }

                    order.OrderNumber = Convert.ToInt32(model.OrderNumber);
                    order.ShopId = shop.Id;
                    order.ShopName = shop.Name;
                    order.ShopPhoneNumber = shop.PhoneNumber ?? shop.ManualPhoneNumber;
                    order.ShopOwnerPhoneNumber = shop.OwnerPhoneNumber;
                    order.TotalPrice = model.ListItems.Sum(i => i.Price);
                    order.TotalProduct = model.ListItems.Count();
                    order.TotalQuantity = model.ListItems.Sum(i => Convert.ToInt32(i.Quantity));
                    order.TotalShopPrice = model.ListItems.Sum(i => i.ShopPrice);
                    order.DateEncoded = DateTime.Now;
                    order.DateUpdated = DateTime.Now;
                    order.Status = 0;
                    order.PaymentModeType = model.PaymentMode == "Online Payment" ? 1 : 2;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    foreach (var item in model.ListItems)
                    {
                        if (item.ItemId != 0)
                        {
                            var product = db.Products.FirstOrDefault(i => i.ItemId == item.ItemId && i.Status == 0);
                            product.HoldOnStok = Convert.ToInt32(item.Quantity);
                            product.Qty = product.Qty - Convert.ToInt32(item.Quantity);
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        var orderItem = _mapper.Map<OrderCreateViewModel.ListItem, OrderItem>(item);
                        orderItem.Status = 0;
                        orderItem.OrderId = order.Id;
                        orderItem.OrdeNumber = order.OrderNumber;
                        db.OrderItems.Add(orderItem);
                        db.SaveChanges();

                        if (item.AddOnListItems != null)
                        {
                            foreach (var addon in item.AddOnListItems)
                            {
                                if (addon.Index == item.AddOnIndex)
                                {
                                    var addonItem = _mapper.Map<OrderCreateViewModel.ListItem.AddOnListItem, OrderItemAddon>(addon);
                                    addonItem.Status = 0;
                                    addonItem.OrderItemId = orderItem.Id;
                                    db.OrderItemAddons.Add(addonItem);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }

                    if (order != null)
                    {
                        var fcmToken = (from c in db.Customers
                                        join s in db.Shops on c.Id equals s.CustomerId
                                        where s.Id == model.ShopId
                                        select c.FcmTocken ?? "").FirstOrDefault().ToString();
                        Helpers.PushNotification.SendbydeviceId("You have received new order.Accept Soon", "Snowch", "a.mp3", fcmToken.ToString());

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
                order.ShopAcceptedTime = DateTime.Now;
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
                    Helpers.PushNotification.SendbydeviceId($"Your order has been accepted by shop({order.ShopName}).", "Snowch", "a.mp3", fcmToken.ToString());
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
                        Helpers.PushNotification.SendbydeviceId($"Your refund of amount {payment.Amount} for order no {payment.OrderNumber} is for {payment.RefundRemark} initiated and you will get credited with in 7 working days.", "Snowch", "a.mp3", fcmToken.ToString());
                    }

                    //Add Wallet Amount to customer
                    if (order.WalletAmount != 0)
                    {
                        customer.WalletAmount += order.WalletAmount;
                        db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    //decrease offer wallet amount
                    //if (order.OfferId != null)
                    //{
                    //    var offer = db.Offers.FirstOrDefault(i => i.Id == order.OfferId);
                    //    if (offer != null)
                    //    {
                    //        if (offer.DiscountType == 2)
                    //        {
                    //            customer.WalletAmount -= order.OfferAmount;
                    //            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //}
                }
                return Json(new { message = "Successfully Updated the Order!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to Update the Order!" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetShopDeliveredOrders(int shopId, int status, int page = 1, int pageSize = 5)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var model = new GetAllOrderListViewModel();
            model.OrderLists = db.Orders.Where(i => i.ShopId == shopId && i.Status == status)
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
                     WalletAmount = i.o.o.WalletAmount,
                     WaitingCharge = i.o.o.WaitingCharge,
                     WaitingRemark = i.o.o.WaitingRemark,
                     RefundAmount = i.o.p.RefundAmount,
                     RefundRemark = i.o.p.RefundRemark,
                     PaymentMode = i.o.p.PaymentMode,
                     //OrderItemList = i.oi.ToList(),
                     OrderItemLists = i.oi.Select(a => new GetAllOrderListViewModel.OrderList.OrderItemList
                     {
                         AddOnType = a.AddOnType,
                         BrandId = a.BrandId,
                         BrandName = a.BrandName,
                         CategoryId = a.CategoryId,
                         CategoryName = a.CategoryName,
                         HasAddon = a.HasAddon,
                         ImagePath = a.ImagePath,
                         OrdeNumber = a.OrdeNumber,
                         OrderId = a.OrderId,
                         Price = a.Price,
                         ProductId = a.ProductId,
                         ProductName = a.ProductName,
                         Quantity = a.Quantity,
                         UnitPrice = a.UnitPrice,
                         OrderItemAddonLists = db.OrderItemAddons.Where(b => b.OrderItemId == a.Id).Select(b => new GetAllOrderListViewModel.OrderList.OrderItemList.OrderItemAddonList
                         {
                             AddonName = b.AddonName,
                             AddonPrice = b.AddonPrice,
                             CrustName = b.CrustName,
                             PortionName = b.PortionName,
                             PortionPrice = b.PortionPrice
                         }).ToList()
                     }).ToList()
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
            db.Configuration.ProxyCreationEnabled = false;
            var model = new TodayDeliveryListViewModel();
            model.ResturantList = db.Orders.Where(i => (i.Status == 4 || i.Status == 5) && i.DeliveryBoyPhoneNumber == phoneNumber /*&& DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now)*/)
                .Join(db.Shops.Where(i => i.ShopCategoryId == 1), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
                .Join(db.Payments, o => o.o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
                .Join(db.DeliveryBoys, o => o.o.o.DeliveryBoyId, d => d.Id, (o, d) => new { o, d })
                .GroupJoin(db.OrderItems, o => o.o.o.o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                .Select(i => new TodayDeliveryListViewModel.OrderList
                {
                    CustomerLatitude = i.o.o.o.o.Latitude,
                    CustomerLongitude = i.o.o.o.o.Longitude,
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
                    WalletAmount = i.o.o.o.o.WalletAmount,
                    TipAmount = i.o.o.o.o.TipsAmount,
                    OrderItemList = i.oi.ToList(),
                    
                }).ToList();

            model.OtherList = db.Orders.Where(i => (i.Status == 4 || i.Status == 5) && i.DeliveryBoyPhoneNumber == phoneNumber  /*&& DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now)*/)
               .Join(db.Shops.Where(i => i.ShopCategoryId != 1), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
               .Join(db.Payments, o => o.o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
               .Join(db.DeliveryBoys, o => o.o.o.DeliveryBoyId, d => d.Id, (o, d) => new { o, d })
               .GroupJoin(db.OrderItems, o => o.o.o.o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
               .Select(i => new TodayDeliveryListViewModel.OrderList
               {
                   CustomerLatitude = i.o.o.o.o.Latitude,
                   CustomerLongitude = i.o.o.o.o.Longitude,
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
                   WalletAmount = i.o.o.o.o.WalletAmount,
                   TipAmount = i.o.o.o.o.TipsAmount,
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
                string notificationMessage = $"Your order on shop({order.ShopName}) is on the way.";
                order.Status = 5;
                order.UpdatedBy = customer.Name;
                order.DateUpdated = DateTime.Now;
                order.OrderPickupTime = DateTime.Now;
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
                    notificationMessage = $"Your order on shop({order.ShopName}) is on the way. Please share the delivery code { models.Otp} with the delivery partner {customer.Name} for verification.";
                }
                var fcmToken = (from c in db.Customers
                                where c.Id == order.CustomerId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                Helpers.PushNotification.SendbydeviceId(notificationMessage, "Snowch", "a.mp3", fcmToken.ToString());
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
            order.DeliveredTime = DateTime.Now;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //Reducing Platformcredits
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == orderNo);
            var shop = db.Shops.FirstOrDefault(i => i.Id == order.ShopId);
            var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
            shopCredits.DeliveryCredit -= payment.DeliveryCharge;
            db.Entry(shopCredits).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var customerDetails = (from c in db.Customers
                                   where c.Id == order.CustomerId
                                   //select c.FcmTocken ?? "").FirstOrDefault().ToString();
                                   select c).FirstOrDefault();

            if (customerDetails.IsReferred == false && !string.IsNullOrEmpty(customerDetails.ReferralNumber))
            {
                //customerDetails.Id = customerDetails.Id;
                customerDetails.IsReferred = true;
                db.Entry(customerDetails).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var referralCustomer = db.Customers.FirstOrDefault(c => c.PhoneNumber == customerDetails.ReferralNumber);
                if (referralCustomer != null)
                {
                    var referalAmount = db.ReferralSettings.Where(r => r.Status == 0 && r.ShopDistrict == shop.DistrictName).Select(r => r.Amount).FirstOrDefault();
                    referralCustomer.WalletAmount = referralCustomer.WalletAmount + referalAmount;
                    db.Entry(referralCustomer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //Wallet History for Referral
                    var walletHistory = new CustomerWalletHistory
                    {
                        Amount = referalAmount,
                        CustomerId = referralCustomer.Id,
                        DateEncoded = DateTime.Now,
                        Description = "Received from referral",
                        Type = 1
                    };
                    db.CustomerWalletHistories.Add(walletHistory);
                    db.SaveChanges();
                }
            }

            //Update Wallet Amount with offers
            var offer = db.Offers.FirstOrDefault(i => i.Id == order.OfferId);
            if (offer != null)
            {
                if (offer.DiscountType == 2)
                {
                    customerDetails.WalletAmount += order.OfferAmount;
                    db.Entry(customerDetails).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //Wallet History for Wallet Offer
                    var walletHistory = new CustomerWalletHistory
                    {
                        Amount = order.OfferAmount,
                        CustomerId = customerDetails.Id,
                        DateEncoded = DateTime.Now,
                        Description = $"Received from offer({offer.Name})",
                        Type = 1
                    };
                    db.CustomerWalletHistories.Add(walletHistory);
                    db.SaveChanges();
                }
            }

            if (order.WalletAmount > 0)
            {
                //Wallet History for Wallet Offer
                var walletHistory = new CustomerWalletHistory
                {
                    Amount = order.WalletAmount,
                    CustomerId = customerDetails.Id,
                    DateEncoded = DateTime.Now,
                    Description = $"Payment to Order(#{order.OrderNumber})",
                    Type = 2
                };
                db.CustomerWalletHistories.Add(walletHistory);
                db.SaveChanges();
            }

            //Update Achievement Wallet
            Helpers.AchievementHelpers.UpdateAchievements(order.CustomerId);

            string fcmtocken = customerDetails.FcmTocken ?? "";

            Helpers.PushNotification.SendbydeviceId($"Your order on shop({order.ShopName}) has been delivered by delivery partner {order.DeliveryBoyName}.", "Snowch", "a.mp3", fcmtocken.ToString());
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
                Helpers.PushNotification.SendbydeviceId("You have a new Order. Accept Soon.", "Snowch", "../../assets/b.mp3", fcmToken.ToString());

                //Customer
                var fcmTokenCustomer = (from c in db.Customers
                                        where c.Id == order.Id
                                        select c.FcmTocken ?? "").FirstOrDefault().ToString();
                Helpers.PushNotification.SendbydeviceId($"Delivery Boy ${order.DeliveryBoyName} is Assigned for your Order.", "Snowch", "../../assets/b.mp3", fcmToken.ToString());

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

        public JsonResult GetShopReAssignOrders(int shopId, int page = 1, int pageSize = 5)
        {
            var model = new CartAcceptListApiViewModel();
            model.List = db.Orders.Where(j => j.Status == 3 && j.ShopId == shopId)
                 .GroupJoin(db.OrderItems, o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                .Join(db.Payments.Where(i => i.PaymentResult == "success" || i.PaymentMode == "Cash On Hand"), c => c.o.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Join(db.Shops, py => py.c.o.ShopId, s => s.Id, (py, s) => new { py, s })
               //.AsEnumerable()
               .Select(i => new CartAcceptListApiViewModel.CartList
               {
                   Id = i.py.c.o.Id,
                   ProductId = i.py.c.oi.FirstOrDefault().ProductId,
                   ShopId = i.py.c.o.ShopId,
                   ShopName = i.py.c.o.ShopName,
                   OrderNumber = i.py.c.o.OrderNumber,
                   PaymentMode = i.py.p.PaymentMode,
                   CustomerName = i.py.c.o.CustomerName,
                   ProductName = "",//GetMasterProductName(i.py.pr.MasterProductId),
                   PhoneNumber = i.py.c.o.ShopPhoneNumber,
                   DeliveryAddress = i.py.c.o.DeliveryAddress,
                   ShopLatitude = i.s.Latitude,
                   ShopLongitude = i.s.Longitude,
                   PackingCharge = i.py.c.o.Packingcharge,
                   ConvinenientCharge = i.py.c.o.Convinenientcharge,
                   Amount = i.py.p.Amount, //GetPayment(i.py.c.o.OrderNumber).Amount,
                   //OriginalAmount = GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount == 0 ? GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).OriginalAmount : GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount,
                   GrossDeliveryCharge = i.py.c.o.DeliveryCharge,
                   ShopDeliveryDiscount = i.py.c.o.ShopDeliveryDiscount,
                   NetDeliveryCharge = i.py.c.o.NetDeliveryCharge,
                   Qty = i.py.c.o.TotalQuantity,
                   Date = i.py.c.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
                   DateEncoded = i.py.c.o.DateEncoded,
                   OrderList = i.py.c.oi.ToList(), //GetOrderPendingList(i.py.c.o.OrderNumber),
                   CartStatus = i.py.c.o.Status,
                   WalletAmount = i.py.c.o.WalletAmount
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
            db.Configuration.ProxyCreationEnabled = false;
            var model = new GetAllOrderListViewModel();

            //if (mode == 0)
            //{
            model.OrderLists = db.Orders.Where(i => i.ShopId == shopId && (mode == 0 ? i.Status == 2 : (i.Status == 3 || i.Status == 4 || i.Status == 8)))
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
                 WalletAmount = i.o.o.WalletAmount,
                 IsPreorder = i.o.o.IsPreorder,
                 PreorderDeliveryDateTime = i.o.o.PreorderDeliveryDateTime,
                     //OrderItemList = i.oi.ToList(),
                     OrderItemLists = i.oi.Select(a => new GetAllOrderListViewModel.OrderList.OrderItemList
                 {
                     AddOnType = a.AddOnType,
                     BrandId = a.BrandId,
                     BrandName = a.BrandName,
                     CategoryId = a.CategoryId,
                     CategoryName = a.CategoryName,
                     HasAddon = a.HasAddon,
                     ImagePath = a.ImagePath,
                     OrdeNumber = a.OrdeNumber,
                     OrderId = a.OrderId,
                     Price = a.Price,
                     ProductId = a.ProductId,
                     ProductName = a.ProductName,
                     Quantity = a.Quantity,
                     UnitPrice = a.UnitPrice,
                     OrderItemAddonLists = db.OrderItemAddons.Where(b => b.OrderItemId == a.Id).Select(b => new GetAllOrderListViewModel.OrderList.OrderItemList.OrderItemAddonList
                     {
                         AddonName = b.AddonName,
                         AddonPrice = b.AddonPrice,
                         CrustName = b.CrustName,
                         PortionName = b.PortionName,
                         PortionPrice = b.PortionPrice
                     }).ToList()
                 }).ToList()
             }).OrderByDescending(i => i.Id).ToList();
            //}
            //else
            //{
            //    model.OrderLists = db.Orders.Where(i => i.ShopId == shopId && (i.Status == 3 || i.Status == 4 || i.Status == 8))
            //         .Join(db.Payments, o => o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
            //         .GroupJoin(db.OrderItems, o => o.o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
            //         .Select(i => new GetAllOrderListViewModel.OrderList
            //         {
            //             Convinenientcharge = i.o.o.Convinenientcharge,
            //             CustomerId = i.o.o.CustomerId,
            //             CustomerName = i.o.o.CustomerName,
            //             CustomerPhoneNumber = i.o.o.CustomerPhoneNumber,
            //         //DateStr = i.o.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
            //         DateEncoded = i.o.o.DateEncoded,
            //             DeliveryAddress = i.o.o.DeliveryAddress,
            //             DeliveryBoyId = i.o.o.DeliveryBoyId,
            //             DeliveryBoyName = i.o.o.DeliveryBoyName,
            //             DeliveryBoyPhoneNumber = i.o.o.DeliveryBoyPhoneNumber,
            //             DeliveryCharge = i.o.o.DeliveryCharge,
            //             Id = i.o.o.Id,
            //             NetDeliveryCharge = i.o.o.NetDeliveryCharge,
            //             OrderNumber = i.o.o.OrderNumber,
            //             Packingcharge = i.o.o.Packingcharge,
            //             PenaltyAmount = i.o.o.PenaltyAmount,
            //             PenaltyRemark = i.o.o.PenaltyRemark,
            //             ShopDeliveryDiscount = i.o.o.ShopDeliveryDiscount,
            //             ShopId = i.o.o.ShopId,
            //             ShopName = i.o.o.ShopName,
            //             ShopOwnerPhoneNumber = i.o.o.ShopOwnerPhoneNumber,
            //             ShopPhoneNumber = i.o.o.ShopPhoneNumber,
            //             Status = i.o.o.Status,
            //             TotalPrice = i.o.o.TotalPrice,
            //             TotalProduct = i.o.o.TotalProduct,
            //             TotalQuantity = i.o.o.TotalQuantity,
            //             NetTotal = i.o.o.NetTotal,
            //             WaitingCharge = i.o.o.WaitingCharge,
            //             WaitingRemark = i.o.o.WaitingRemark,
            //             RefundAmount = i.o.p.RefundAmount,
            //             RefundRemark = i.o.p.RefundRemark,
            //             PaymentMode = i.o.p.PaymentMode,
            //             WalletAmount = i.o.o.WalletAmount,
            //             IsPreorder = i.o.o.IsPreorder,
            //             PreorderDeliveryDateTime = i.o.o.PreorderDeliveryDateTime,
            //             //OrderItemList = i.oi.ToList(),
            //             OrderItemLists = i.oi.Select(a => new GetAllOrderListViewModel.OrderList.OrderItemList
            //             {
            //                 AddOnType = a.AddOnType,
            //                 BrandId = a.BrandId,
            //                 BrandName = a.BrandName,
            //                 CategoryId = a.CategoryId,
            //                 CategoryName = a.CategoryName,
            //                 HasAddon = a.HasAddon,
            //                 ImagePath = a.ImagePath,
            //                 OrdeNumber = a.OrdeNumber,
            //                 OrderId = a.OrderId,
            //                 Price = a.Price,
            //                 ProductId = a.ProductId,
            //                 ProductName = a.ProductName,
            //                 Quantity = a.Quantity,
            //                 UnitPrice = a.UnitPrice,
            //                 OrderItemAddonLists = db.OrderItemAddons.Where(b => b.OrderItemId == a.Id).Select(b => new GetAllOrderListViewModel.OrderList.OrderItemList.OrderItemAddonList
            //                 {
            //                     AddonName = b.AddonName,
            //                     AddonPrice = b.AddonPrice,
            //                     CrustName = b.CrustName,
            //                     PortionName = b.PortionName,
            //                     PortionPrice = b.PortionPrice
            //                 }).ToList()
            //             }).ToList()
            //         }).OrderByDescending(i => i.Id).ToList();
            //}

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

            var model = new GetAllOrderListViewModel();
            model.OrderLists = db.Orders.Where(i => i.ShopId == shopId && (i.Status == 3 || i.Status == 4 || i.Status == 5 || i.Status == 8))
                 .Join(db.Payments, o => o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
                 //.Join(db.DeliveryBoys, o => o.o.DeliveryBoyId, d => d.Id, (o, d) => new { o, d })
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
                     RefundAmount = i.o.p.RefundAmount,
                     RefundRemark = i.o.p.RefundRemark,
                     PaymentMode = i.o.p.PaymentMode,
                     Onwork = db.DeliveryBoys.Any(a => a.Id == i.o.o.DeliveryBoyId) ? db.DeliveryBoys.FirstOrDefault(a => a.Id == i.o.o.DeliveryBoyId).OnWork : 0,
                     WalletAmount = i.o.o.WalletAmount,
                     OrderReadyTime = i.o.o.OrderReadyTime,
                     IsPreorder = i.o.o.IsPreorder,
                     PreorderDeliveryDateTime = i.o.o.PreorderDeliveryDateTime,
                     //OrderItemList = i.oi.ToList(), 
                     OrderItemLists = i.oi.Select(a => new GetAllOrderListViewModel.OrderList.OrderItemList
                     {
                         AddOnType = a.AddOnType,
                         BrandId = a.BrandId,
                         BrandName = a.BrandName,
                         CategoryId = a.CategoryId,
                         CategoryName = a.CategoryName,
                         HasAddon = a.HasAddon,
                         ImagePath = a.ImagePath,
                         OrdeNumber = a.OrdeNumber,
                         OrderId = a.OrderId,
                         Price = a.Price,
                         ProductId = a.ProductId,
                         ProductName = a.ProductName,
                         Quantity = a.Quantity,
                         UnitPrice = a.UnitPrice,
                         OrderItemAddonLists = db.OrderItemAddons.Where(b => b.OrderItemId == a.Id).Select(b => new GetAllOrderListViewModel.OrderList.OrderItemList.OrderItemAddonList
                         {
                             AddonName = b.AddonName,
                             AddonPrice = b.AddonPrice,
                             CrustName = b.CrustName,
                             PortionName = b.PortionName,
                             PortionPrice = b.PortionPrice
                         }).ToList()
                     }).ToList()
                 }).OrderByDescending(i => i.DateEncoded).ToList();

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

        public JsonResult GetShopDetailsNew(int shopId = 0, int categoryId = 0, string str = "",int customerId=0)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == shopId);
            ShopDetails model = _mapper.Map<Shop, ShopDetails>(shop);
            var rate = db.CustomerReviews.Where(j => j.ShopId == shop.Id).ToList();
            var reviewCount = db.CustomerReviews.Where(j => j.ShopId == shop.Id).Count();
            if (reviewCount > 0)
                model.Rating = rate.Sum(l => l.Rating) / reviewCount;
            else
                reviewCount = 0;
            model.CustomerReview = reviewCount;
            
            if (shop.ShopCategoryId == 4)
            {
                model.CategoryLists = db.Database.SqlQuery<ShopDetails.CategoryList>($"select distinct CategoryId as Id, c.Name as Name,c.ImagePath,c.OrderNo from Products p join Categories c on c.Id = p.CategoryId where ShopId ={shopId}  and c.Status = 0 and CategoryId !=0 and c.Name is not null group by CategoryId,c.Name,c.ImagePath,c.OrderNo order by Name")
                    .Select(i => new ShopDetails.CategoryList
                    {
                        Id = i.Id,
                        ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                        Name = i.Name,
                        OrderNo = i.OrderNo
                    }).ToList<ShopDetails.CategoryList>();

                model.TrendingCategoryLists = model.CategoryLists.Where(i=>i.OrderNo !=0).OrderBy(i => i.OrderNo).Take(8).Select(i => new ShopDetails.CategoryList
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    Name = i.Name,
                    OrderNo = i.OrderNo
                }).ToList();
            }
            else
            {
                model.CategoryLists = db.Database.SqlQuery<ShopDetails.CategoryList>($"select distinct CategoryId as Id, c.Name as Name from Products p join Categories c on c.Id = p.CategoryId where ShopId ={shopId}  and c.Status = 0 and CategoryId !=0 and c.Name is not null group by CategoryId,c.Name order by Name").ToList<ShopDetails.CategoryList>();
            }
            if (shop.ShopCategoryId == 1)
            {
                //model.ProductLists = (from pl in db.Products
                //                      join m in db.MasterProducts on pl.MasterProductId equals m.Id
                //                      join c in db.Categories on m.CategoryId equals c.Id
                //                      where pl.ShopId == shopId && pl.Status == 0 && pl.Price != 0 && (categoryId != 0 ? m.CategoryId == categoryId : true)
                //                      select new ShopDetails.ProductList
                //                      {
                //                          Id = pl.Id,
                //                          Name = m.Name,
                //                          ShopId = pl.ShopId,
                //                          ShopName = pl.ShopName,
                //                          CategoryId = c.Id,
                //                          CategoryName = c.Name,
                //                          ColorCode = m.ColorCode,
                //                          Price = pl.MenuPrice,
                //                          ImagePath = ((!string.IsNullOrEmpty(m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                //                          Status = pl.Status,
                //                          Customisation = pl.Customisation,
                //                          DiscountCategoryPercentage = pl.Percentage,
                //                          IsOnline = pl.IsOnline,
                //                          NextOnTime = pl.NextOnTime,
                //                          Size = m.SizeLWH,
                //                          Weight = m.Weight,
                //                          IsPreorder = pl.IsPreorder,
                //                          PreorderHour = pl.PreorderHour,
                //                          OfferQuantityLimit = pl.OfferQuantityLimit
                //                      }).Where(i => i.Price != 0 && (str != "" ? i.Name.ToLower().Contains(str) : true)).ToList();
                //model.count1 = model.ProductLists.Count();


                model.ProductLists = db.Products.Where(i => i.ShopId == shopId && i.Status == 0 && i.Price != 0 && i.MenuPrice != 0 && (categoryId != 0 ? i.CategoryId == categoryId : true))
                    .Join(db.MasterProducts.Where(i => str != "" ? i.Name.ToLower().Contains(str.ToLower()) : true), p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                    .Join(db.Categories, p => p.p.CategoryId, c => c.Id, (p, c) => new { p, c })
                    .GroupJoin(db.CustomerFavorites, p => p.p.p.Id, cf => cf.ProductId, (p, cf) => new { p, cf })
                    .Select(i => new ShopDetails.ProductList
                    {
                        Id = i.p.p.p.Id,
                        Name = i.p.p.m.Name,
                        ShopId = i.p.p.p.ShopId,
                        ShopName = i.p.p.p.ShopName,
                        CategoryId = i.p.c.Id,
                        CategoryName = i.p.c.Name,
                        ColorCode = i.p.p.m.ColorCode,
                        Price = i.p.p.p.MenuPrice,
                        ImagePath = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                        Status = i.p.p.p.Status,
                        Customisation = i.p.p.p.Customisation,
                        DiscountCategoryPercentage = i.p.p.p.Percentage,
                        IsOnline = i.p.p.p.IsOnline,
                        NextOnTime = i.p.p.p.NextOnTime,
                        Size = i.p.p.m.SizeLWH,
                        Weight = i.p.p.m.Weight,
                        IsPreorder = i.p.p.p.IsPreorder,
                        PreorderHour = i.p.p.p.PreorderHour,
                        OfferQuantityLimit = i.p.p.p.OfferQuantityLimit,
                        IsLiked = i.cf.Any(a => a.CustomerId == customerId && a.IsFavorite == true && a.ProductId == i.p.p.p.Id),
                        LikeText = (i.cf.Any(a => a.CustomerId == customerId && a.IsFavorite == true && a.ProductId == i.p.p.p.Id) == true && i.cf.Where(a => a.ProductId == i.p.p.p.Id && a.IsFavorite == true).Count() == 1) ? "You Liked" : i.cf.Any(a => a.CustomerId == customerId && a.IsFavorite == true && a.ProductId == i.p.p.p.Id) == true ? "You & " + (i.cf.Where(a => a.ProductId == i.p.p.p.Id && a.IsFavorite == true).Count() - 1) + " more" : i.cf.Where(a => a.ProductId == i.p.p.p.Id && a.IsFavorite == true).Count() > 0 ? i.cf.Where(a => a.ProductId == i.p.p.p.Id && a.IsFavorite == true).Count() + " like" : "",
                        ShopPrice = i.p.p.p.ShopPrice
                    }).ToList();

                if (!string.IsNullOrEmpty(str))
                {
                    CustomerSearchData sData = new CustomerSearchData();
                    sData.ResultCount = model.ProductLists.Count();
                    sData.SearchKeyword = str;
                    sData.DateEncoded = DateTime.Now;
                    db.CustomerSearchDatas.Add(sData);
                    db.SaveChanges();

                }
            }
            else if (shop.ShopCategoryId == 2)
            {
                //model.ProductLists = (from pl in db.Products
                //                      join m in db.MasterProducts on pl.MasterProductId equals m.Id
                //                      join nsc in db.NextSubCategories on m.NextSubCategoryId equals nsc.Id into cat
                //                      from nsc in cat.DefaultIfEmpty()
                //                      where pl.ShopId == shopId && pl.Status == 0 && pl.Price != 0 && m.Name.ToLower().Contains(str) && (categoryId != 0 ? nsc.Id == categoryId : true)
                //                      select new ShopDetails.ProductList
                //                      {
                //                          Id = pl.Id,
                //                          Name = m.Name,
                //                          ShopId = pl.ShopId,
                //                          ShopName = pl.ShopName,
                //                          CategoryId = nsc.Id,
                //                          CategoryName = nsc.Name,
                //                          ColorCode = m.ColorCode,
                //                          Price = pl.MenuPrice,
                //                          ImagePath = ((!string.IsNullOrEmpty(m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                //                          Status = pl.Status,
                //                          Customisation = pl.Customisation,
                //                          DiscountCategoryPercentage = pl.Percentage,
                //                          IsOnline = pl.IsOnline,
                //                          NextOnTime = pl.NextOnTime,
                //                          Size = m.SizeLWH,
                //                          Weight = m.Weight,
                //                          IsPreorder = pl.IsPreorder,
                //                          PreorderHour = pl.PreorderHour,
                //                          OfferQuantityLimit = pl.OfferQuantityLimit,
                //                          //LikeText = GetProductFavorites(customerId, pl.Id)
                //                      }).Where(i => i.Price != 0).ToList();

                model.ProductLists = db.Products.Where(i => i.ShopId == shopId && i.Status == 0 && i.Price != 0 && i.MenuPrice != 0)
                    .Join(db.MasterProducts.Where(i => str != "" ? i.Name.ToLower().Contains(str.ToLower()) : true), p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                    .GroupJoin(db.NextSubCategories.Where(i => categoryId != 0 ? i.Id == categoryId : true), p => p.m.NextSubCategoryId, c => c.Id, (p, c) => new { p, c })
                    .GroupJoin(db.CustomerFavorites, p => p.p.p.Id, cf => cf.ProductId, (p, cf) => new { p, cf })
                    .Select(i => new ShopDetails.ProductList
                    {
                        Id = i.p.p.p.Id,
                        Name = i.p.p.m.Name,
                        ShopId = i.p.p.p.ShopId,
                        ShopName = i.p.p.p.ShopName,
                        CategoryId =i.p.c.Any()? i.p.c.FirstOrDefault().Id:0,
                        CategoryName = i.p.c.Any()? i.p.c.FirstOrDefault().Name:"Unknown",
                        ColorCode = i.p.p.m.ColorCode,
                        Price = i.p.p.p.MenuPrice,
                        ImagePath = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                        Status = i.p.p.p.Status,
                        Customisation = i.p.p.p.Customisation,
                        DiscountCategoryPercentage = i.p.p.p.Percentage,
                        IsOnline = i.p.p.p.IsOnline,
                        NextOnTime = i.p.p.p.NextOnTime,
                        Size = i.p.p.m.SizeLWH,
                        Weight = i.p.p.m.Weight,
                        IsPreorder = i.p.p.p.IsPreorder,
                        PreorderHour = i.p.p.p.PreorderHour,
                        OfferQuantityLimit = i.p.p.p.OfferQuantityLimit,
                        IsLiked = i.cf.Any(a => a.CustomerId == customerId && a.IsFavorite == true && a.ProductId == i.p.p.p.Id),
                        LikeText = (i.cf.Any(a => a.CustomerId == customerId && a.IsFavorite == true && a.ProductId == i.p.p.p.Id) == true && i.cf.Where(a => a.ProductId == i.p.p.p.Id && a.IsFavorite == true).Count() == 1) ? "You Liked" : i.cf.Any(a => a.CustomerId == customerId && a.IsFavorite == true && a.ProductId == i.p.p.p.Id) == true ? "You & " + (i.cf.Where(a => a.ProductId == i.p.p.p.Id && a.IsFavorite == true).Count() - 1) + " more" : i.cf.Where(a => a.ProductId == i.p.p.p.Id && a.IsFavorite == true).Count() > 0 ? i.cf.Where(a => a.ProductId == i.p.p.p.Id && a.IsFavorite == true).Count() + " like" : "",
                        ShopPrice = i.p.p.p.ShopPrice
                    }).ToList();

                if (!string.IsNullOrEmpty(str))
                {
                    CustomerSearchData sData = new CustomerSearchData();
                    sData.ResultCount = model.ProductLists.Count();
                    sData.SearchKeyword = str;
                    sData.DateEncoded = DateTime.Now;
                    db.CustomerSearchDatas.Add(sData);
                    db.SaveChanges();

                }
            }
            return new JsonResult()
            {
                ContentType = "application/json",
                Data = model,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        //public string GetProductFavorites(int custId, long prodId)
        //{
        //    var custFav = db.CustomerFavorites.FirstOrDefault(i => i.CustomerId == custId && i.ProductId == prodId && i.IsFavorite == true);
        //    int totalLikes = db.CustomerFavorites.Where(i => i.ProductId == prodId && i.IsFavorite == true).Count();
        //    if (custFav != null)
        //        return $"You & {totalLikes} others";
        //    else if (totalLikes > 0)
        //        return $"{totalLikes} others";
        //    else
        //        return "No Likes";
        //}

        public JsonResult GetCustomerRefered(int CustomerId, int shopid)
        {
            var customer = db.Customers.FirstOrDefault(i => i.Id == CustomerId);
            if (customer != null)
            {
                var referralCount = db.Customers.Where(c => c.ReferralNumber != null && c.Id == CustomerId).Count();
                var shopDistrict = db.Shops.FirstOrDefault(i => i.Id == shopid).DistrictName;
                if (!string.IsNullOrEmpty(shopDistrict))
                {
                    var referralPaymentMode = db.ReferralSettings.Where(r => r.Status == 0 && r.ShopDistrict == shopDistrict).Select(r => r.PaymentMode).FirstOrDefault();
                    if (referralCount <= 0)
                        return Json(new { Status = true, paymentMode = referralPaymentMode, walletAmount = customer.WalletAmount }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { Status = false, paymentMode = referralPaymentMode, walletAmount = customer.WalletAmount }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Status = false, paymentMode = -1, walletAmount = customer.WalletAmount }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Status = false, paymentMode = -1, walletAmount = 0 }, JsonRequestBehavior.AllowGet);
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

            if (!string.IsNullOrEmpty(str) && page==1)
            {
                CustomerSearchData sData = new CustomerSearchData();
                sData.ResultCount = count;
                sData.SearchKeyword = str;
                sData.DateEncoded = DateTime.Now;
                db.CustomerSearchDatas.Add(sData);
                db.SaveChanges();

            }

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

            return Json(new { Page = paginationMetadata, items }, JsonRequestBehavior.AllowGet);
        }

        public class Productss
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public long MasterProductId { get; set; }
            public int ShopId { get; set; }
            public int ShopCategoryId { get; set; }

            public double Price { get; set; }
            public int Qty { get; set; }
            public int ProductTypeId { get; set; }
            public int MinSelectionLimit { get; set; }
            public int MaxSelectionLimit { get; set; }
            public bool Customisation { get; set; }
            public double MenuPrice { get; set; }
            public int IBarU { get; set; }
            public int ItemId { get; set; }
            public double Percentage { get; set; }
            public int DiscountCategoryId { get; set; }
            public string DiscountCategoryName { get; set; }
            public int DataEntry { get; set; }
            public int AppliesOnline { get; set; }
            public bool IsOnline { get; set; }
            public int HoldOnStok { get; set; }
            public int PackingType { get; set; }
            public double TaxPercentage { get; set; }
            public double SpecialCostOfDelivery { get; set; }
            public int OutletId { get; set; }
            public string ItemTimeStamp { get; set; }
            public double LoyaltyPoints { get; set; }
            public double PackingCharge { get; set; }

            public int Status { get; set; }
            public System.DateTime DateEncoded { get; set; }
            public System.DateTime DateUpdated { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }

        }
        //public JsonResult GetPp(string str = "")
        //{
        //    sncEntities context = new sncEntities();
        //    List<Products1> updateList = new List<Products1>();
        //    List<Products1> creatlisdt = new List<Products1>();
        //    Products1 varProduct = new Products1();
        //    //varProduct.Id = 0;

        //    varProduct.MasterProductId = 0;
        //    varProduct.ShopId = 212;
        //    // varProduct.ShopName = "anna";
        //    varProduct.ShopCategoryId = 4;
        //    // varProduct.ShopCategoryName = "veg";
        //    varProduct.GTIN = "";
        //    varProduct.UPC = "";
        //    varProduct.GTIN14 = "";
        //    varProduct.EAN = "";
        //    varProduct.ISBN = "";
        //    varProduct.Price = 10;
        //    varProduct.Qty = 2;
        //    varProduct.ProductTypeName = "ytesg";
        //    varProduct.ProductTypeId = 0;
        //    varProduct.MinSelectionLimit = 0;
        //    varProduct.MaxSelectionLimit = 0;
        //    varProduct.Customisation = false;
        //    varProduct.MenuPrice = 12;
        //    varProduct.IBarU = 5;
        //    varProduct.ItemId = 1;
        //    varProduct.Percentage = 0;
        //    varProduct.DiscountCategoryId = 0;
        //    varProduct.DiscountCategoryName = "jhuhu";
        //    varProduct.DataEntry = 0;
        //    varProduct.AppliesOnline = 1;
        //    varProduct.IsOnline = true;
        //    varProduct.HoldOnStok = 0;
        //    varProduct.PackingType = 0;
        //    varProduct.TaxPercentage = 10.2;
        //    varProduct.SpecialCostOfDelivery = 0;
        //    varProduct.OutletId = Convert.ToInt32(2);
        //    varProduct.ItemTimeStamp = "12486365858";
        //    varProduct.LoyaltyPoints = 0;
        //    varProduct.PackingCharge = 0;
        //    varProduct.BrandOwnerMiddlePercentage = 1;
        //    varProduct.ShopownnerPrice = 10.2;
        //    varProduct.Status = 0;
        //    varProduct.DateEncoded = DateTime.Now;
        //    varProduct.DateUpdated = DateTime.Now;
        //    varProduct.CreatedBy = "serveice";
        //    varProduct.UpdatedBy = "serveice";
        //    varProduct.Name = "sdfsdfd";
        //    creatlisdt.Add(varProduct);
        //    db.BulkInsert(creatlisdt);
        //    //db.BulkSaveChanges

        //    return Json(new { Page = "" }, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetP(string str = "")
        {

            sncEntities context = new sncEntities();

            var apiSettings = db.ApiSettings.Where(m => m.Status == 0).ToList();

            if (apiSettings.Count > 0)
            {
                foreach (var api in apiSettings)
                {
                    var shop = db.Shops.Where(m => m.Id == api.ShopId).Select(i => new
                    {
                        Id=i.Id,
                        shopname=i.Name,
                        shopcategoryid=i.ShopCategoryId,
                        shopcategoryname=i.ShopCategoryName
                    }).ToList();
                    string s = "";
                    string Url = api.Url + "items?q=itemTimeStamp>=20210825121004,status==R,outletId==" + api.OutletId + "&limit=200000";
                    using (WebClient client = new WebClient())
                    {
                        client.Headers["X-Auth-Token"] = api.AuthKey; //"62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";

                        s = client.DownloadString(Url);
                    }

                    //var lst = db.Products.Where(m => m.ShopId == api.ShopId).Select(i => new
                    //{
                    //    ItemId = i.ItemId,
                    //    Id = i.Id,
                    //    MasterProductId = i.MasterProductId,
                    //    ShopName=i.ShopName,
                    //    CategoryId=i.CategoryId

                    //}).ToList();
                    var lst = db.Products.Where(m => m.ShopId == api.ShopId).ToList();
                    List<Product> updateList = new List<Product>();
                    List<Product> createList = new List<Product>();
                    List<DiscountCategory> createCategoryList = new List<DiscountCategory>();
                    DiscountCategory dc = new DiscountCategory();
                    Product varProduct = new Product();
                    List<DiscountCategory> lstDiscount = new List<DiscountCategory>();
                    goto GetDiscoutCatecories;
                GetDiscoutCatecories:
                    lstDiscount = db.DiscountCategories.Where(m => m.ShopId == api.ShopId).ToList();



                    dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(s, new ExpandoObjectConverter());
                    foreach (var pro in ((IEnumerable<dynamic>)config.items).Where(t => t.status == "R"))
                    {

                        varProduct.ItemId = Convert.ToInt32(pro.itemId);
                        varProduct.Name = pro.itemName;
                        varProduct.IBarU = Convert.ToInt32(pro.iBarU);
                        varProduct.DateUpdated = DateTime.Now;
                        varProduct.ShopCategoryId = 4;
                        varProduct.ShopId = shop[0].Id;

                        varProduct.ProductTypeId = 0;
                        varProduct.MinSelectionLimit = 0;
                        varProduct.MaxSelectionLimit = 0;
                        varProduct.Customisation = false;
                        varProduct.Percentage = 0;
                        varProduct.DiscountCategoryId = 0;
                        varProduct.DataEntry = 0; varProduct.IsOnline = true;
                        varProduct.HoldOnStok = 0;
                        varProduct.PackingType = 0;
                        varProduct.SpecialCostOfDelivery = 0;
                        varProduct.LoyaltyPoints = 0;
                        varProduct.PackingCharge = 0;
                        varProduct.Status = 0;
                        varProduct.DateEncoded = DateTime.Now;
                        varProduct.CreatedBy = "service";
                        varProduct.UpdatedBy = "service";
                        foreach (var med in pro.stock)
                        {
                            varProduct.Qty = Convert.ToInt32(Math.Floor(Convert.ToDecimal(med.stock)));
                            varProduct.MenuPrice = Convert.ToDouble(med.mrp);
                            varProduct.Price = Convert.ToDouble(med.salePrice);
                            varProduct.TaxPercentage = Convert.ToDouble(med.taxPercentage);
                            varProduct.ItemTimeStamp = Convert.ToString(med.itemTimeStamp);
                            varProduct.AppliesOnline = Convert.ToInt32(pro.appliesOnline);
                            varProduct.OutletId = Convert.ToInt32(med.outletId);
                            if (api.Category == 1)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat1);
                            else if (api.Category == 2)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat2);
                            else if (api.Category == 3)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat3);
                            else if (api.Category == 4)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat4);
                            else if (api.Category == 5)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat5);
                            else if (api.Category == 6)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat6);
                            else if (api.Category == 7)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat7);
                            else if (api.Category == 8)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat8);
                            else if (api.Category == 9)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat9);
                            else if (api.Category == 10)
                                varProduct.DiscountCategoryName = Convert.ToString(med.Cat10);

                            if (varProduct.DiscountCategoryName != null)
                                varProduct.DiscountCategoryName = varProduct.DiscountCategoryName.Trim();
                            else
                                varProduct.DiscountCategoryName = varProduct.DiscountCategoryName;

                            var catCout = lstDiscount.Where(c => c.ShopId == api.ShopId && c.Name == varProduct.DiscountCategoryName).Count();
                            if (catCout <= 0)
                            {
                                if (varProduct.DiscountCategoryName != null)
                                    dc.Name = varProduct.DiscountCategoryName.Trim();
                                else
                                    dc.Name = varProduct.DiscountCategoryName;
                                dc.ShopId = api.ShopId;
                                dc.DateEncoded = DateTime.Now;
                                dc.DateUpdated = DateTime.Now;
                                //int catExist = createCategoryList.Where(c => c.Name == dc.Name && c.ShopId == dc.ShopId).Count();
                                //if (catExist <= 0)
                                //    createCategoryList.Add(dc);
                                db.DiscountCategories.Add(dc);
                                db.SaveChanges();
                                varProduct.DiscountCategoryId = dc.Id;
                                lstDiscount = db.DiscountCategories.Where(m => m.ShopId == api.ShopId).ToList();
                            }
                            else
                            {
                                var catId = lstDiscount.Where(c => c.ShopId == api.ShopId && c.Name == varProduct.DiscountCategoryName).Select(c => c.Id).ToList();
                                varProduct.DiscountCategoryId = Convert.ToInt32(catId[0]);
                            }

                        }
                        int idx = lst.FindIndex(a => a.ItemId == pro.itemId);
                        if (idx >= 0)
                        {

                            updateList.Add(new Product
                            {
                                Id = lst[idx].Id,
                                Name = varProduct.Name,
                                MasterProductId = lst[idx].MasterProductId,
                                CategoryId= lst[idx].CategoryId,
                                ShopId = shop[0].Id,
                                ShopName = shop[0].shopname,
                                ShopCategoryId = shop[0].shopcategoryid,
                                ShopCategoryName= shop[0].shopcategoryname,
                                Price = varProduct.Price,
                                Qty = varProduct.Qty,
                                ProductTypeName=lst[idx].ProductTypeName,
                                ProductTypeId = lst[idx].ProductTypeId,
                                MinSelectionLimit = lst[idx].MinSelectionLimit,
                                MaxSelectionLimit = lst[idx].MaxSelectionLimit,
                                Customisation = lst[idx].Customisation,
                                MenuPrice = varProduct.MenuPrice,
                                IBarU = Convert.ToInt32(pro.iBarU),
                                ItemId = varProduct.ItemId,
                                Percentage = lst[idx].Percentage,
                                DiscountCategoryId = varProduct.DiscountCategoryId,
                                DiscountCategoryName = varProduct.DiscountCategoryName,
                                DataEntry = lst[idx].DataEntry,
                                AppliesOnline = varProduct.AppliesOnline,
                                IsOnline = lst[idx].IsOnline,
                                HoldOnStok = lst[idx].HoldOnStok,
                                PackingType = lst[idx].PackingType,
                                TaxPercentage = varProduct.TaxPercentage,
                                SpecialCostOfDelivery = lst[idx].SpecialCostOfDelivery,
                                OutletId = varProduct.OutletId,
                                ItemTimeStamp = varProduct.ItemTimeStamp,
                                LoyaltyPoints = lst[idx].LoyaltyPoints,
                                PackingCharge = lst[idx].PackingCharge,
                                BrandOwnerMiddlePercentage= lst[idx].BrandOwnerMiddlePercentage,
                                ShopOwnerPrice= lst[idx].ShopOwnerPrice,
                                HasSchedule= lst[idx].HasSchedule,
                                NextOnTime= lst[idx].NextOnTime,
                                IsPreorder= lst[idx].IsPreorder,
                                PreorderHour= lst[idx].PreorderHour,
                                OfferQuantityLimit= lst[idx].OfferQuantityLimit,
                                MappedDate= lst[idx].MappedDate,
                                Status = lst[idx].Status,
                                DateEncoded = lst[idx].DateEncoded,
                                DateUpdated = DateTime.Now,
                                UpdatedBy="Service"
                                
                            });
                            //update
                        }
                        else
                        {
                            long masterid = 0;
                            long id = 0;
                            createList.Add(new Product
                            {
                                Id = id,
                                Name = varProduct.Name,
                                ShopId = shop[0].Id,
                                ShopName = shop[0].shopname,
                                ShopCategoryId = shop[0].shopcategoryid,
                                ShopCategoryName = shop[0].shopcategoryname,
                                Price = varProduct.Price,
                                Qty = varProduct.Qty,
                                ProductTypeId = 0,
                                MinSelectionLimit = varProduct.MinSelectionLimit,
                                MaxSelectionLimit = varProduct.MaxSelectionLimit,
                                Customisation = false,
                                MenuPrice = varProduct.MenuPrice,
                                IBarU = Convert.ToInt32(pro.iBarU),
                                ItemId = varProduct.ItemId,
                                Percentage = 0,
                                DiscountCategoryId = varProduct.DiscountCategoryId,
                                DiscountCategoryName = varProduct.DiscountCategoryName,
                                DataEntry = varProduct.DataEntry,
                                AppliesOnline = varProduct.AppliesOnline,
                                IsOnline = true,
                                HoldOnStok = 0,
                                PackingType = 0,
                                TaxPercentage = varProduct.TaxPercentage,
                                SpecialCostOfDelivery = 0,
                                OutletId = varProduct.OutletId,
                                ItemTimeStamp = varProduct.ItemTimeStamp,
                                LoyaltyPoints = 0,
                                PackingCharge = 0,
                                BrandOwnerMiddlePercentage = 0,
                                ShopOwnerPrice = 0,
                                HasSchedule = false,
                                NextOnTime = null,
                                IsPreorder = false,
                                PreorderHour = 0,
                                OfferQuantityLimit = 0,
                                Status = 0,
                                DateEncoded = DateTime.Now,
                                DateUpdated= DateTime.Now,
                                CreatedBy = "Service"
                            });
                            //createList.Add(varProduct);
                        }

                    }
                    db.BulkInsert(createList);
                    if (updateList.Count > 0)
                        db.BulkUpdate(updateList);
                    //db.Products1.AddRange(createList);
                    db.BulkSaveChanges();
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
            return Json(new { Page = "" }, JsonRequestBehavior.AllowGet);
        }

        public static double GetStockQty(string code, int outletid)
        {

            using (WebClient myData = new WebClient())
            {

                myData.Headers["X-Auth-Token"] = "62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";
                myData.Headers[HttpRequestHeader.Accept] = "application/json";
                myData.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string getList = "";
                if (outletid == 0)
                    getList = myData.DownloadString("http://joyrahq.gofrugal.com/RayMedi_HQ/api/v1/items?q=status==R,outletId==2,itemId==" + code);
                else
                    getList = myData.DownloadString("http://joyrahq.gofrugal.com/RayMedi_HQ/api/v1/items?q=status==R,outletId==" + outletid + ",itemId==" + code);

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

        public JsonResult GetShopCategoryList(int shopId = 0, int CategoryId = 0,int customerId=0, string str = "", int page = 1, int pageSize = 20)
        {
            //  var shid = db.Shops.Where(s => s.Id == shopId).FirstOrDefault();
            int? count = 0;
            var total = db.GetShopCategoryProductCount(shopId, CategoryId, str).ToList();
            if (total.Count() > 0)
                count = total[0];
            var skip = page - 1;
            var model = db.GetShopCategoryProducts(shopId, CategoryId, str, skip, pageSize, customerId).ToList();
            int CurrentPage = page;
            int PageSize = pageSize;
            int? TotalCount = count;


            if (!string.IsNullOrEmpty(str) && page ==1)
            {
                CustomerSearchData sData = new CustomerSearchData();
                sData.ResultCount = TotalCount ?? 0;
                sData.SearchKeyword = str;
                sData.DateEncoded = DateTime.Now;
                db.CustomerSearchDatas.Add(sData);
                db.SaveChanges();

            }

            int TotalPages = (int)Math.Ceiling(count.Value / (double)PageSize);
            var items = model;
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetShopCategoryList?shopId=" + shopId + "&categoryId=" + CategoryId + "&customerid=" + customerId + "&str=" + str + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "/Api/GetShopCategoryList?shopId=" + shopId + "&categoryId=" + CategoryId + "&customerid=" + customerId + "&str=" + str + "&page=" + current;
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
            //var shid = db.Shops.Where(s => s.Id == id).FirstOrDefault();
            var shop = db.Shops.FirstOrDefault(i => i.Id == id);
            ShopSingleUpdateViewModel model = _mapper.Map<Shop, ShopSingleUpdateViewModel>(shop);
            model.ImagePath = model.ImagePath =="Rejected" ? "Rejected": ((!string.IsNullOrEmpty(model.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + model.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg");
            model.ImageAadharPath = model.ImageAadharPath == "Rejected" ? "Rejected" : ((!string.IsNullOrEmpty(model.ImageAadharPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + model.ImageAadharPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/notavailable.png");
            model.ImageAccountPath = model.ImageAccountPath == "Rejected" ? "Rejected" : ((!string.IsNullOrEmpty(model.ImageAccountPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + model.ImageAccountPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/notavailable.png");
            model.ImageFSSAIPath = model.ImageFSSAIPath == "Rejected" ? "Rejected" : ((!string.IsNullOrEmpty(model.ImageFSSAIPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + model.ImageFSSAIPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/notavailable.png");
            model.ImagePanPath = model.ImagePanPath == "Rejected" ? "Rejected" : ((!string.IsNullOrEmpty(model.ImagePanPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + model.ImagePanPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/notavailable.png");
            model.ImageGSTINPath = model.ImageGSTINPath == "Rejected" ? "Rejected" : ((!string.IsNullOrEmpty(model.ImageGSTINPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + model.ImageGSTINPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/notavailable.png");
            model.ImageDrugPath = model.ImageDrugPath == "Rejected" ? "Rejected" : ((!string.IsNullOrEmpty(model.ImageDrugPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + model.ImageDrugPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/notavailable.png");
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetShopBalanceNotification(int customerId)
        {
            var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == customerId);
            if (shopCredits != null)
            {
                if (shopCredits.PlatformCredit <= 100)
                {
                    return Json(new { message = "Recharge Immediately" }, JsonRequestBehavior.AllowGet);
                }
                else if (shopCredits.DeliveryCredit <= 150)
                {
                    return Json(new { message = "Recharge Immediately" }, JsonRequestBehavior.AllowGet);
                }
                else if (shopCredits.PlatformCredit <= 200 && shopCredits.PlatformCredit > 100)
                {
                    return Json(new { message = "Your Credit are Low !" }, JsonRequestBehavior.AllowGet);
                }
                else if (shopCredits.DeliveryCredit >= 150 && shopCredits.DeliveryCredit <= 250)
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
                         }).OrderByDescending(i => i.Id).ToList();
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
            db.Configuration.ProxyCreationEnabled = false;
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.Id);
            shop.ImagePath = model.ImagePath;
            shop.UpdatedBy = shop.CustomerName;
            shop.DateUpdated = DateTime.Now;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { message = "Successfully Updated Your Shop Image!", Details = shop });
        }

        //public JsonResult GetProductDetails(int id)
        //{
        //    var product = db.Products.FirstOrDefault(i => i.Id == id);
        //    ProductDetailsViewModel model = _mapper.Map<Product, ProductDetailsViewModel>(product);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult ProductQuickUpdate(ProductQuickUpdateViewModel model)
        {
            db.Configuration.ProxyCreationEnabled = false;
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
            model.ImagePath = model.ImagePath != null ? model.ImagePath.Contains("https://s3.ap-south-1.amazonaws.com/") ? model.ImagePath : "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + model.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : model.ImagePath;
            model.ImageAadharPath = model.ImageAadharPath != null ? model.ImageAadharPath.Contains("https://s3.ap-south-1.amazonaws.com/") ? model.ImageAadharPath : "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + model.ImageAadharPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : model.ImageAadharPath;
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
                                ImagePath = ((!string.IsNullOrEmpty(i.ss.s.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ss.s.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                                DistrictName = i.ss.s.DistrictName,
                                Verify = i.ss.s.Verify,
                                OtpVerify = i.o.Any() ? i.o.LastOrDefault().Verify : false,
                                CustomerId = i.ss.s.CustomerId,
                                DateEncoded = i.o.Any() ? i.o.LastOrDefault().DateEncoded.ToString("dd/MMM/yyyy") : "N/A",
                                NextOnTime = i.ss.s.NextOnTime
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
            if (delivaryBoy != null)
            {
                if (delivaryBoy.Active == 1)
                {
                    return Json("You are Active", JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("You are InActive", JsonRequestBehavior.AllowGet);
            }
            else
                return Json("Delivery Boy not available", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelivaryBoyActive(int customerId, int state)
        {
            if (state == 1 && (customerId != 0))
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                if (customer != null)
                {
                    var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.CustomerId == customerId && i.Status == 0);
                    if (delivaryBoy != null)
                    {
                        delivaryBoy.Active = 1;
                        delivaryBoy.UpdatedBy = customer.Name;
                        db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json("You are Active", JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json("Delivery boy not available", JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("Customer not available", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                if (customer != null)
                {
                    var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.CustomerId == customerId && i.Status == 0);
                    if (delivaryBoy != null)
                    {
                        delivaryBoy.Active = 0;
                        delivaryBoy.UpdatedBy = customer.Name;
                        db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json("You are InActive", JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json("Delivery boy not available", JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("Customer not available", JsonRequestBehavior.AllowGet);
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
            string query8KM = "SELECT * " +
                               " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and Status = 0 and Latitude != 0 and Longitude != 0" +
                               " order by IsOnline desc,Adscore desc,Rating desc";
            var list8KM = db.Shops.SqlQuery(query8KM,
             new SqlParameter("Latitude", Latitude),
             new SqlParameter("Longitude", Longitude)).Select(i => new NearShopImages.shops
             {
                 id = i.Id,
                 image = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
                 IsOnline = i.IsOnline,
                 NextOnTime = i.NextOnTime
             }).ToList();

            string query16KM = "SELECT * " +
                               " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 16 and ShopCategoryId=4 and Status = 0 and Latitude != 0 and Longitude != 0" +
                               " order by IsOnline desc,Adscore desc,Rating desc";
            var list16KM = db.Shops.SqlQuery(query16KM,
             new SqlParameter("Latitude", Latitude),
             new SqlParameter("Longitude", Longitude)).Select(i => new NearShopImages.shops
             {
                 id = i.Id,
                 image = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
                 IsOnline = i.IsOnline,
                 NextOnTime = i.NextOnTime
             }).ToList();

            model.NearShops = list8KM.Concat(list16KM).ToList();
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
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 16 and ShopCategoryId = 4 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
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
                 new SqlParameter("Longitude", Longitude))
                 .Select(i => new PlacesListView.Places
                 {
                     Id = i.Id,
                     Name = i.Name,
                     DistrictName = i.StreetName,
                     Rating = RatingCalculation(i.Id),
                     ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                     ShopCategoryId = i.ShopCategoryId,
                     ShopCategoryName = i.ShopCategoryName,
                     List = GetBannerImageList(i.Id,i.Name),
                     Latitude = i.Latitude,
                     Longitude = i.Longitude,
                     Status = i.Status,
                     isOnline = i.IsOnline,
                     ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                     Address = i.Address,
                     NextOnTime = i.NextOnTime,
                     OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status==0).Max(b => b.Percentage)
                 }).ToList();
                model.SuperMarketList = db.Shops.SqlQuery(querySuperMarketList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
                }).ToList();
                model.GroceriesList = db.Shops.SqlQuery(queryGroceriesList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
                }).ToList();
                model.HealthList = db.Shops.SqlQuery(queryHealthList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
                }).ToList();

                model.ElectronicsList = db.Shops.SqlQuery(queryElectronicsList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
                }).ToList();
                model.ServicesList = db.Shops.SqlQuery(qServicesList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
                }).ToList();
                model.OtherList = db.Shops.SqlQuery(queryOtherList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    List = GetBannerImageList(i.Id, i.Name),
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
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
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
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
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),// db.Banners.Where(j => j.Status == 0 && j.ShopCode == i.Code).ToList(),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
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
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
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
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
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
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
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
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
                }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string queryOtherList = "SELECT top(6) * " +
              " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId = 7 and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0 " +
              " order by IsOnline desc,Adscore desc,Rating desc";
                model.OtherList = db.Shops.SqlQuery(queryOtherList,
                new SqlParameter("Latitude", Latitude),
                new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
                {
                    Id = i.Id,
                    Name = i.Name,
                    DistrictName = i.StreetName,
                    Rating = RatingCalculation(i.Id),
                    ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                    ShopCategoryId = i.ShopCategoryId,
                    ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id, i.Name),
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Status = i.Status,
                    isOnline = i.IsOnline,
                    ReviewCount = db.CustomerReviews.Where(c => c.ShopId == i.Id).Count(),
                    Address = i.Address,
                    NextOnTime = i.NextOnTime,
                    OfferPercentage = db.Products.Where(b => b.ShopId == i.Id && b.Status == 0).Max(b => b.Percentage)
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
                model.List = db.Orders.Where(i => i.Status == 6 && i.ShopId == shopId && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now))
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                   .Select(i => new ShopOrderAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       ShopPaymentStatus = i.c.ShopPaymentStatus,
                       Amount = i.p.OriginalAmount,
                       DateEncoded = i.c.DateEncoded
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
                model.List = db.Orders.Where(i => i.Status == 6 && i.ShopId == shopId && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(from1) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(to1)))
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                   .Select(i => new ShopOrderAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       ShopPaymentStatus = i.c.ShopPaymentStatus,
                       Amount = i.p.OriginalAmount,
                       DateEncoded = i.c.DateEncoded
                   }).ToList();
                if (model.List.Count() != 0)
                {
                    model.TotalAmount = model.List.Sum(i => Convert.ToDouble(i.Amount));
                    model.ShopPaymentStatus = model.List.FirstOrDefault().ShopPaymentStatus;
                }
            }
            else
            {
                model.List = db.Orders.Where(i => i.Status == 6 && i.ShopId == shopId && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now))
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                   .Select(i => new ShopOrderAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       ShopPaymentStatus = i.c.ShopPaymentStatus,
                       Amount = i.p.OriginalAmount,
                       DateEncoded = i.c.DateEncoded
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
            var previousurl = apipath + "Api/GetShopOrderReport?shopId=" + shopId + "&dt=" + dt + "&from=" + from + "&to=" + to + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "Api/GetShopOrderReport?shopId=" + shopId + "&dt=" + dt + "&from=" + from + "&to=" + to + "&page=" + current;
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
            model.List = db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPhoneNumber == phoneNumber && i.DeliveryOrderPaymentStatus == 0)
                 .Join(db.Payments.Where(i => i.PaymentMode != "Online Payment"), c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                {
                    Amount = (i.p.Amount - (i.p.RefundAmount ?? 0))

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
                model.List = db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPhoneNumber == phoneNumber && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now))
                            .Join(db.Shops, scc => scc.ShopId, s => s.Id, (scc, s) => new { scc, s })
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
                model.List = db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPhoneNumber == phoneNumber && ((DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(from1)) &&
            (DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(to1))))
                            .Join(db.Shops, scc => scc.ShopId, s => s.Id, (scc, s) => new { scc, s })
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
                   }).OrderByDescending(j => j.DateEncoded).ToList();
                if (model.List.Count() != 0)
                {
                    model.EarningOfToday = model.List.Sum(i => i.GrossDeliveryCharge);
                }
            }
            else
            {
                model.List = db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPhoneNumber == phoneNumber && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now))
                            .Join(db.Shops, scc => scc.ShopId, s => s.Id, (scc, s) => new { scc, s })
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
            var previousurl = apipath + "Api/GetDelivaryBoyReport?phoneNumber=" + phoneNumber + "&dt=" + dt + "&from=" + from + "&to=" + to + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";
            var current = CurrentPage + 1;
            var nexturl = apipath + "Api/GetDelivaryBoyReport?phoneNumber=" + phoneNumber + "&dt=" + dt + "&from=" + from + "&to=" + to + "&page=" + current;
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
                model.List = db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPhoneNumber == phoneNumber && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now) && i.DeliveryOrderPaymentStatus == 0)
                    .Join(db.Payments.Where(i => i.PaymentMode != "Online Payment"), c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                   .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       GrossDeliveryCharge = i.c.DeliveryCharge,
                       DeliveryBoyPaymentStatus = i.c.DeliveryBoyPaymentStatus,
                       Amount = i.p.Amount,
                       DateEncoded = i.c.DateEncoded
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
                model.List = db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPhoneNumber == phoneNumber && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(from1) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(to1)) && i.DeliveryOrderPaymentStatus == 0)
                    .Join(db.Payments.Where(i => i.PaymentMode != "Online Payment"), c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                   .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       GrossDeliveryCharge = i.c.DeliveryCharge,
                       DeliveryBoyPaymentStatus = i.c.DeliveryBoyPaymentStatus,
                       Amount = i.p.Amount,
                       DateEncoded = i.c.DateEncoded
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
                model.List = db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPhoneNumber == phoneNumber && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now) && i.DeliveryOrderPaymentStatus == 0)
                    .Join(db.Payments.Where(i => i.PaymentMode != "Online Payment"), c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                   .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                   {
                       OrderNumber = i.c.OrderNumber,
                       CartStatus = i.c.Status,
                       GrossDeliveryCharge = i.c.DeliveryCharge,
                       DeliveryBoyPaymentStatus = i.c.DeliveryBoyPaymentStatus,
                       Amount = i.p.Amount,
                       DateEncoded = i.c.DateEncoded
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

        List<BannerImages> GetBannerImageList(int id, string name)
        {
            try
            {
                var d = DateTime.Now.Date.Date;
                //var banners = (from s in db.Banners
                //               where (s.Status == 0 || s.Status == 6) && s.ShopId == id && (DbFunctions.TruncateTime(s.FromDate) <= DbFunctions.TruncateTime(DateTime.Now) && DbFunctions.TruncateTime(s.Todate) >= DbFunctions.TruncateTime(DateTime.Now))
                //               select new BannerImages { Bannerpath = (s.BannerPath != null) ? s.BannerPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "", ShopId = s.ShopId, ProductId = s.MasterProductId }).ToList();

                var banners = db.Banners.Where(i => i.Status == 0 && i.ShopId == id && (DbFunctions.TruncateTime(i.FromDate) <= DbFunctions.TruncateTime(DateTime.Now) && DbFunctions.TruncateTime(i.Todate) >= DbFunctions.TruncateTime(DateTime.Now)))
                .GroupJoin(db.MasterProducts, b => b.MasterProductId, m => m.Id, (b, m) => new { b, m })
                .Select(i => new BannerImages
                {
                    Bannerpath = !(string.IsNullOrEmpty(i.b.BannerPath)) ? i.b.BannerPath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
                    ProductId = i.b.MasterProductId,
                    ProductName = i.m.Any()? i.m.FirstOrDefault().Name :"",
                    ShopId = i.b.ShopId,
                    ShopName = name
                }).ToList();
                return banners;
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
            model.List = db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPhoneNumber == phoneNo && ((DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(startDate)) && (DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(endDate))))
            //.Join(db.DeliveryBoys.Where(i => i.PhoneNumber == phoneNo), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
            .GroupBy(i => DbFunctions.TruncateTime(i.DateEncoded))
            .AsEnumerable()
            .Select(i => new DelivaryBoyPayoutReportViewModel.PayoutOut
            {
                //Date = i.Any() ? i.FirstOrDefault().DateEncoded.ToString("dd-MMM-yyyy HH:ss") : "",
                Date = i.FirstOrDefault().DateEncoded,
                TotalAmount = i.Sum(a => a.DeliveryCharge),
                PaidAmount = GetPaidAmount(i.Key.Value, phoneNo),
            }).OrderByDescending(j => j.Date).ToList();
            int count = model.List.Count();
            int CurrentPage = page;
            int PageSize = pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.List;
            //var items = model.List.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            //var previous = CurrentPage - 1;
            //var previousurl = apipath + "Api/GetDelivaryBoyReport?startDate=" + startDate + "&endDate=" + endDate + "&phoneNo=" + phoneNo + "&page=" + previous;
            //var previousPage = CurrentPage > 1 ? previousurl : "No";
            //var current = CurrentPage + 1;
            //var nexturl = apipath + "Api/GetDelivaryBoyReport?startDate=" + startDate + "&endDate=" + endDate + "&phoneNo=" + phoneNo + "&page=" + current;
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
                 //.Join(db.Shops.Where(i => i.Id == shopId), p => p.ShopId, s => s.Id, (p, s) => new { p, s })
                 .Join(db.Orders.Where(i => i.Status == 6 && i.ShopId == shopId), p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                 .GroupBy(i => DbFunctions.TruncateTime(i.p.DateEncoded))
                 .AsEnumerable()
                 .Select(i => new ShopApiReportsViewModel.EarningListItem
                 {
                     // Date = i.Any() ? i.FirstOrDefault().p.DateEncoded.ToString("dd-MMM-yyyy HH:ss") : "",
                     DateEncoded = i.FirstOrDefault().p.DateEncoded,
                     Earning = i.Sum(a => a.p.Amount),
                     Paid = GetShopPaidAmount(i.Key.Value, shopId),
                 }).OrderByDescending(j => j.DateEncoded).ToList();
                return Json(model.ListItems, JsonRequestBehavior.AllowGet);
            }
            else if (type == 2) //Refund & Delivery
            {
                model.RefundLists = db.Payments
                    .Where(i => ((DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(startDate)) &&
                (DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(endDate))))
                //.Join(db.Shops.Where(i => i.Id == shopId), p => p.ShopId, s => s.Id, (p, s) => new { p, s })
                .Join(db.Orders.Where(i => i.Status == 6 && i.ShopId == shopId), p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                // .Join(db.ShopCharges.Where(i => i.Status == 0), p => p.p.p.OrderNo, sc => sc.OrderNo, (p, sc) => new { p, sc })
                // .AsEnumerable()
                .Select(i => new ShopApiReportsViewModel.RefundListItem
                {
                    // Date = i.p.DateEncoded.ToString("dd-MMM-yyyy HH:ss"),
                    DateEncoded = i.p.DateEncoded,
                    Earning = i.p.Amount,
                    Refund = i.p.RefundAmount ?? 0,
                    DeliveryCredits = i.c.DeliveryCharge,
                    OrderNo = i.p.OrderNumber
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

        public JsonResult GetAddonList(long productId)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == productId);
            if (product != null)
            {
                var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == product.MasterProductId);
                var list = db.ShopDishAddOns.Where(i => i.ProductId == productId && i.IsActive == true).ToList();
                if (list.Count > 0)
                {
                    var model = new ApiProductAddonViewModel();
                    model.Type = list.FirstOrDefault().AddOnType;
                    model.MinLimit = product.MinSelectionLimit;
                    model.MaxLimit = product.MaxSelectionLimit;
                    if (list.FirstOrDefault().AddOnType == 1)
                    {
                        model.PortionListItems = list.Where(i => i.AddOnType == 1).Select((i, index) => new ApiProductAddonViewModel.PortionListItem
                        {
                            Index = index + 1,
                            AddonId = i.Id,
                            PortionId = i.PortionId,
                            PortionName = i.PortionName,
                            PortionPrice = i.PortionPrice
                        }).ToList();
                    }
                    if (list.FirstOrDefault().AddOnType == 2)
                    {
                        model.AddonListItems = list.Where(i => i.AddOnType == 2).Select((i, index) => new ApiProductAddonViewModel.AddonListItem
                        {
                            Index = index + 1,
                            AddonId = i.Id,
                            AddonName = i.AddOnItemName,
                            AddonPrice = i.AddOnsPrice,
                            AddonCategoryName = i.AddOnCategoryName,
                            ColorCode = masterProduct.ColorCode,
                            PortionId = i.PortionId
                        }).ToList();
                    }

                    if (list.FirstOrDefault().AddOnType == 3)
                    {
                        model.PortionListItems = list.Where(i => i.AddOnType == 3).GroupBy(i => i.PortionId).Select((i, index) => new ApiProductAddonViewModel.PortionListItem
                        {
                            Index = index + 1,
                            AddonId = i.FirstOrDefault().Id,
                            PortionId = i.Key,
                            PortionName = i.FirstOrDefault().PortionName,
                            PortionPrice = i.FirstOrDefault().PortionPrice
                        }).ToList();

                        model.AddonListItems = list.Where(i => i.AddOnType == 3).Select((i, index) => new ApiProductAddonViewModel.AddonListItem
                        {
                            Index = index + 1,
                            AddonId = i.Id,
                            PortionId = i.PortionId,
                            AddonName = i.AddOnItemName,
                            AddonPrice = i.AddOnsPrice,
                            AddonCategoryName = i.AddOnCategoryName,
                            ColorCode = masterProduct.ColorCode
                        }).ToList();
                    }

                    if (list.FirstOrDefault().AddOnType == 4)
                    {
                        model.PortionListItems = list.Where(i => i.AddOnType == 4).GroupBy(i => new { i.PortionId, i.CrustId }).Select((i, index) => new ApiProductAddonViewModel.PortionListItem
                        {
                            Index = index + 1,
                            AddonId = i.FirstOrDefault().Id,
                            PortionId = i.Key.PortionId,
                            PortionName = i.FirstOrDefault().PortionName,
                            PortionPrice = i.FirstOrDefault().PortionPrice,
                            CrustId = i.Key.CrustId
                        }).ToList();

                        model.AddonListItems = list.Where(i => i.AddOnType == 4).Select((i, index) => new ApiProductAddonViewModel.AddonListItem
                        {
                            Index = index + 1,
                            AddonId = i.Id,
                            PortionId = i.PortionId,
                            AddonName = i.AddOnItemName,
                            AddonPrice = i.AddOnsPrice,
                            AddonCategoryName = i.AddOnCategoryName,
                            ColorCode = masterProduct.ColorCode,
                            CrustId = i.CrustId
                        }).ToList();

                        model.CrustListItems = list.Where(i => i.AddOnType == 4).GroupBy(i => i.CrustId).Select((i, index) => new ApiProductAddonViewModel.CrustListItem
                        {
                            Index = index + 1,
                            CrustName = i.FirstOrDefault().CrustName,
                            CrustId = i.FirstOrDefault().CrustId,
                            AddonId = i.FirstOrDefault().Id
                        }).ToList();
                    }
                    return Json(new { result = model }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetAllOrders(int customerId, int page = 1, int pageSize = 5, int type = 0) //1-Live,2-Past
        {
            db.Configuration.ProxyCreationEnabled = false;
            var model = new GetAllOrderListViewModel();
            model.OrderLists = db.Orders.Where(i => i.CustomerId == customerId && (type == 1 ? (i.Status >= 2 && i.Status <= 5) || i.Status == 8 : (i.Status == 6 || i.Status == 7 || i.Status == 9 || i.Status == 10)))
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
                     RefundAmount = i.o.p.RefundAmount,
                     RefundRemark = i.o.p.RefundRemark,
                     PaymentMode = i.o.p.PaymentMode,
                     WalletAmount = i.o.o.WalletAmount,
                     TipsAmount = i.o.o.TipsAmount,
                     OrderItemLists = i.oi.Select(a => new GetAllOrderListViewModel.OrderList.OrderItemList {
                         AddOnType = a.AddOnType,
                         BrandId = a.BrandId,
                         BrandName = a.BrandName,
                         CategoryId = a.CategoryId,
                         CategoryName = a.CategoryName,
                         HasAddon = a.HasAddon,
                         ImagePath = a.ImagePath,
                         OrdeNumber = a.OrdeNumber,
                         OrderId = a.OrderId,
                         Price = a.Price,
                         ProductId = a.ProductId,
                         ProductName = a.ProductName,
                         Quantity = a.Quantity,
                         UnitPrice = a.UnitPrice,
                         ShopId = i.o.o.ShopId,
                         ShopName = i.o.o.ShopName,
                         OutletId = db.Products.FirstOrDefault(b=>b.Id == a.ProductId).OutletId,
                         //OfferQuantityLimit = db.Products.FirstOrDefault(b=>b.Id == a.ProductId).OfferQuantityLimit,
                         OrderItemAddonLists = db.OrderItemAddons.Where(b => b.OrderItemId == a.Id).Select(b => new GetAllOrderListViewModel.OrderList.OrderItemList.OrderItemAddonList {
                             AddonName = b.AddonName,
                             AddonPrice = b.AddonPrice,
                             CrustName = b.CrustName,
                             PortionName = b.PortionName,
                             PortionPrice = b.PortionPrice
                         }).ToList()
                     }).ToList()
                 }).OrderByDescending(i => i.DateEncoded).ToList();

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

        public JsonResult GetAllOffers(double Latitude, double Longitude)
        {
            string query = "SELECT * " +
                               " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and Status = 0  and Latitude != 0 and Longitude != 0";
            var model = new OfferApiListViewModel();
            model.OfferListItems = db.Offers.ToList().Where(i => i.Status == 0 && i.Type == 1) //now only for cart offer
                .Join(db.OfferShops, o => o.Id, oShp => oShp.OfferId, (o, oShp) => new { o, oShp })
             .Join(db.Shops.SqlQuery(query,
                 new SqlParameter("Latitude", Latitude),
                 new SqlParameter("Longitude", Longitude)), o => o.oShp.ShopId, s => s.Id, (o, s) => new { o, s })
                 .GroupBy(i => i.o.o.Id)
                .Select(i => new OfferApiListViewModel.OfferListItem
                {
                    AmountLimit = i.FirstOrDefault().o.o.AmountLimit,
                    BrandId = i.FirstOrDefault().o.o.BrandId,
                    CustomerCountLimit = i.FirstOrDefault().o.o.CustomerCountLimit,
                    DiscountType = i.FirstOrDefault().o.o.DiscountType,
                    Id = i.FirstOrDefault().o.o.Id,
                    IsForBlackListAbusers = i.FirstOrDefault().o.o.IsForBlackListAbusers,
                    IsForFirstOrder = i.FirstOrDefault().o.o.IsForFirstOrder,
                    IsForOnlinePayment = i.FirstOrDefault().o.o.IsForOnlinePayment,
                    MinimumPurchaseAmount = i.FirstOrDefault().o.o.MinimumPurchaseAmount,
                    Name = i.FirstOrDefault().o.o.Name,
                    OfferCode = i.FirstOrDefault().o.o.OfferCode,
                    //OwnerType = i.FirstOrDefault().o.o.OwnerType,
                    Percentage = i.FirstOrDefault().o.o.Percentage,
                    QuantityLimit = i.FirstOrDefault().o.o.QuantityLimit,
                    Type = i.FirstOrDefault().o.o.Type,
                    Description = i.FirstOrDefault().o.o.Description
                }).ToList();
            return Json(new { list = model.OfferListItems }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOfferList(int id)
        {
            var model = new OfferRelatedApiListViewModel();
            var offer = db.Offers.FirstOrDefault(i => i.Id == id);
            if (offer != null)
            {
                if (offer.Type == 1)
                {
                    model.ShopOfferListItems = db.Offers.Where(i => i.Status == 0 && i.Id == id && i.Type == 1)
                        .Join(db.OfferShops, o => o.Id, oShp => oShp.OfferId, (o, oShp) => new { o, oShp })
                     .Join(db.Shops, o => o.oShp.ShopId, s => s.Id, (o, s) => new { o, s })
                        .Select(i => new OfferRelatedApiListViewModel.ShopOfferListItem
                        {
                            AmountLimit = i.o.o.AmountLimit,
                            BrandId = i.o.o.BrandId,
                            CustomerCountLimit = i.o.o.CustomerCountLimit,
                            DiscountType = i.o.o.DiscountType,
                            Id = i.o.o.Id,
                            IsForBlackListAbusers = i.o.o.IsForBlackListAbusers,
                            IsForFirstOrder = i.o.o.IsForFirstOrder,
                            IsForOnlinePayment = i.o.o.IsForOnlinePayment,
                            MinimumPurchaseAmount = i.o.o.MinimumPurchaseAmount,
                            Name = i.o.o.Name,
                            OfferCode = i.o.o.OfferCode,
                            OwnerType = i.o.o.OwnerType,
                            Percentage = i.o.o.Percentage,
                            QuantityLimit = i.o.o.QuantityLimit,
                            Type = i.o.o.Type,
                            Description = i.o.o.Description,
                            ShopId = i.o.oShp.ShopId,
                            ShopImage = i.s.ImagePath,
                            ShopName = i.s.Name
                        }).ToList();
                    return Json(new { list = model.ShopOfferListItems }, JsonRequestBehavior.AllowGet);
                }

                if (offer.Type == 2)
                {
                    model.ProductOfferListItems = db.Offers.Where(i => i.Status == 0 && i.Id == id && i.Type == 2)
                        .Join(db.OfferProducts, o => o.Id, oPro => oPro.OfferId, (o, oPro) => new { o, oPro })
                     .Join(db.Products, o => o.oPro.ProductId, p => p.Id, (o, p) => new { o, p })
                     .Join(db.MasterProducts, o => o.p.MasterProductId, m => m.Id, (o, m) => new { o, m })
                        .Select(i => new OfferRelatedApiListViewModel.ProductOfferListItem
                        {
                            AmountLimit = i.o.o.o.AmountLimit,
                            BrandId = i.o.o.o.BrandId,
                            CustomerCountLimit = i.o.o.o.CustomerCountLimit,
                            DiscountType = i.o.o.o.DiscountType,
                            Id = i.o.o.o.Id,
                            IsForBlackListAbusers = i.o.o.o.IsForBlackListAbusers,
                            IsForFirstOrder = i.o.o.o.IsForFirstOrder,
                            IsForOnlinePayment = i.o.o.o.IsForOnlinePayment,
                            MinimumPurchaseAmount = i.o.o.o.MinimumPurchaseAmount,
                            Name = i.o.o.o.Name,
                            OfferCode = i.o.o.o.OfferCode,
                            OwnerType = i.o.o.o.OwnerType,
                            Percentage = i.o.o.o.Percentage,
                            QuantityLimit = i.o.o.o.QuantityLimit,
                            Type = i.o.o.o.Type,
                            Description = i.o.o.o.Description,
                            ProductId = i.o.p.Id,
                            ProductImage = i.m.ImagePath1,
                            ProductName = i.m.Name
                        }).ToList();
                    return Json(new { list = model.ProductOfferListItems }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCartOfferList(int shopId)
        {
            var model = new CartOfferApiListViewModel();

            model.OfferListItems = db.Offers.Where(i => i.Status == 0 && i.Type == 1)
                .Join(db.OfferShops.Where(i => i.ShopId == shopId), o => o.Id, oShp => oShp.OfferId, (o, oShp) => new { o, oShp })
                .Select(i => new CartOfferApiListViewModel.OfferListItem
                {
                    AmountLimit = i.o.AmountLimit,
                    DiscountType = i.o.DiscountType,
                    Id = i.o.Id,
                    MinimumPurchaseAmount = i.o.MinimumPurchaseAmount,
                    Name = i.o.Name,
                    OfferCode = i.o.OfferCode,
                    Percentage = i.o.Percentage,
                    Description = i.o.Description,
                }).ToList();
            return Json(new { list = model.OfferListItems }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductFreeOffer(int shopId)
        {
            var model = new ProductFreeOfferApiViewModel();
            model = db.Offers.Where(i => i.Status == 0 && i.Type == 3)
                .Join(db.OfferShops.Where(i => i.ShopId == shopId), o => o.Id, oShp => oShp.OfferId, (o, oShp) => new { o, oShp })
                .Join(db.OfferProducts, o => o.o.Id, oPro => oPro.OfferId, (o, oPro) => new { o, oPro })
                .Join(db.Products, o => o.oPro.ProductId, p => p.Id, (o, p) => new { o, p })
                .Join(db.MasterProducts, o => o.p.MasterProductId, m => m.Id, (o, m) => new { o, m })
                .Select(i => new ProductFreeOfferApiViewModel
                {
                    Id = i.o.o.o.o.Id,
                    MinimumPurchaseAmount = i.o.o.o.o.MinimumPurchaseAmount,
                    Name = i.o.o.o.o.Name,
                    OfferCode = i.o.o.o.o.OfferCode,
                    Description = i.o.o.o.o.Description,
                    ProductId = i.o.p.Id,
                    ProductName = i.m.Name,
                    ProductImage = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + i.m.ImagePath1
                }).FirstOrDefault();
            return Json(new { offer = model }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetCartOffer(int shopid, int customerid, double amount, int paymentMode) //1-Online, 2-COH
        //{
        //    //Have to check IsFor1stOrder and Paymentmode and 1st order
        //    var model = new CartOfferApiListViewModel();
        //    model.OfferListItems = db.Offers.Where(i => i.Status == 0 && (i.MinimumPurchaseAmount != 0 ? i.MinimumPurchaseAmount >= amount : true))
        //        .Join(db.OfferShops.Where(i => i.ShopId == shopid), o => o.Id, oShp => oShp.OfferId, (o, oShp) => new { o, oShp })
        //        .Select(i => new CartOfferApiListViewModel.OfferListItem
        //        {
        //            AmountLimit = i.o.AmountLimit,
        //            Description = i.o.Description,
        //            DiscountType = i.o.DiscountType,
        //            Id = i.o.Id,
        //            MinimumPurchaseAmount = i.o.MinimumPurchaseAmount,
        //            Name = i.o.Name,
        //            OfferCode = i.o.OfferCode,
        //            Percentage = i.o.Percentage
        //        }).ToList();
        //    return Json(new { list = model }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetCheckOffer(int shopId, int customerId, double amount, bool isOnlinePayment, string offerCode)
        {

            var offer = db.Offers.FirstOrDefault(i => i.OfferCode == offerCode && i.Status == 0);
            if (offer != null)
            {
                var orderCount = db.Orders.Where(i => i.CustomerId == customerId && i.Status != 0 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(DateTime.Now))).Count();

                var offercount = db.Offers.Where(i => i.Type == 1 && (offer.MinimumPurchaseAmount != 0 ? offer.MinimumPurchaseAmount <= amount : true) && (offer.IsForOnlinePayment != false ? offer.IsForOnlinePayment == isOnlinePayment : true) && (offer.IsForFirstOrder != false ? orderCount == 0 : true))
                    .Join(db.OfferShops.Where(i => i.ShopId == shopId), o => o.Id, oShp => oShp.OfferId, (o, oShp) => new { o, oShp })
                    .Count();
                if (offercount > 0)
                    return Json(new { status = true, offerPercentage = offer.Percentage, offerAmountLimit = offer.AmountLimit, discountType = offer.DiscountType, id = offer.Id }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAllAchievements(int customerid = 0)
        {
            var model = new AchievementApiListViewModel();
            model.AchievementListItems = db.AchievementSettings.Where(i => i.Status == 0).ToList()
                .GroupJoin(db.AchievementShops, a => a.Id, ashop => ashop.AchievementId, (a, ashop) => new { a, ashop })
                .GroupJoin(db.AchievementProducts, a => a.a.Id, apro => apro.AchievementId, (a, apro) => new { a, apro })
                .GroupJoin(db.CustomerAchievements, a => a.a.a.Id, ca => ca.AchievementId, (a, ca) => new { a, ca })
                .Select(i => new AchievementApiListViewModel.AchievementListItem
                {
                    ActivateAfterId = i.a.a.a.ActivateAfterId,
                    ActivateType = i.a.a.a.ActivateType,
                    Amount = i.a.a.a.Amount,
                    CountType = i.a.a.a.CountType,
                    CountValue = i.a.a.a.CountValue,
                    DayLimit = i.a.a.a.DayLimit,
                    HasAccept = i.a.a.a.HasAccept,
                    Id = i.a.a.a.Id,
                    IsForBlackListAbusers = i.a.a.a.IsForBlackListAbusers,
                    Name = i.a.a.a.Name,
                    RepeatCount = i.a.a.a.RepeatCount,
                    ShopDistrict = i.a.a.a.ShopDistrict,
                    Description = i.a.a.a.Description,
                    ProductListItems = i.a.apro.Select(b => new AchievementApiListViewModel.AchievementListItem.ProductListItem { Id = b.ProductId }).ToList(),
                    ShopListItems = i.a.a.ashop.Select(b => new AchievementApiListViewModel.AchievementListItem.ShopListItem { Id = b.ShopId }).ToList(),
                    IsCustomerAccepted = i.ca.Any() ? i.ca.Any(a => a.Status == 1) : false,
                    ExpiryDate = (i.a.a.a.DayLimit != 0 && i.ca.Any(a => a.Status == 1) == true) ? i.ca.FirstOrDefault().DateEncoded.AddDays(Convert.ToDouble(i.a.a.a.DayLimit)).ToString("dd-MMM-yyyy") : ""
                }).ToList();
            return Json(new { list = model.AchievementListItems }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllAchievement()
        {
            var achievements = db.AchievementSettings.Where(i => i.Status == 0).ToList();
            return Json(new { list = achievements }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderDetails(long id)
        {
            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            var model = new OrderDetailsApiViewModel();
            if (order != null)
            {
                _mapper.Map(order, model);
                var shop = db.Shops.FirstOrDefault(i => i.Id == order.ShopId);
                model.ShopId = shop.Id;
                model.ShopName = shop.Name;
                model.ShopAddress = shop.Address;
                model.ShopLatitude = shop.Latitude;
                model.ShopLongitude = shop.Longitude;
                model.ShopImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + shop.ImagePath;
                model.ShopCategoryId = shop.ShopCategoryId;
                model.ShopCategoryName = shop.ShopCategoryName;
                model.IsShopOnline = shop.IsOnline;
                model.ShopPhoneNumber = shop.PhoneNumber;
                var shopReview = db.CustomerReviews.Where(i => i.ShopId == shop.Id).ToList();
                model.ShopReview = shopReview.Count();
                if (model.ShopReview > 0)
                    model.ShopRating = shopReview.Sum(l => l.Rating) / model.ShopReview;
                else
                    model.ShopRating = 0;

                if (shop.ShopCategoryId == 4)
                {
                    model.MedicalOrderItemLists = db.OrderItems.Where(i => i.OrderId == order.Id)
                   .Join(db.Products, oi => oi.ProductId, p => p.Id, (oi, p) => new { oi, p })
                   .Join(db.MasterProducts, p => p.p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                   .Join(db.DiscountCategories, p => p.p.p.DiscountCategoryId, dc => dc.Id, (p, dc) => new { p, dc })
                   .Select(i => new OrderDetailsApiViewModel.MedicalOrderItemList
                   {
                       CategoryId = i.p.m.CategoryId,
                       CategoryName = i.p.p.oi.CategoryName,
                       ImagePath = i.p.p.oi.ImagePath,
                       ProductId = i.p.p.p.Id,
                       ProductName = i.p.m.Name,
                       Qty = i.p.p.oi.Quantity,
                       Price = i.p.p.p.Price,
                       Status = i.p.p.p.Status,
                       IsProductOnline = i.p.p.p.IsOnline,
                       IBarU = i.p.p.p.IBarU,
                       ImagePathLarge1 = ((!string.IsNullOrEmpty(i.p.m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                       ImagePathLarge2 = ((!string.IsNullOrEmpty(i.p.m.ImagePath2)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.m.ImagePath2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                       ImagePathLarge3 = ((!string.IsNullOrEmpty(i.p.m.ImagePath3)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.m.ImagePath3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                       ImagePathLarge4 = ((!string.IsNullOrEmpty(i.p.m.ImagePath4)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.m.ImagePath4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                       ImagePathLarge5 = ((!string.IsNullOrEmpty(i.p.m.ImagePath5)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.m.ImagePath5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                       Itemid = i.p.p.p.ItemId,
                       MRP = i.p.p.p.MenuPrice,
                       SalePrice = i.p.p.p.Price,
                       Quantity = i.p.p.p.Qty,
                       ShopId = shop.Id,
                       ShopName = shop.Name,
                       DiscountCategoryPercentage = i.dc.Percentage,
                       Size = i.p.m.SizeLWH,
                       Weight = i.p.m.Weight,
                       IsPreorder = i.p.p.p.IsPreorder,
                       PreorderHour = i.p.p.p.PreorderHour,
                       OfferQuantityLimit = i.p.p.p.OfferQuantityLimit
                   }).ToList();
                }
                else
                {
                    model.OrderItemLists = db.OrderItems.Where(i => i.OrderId == order.Id)
                    .Join(db.Products, oi => oi.ProductId, p => p.Id, (oi, p) => new { oi, p })
                    .Join(db.MasterProducts, p => p.p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                    .Select(i => new OrderDetailsApiViewModel.OrderItemList
                    {
                        CategoryId = i.m.CategoryId,
                        CategoryName = i.p.oi.CategoryName,
                        ImagePath = i.p.oi.ImagePath,
                        TotalPrice = i.p.oi.Price,
                        Id = i.p.p.Id,
                        Name = i.m.Name,
                        ShopId = i.p.p.ShopId,
                        ShopName = i.p.p.ShopName,
                        Quantity = i.p.oi.Quantity,
                        Price = i.p.oi.Price,
                        ColorCode = i.m.ColorCode,
                        Customisation = i.p.p.Customisation,
                        Status = i.p.p.Status,
                        IsProductOnline = i.p.p.IsOnline,
                        DiscountCategoryPercentage = i.p.p.Percentage,
                        Size = i.m.SizeLWH,
                        Weight = i.m.Weight,
                        IsPreorder = i.p.p.IsPreorder,
                        PreorderHour = i.p.p.PreorderHour,
                        AddOnType = i.p.oi.AddOnType,
                        HasAddon = i.p.oi.HasAddon,
                        OfferQuantityLimit = i.p.p.OfferQuantityLimit,
                        OrderItemAddonLists = db.OrderItemAddons.Where(a => a.OrderItemId == i.p.oi.Id).Select(a => new OrderDetailsApiViewModel.OrderItemList.OrderItemAddonList {
                            AddonId = a.AddonId,
                            AddonName = a.AddonName,
                            AddonPrice = a.AddonPrice,
                            CrustId = a.CrustId,
                            CrustName = a.CrustName,
                            OrderItemId = a.OrderItemId,
                            PortionId = a.PortionId,
                            PortionName = a.PortionName,
                            PortionPrice = a.PortionPrice
                        }).ToList()
                    }).ToList();
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLatestVersion()
        {
            string version = "0";
            var appDetails = db.AppDetails.FirstOrDefault(i => i.Status == 0);
            if (appDetails != null)
                version = appDetails.Version;
            return Json(new { version = version }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLiveOrderCount(int customerid)
        {
            var liveOrdercount = db.Orders.Where(i => i.CustomerId == customerid && i.Status != 0 && i.Status != 6 && i.Status != 7 && i.Status != 9 && i.Status != 10 && i.Status != -1).Count();
            return Json(new { count = liveOrdercount }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCheckOldCart(OldCartCheckViewModel model)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
            if (shop != null)
            {
                var response = new OldCartResponseViewModel();
                response.ShopId = shop.Id;
                response.ShopIsOnline = shop.IsOnline;
                response.ShopNextOnTime = shop.NextOnTime;
                response.ProductListItems = db.Products.AsEnumerable().Where(i => model.ProductListItems.Select(a => a.Id).ToArray().Contains(i.Id))

                    .Select(i => new OldCartResponseViewModel.ProductListItem
                    {
                        Id = i.Id,
                        IsOnline = i.IsOnline,
                        Price = i.Price,
                        IsActive = i.Status == 0 ? true : false,
                        NextOnTime = i.NextOnTime
                    }).ToList();
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOrderTiming(int orderId, int type) //type 1-Order Ready, 2-DBoy Reach Shop, 3-DBoy Location Reach
        {
            var order = db.Orders.FirstOrDefault(i => i.Id == orderId);
            if (order != null)
            {
                if (type == 1)
                {
                    order.OrderReadyTime = DateTime.Now;
                    order.Status = 8;
                }
                else if (type == 2)
                    order.DeliveryBoyShopReachTime = DateTime.Now;
                else if (type == 3)
                    order.DeliveryLocationReachTime = DateTime.Now;

                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddWaitingCharge(int orderId, string remark, double amount)
        {
            var order = db.Orders.FirstOrDefault(i => i.Id == orderId);
            if (order != null)
            {
                order.WaitingCharge = amount;
                order.WaitingRemark = remark;
                if (order.DeliveryLocationReachTime != null && order.DeliveredTime != null)
                    order.WaitingTime = (order.DeliveryLocationReachTime.Value - order.DeliveredTime.Value).Minutes;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var customer = db.Customers.FirstOrDefault(i => i.Id == order.CustomerId);
                if (customer != null)
                {
                    customer.DeliveryWaitingCharge += amount;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddPenaltyCharge(int orderId, string remark, double amount)
        {
            var order = db.Orders.FirstOrDefault(i => i.Id == orderId);
            if (order != null)
            {
                order.PenaltyAmount = amount;
                order.PenaltyRemark = remark;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var customer = db.Customers.FirstOrDefault(i => i.Id == order.CustomerId);
                if (customer != null)
                {
                    customer.PenaltyAmount += amount;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AcceptAchievement(int customerId, int id)
        {
            var customerAchievement = new CustomerAchievement()
            {
                AchievementId = id,
                CustomerId = customerId,
                DateEncoded = DateTime.Now,
                DateUpdated = DateTime.Now,
                Status = 1
            };
            db.CustomerAchievements.Add(customerAchievement);
            db.SaveChanges();
            if (customerAchievement != null)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductOfferList(long productId)
        {
            var offerList = db.OfferProducts.Where(i => i.ProductId == productId)
                .Join(db.Offers.Where(i => i.Type == 2 && i.Status == 0), op => op.OfferId, o => o.Id, (op, o) => new { op, o })
                .Select(i => new
                {
                    id = i.o.Id,
                    offerCode = i.o.OfferCode,
                    name = i.o.Name,
                    offerPercentage = i.o.Percentage,
                    amountLimit = i.o.AmountLimit,
                    discountType = i.o.DiscountType
                }).ToList();
            return Json(offerList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWalletHistory(int customerId)
        {
            var model = new WalletHistoryViewModel();
            //var debitList = db.Orders.Where(i => i.CustomerId == customerId && i.Status == 2 && i.WalletAmount > 0)
            //    .Select(i => new WalletHistoryViewModel.ListItem
            //    {
            //        Text = "Payment to " + i.ShopName,
            //        Amount = i.WalletAmount,
            //        Type = 2,
            //        Date = i.DeliveredTime ?? DateTime.Now
            //    }).ToList();

            //var creditList = db.CustomerAchievements.Where(i => i.CustomerId == customerId && i.Status == 0)
            //    .Join(db.AchievementSettings, ca => ca.AchievementId, a => a.Id, (ca, a) => new { ca, a })
            //   .Select(i => new WalletHistoryViewModel.ListItem
            //   {
            //       Text = "Received from " + i.a.Name,
            //       Amount = i.a.Amount,
            //       Type = 1,
            //       Date = i.ca.DateEncoded
            //   }).ToList();
            //model.ListItems = debitList.Concat(creditList).OrderByDescending(i => i.Date).ToList();
            var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
            if (customer != null)
            {
                model.WalletAmount = customer.WalletAmount;
                model.ListItems = db.CustomerWalletHistories.Where(i => i.CustomerId == customerId).OrderByDescending(i => i.DateEncoded)
                    .Select(i => new WalletHistoryViewModel.ListItem
                    {
                        Amount = i.Amount,
                        Date = i.DateEncoded,
                        Description = i.Description,
                        Type = i.Type
                    }).ToList();
            }
            return Json(new { amount = model.WalletAmount, list = model.ListItems }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeliveryMode(double totalSize, double totalWeight)
        {
            int mode = 0; //0-NA,1-bike,2-carrier bike,3-Auto
            double liters = totalSize / 1000;

            if (totalWeight <= 20 || liters <= 60)
                mode = 1;
            if ((totalWeight > 20 && totalWeight <= 40) || (liters > 60 && liters <= 120))
                mode = 2;
            if (totalWeight > 40 || liters > 120)
                mode = 3;

            return Json(mode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerPaymentMode(int customerId)//If Customer Cancel the order(not pick up the phone on delivery) next 5 orders should be Online Mode
        {
            var lastCancelledOrder = db.Orders.AsEnumerable().LastOrDefault(i => i.CustomerId == customerId && (i.Status == 9 || i.Status == 10));
            if (lastCancelledOrder != null)
            {
                int orderCountAfterLC = db.Orders.Where(i => i.CustomerId == customerId && i.Status == 6 && (i.DateEncoded > lastCancelledOrder.DateEncoded)).Count();
                if (orderCountAfterLC < 5)
                    return Json(new { isOnlinePayment = false }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { isOnlinePayment = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { isOnlinePayment = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCustomerDistrict(int customerId, string district)
        {
            var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
            if (customer != null)
            {
                customer.DistrictName = district;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerCancelOrder(long orderId)
        {
            var order = db.Orders.FirstOrDefault(i => i.Id == orderId);
            if (order != null)
            {
                order.Status = 9;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();




                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePrescription(SavePrescriptionViewModel model)
        {
            var prescription = _mapper.Map<SavePrescriptionViewModel, CustomerPrescription>(model);
            prescription.DateEncoded = DateTime.Now;
            prescription.Status = 0;
            db.CustomerPrescriptions.Add(prescription);
            db.SaveChanges();

            if (model.ImageListItems != null)
            {
                foreach (var image in model.ImageListItems)
                {
                    var prescriptionImage = new CustomerPrescriptionImage
                    {
                        CustomerPrescriptionId = prescription.Id,
                        ImagePath = image.ImagePath
                    };
                    db.CustomerPrescriptionImages.Add(prescriptionImage);
                    db.SaveChanges();
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPrescriptionList(int customerid)
        {
            var model = new CustomerPrescriptionListViewModel();
            model.ListItems = db.CustomerPrescriptions.Where(i => i.CustomerId == customerid && i.Status !=2)
                .AsEnumerable()
                .Select(i => new CustomerPrescriptionListViewModel.ListItem
                {
                    AudioPath = (!string.IsNullOrEmpty(i.AudioPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Audio/" + i.AudioPath : "",
                    DateEncoded = i.DateEncoded,
                    Id = i.Id,
                    //ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/notavailable.png"),
                    ImagePath = GetFirstImage(i.Id),
                    Remarks = i.Remarks,
                    Status = i.Status
                }).ToList();
            return Json(new { list = model.ListItems }, JsonRequestBehavior.AllowGet);
        }

        public string GetFirstImage(int id)
        {
            var imagePath = "";
            var image = db.CustomerPrescriptionImages.FirstOrDefault(i => i.CustomerPrescriptionId == id);
            if (image != null)
                imagePath = ((!string.IsNullOrEmpty(image.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + image.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/notavailable.png");
            return imagePath;
        }

        [HttpGet]
        public JsonResult GetCompletedPrescriptionDetails(int id)
        {
            var prescription = db.CustomerPrescriptions.FirstOrDefault(i => i.Id == id);
            var model = new PrescriptionDetailsApiViewModel();
            if (prescription != null)
            {
                model.CustomerId = prescription.CustomerId;
                model.CustomerName = prescription.CustomerName;
                model.CustomerPhoneNumber = prescription.CustomerPhoneNumber;
                var shop = db.Shops.FirstOrDefault(i => i.Id == prescription.ShopId);
                model.ShopId = shop.Id;
                model.ShopName = shop.Name;
                model.ShopAddress = shop.Address;
                model.ShopLatitude = shop.Latitude;
                model.ShopLongitude = shop.Longitude;
                model.ShopImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + shop.ImagePath;
                model.ShopCategoryId = shop.ShopCategoryId;
                model.ShopCategoryName = shop.ShopCategoryName;
                model.IsShopOnline = shop.IsOnline;
                model.ShopPhoneNumber = shop.PhoneNumber;
                var shopReview = db.CustomerReviews.Where(i => i.ShopId == shop.Id).ToList();
                model.ShopReview = shopReview.Count();
                if (model.ShopReview > 0)
                    model.ShopRating = shopReview.Sum(l => l.Rating) / model.ShopReview;
                else
                    model.ShopRating = 0;

                model.ItemLists = db.CustomerPrescriptionItems.Where(i => i.CustomerPrescriptionId == prescription.Id)
               .Join(db.Products, cp => cp.ProductId, p => p.Id, (cp, p) => new { cp, p })
               .Join(db.MasterProducts, p => p.p.MasterProductId, m => m.Id, (p, m) => new { p, m })
               .Join(db.Categories, p => p.p.p.CategoryId, c => c.Id, (p, c) => new { p, c })
               .Join(db.DiscountCategories, p => p.p.p.p.DiscountCategoryId, dc => dc.Id, (p, dc) => new { p, dc })
               .Select(i => new PrescriptionDetailsApiViewModel.ItemList
               {
                   CategoryId = i.p.p.m.CategoryId,
                   CategoryName = i.p.c.Name,
                   ImagePath = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                   ProductId = i.p.p.p.p.Id,
                   ProductName = i.p.p.m.Name,
                   Qty = i.p.p.p.cp.Quantity,
                   Price = i.p.p.p.p.Price,
                   Status = i.p.p.p.p.Status,
                   IsProductOnline = i.p.p.p.p.IsOnline,
                   IBarU = i.p.p.p.p.IBarU,
                   ImagePathLarge1 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                   ImagePathLarge2 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath2)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                   ImagePathLarge3 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath3)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                   ImagePathLarge4 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath4)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                   ImagePathLarge5 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath5)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                   Itemid = i.p.p.p.p.ItemId,
                   MRP = i.p.p.p.p.MenuPrice,
                   SalePrice = i.p.p.p.p.Price,
                   Quantity = i.p.p.p.p.Qty,
                   ShopId = shop.Id,
                   ShopName = shop.Name,
                   DiscountCategoryPercentage = i.dc.Percentage,
                   Size = i.p.p.m.SizeLWH,
                   Weight = i.p.p.m.Weight
               }).ToList();

            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCheckCustomerProductOffer(int customerid, int productid)
        {
            int customerPurchasedQuantity = 0;
            var banner = db.Banners.FirstOrDefault(i => i.ProductId == productid && i.Status == 0);
            if (banner != null)
            {
                var product = db.Products.FirstOrDefault(i => i.Id == banner.ProductId);
                //bool isAvailable = db.Orders.Where(i => i.CustomerId == customerid && i.Status != 0 && i.Status != 7 && i.Status != 9 && i.Status != 10 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(banner.FromDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(banner.Todate)))
                //    .Join(db.OrderItems.Where(i => i.ProductId == productid), o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi }).Any();

                var customerOders = db.Orders.Where(i => i.CustomerId == customerid && i.Status != 0 && i.Status != 7 && i.Status != 9 && i.Status != 10 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(banner.FromDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(banner.Todate)))
                    .Join(db.OrderItems.Where(i => i.ProductId == productid), o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi }).ToList();
                if (customerOders == null)
                    customerPurchasedQuantity = 0;
                else
                    customerPurchasedQuantity = customerOders.Sum(i => i.oi.Quantity);
                if (customerPurchasedQuantity >= product.OfferQuantityLimit)
                    return Json(new { isAvailable = false,alreadyPurchasedQuantity= customerPurchasedQuantity }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { isAvailable = true, alreadyPurchasedQuantity = customerPurchasedQuantity }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { isAvailable = true, alreadyPurchasedQuantity = customerPurchasedQuantity }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProductDetails(int productid, double latitude, double longitude)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == productid);
            var model = new ProductDetailsApiViewModel();
            if (product != null)
            {
                var shop = db.Shops.FirstOrDefault(s => s.Id == product.ShopId);
                string defaultImagePath = "../../assets/images/noimageres.svg";
                //if (shop.ShopCategoryId == 4)
                //{
                //    defaultImagePath = "../../assets/images/1.5-cm-X-1.5-cm.png";
                //    nearbydistance = 16;
                //    model.DiscountCategoryPercentage = db.DiscountCategories.FirstOrDefault(i => i.Name == product.DiscountCategoryName && i.ShopId == shop.Id)?.Percentage;
                //}
                //else
                //{
                    model.DiscountCategoryPercentage = product.Percentage;
                //}

                _mapper.Map(product, model);
                model.ShopCategoryId = shop.ShopCategoryId;
                model.ShopCategoryName = shop.ShopCategoryName;
                model.ShopIsOnline = shop.IsOnline;
                model.ShopNextOnTime = shop.NextOnTime;
                var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == product.MasterProductId);
                if (masterProduct != null)
                {
                    model.ImagePath = ((!string.IsNullOrEmpty(masterProduct.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : defaultImagePath);
                    model.ImagePath1 = ((!string.IsNullOrEmpty(masterProduct.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : defaultImagePath);
                    model.ImagePath2 = ((!string.IsNullOrEmpty(masterProduct.ImagePath2)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : null);
                    model.ImagePath3 = ((!string.IsNullOrEmpty(masterProduct.ImagePath3)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : null);
                    model.ImagePath4 = ((!string.IsNullOrEmpty(masterProduct.ImagePath4)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : null);
                    model.ImagePath5 = ((!string.IsNullOrEmpty(masterProduct.ImagePath5)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : null);
                    model.CategoryName = shop.ShopCategoryId == 2 ? db.NextSubCategories.FirstOrDefault(i => i.Id == masterProduct.NextSubCategoryId)?.Name : db.Categories.FirstOrDefault(i => i.Id == product.CategoryId)?.Name;
                    model.ColorCode = masterProduct.ColorCode;
                    model.LongDescription = masterProduct.LongDescription;
                    model.ShortDescription = masterProduct.ShortDescription;
                    model.BrandName = masterProduct.BrandName;
                    model.Size = masterProduct.SizeLWH;
                    model.Weight = masterProduct.Weight;
                }


                string query = "SELECT * " +
                                   " FROM Shops where(3959 * acos(cos(radians(@latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@longitude)) + sin(radians(@latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryId =" + shop.ShopCategoryId + " and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
                                   " order by IsOnline desc,Adscore desc,Rating desc";

                //if (shop.ShopCategoryId == 4)
                //{
                //    model.SimilarProductsListItems = db.Shops.SqlQuery(query, new SqlParameter("latitude", latitude), new SqlParameter("longitude", longitude))
                //       .Join(db.Products.Where(i => i.MasterProductId == product.MasterProductId), s => s.Id, p => p.ShopId, (s, p) => new { s, p })
                //           .Join(db.MasterProducts, p => p.p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                //           .Join(db.DiscountCategories, p => p.p.p.DiscountCategoryName, dc => dc.Name, (p, dc) => new { p, dc })
                //           .AsEnumerable()
                //           .Select(i => new ProductDetailsApiViewModel.SimilarProductsListItem
                //           {
                //               DiscountPercentage = i.dc.Percentage,
                //               MenuPrice = i.p.p.p.MenuPrice,
                //               Name = i.p.m.Name,
                //               Price = i.p.p.p.Price,
                //               ShopName = i.p.p.p.ShopName,
                //               Distance = Math.Round((((Math.Acos(Math.Sin((i.p.p.s.Latitude * Math.PI / 180)) * Math.Sin((latitude * Math.PI / 180)) + Math.Cos((i.p.p.s.Latitude * Math.PI / 180)) * Math.Cos((latitude * Math.PI / 180))
                //            * Math.Cos(((i.p.p.s.Longitude - longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000, 2)
                //           }).ToList();
                //}
                //else
                //{
                    model.SimilarProductsListItems = db.Shops.SqlQuery(query, new SqlParameter("latitude", latitude), new SqlParameter("longitude", longitude))
                       .Join(db.Products.Where(i => i.MasterProductId == product.MasterProductId && i.Status ==0), s => s.Id, p => p.ShopId, (s, p) => new { s, p })
                           .Join(db.MasterProducts.Where(i=>i.Id==product.MasterProductId), p => p.p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                           .AsEnumerable()
                           .Select(i => new ProductDetailsApiViewModel.SimilarProductsListItem
                           {
                               DiscountPercentage = i.p.p.Percentage,
                               MenuPrice = i.p.p.MenuPrice,
                               Name = i.m.Name,
                               Price = i.p.p.Price,
                               ShopName = i.p.p.ShopName,
                               //   Distance = Math.Round((((Math.Acos(Math.Sin((i.p.s.Latitude * Math.PI / 180)) * Math.Sin((latitude * Math.PI / 180)) + Math.Cos((i.p.s.Latitude * Math.PI / 180)) * Math.Cos((latitude * Math.PI / 180))
                               //* Math.Cos(((i.p.s.Longitude - longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000, 2)
                               Distance = Math.Round((double)(GetMeters(latitude, longitude, i.p.s.Latitude, i.p.s.Longitude) / 1000), 2),
                               ProductId = i.p.p.Id,
                               ShopPrice = i.p.p.ShopPrice,
                               ShopAddress = i.p.s.Address,
                               ShopCategoryId = i.p.s.ShopCategoryId,
                               ShopId = i.p.s.Id,
                               ShopImagePath = ((!string.IsNullOrEmpty(i.p.s.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.s.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                               ShopIsOnline = i.p.s.IsOnline,
                               ShopLatitude = i.p.s.Latitude,
                               ShopLongitude = i.p.s.Longitude,
                               ShopPhoneNumber = i.p.s.PhoneNumber,
                               ShopStatus = i.p.s.Status,
                               ShopNextOnTime = i.p.s.NextOnTime
                           }).ToList();
                //}
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetMedicalProductDetails(int productid, double latitude, double longitude)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == productid);
            var model = new MedicalProductDetailsApiViewModel();
            if (product != null)
            {
                var shop = db.Shops.FirstOrDefault(s => s.Id == product.ShopId);
                string defaultImagePath = "../../assets/images/1.5-cm-X-1.5-cm.png";
                    model.DiscountCategoryPercentage = db.DiscountCategories.FirstOrDefault(i => i.Name == product.DiscountCategoryName && i.ShopId == shop.Id)?.Percentage;
                _mapper.Map(product, model);
                model.ProductId = product.Id;
                model.ShopCategoryId = shop.ShopCategoryId;
                model.ShopCategoryName = shop.ShopCategoryName;
                model.ShopIsOnline = shop.IsOnline;
                model.ShopNextOnTime = shop.NextOnTime;
                model.Quantity = product.Qty;
                model.MRP = product.MenuPrice;
                model.SalePrice = product.Price;
                model.Itemid = product.ItemId;
                var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == product.MasterProductId);
                if (masterProduct != null)
                {
                    model.ImagePath = ((!string.IsNullOrEmpty(masterProduct.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : defaultImagePath);
                    model.ImagePath1 = ((!string.IsNullOrEmpty(masterProduct.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : defaultImagePath);
                    model.ImagePath2 = ((!string.IsNullOrEmpty(masterProduct.ImagePath2)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : null);
                    model.ImagePath3 = ((!string.IsNullOrEmpty(masterProduct.ImagePath3)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : null);
                    model.ImagePath4 = ((!string.IsNullOrEmpty(masterProduct.ImagePath4)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : null);
                    model.ImagePath5 = ((!string.IsNullOrEmpty(masterProduct.ImagePath5)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + masterProduct.ImagePath5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : null);
                    model.CategoryName = shop.ShopCategoryId == 2 ? db.NextSubCategories.FirstOrDefault(i => i.Id == masterProduct.NextSubCategoryId)?.Name : db.Categories.FirstOrDefault(i => i.Id == product.CategoryId)?.Name;
                    model.ColorCode = masterProduct.ColorCode;
                    model.DrugCompoundDetailIds = masterProduct.DrugCompoundDetailIds;
                    model.DrugCompoundDetailName = masterProduct.DrugCompoundDetailName;
                    model.LongDescription = masterProduct.LongDescription;
                    model.ShortDescription = masterProduct.ShortDescription;
                    model.BrandName = masterProduct.BrandName;
                    model.Size = masterProduct.SizeLWH;
                    model.Weight = masterProduct.Weight;
                    model.PriscriptionCategory = masterProduct.PriscriptionCategory;
                    model.ProductName = masterProduct.Name;
                }


                string query = "SELECT * " +
                                   " FROM Shops where(3959 * acos(cos(radians(@latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@longitude)) + sin(radians(@latitude)) * sin(radians(Latitude)))) < 16 and ShopCategoryId =" + shop.ShopCategoryId + " and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
                                   " order by IsOnline desc,Adscore desc,Rating desc";
                if (shop.Id == 347)
                {
                    model.SimilarProductsListItems = db.Shops.SqlQuery(query, new SqlParameter("latitude", latitude), new SqlParameter("longitude", longitude))
                                      .Join(db.Products.Where(i => i.MasterProductId == product.MasterProductId && i.Status == 0), s => s.Id, p => p.ShopId, (s, p) => new { s, p })
                                          .Join(db.MasterProducts.Where(i => i.Id == product.MasterProductId), p => p.p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                                          .AsEnumerable()
                                          .Select(i => new MedicalProductDetailsApiViewModel.SimilarProductsListItem
                                          {
                                              DiscountPercentage = i.p.p.Percentage,
                                              MenuPrice = i.p.p.MenuPrice,
                                              Name = i.m.Name,
                                              Price = i.p.p.Price,
                                              ShopName = i.p.p.ShopName,
                           Distance = Math.Round((double)(GetMeters(latitude, longitude, i.p.s.Latitude, i.p.s.Longitude) / 1000), 2),
                                              ProductId = i.p.p.Id,
                                              ShopPrice = i.p.p.ShopPrice,
                                              ShopAddress = i.p.s.Address,
                                              ShopCategoryId = i.p.s.ShopCategoryId,
                                              ShopId = i.p.s.Id,
                                              ShopImagePath = ((!string.IsNullOrEmpty(i.p.s.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.s.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                                              ShopIsOnline = i.p.s.IsOnline,
                                              ShopLatitude = i.p.s.Latitude,
                                              ShopLongitude = i.p.s.Longitude,
                                              ShopPhoneNumber = i.p.s.PhoneNumber,
                                              ShopStatus = i.p.s.Status,
                                              ShopNextOnTime = i.p.s.NextOnTime
                                          }).ToList();
                }
                else
                {
                    model.SimilarProductsListItems = db.Shops.SqlQuery(query, new SqlParameter("latitude", latitude), new SqlParameter("longitude", longitude))
                  .Join(db.Products.Where(i => i.MasterProductId == product.MasterProductId), s => s.Id, p => p.ShopId, (s, p) => new { s, p })
                      .Join(db.MasterProducts, p => p.p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                      .Join(db.DiscountCategories, p => p.p.p.DiscountCategoryId, dc => dc.Id, (p, dc) => new { p, dc })
                      .AsEnumerable()
                      .Select(i => new MedicalProductDetailsApiViewModel.SimilarProductsListItem
                      {
                          DiscountPercentage = i.dc.Percentage,
                          MenuPrice = i.p.p.p.MenuPrice,
                          Name = i.p.m.Name,
                          Price = i.p.p.p.Price,
                          ShopName = i.p.p.p.ShopName,
                           //   Distance = Math.Round((((Math.Acos(Math.Sin((i.p.p.s.Latitude * Math.PI / 180)) * Math.Sin((latitude * Math.PI / 180)) + Math.Cos((i.p.p.s.Latitude * Math.PI / 180)) * Math.Cos((latitude * Math.PI / 180))
                           //* Math.Cos(((i.p.p.s.Longitude - longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000, 2)
                           Distance = Math.Round((double)(GetMeters(latitude, longitude, i.p.p.s.Latitude, i.p.p.s.Longitude) / 1000), 2),
                          ProductId = i.p.p.p.Id,
                          ShopPrice = i.p.p.p.ShopPrice,
                          ShopAddress = i.p.p.s.Address,
                          ShopCategoryId = i.p.p.s.ShopCategoryId,
                          ShopId = i.p.p.s.Id,
                          ShopImagePath = ((!string.IsNullOrEmpty(i.p.p.s.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.s.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/noimageres.svg"),
                          ShopIsOnline = i.p.p.s.IsOnline,
                          ShopLatitude = i.p.p.s.Latitude,
                          ShopLongitude = i.p.p.s.Longitude,
                          ShopPhoneNumber = i.p.p.s.PhoneNumber,
                          ShopStatus = i.p.p.s.Status,
                          ShopNextOnTime = i.p.p.s.NextOnTime
                      }).ToList();
                }
               
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCheckDeliveryAddressWithShop(int shopid, double latitude, double longitude)
        {
            bool isValid = false;
            var shop = db.Shops.FirstOrDefault(i => i.Id == shopid);
            if (shop != null)
            {
                int nearbydistance = 8;
                if (shop.ShopCategoryId == 4)
                    nearbydistance = 16;

                string query = "SELECT * " +
                                   " FROM Shops where(3959 * acos(cos(radians(@latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@longitude)) + sin(radians(@latitude)) * sin(radians(Latitude)))) < " + nearbydistance + " and ShopCategoryId =" + shop.ShopCategoryId + " and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
                                   " order by IsOnline desc,Adscore desc,Rating desc";
                isValid = db.Shops.SqlQuery(query, new SqlParameter("latitude", latitude), new SqlParameter("longitude", longitude)).Select(i => i.Id).Contains(shopid);
                
            }
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerLikedList(int customerid)
        {
            var model = new CustomerFavoriteListApiViewModel();
            model.ListItems = db.CustomerFavorites.Where(i => i.CustomerId == customerid)
                .Join(db.Products, cf => cf.ProductId, p => p.Id, (cf, p) => new { cf, p })
                .Join(db.MasterProducts, p => p.p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                .Join(db.Categories, p => p.p.p.CategoryId, c => c.Id, (p, c) => new { p, c })
                .Select(i => new CustomerFavoriteListApiViewModel.ListItem
                {
                    CategoryId = i.c.Id,
                    CategoryName = i.c.Name,
                    ImagePath = ((!string.IsNullOrEmpty(i.p.m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + i.p.m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : ""),
                    Itemid = i.p.p.p.ItemId,
                    MRP = i.p.p.p.MenuPrice,
                    Percentage = i.p.p.p.Percentage,
                    Price = i.p.p.p.Price,
                    ProductId = i.p.p.p.Id,
                    ProductName = i.p.p.p.Name,
                    Quantity = i.p.p.p.Qty,
                    ShopId = i.p.p.p.ShopId,
                    ShopName = i.p.p.p.ShopName
                }).ToList();
            return Json(new { list = model.ListItems }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddUpdateCustomerFavorite(CustomerFavoriteAddUpdateViewModel model)
        {
            try
            {
                var customerFavorite = db.CustomerFavorites.FirstOrDefault(i => i.CustomerId == model.CustomerId && i.ProductId == model.ProductId);
                if (customerFavorite != null)
                {
                    customerFavorite.IsFavorite = model.IsFavorite;
                    customerFavorite.DateUpdated = DateTime.Now;
                    db.Entry(customerFavorite).State = EntityState.Modified;
                    db.SaveChanges();

                    int count = db.CustomerFavorites.Where(i => i.ProductId == model.ProductId && i.IsFavorite == true).Count();
                    if (model.IsFavorite == true && count == 1)
                        return Json(new { status = true, isLiked = model.IsFavorite, likeText = $"You Liked", productId = model.ProductId }, JsonRequestBehavior.AllowGet);
                    else if (model.IsFavorite == true && count > 1)
                        return Json(new { status = true, isLiked = model.IsFavorite, likeText = $"You & {count - 1} more", productId = model.ProductId }, JsonRequestBehavior.AllowGet);
                    else if (model.IsFavorite == false && count == 0)
                        return Json(new { status = true, isLiked = model.IsFavorite, likeText = $"", productId = model.ProductId }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = true, isLiked = model.IsFavorite, likeText = $"{count} like", productId = model.ProductId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var custFav = new CustomerFavorite
                    {
                        CustomerId = model.CustomerId,
                        ProductId = model.ProductId,
                        IsFavorite = model.IsFavorite,
                        DateEncoded = DateTime.Now,
                        DateUpdated = DateTime.Now
                    };
                    db.CustomerFavorites.Add(custFav);
                    db.SaveChanges();

                    int count = db.CustomerFavorites.Where(i => i.ProductId == model.ProductId && i.IsFavorite == true).Count();
                    if (model.IsFavorite == true && count == 1)
                        return Json(new { status = true, isLiked = model.IsFavorite, likeText = $"You Liked", productId = model.ProductId }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = true, isLiked = model.IsFavorite, likeText = $"You & {count - 1} more", productId = model.ProductId }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTopCategoryAndProducts(int shopid)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == shopid);
            var model = new TopCategoriesAndProductsViewModel();
            model.CategoryListItems = db.Database.SqlQuery<TopCategoriesAndProductsViewModel.CategoryListItem>($"select distinct top 8 CategoryId as Id, c.Name as Name,c.ImagePath,c.OrderNo from Products p join Categories c on c.Id = p.CategoryId where ShopId ={shop.Id} and OrderNo !=0 and c.Status = 0 and CategoryId !=0 and c.Name is not null group by CategoryId,c.Name,c.ImagePath,c.OrderNo order by OrderNo")
                    .Select(i => new TopCategoriesAndProductsViewModel.CategoryListItem
                    {
                        Id = i.Id,
                        ImagePath = ((!string.IsNullOrEmpty(i.ImagePath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                        Name = i.Name
                    }).ToList<TopCategoriesAndProductsViewModel.CategoryListItem>();

            var discountCategory = db.DiscountCategories.Where(i => i.ShopId == shopid).OrderByDescending(i => i.Percentage).Select(i=>i.Name).Take(6).ToList();

            model.ProductListItems = db.Products.Where(i => discountCategory.Contains(i.DiscountCategoryName) && i.ShopId == shopid && i.MasterProductId != 0 && i.Status == 0 && i.MenuPrice != 0 && i.Price != 0 && i.CategoryId != 0).Take(6)
                .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                .Join(db.Categories, p => p.p.CategoryId, c => c.Id, (p, c) => new { p, c })
                .Join(db.DiscountCategories, p => p.p.p.DiscountCategoryName, dc => dc.Name, (p, dc) => new { p, dc })
                .Select(i => new TopCategoriesAndProductsViewModel.ProductListItem
                {
                    BrandName = i.p.p.m.BrandName,
                    CategoryId = i.p.p.p.CategoryId,
                    CategoryName = i.p.c.Name,
                    ColorCode = i.p.p.m.ColorCode,
                    Customisation = i.p.p.m.Customisation,
                    DiscountCategoryPercentage = i.dc.Percentage,
                    DrugCompoundDetailIds = i.p.p.m.DrugCompoundDetailIds,
                    DrugCompoundDetailName = i.p.p.m.DrugCompoundDetailName,
                    iBarU = i.p.p.m.IBarU,
                    ImagePath = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                    ImagePath1 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath1)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                    ImagePath2 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath2)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                    ImagePath3 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath3)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                    ImagePath4 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath4)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                    ImagePath5 = ((!string.IsNullOrEmpty(i.p.p.m.ImagePath5)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.p.p.m.ImagePath5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "../../assets/images/1.5-cm-X-1.5-cm.png"),
                    IsOnline = i.p.p.p.IsOnline,
                    IsPreorder = i.p.p.p.IsPreorder,
                    Itemid = i.p.p.p.ItemId,
                    LongDescription = i.p.p.m.LongDescription,
                    MRP = i.p.p.p.MenuPrice,
                    NextOnTime = i.p.p.p.NextOnTime,
                    OfferQuantityLimit = i.p.p.p.OfferQuantityLimit,
                    PreorderHour= i.p.p.p.PreorderHour,
                    PriscriptionCategory= i.p.p.m.PriscriptionCategory,
                    ProductId= i.p.p.p.Id,
                    ProductName= i.p.p.m.Name,
                    Quantity = i.p.p.p.Qty,
                    SalePrice = i.p.p.p.Price,
                    ShopCategoryId = shop.ShopCategoryId,
                    ShopCategoryName = shop.ShopCategoryName,
                    ShopId = shop.Id,
                    ShopIsOnline = shop.IsOnline,
                    ShopName = shop.Name,
                    ShopNextOnTime = shop.NextOnTime,
                    ShortDescription = i.p.p.m.ShortDescription,
                    Size = i.p.p.m.SizeLWH,
                    Status = i.p.p.p.Status,
                    Weight = i.p.p.m.Weight
                }).OrderByDescending(i=>i.DiscountCategoryPercentage).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRouteAudioPath(int addressId,string deliveryBoyName, string audiopath)
        {
            var customerAddress = db.CustomerAddresses.FirstOrDefault(i => i.Id == addressId);
            customerAddress.RouteAudioPath = audiopath;
            customerAddress.RouteAudioUploadedBy = deliveryBoyName;
            customerAddress.RouteAudioUploadedDateTime = DateTime.Now;
            db.Entry(customerAddress).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveReviewReply(int reviewId,string reply,string repliedBy)
        {
            var customerReviewReply = new CustomerReviewReply
            {
                CreatedBy = repliedBy,
                CustomerReviewId = reviewId,
                DateEncoded = DateTime.Now,
                ReplyText = reply,
                Status = 0
            };
            db.CustomerReviewReplies.Add(customerReviewReply);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerSearchHistory(int customerid)
        {
            var list = db.CustomerSearchHistories.Where(i => i.Status == 0 && i.CustomerId == customerid).OrderByDescending(i => i.DateEncoded).Take(10).ToList();
            return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCustomerSearchHistory(int customerid,int searchid,string serachtext,int type)
        {
            var customerSH = new CustomerSearchHistory
            {
                CustomerId = customerid,
                DateEncoded = DateTime.Now,
                SearchId = searchid,
                SearchText = serachtext,
                Status = 0,
                Type = type // 1- Product, 2-Shop
            };
            db.CustomerSearchHistories.Add(customerSH);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveCustomerSearchHistory(int id)
        {
            var search = db.CustomerSearchHistories.FirstOrDefault(i => i.Id == id);
            search.Status = 2;
            db.Entry(search).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopProductSearchResult(int shopid, string keyword)
        {
            var list = db.GetShopProductSearch(shopid, keyword).Select(i=>i.ProductName).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteSearchResult(int customerid,double latitude, double longitude, string keyword)
        {
           // var list = db.GetAutoCompleteSearch(longitude, latitude, keyword, customerid);
            AutoCompleteSearchResult categProd = new AutoCompleteSearchResult();
            categProd.PreferedText = new List<PreferedText_Result>();
            categProd.Shop = new List<Shop_Result>();
            categProd.Products = new List<Product_Result>();

            //using (var dbContext = new sncEntities())
            //{
            //    var results = dbContext.GetAutoCompleteSearch(longitude, latitude, keyword, customerid);

            //    //Get first enumerate result set
            //    categProd.PreferedText.AddRange(results);

            //    //Get second result set
            //    var products = results.GetNextResult<Product_Result>();
            //    categProd.Products.AddRange(products);

            //    //Return all result sets

            //}

            var productTakeCount = 3;
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"GetAutoCompleteSearch", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("customerid", customerid);
                    command.Parameters.AddWithValue("str", keyword.Trim());
                    command.Parameters.AddWithValue("Longitude", longitude);
                    command.Parameters.AddWithValue("Latitude", latitude);
                    DataSet dsdocCount = new DataSet();
                    SqlDataAdapter daDcocount = new SqlDataAdapter();
                    daDcocount.SelectCommand = command;
                    daDcocount.Fill(dsdocCount);
                    if (dsdocCount.Tables.Count > 1)
                    {

                        if (dsdocCount.Tables[0].Rows.Count == 0 && dsdocCount.Tables[1].Rows.Count == 0)
                            productTakeCount = 8;
                        else if(dsdocCount.Tables[0].Rows.Count == 0 || dsdocCount.Tables[1].Rows.Count == 0)
                            productTakeCount = 6;

                        categProd.PreferedText = (from DataRow row in dsdocCount.Tables[0].Rows

                                                  select new PreferedText_Result()
                                                  {
                                                      correctword = row["source"].ToString()
                                                  }).ToList();
                        categProd.Shop= (from DataRow row in dsdocCount.Tables[1].Rows

                                         select new Shop_Result()
                                         {
                                             ID = Convert.ToInt32(row["Id"].ToString()),
                                             Name = row["Name"].ToString(),
                                             ImagePath = row["ImagePath"].ToString(),
                                             ShopCategoryId = Convert.ToInt32(row["ShopCategoryId"].ToString()),
                                             OnlineStatus = Convert.ToBoolean(row["OnlineStatus"].ToString()),
                                             Rating = RatingCalculation(Convert.ToInt32(row["ShopId"].ToString())),
                                             ReviewCount = db.CustomerReviews.ToList().Where(c => c.ShopId == Convert.ToInt32(row["ShopId"].ToString())).Count(),
                                             ShopAddress = row["ShopAddress"].ToString(),
                                             ShopLatitude = Convert.ToDouble(row["ShopLatitude"].ToString()),
                                             ShopLongitude = Convert.ToDouble(row["ShopLongitude"].ToString()),
                                             Status = Convert.ToInt32(row["Status"].ToString())
                                         }).ToList();
                        categProd.Products = (from DataRow row in dsdocCount.Tables[2].Rows
                                             group row by row["ProductName"].ToString() into g
                                              select new Product_Result()
                                          {
                                              ID = Convert.ToInt32(g.FirstOrDefault()["Id"].ToString()),
                                              Name = g.FirstOrDefault()["ProductName"].ToString(),
                                              ImagePath = g.FirstOrDefault()["ImagePath"].ToString(),
                                              ShopCategoryId = Convert.ToInt32(g.FirstOrDefault()["ShopCategoryId"].ToString()),
                                              ShopId = Convert.ToInt32(g.FirstOrDefault()["ShopId"].ToString()),
                                              OnlineStatus= Convert.ToBoolean(g.FirstOrDefault()["OnlineStatus"].ToString()),
                                              Rating = RatingCalculation(Convert.ToInt32(g.FirstOrDefault()["ShopId"].ToString())),
                                              ReviewCount = db.CustomerReviews.ToList().Where(c => c.ShopId == Convert.ToInt32(g.FirstOrDefault()["ShopId"].ToString())).Count(),
                                              ShopAddress = g.FirstOrDefault()["ShopAddress"].ToString(),
                                              ShopImagePath = g.FirstOrDefault()["ShopImagePath"].ToString(),
                                              ShopLatitude = Convert.ToDouble(g.FirstOrDefault()["ShopLatitude"].ToString()),
                                              ShopLongitude = Convert.ToDouble(g.FirstOrDefault()["ShopLongitude"].ToString()),
                                              ShopName= g.FirstOrDefault()["ShopName"].ToString(),
                                              Status = Convert.ToInt32(g.FirstOrDefault()["Status"].ToString())
                                          }).Take(productTakeCount).ToList();
                    }
                    connection.Close();
                }
            }
               // return Json(new { result = categProd }, JsonRequestBehavior.AllowGet);

            var jResult = Json(new { result = categProd }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;
            return jResult;
        }
        public class AutoCompleteSearchResult
        {
            public List<PreferedText_Result> PreferedText { get; set; }
            public List<Shop_Result> Shop { get; set; }
            public List<Product_Result> Products { get; set; }
        }
        public partial class PreferedText_Result
        {
            public string correctword { get; set; }

        }
        public partial class Shop_Result
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public int ShopCategoryId { get; set; }
            public bool OnlineStatus { get; set; }
            public string ShopAddress { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public int Status { get; set; }
            public double Rating { get; set; }
            public double ReviewCount { get; set; }
        }
        public partial class Product_Result
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public int ShopCategoryId { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string ShopImagePath { get; set; }
            public string ShopAddress { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public bool OnlineStatus { get; set; }
            public int Status { get; set; }
            public double Rating { get; set; }
            public double ReviewCount { get; set; }
            // public int count { get; set; }
        }
        //Calls
        public JsonResult SetCallActive(int orderno)
        {
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderno);
            if (order != null)
            {
                order.IsCallActive = true;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCallerDetails(string from)
        {
            string toNumber = String.Empty;
            int orderNo = 0;

            var deliveryBoyCallOrder = db.Orders.FirstOrDefault(i => i.DeliveryBoyPhoneNumber == from && (i.Status == 4 || i.Status == 5) && i.IsCallActive == true);
            if (deliveryBoyCallOrder != null)
            {
                orderNo = deliveryBoyCallOrder.OrderNumber;
                toNumber = deliveryBoyCallOrder.CustomerPhoneNumber;
            }

            var customerCallOrder = db.Orders.FirstOrDefault(i => i.CustomerPhoneNumber == from && (i.Status == 4 || i.Status == 5) && i.IsCallActive == true);
            if (customerCallOrder != null)
            {
                orderNo = customerCallOrder.OrderNumber;
                toNumber = customerCallOrder.DeliveryBoyPhoneNumber;
            }

            return Json(new { to = toNumber, orderNo = orderNo, callType = 1 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetShopCallerDetails(string from)
        {
            string toNumber = String.Empty;
            int orderNo = 0;
            var shopCallOrder = db.Orders.FirstOrDefault(i => i.ShopPhoneNumber == from && (i.Status==2 || i.Status == 3 || i.Status==4) && i.IsCallActive == true);
            if (shopCallOrder != null)
            {
                orderNo = shopCallOrder.OrderNumber;
                toNumber = shopCallOrder.CustomerPhoneNumber;
            }
            var customerCallOrder = db.Orders.FirstOrDefault(i => i.CustomerPhoneNumber == from && (i.Status == 2 || i.Status == 3 || i.Status == 4 || i.Status == 8) && i.IsCallActive == true);
            if (customerCallOrder != null)
            {
                orderNo = customerCallOrder.OrderNumber;
                toNumber = customerCallOrder.ShopPhoneNumber;
            }

            return Json(new { to = toNumber, orderNo = orderNo, callType = 2 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PostCallerDetails(SaveCallRecordViewModel model)
        {
            CallRecord callRecord = new CallRecord
            {
                CallDate = model.calldate,
                CallDuration = model.callduration,
                Caller = model.from,
                CallId = model.callid,
                CallType = model.calltype,
                OrderNumber = model.OrderNo,
                Receiver = model.to,
                RecordId = model.recordId,
                StatusText = model.Status,
                Status = model.Status == "ANSWER" ? 1 : model.Status == "CANCEL" ? 2 : 3
            };
            db.CallRecords.Add(callRecord);
            db.SaveChanges();

            //Update call active --have to check for status
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == model.OrderNo);
            if (order != null)
            {
                order.IsCallActive = false;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(model.recordId);
        }

        public ActionResult AddGiftCard(int customerid, string giftcardcode)
        {
            var customer = db.Customers.FirstOrDefault(i => i.Id == customerid);
            if (customer != null)
            {

                var giftCard = db.CustomerGiftCards.FirstOrDefault(i => i.CustomerPhoneNumber == customer.PhoneNumber && i.GiftCardCode == giftcardcode.Trim());
                if (giftCard == null)
                    return Json(new { status = false, message = "Invalid Gift Card" }, JsonRequestBehavior.AllowGet);

                //var giftCard = db.CustomerGiftCards.FirstOrDefault(i => i.CustomerId == customerid && i.GiftCardCode == giftcardcode.Trim() && i.Status == 0 && DbFunctions.TruncateTime(i.ExpiryDate) >= DbFunctions.TruncateTime(DateTime.Now));
                else if (giftCard.Status == 0 && (giftCard.ExpiryDate.Date >= DateTime.Now.Date))
                {
                    giftCard.Status = 1;
                    db.Entry(giftCard).State = EntityState.Modified;
                    db.SaveChanges();


                    customer.WalletAmount += giftCard.Amount;
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();

                    //Wallet History for Gift Card
                    var walletHistory = new CustomerWalletHistory
                    {
                        Amount = giftCard.Amount,
                        CustomerId = customer.Id,
                        DateEncoded = DateTime.Now,
                        Description = "Received from gift card",
                        Type = 1
                    };
                    db.CustomerWalletHistories.Add(walletHistory);
                    db.SaveChanges();

                    return Json(new { status = true, message = $"Successfully ₹{giftCard.Amount} Added to Wallet" }, JsonRequestBehavior.AllowGet);
                }
                else if (giftCard.Status == 1)
                    return Json(new { status = false, message = "Already Applied!" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = "Gift Card is Expired!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Not a Customer!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveUpdateCustomerDeviceAppInfo(SaveCustomerDeviceAppInfoViewModel model)
        {
            var customerAppInfo = db.CustomerAppInfoes.FirstOrDefault(i => i.CustomerId == model.CustomerId);
            if (customerAppInfo == null)
            {
                var appInfo = new CustomerAppInfo
                {
                    AppBuild = model.AppBuild,
                    AppId = model.AppId,
                    AppName = model.AppName,
                    CustomerId = model.CustomerId,
                    CustomerPhoneNumber = model.CustomerPhoneNumber,
                    DateEncoded = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Version = model.Version
                };
                db.CustomerAppInfoes.Add(appInfo);
                db.SaveChanges();
            }
            else
            {
                customerAppInfo.DateUpdated = DateTime.Now;
                customerAppInfo.AppBuild = model.AppBuild;
                customerAppInfo.AppId = model.AppId;
                customerAppInfo.AppName = model.AppName;
                customerAppInfo.Version = model.Version;
                db.Entry(customerAppInfo).State = EntityState.Modified;
                db.SaveChanges();
            }

            var customerDeviceInfo = db.CustomerDeviceInfoes.FirstOrDefault(i => i.CustomerId == model.CustomerId);
            if (customerDeviceInfo == null)
            {
                var deviceInfo = new CustomerDeviceInfo
                {
                    CustomerId = model.CustomerId,
                    CustomerPhoneNumber = model.CustomerPhoneNumber,
                    DateEncoded = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Manufacturer = model.Manufacturer,
                    OSVersion = model.OSVersion,
                    PhoneModel = model.PhoneModel,
                    Platform = model.Platform
                };
                db.CustomerDeviceInfoes.Add(deviceInfo);
                db.SaveChanges();
            }
            else
            {
                customerDeviceInfo.DateUpdated = DateTime.Now;
                customerDeviceInfo.Manufacturer = model.Manufacturer;
                customerDeviceInfo.OSVersion = model.OSVersion;
                customerDeviceInfo.PhoneModel = model.PhoneModel;
                customerDeviceInfo.Platform = model.Platform;
                db.Entry(customerDeviceInfo).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SendTestNotification(string deviceId = "", string title = "", string body = "")
        {
            Helpers.PushNotification.SendbydeviceId(body, title, "", deviceId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        
        public class OldOrder
        {

            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ShopPhoneNumber { get; set; }
            public string ShopOwnerPhoneNumber { get; set; }
            public int OrderNumber { get; set; }
            public int DeliveryBoyId { get; set; }
            public string DeliveryBoyName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public double DeliveryCharge { get; set; }
            public double ShopDeliveryDiscount { get; set; }
            public double NetDeliveryCharge { get; set; }
            public double Convinenientcharge { get; set; }
            public double Packingcharge { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Distance { get; set; }
            public int ShopPaymentStatus { get; set; }
            public int DeliveryBoyPaymentStatus { get; set; }
            public int DeliveryOrderPaymentStatus { get; set; }
            public double? RatePerOrder { get; set; }
            public int Status { get; set; }
            public System.DateTime DateEncoded { get; set; }
            public System.DateTime DateUpdated { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }
            public double Price { get; set; }
            public int Qty { get; set; }
            public double TotalPrice { get; set; }

            public string ProductName { get; set; }
            public string BrandName { get; set; }
            public string CategoryName { get; set; }
            public string ImagePath { get; set; }
            public string PaymentMode { get; set; }
            public int BrandId { get; set; }
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
        }

        public JsonResult SaveOrders()
        {
            using (WebClient myData = new WebClient())
            {
                myData.Headers.Add("X-ApiKey", "Tx9ANC5RqngpTOM9VJ0JP2+1LbZvo1LI");
                string getDetails = myData.DownloadString("http://localhost:45679/api/GetAllCartItems");
                var result = JsonConvert.DeserializeObject<List<OldOrder>>(getDetails).OrderBy(i=>i.DateEncoded);
                // var list = JsonConvert.SerializeObject(result.Where(i => i.OrderNumber == 253051825));
                foreach (var item in result.GroupBy(i => i.OrderNumber))
                {
                    var order = new Models.Order
                    {
                        Convinenientcharge = item.FirstOrDefault().Convinenientcharge,
                        CreatedBy = item.FirstOrDefault().CreatedBy,
                        CustomerId = item.FirstOrDefault().CustomerId,
                        CustomerName = item.FirstOrDefault().CustomerName,
                        CustomerPhoneNumber = item.FirstOrDefault().CustomerPhoneNumber,
                        DateEncoded = item.FirstOrDefault().DateEncoded,
                        DateUpdated = item.FirstOrDefault().DateUpdated,
                        DeliveredTime = null,
                        DeliveryAddress = item.FirstOrDefault().DeliveryAddress,
                        DeliveryBoyId = item.FirstOrDefault().DeliveryBoyId,
                        DeliveryBoyName = item.FirstOrDefault().DeliveryBoyName,
                        DeliveryBoyPaymentStatus = item.FirstOrDefault().DeliveryBoyPaymentStatus,
                        DeliveryBoyPhoneNumber = item.FirstOrDefault().DeliveryBoyPhoneNumber,
                        DeliveryBoyShopReachTime = null,
                        DeliveryCharge = item.FirstOrDefault().NetDeliveryCharge,
                        DeliveryLocationReachTime = null,
                        DeliveryOrderPaymentStatus = item.FirstOrDefault().DeliveryOrderPaymentStatus,
                        Distance = 0,
                        Latitude = item.FirstOrDefault().Latitude,
                        Longitude = item.FirstOrDefault().Longitude,
                        NetDeliveryCharge = item.FirstOrDefault().DeliveryCharge,
                        NetTotal = item.Sum(i => i.TotalPrice) + item.FirstOrDefault().Packingcharge + item.FirstOrDefault().Convinenientcharge + item.FirstOrDefault().DeliveryCharge,
                        OrderNumber = Convert.ToInt32(item.FirstOrDefault().OrderNumber),
                        OrderPickupTime = null,
                        OrderReadyTime = null,
                        Packingcharge = item.FirstOrDefault().Packingcharge,
                        PenaltyAmount = 0,
                        PenaltyRemark = null,
                        RatePerOrder = item.FirstOrDefault().RatePerOrder ?? 0,
                        ShopDeliveryDiscount = item.FirstOrDefault().ShopDeliveryDiscount,
                        ShopId = item.FirstOrDefault().ShopId,
                        ShopName = item.FirstOrDefault().ShopName,
                        ShopOwnerPhoneNumber = item.FirstOrDefault().ShopOwnerPhoneNumber,
                        ShopPaymentStatus = item.FirstOrDefault().ShopPaymentStatus,
                        ShopPhoneNumber = item.FirstOrDefault().ShopPhoneNumber,
                        Status = item.FirstOrDefault().Status,
                        TotalPrice = item.Sum(i => i.TotalPrice),
                        TotalProduct = item.Count(),
                        TotalQuantity = item.Sum(i => Convert.ToInt32(i.Qty)),
                        UpdatedBy = item.FirstOrDefault().UpdatedBy,
                        WaitingCharge = 0,
                        WaitingRemark = "",
                        WaitingTime = 0,
                        WalletAmount = 0,
                        CancelledRemark = "",
                        IsCallActive = false,
                        IsPreorder = false,
                        OfferAmount = 0,
                        OfferId = 0,
                        PaymentMode = item.FirstOrDefault().PaymentMode,
                        PaymentModeType = item.FirstOrDefault().PaymentMode == "Online Payment" ? 1 : 0,
                        PreorderDeliveryDateTime = null,
                        TipsAmount = 0,
                        ShopAcceptedTime = null,
                        TotalShopPrice=0
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    if (order != null)
                    {
                        foreach (var itemlist in result.Where(i => i.OrderNumber == order.OrderNumber).ToList())
                        {
                            var orderItem = new OrderItem
                            {
                                //BrandId = GetBrandId(itemlist.BrandName),
                                BrandId = itemlist.BrandId,
                                BrandName = itemlist.BrandName,
                                //CategoryId = GetCategoryId(itemlist.CategoryName),
                                CategoryId = itemlist.CategoryId,
                                CategoryName = itemlist.CategoryName,
                                ImagePath = itemlist.ImagePath,
                                OrdeNumber = order.OrderNumber,
                                OrderId = order.Id,
                                Price = itemlist.TotalPrice,
                                //ProductId = GetProductId(itemlist.ProductName),
                                ProductId = itemlist.ProductId,
                                ProductName = itemlist.ProductName,
                                Quantity = itemlist.Qty,
                                UnitPrice = itemlist.Price,
                                Status = 0,
                                ShopPrice=0
                            };

                            db.OrderItems.Add(orderItem);
                            db.SaveChanges();
                        }
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public int GetBrandId(string name)
        {
            var model = db.Brands.FirstOrDefault(i => i.Name.Trim().ToLower() == name.Trim().ToLower());
            if (model != null)
                return model.Id;
            else
                return 0;
        }

        public int GetCategoryId(string name)
        {
            var model = db.Categories.FirstOrDefault(i => i.Name.Trim().ToLower() == name.Trim().ToLower());
            if (model != null)
                return model.Id;
            else
                return 0;
        }
        public bool GetOfferCheck(long id)
        {
         //var offCount= (from varOffer in db.Offers
         //    join op in db.OfferProducts on id equals op.ProductId
         //    where varOffer.Type == 2 && varOffer.Status ==0
         //    select varOffer).Count();

            var offCounts = db.Offers.Where(i => i.Type == 2 && i.Status == 0)
           .Join(db.OfferProducts, o => o.Id, p => p.OfferId, (o, p) => new { o, p }).Where(i=>i.p.ProductId ==id).Count();
            if (offCounts > 0)
                return true;
            else
                return false;
        }

        public long GetProductId(string name)
        {
            var model = db.MasterProducts.FirstOrDefault(i => i.Name.Trim().ToLower() == name.Trim().ToLower());
            if (model != null)
            {
                var product = db.Products.FirstOrDefault(i => i.MasterProductId == model.Id);
                if (product != null)
                    return product.Id;
                else
                    return 0;
            }
            else
                return 0;
        }

        //Update search data if save as space seperated
        public JsonResult UpdateSearchData()
        {
            var list = db.SearchDatas.Where(i => i.KeyValue.Length > 100).ToList();
            foreach (var item in list)
            {
                string[] keyArr = item.KeyValue.Split(null);
                foreach (var li in keyArr)
                {
                    var searchdata = new SearchData
                    {
                        KeyValue = li,
                        Source = item.Source
                    };
                    db.SearchDatas.Add(searchdata);
                    db.SaveChanges();
                }

                var oldSD = db.SearchDatas.FirstOrDefault(i => i.Id == item.Id);
                db.SearchDatas.Remove(oldSD);
                db.SaveChanges();
            }

            return Json(true);
        }

        public JsonResult AddPaymentData(string code,int ordernumber) 
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                     SecurityProtocolType.Tls11 |
                                                     SecurityProtocolType.Tls12;
            string key = BaseClass.razorpaykey;// "rzp_live_PNoamKp52vzWvR";
            string secret = BaseClass.razorpaySecretkey;//"yychwOUOsYLsSn3XoNYvD1HY";

            RazorpayClient client = new RazorpayClient(key, secret);
            Razorpay.Api.Payment varpayment = new Razorpay.Api.Payment();
            var s = varpayment.Fetch(code);
            PaymentsData pay = new PaymentsData();
            pay.OrderNumber = ordernumber;
            pay.PaymentId = code;

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
            return Json(true,JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckLog()
        {
            Helpers.LogFile.WriteToFile("Logged Successfully");
            return Json(true);
        }
      
    }
}