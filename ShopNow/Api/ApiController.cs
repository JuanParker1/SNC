using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using Razorpay.Api;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.MessageHandlers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
        
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private string apipath= "https://admin.shopnowchat.in/";

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
                //config.CreateMap<CartCreateViewModel, Cart>();
                config.CreateMap<PaymentCreateApiViewModel, Models.Payment>();
                config.CreateMap<ShopSingleEditViewModel, Shop>();
                config.CreateMap<ShopReviewViewModel, CustomerReview>();
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

        public static String SendNotificationFromFirebaseCloud(string key, string title,string body)
        {
            var serverKey = string.Format("key={0}", "AAAASx4c4GY:APA91bEYyUEFT9F1XhO44epVtF0Mxq2SNbqIZUSQ3Xroov65JF9TzH7v9TghwG4JiWVa8HgqJVJnfklHIqhFuCQfW9T8b8TzrOOMYJd9eh2H1HcJFg06Vnjqz0aJk1tCSSuUL9BeUrsD");
            var result = "-1";
            var webAddr = "https://fcm.googleapis.com/fcm/send";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, serverKey);
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string strNJson = @"{
                    ""to"": ""cKxMgjNfjR4:APA91bHLw_jaHPk7bJW7ScG4ZKj6IbZ8bdX09bJqXpYNhDsdobhyWDw8dV_ZV63JExoVanzpB_ctkzhr341J3G1Ohj66gu4_0ntzLRCDKN6O5HMNgRJkLAcVJYiO3XbpDkCGkbV1rGNt"",
  ""notification"": {
                    ""title"": title,
    ""text"": body,
""sound"":""default""
  }
            }
            ";
                string not_title, not_body;
                not_title = title;
                not_body = body;
                string strNJsonn = @"{{'to':'{0}','notification':{{'title':'{1}','text':'{2}','sound': 'default'}}}}";

                strNJsonn = string.Format(strNJsonn, key, not_title, not_body);

                streamWriter.Write(strNJson);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
        public JsonResult NotifyAsync()
        {
           // SendNotificationFromFirebaseCloud();
            var serverKey = string.Format("key={0}", "AAAASx4c4GY:APA91bEYyUEFT9F1XhO44epVtF0Mxq2SNbqIZUSQ3Xroov65JF9TzH7v9TghwG4JiWVa8HgqJVJnfklHIqhFuCQfW9T8b8TzrOOMYJd9eh2H1HcJFg06Vnjqz0aJk1tCSSuUL9BeUrsD");

            // Get the sender id from FCM console
            var senderId = string.Format("id={0}", "322627756134");
            string to = "fXdD2vHnDVg:APA91bH0W1Grr4w07ghPmQiD7TtUETiLVupS9DGzryTtly8Y0sj35tiIgH3OsD6CjV5yvJni5lmJZWpsjLpVKV5u67mAuZPNDrCk1Dq1r3lUPCwT5ZBA8k4g4OdmLMbcgLvNgZ8XBySB";
            var result = "-1";
            var webAddr = "https://fcm.googleapis.com/fcm/send";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, serverKey);
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string strNJson = @"{
                    ""to"": ""cKxMgjNfjR4:APA91bHLw_jaHPk7bJW7ScG4ZKj6IbZ8bdX09bJqXpYNhDsdobhyWDw8dV_ZV63JExoVanzpB_ctkzhr341J3G1Ohj66gu4_0ntzLRCDKN6O5HMNgRJkLAcVJYiO3XbpDkCGkbV1rGNt"",
                    ""data"": {
                        ""ShortDesc"": ""Some short desc"",
                        ""IncidentNo"": ""any number"",
                        ""Description"": ""detail desc""
},
  ""notification"": {
                ""title"": "" You Have a New Order"",
    ""text"": ""ShopNowChat"",
""sound"":""default""
  }
        }";
                streamWriter.Write(strNJson);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return Json(new { result, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
        
        public  async Task<bool> NotifyAsynca()
        {
            try
            {
                //SendNotificationFromFirebaseCloud();
                   // Get the server key from FCM console
                   var serverKey = string.Format("key={0}", "AAAASx4c4GY:APA91bEYyUEFT9F1XhO44epVtF0Mxq2SNbqIZUSQ3Xroov65JF9TzH7v9TghwG4JiWVa8HgqJVJnfklHIqhFuCQfW9T8b8TzrOOMYJd9eh2H1HcJFg06Vnjqz0aJk1tCSSuUL9BeUrsD");
                
                // Get the sender id from FCM console
                var senderId = string.Format("id={0}", "322627756134");
               string to = "fXdD2vHnDVg:APA91bH0W1Grr4w07ghPmQiD7TtUETiLVupS9DGzryTtly8Y0sj35tiIgH3OsD6CjV5yvJni5lmJZWpsjLpVKV5u67mAuZPNDrCk1Dq1r3lUPCwT5ZBA8k4g4OdmLMbcgLvNgZ8XBySB";
                string title = "Order Notification";
                string body = "snc meses You have new order";
                string sound = "default";
                var data = new
                {
                    to, // Recipient device token
                    notification = new { title, body, sound }
                };

                // Using Newtonsoft.Json
                var jsonBody = JsonConvert.SerializeObject(data);

                using (var httpRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                {
                    httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                    httpRequest.Headers.TryAddWithoutValidation("Sender", senderId);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    using (var httpClient = new HttpClient())
                    {
                        var result = await httpClient.SendAsync(httpRequest);

                         if (result.IsSuccessStatusCode)
                        {
                            return true;
                        }
                        else
                        {
                            Console.WriteLine(result.StatusCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
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
                Admin admin = new Admin();
                admin.AnonymisedID = user.Id.ToString();
                //admin.Code = ShopNow.Helpers.DRC.Generate("ADM");
                admin.OfficialID = AdminHelpers.SecureData(admin.Id.ToString());
                admin.AnonymisedID = AdminHelpers.SecureData(admin.AnonymisedID);
                admin.Status = 0;
                admin.DateEncoded = DateTime.Now;
                admin.DateUpdated = DateTime.Now;
                db.Admins.Add(admin);
                db.SaveChanges();
                if (user.Id !=0)
                {
                    var otpmodel = new OtpViewModel();
                    var models = _mapper.Map<OtpViewModel, OtpVerification>(otpmodel);
                    models.CustomerId = user.Id;
                    models.CustomerName = user.Name;
                    models.PhoneNumber = model.PhoneNumber;
                    models.Otp = _generatedCode;
                    models.ReferenceCode = _referenceCode;
                    models.Verify = false;
                    models.CreatedBy = user.Name;
                    models.UpdatedBy = user.Name;
                    models.DateEncoded = DateTime.Now;
                    var dateAndTime = DateTime.Now;
                    var date = dateAndTime.ToString("d");
                    var time = dateAndTime.ToString("HH:mm");

                    string joyra = "04448134440";
                    string Msg = "Hi, " + models.Otp + " is the OTP for (Shop Now Chat) Verification at " + time + " with " + models.ReferenceCode + " reference - Joyra";

                    string result = SendSMS.execute(joyra, model.PhoneNumber, Msg);
                    models.Status = 0;
                    models.DateEncoded = DateTime.Now;
                    models.DateUpdated = DateTime.Now;
                    db.OtpVerifications.Add(models);
                    db.SaveChanges();

                    if (model.Id != 0)
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
                var otpmodel = new OtpViewModel();
                var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber);
                var models = _mapper.Map<OtpViewModel, OtpVerification>(otpmodel);
                models.CustomerId = customer.Id;
                models.CustomerName = customer.Name;
                models.PhoneNumber = model.PhoneNumber;
                models.Otp = _generatedCode;
                models.ReferenceCode = _referenceCode;
                models.Verify = false;

                var dateAndTime = DateTime.Now;
                var date = dateAndTime.ToString("d");
                var time = dateAndTime.ToString("HH:mm");

                string joyra = "04448134440";
                string Msg = "Hi, " + models.Otp + " is the OTP for (Shop Now Chat) Verification at " + time + " with " + models.ReferenceCode + " reference - Joyra";

                string result = SendSMS.execute(joyra, model.PhoneNumber, Msg);
                models.Status = 0;
                models.DateEncoded = DateTime.Now;
                models.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(models);
                db.SaveChanges();
                if (models.Id != 0)
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
                if (user.Id != 0)
                {
                    return Json(new { message = "Successfully Added Addons Address!", Details = user }, JsonRequestBehavior.AllowGet);
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

            return Json(new { message = "Successfully Updated Your Address!", Details = customerAddress });
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
                               where c.Id == customerId && c.Position ==1
                               select c.Name).FirstOrDefault();
            var orderCount = (from s in db.Orders
                              join sh in db.Shops on s.Shopid equals sh.Id
                              join c in db.Customers on sh.CustomerId equals c.Id
                              where sh.CustomerId == customerId && (s.Status >= 2)
                             select s).Count();
                
            var platformcredits = (from ss in db.Payments
                                   where ss.CustomerId == customerId && ss.Status == 0 && ss.CreditType == 0
                                   select (Double?)ss.OriginalAmount).Sum() ?? 0;

            var platformorder = (Convert.ToInt32(orderCount)  * (db.PlatFormCreditRates.FirstOrDefault().RatePerOrder));         
            var varDelivery = (from ss in db.Payments
                               where ss.CustomerId == customerId && ss.Status == 0 && ss.CreditType == 1
                               select (Double?)ss.OriginalAmount).Sum() ?? 0;

            var varDeliveryCharges = (from ss in db.ShopCharges
                                      join sh in db.Shops on ss.ShopId equals sh.Id
                                      where sh.CustomerId == customerId && ss.Status >= 2
                                      select (Double?)ss.GrossDeliveryCharge).Sum() ?? 0;

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
                              Name=m.Name,
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
            var previousurl =apipath+ "/Api/GetProducts?shopId=" + shopId + "&page=" + previous;

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
            var model = db.Products.Join(db.MasterProducts, p => p.MasterProductId, m =>m.Id,(p,m) => new {p,m })//, (c, p) => new { c, p })
                            .AsEnumerable()
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
                                Status = i.p.Status
                            }).ToList();


            int count = model.Count();

            int CurrentPage = page;

            int PageSize = pageSize;

            int TotalCount = count;

            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            var items = model.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath+ "/Api/GetShopItemList?shopId=" + shopId + "&str=" + str + "&page=" + previous;

            var previousPage = CurrentPage > 1 ? previousurl : "No";

            var current = CurrentPage + 1;

            var nexturl = apipath+ "/Api/GetShopItemList?shopId=" + shopId + "&str=" + str + "&page=" + current;
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
        public JsonResult SaveCustomerToken(int customerId,string token)
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


                if (deliveryBoy.Id !=0)
                {
                    return Json(new { message = "Successfully Created a Delivery Boy!", Position = customer.Position });

                }
                else
                    return Json(new { message = "Failed to Create a Delivery Boy!" });
            }
            else
                return Json(new { message = "This Delivery Boy Already Exist!" });

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
                        
                             stock =Math.Floor(GetStockQty(item.ItemId.ToString()));
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
            catch(Exception ex)
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
                    var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                    payment.CustomerName = customer.Name;
                    payment.CreatedBy = customer.Name;
                    payment.UpdatedBy = customer.Name;

                if (model.OrderNo != 0)
                {
                    //var cartList = db.Carts.Where(i => i.OrderNo == model.OrderNo).ToList();
                    //foreach (var c in cartList)
                    //{
                    //    var cart = db.Carts.FirstOrDefault(i => i.Code == c.Code);

                    //    cart.CartStatus = 2;
                    //    cart.UpdatedBy = customer.Name;
                    //    cart.Rateperorder = Convert.ToDouble(perOrderAmount.RatePerOrder);
                    //    cart.DateUpdated = DateTime.Now;
                    //    db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                    //    db.SaveChanges();

                    //}
                    var order = db.Orders.FirstOrDefault(i => i.OrderNumber == model.OrderNo);
                    order.Status = 2;
                    order.UpdatedBy = customer.Name;
                    order.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
                    order.DateUpdated = DateTime.Now;
                    db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    if (model.PaymentMode == "Online Payment")
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
                        string key = "rzp_live_PNoamKp52vzWvR";
                        string secret = "yychwOUOsYLsSn3XoNYvD1HY";

                        RazorpayClient client = new RazorpayClient(key, secret);
                        Razorpay.Api.Payment varpayment = new Razorpay.Api.Payment();
                        var s = varpayment.Fetch(model.ReferenceCode);
                        PaymentsData pay = new PaymentsData();

                        pay.OrderNo = Convert.ToInt32(model.OrderNo);
                        pay.paymentId = model.ReferenceCode;

                        pay.invoice_id = s["invoice_id"];
                        if (s["status"] == "created")
                            pay.status = 0;
                        else if (s["status"] == "authorized")
                            pay.status = 1;
                        else if (s["status"] == "captured")
                            pay.status = 2;
                        else if (s["status"] == "refunded")
                            pay.status = 3;
                        else if (s["status"] == "failed")
                            pay.status = 4;
                        pay.order_id = s["order_id"];
                        if (s["fee"] != null && s["fee"] > 0)
                            pay.fee = (decimal)s["fee"] / 100;
                        else
                            pay.fee = s["fee"];
                        pay.entity = s["entity"];
                        pay.currency = s["currency"];
                        pay.method = s["method"];
                        if (s["tax"] != null && s["tax"] > 0)
                            pay.tax = (decimal)s["tax"] / 100;
                        else
                            pay.tax = s["tax"];
                        if (s["amount"] != null && s["amount"] > 0)
                            pay.amount = s["amount"] / 100;
                        else
                            pay.amount = s["amount"];
                        pay.DateEncoded = DateTime.Now;
                        db.PaymentsDatas.Add(pay);
                        db.SaveChanges();
                    }
                    var fcmToken = (from c in db.Customers
                                    join s in db.Shops on c.Id equals s.CustomerId
                                    where s.Id == model.ShopId
                                    select c.FcmTocken ?? "" ).FirstOrDefault().ToString();
                    string rtnMessage = Helpers.PushNotification.SendbydeviceId("You have a new Order", "ShopNowChat", "../../assets/a.mp3", fcmToken.ToString());
                  
                }
                    
                    if (model.CreditType == 0 || model.CreditType == 1)
                    {
                        payment.PaymentCategoryType = 1;
                        payment.Credits = model.OriginalAmount.ToString();
                        //var top = db.TopUps.OrderByDescending(q => q.Id).FirstOrDefault(i => i.CustomerCode == model.CustomerCode && i.CreditType == model.CreditType && i.Status == 0); //TopUp.GetCustomer(model.CustomerCode, model.CreditType);
                        //if (top == null)
                        //{
                        //    TopUp topup = new TopUp();
                        //    topup.CustomerCode = model.CustomerCode;
                        //    topup.CustomerName = model.CustomerName;
                        //    topup.CustomerPhoneNumber = customer.PhoneNumber;
                        //    topup.CreditType = model.CreditType;
                        //    topup.CreditAmount = model.OriginalAmount;
                        //    topup.CreatedBy = customer.Name;
                        //    topup.UpdatedBy = customer.Name;
                        //    topup.Code = _generateCode("TOP");
                        //    topup.DateEncoded = DateTime.Now;
                        //    topup.DateUpdated = DateTime.Now;
                        //    topup.Status = 0;
                        //    db.TopUps.Add(topup);
                        //    db.SaveChanges();
                        //}
                        //else
                        //{
                        //    top.CreditAmount = top.CreditAmount + model.OriginalAmount;
                        //    top.UpdatedBy = customer.Name;
                        //    top.DateUpdated = DateTime.Now;
                        //    db.Entry(top).State = System.Data.Entity.EntityState.Modified;
                        //    db.SaveChanges();
                        //}
                    }
                    else
                    {
                        payment.PaymentCategoryType = 0;
                        payment.Credits = "N/A";

                        ShopCharge detail = new ShopCharge();
                        detail.CustomerId = customer.Id;
                        detail.CustomerName = customer.Name;
                        detail.OrderNo = model.OrderNo;
                        detail.ShopId = model.ShopId;
                        detail.ShopName = model.ShopName;
                        detail.NetDeliveryCharge = model.NetDeliveryCharge;
                        detail.GrossDeliveryCharge = model.GrossDeliveryCharge;
                        detail.ShopDeliveryDiscount = model.ShopDeliveryDiscount;
                        detail.OrderStatus = 2;
                        detail.UpdatedBy = customer.Name;
                        detail.CreatedBy = customer.Name;
                        detail.Packingcharge = model.PackagingCharge;
                        detail.Convinenientcharge = model.ConvenientCharge;
                        detail.DateEncoded = DateTime.Now;
                        detail.DateUpdated = DateTime.Now;
                        detail.Status = 0;
                        db.ShopCharges.Add(detail);
                        db.SaveChanges();
                    }

                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    payment.Rateperorder = Convert.ToDouble(perOrderAmount.RatePerOrder);
                    payment.refundStatus = 1;
                    db.Payments.Add(payment);
                    db.SaveChanges();

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
            string key = "rzp_live_PNoamKp52vzWvR";
            string secret = "yychwOUOsYLsSn3XoNYvD1HY";

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", model.Price);
            input.Add("currency", "INR");
            input.Add("receipt", "order_rcptid_11");
            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);


            Orderid = order["id"].ToString();
            return Json(new { message = "Success",Orderid= Orderid });
        }
    

        [HttpPost]
        public JsonResult UpdatedPayment(PaymentUpdatedApiViewModel model)
        {
            //int errorCode = 0;
            var payment = db.Payments.FirstOrDefault(i => i.OrderNo == model.OrderNo); // Payment.GetOrderNo(model.OrderNo);
            payment.UpdatedOriginalAmount = model.UpdatedOriginalAmount;
            payment.UpdatedAmount = model.UpdatedAmount;
            if (model.RefundAmount >0)
            {
                payment.refundAmount = model.RefundAmount;
                payment.refundRemark = model.RefundRemark;
            }
            if (model.CustomerId !=0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);// Customer.Get(model.CustomerCode);
                payment.UpdatedBy = customer.Name;
                payment.DateUpdated = DateTime.Now;
                db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //Payment.Edit(payment, out errorCode);
                return Json(new { message = "Successfully Updated Cart Payment!", Details = model });
            }
            else
                return Json(new { message = "Failed to Update Cart Payment !" });

        }

        //[HttpPost]
        //public JsonResult AddCart(CartCreateViewModel model)
        //{
        //    var shop = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);
        //    var orderCount = (from s in db.Carts
        //                      join sh in db.Shops on s.ShopCode equals sh.Code
        //                      join c in db.Customers on sh.CustomerCode equals c.Code
        //                      where sh.CustomerCode == shop.CustomerCode && (s.CartStatus >= 2)
        //                      group s by s.OrderNo into g
        //                      select g).Count();

        //    var platform = (from ss in db.Payments
        //                    join sh in db.Shops on ss.ShopCode equals sh.Code
        //                    join c in db.Customers on sh.CustomerCode equals c.Code
        //                    where sh.CustomerCode == shop.CustomerCode && ss.Status == 0 && ss.CreditType == 0
        //                    select (Double?)ss.OriginalAmount).Sum() ?? 0;

        //    var platformorder = (Convert.ToInt32(orderCount) * (db.PlatFormCreditRates.FirstOrDefault().RatePerOrder));
        //    var varDelivery = (from ss in db.Payments
        //                       join sh in db.Shops on ss.ShopCode equals sh.Code
        //                       join c in db.Customers on sh.CustomerCode equals c.Code
        //                       where sh.CustomerCode == shop.CustomerCode && ss.Status == 0 && ss.CreditType == 1
        //                       select (Double?)ss.OriginalAmount).Sum() ?? 0;

        //    var varDeliveryCharges = (from ss in db.ShopCharges
        //                              join sh in db.Shops on ss.ShopCode equals sh.Code
        //                              join c in db.Customers on sh.CustomerCode equals c.Code
        //                              where sh.CustomerCode == shop.CustomerCode && ss.CartStatus >= 2
        //                              select (Double?)ss.GrossDeliveryCharge).Sum() ?? 0;
        //    var DeliveryCredits = varDelivery - varDeliveryCharges;
        //    var PlatformCredits = platform - platformorder;

        //    if ((PlatformCredits < 26 && DeliveryCredits < 67))
        //    {
        //        //Shop DeActivate

        //        shop.Status = 6;
        //        db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();

        //        return Json(new { message = "This shop is currently unservicable." }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        if (model.ItemId != 0) //model.ItemId != "N/A"
        //        {
        //            var product = db.Products.FirstOrDefault(i => i.ItemId == model.ItemId && i.Status == 0); // ProductMedicalStock.GetItemId(model.ItemId);
        //            product.HoldOnStok = Convert.ToInt32(model.Qty);
        //            product.Qty = product.Qty - Convert.ToInt32(model.Qty);
        //            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();

        //            var cart = _mapper.Map<CartCreateViewModel, Cart>(model);
        //            cart.CartStatus = 0;
        //            if (model.CustomerCode != null)
        //            {
        //                var customer = db.Customers.FirstOrDefault(i => i.Code == model.CustomerCode);// Customer.Get(model.CustomerCode);
        //                cart.CreatedBy = customer.Name;
        //                cart.UpdatedBy = customer.Name;
        //                cart.CustomerName = customer.Name;
        //            }

        //            cart.Code = _generateCode("CAR");
        //            cart.DateEncoded = DateTime.Now;
        //            cart.DateUpdated = DateTime.Now;
        //            db.Carts.Add(cart);
        //            db.SaveChanges();
        //            if (cart.Code != null || cart.Code != "")
        //            {
        //                return Json(new { message = "Successfully Added to Cart!", Details = cart });
        //            }
        //            else

        //                return Json(new { message = "Network Issue!" });
        //        }
        //        else
        //        {
        //            var carts = _mapper.Map<CartCreateViewModel, Cart>(model);
        //            carts.CartStatus = 0;
        //            if (model.CustomerCode != null)
        //            {
        //                var customer = db.Customers.FirstOrDefault(i => i.Code == model.CustomerCode);// Customer.Get(model.CustomerCode);
        //                carts.CreatedBy = customer.Name;
        //                carts.UpdatedBy = customer.Name;
        //                carts.CustomerName = customer.Name;
        //            }
        //            carts.Code = _generateCode("CAR");
        //            carts.DateEncoded = DateTime.Now;
        //            carts.DateUpdated = DateTime.Now;
        //            db.Carts.Add(carts);
        //            db.SaveChanges();

        //            if (carts.Code != null || carts.Code != "")
        //            {
        //                return Json(new { message = "Successfully Added to Cart!", Details = carts });
        //            }
        //            else
        //                return Json(new { message = "Failed to Add Cart!" });
        //        }

        //    }

        //}


        [HttpPost]
        public JsonResult AddOrder(OrderCreateViewModel model)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
            var orderCount = (from s in db.Orders
                              join sh in db.Shops on s.Shopid equals sh.Id
                              join c in db.Customers on sh.CustomerId equals c.Id
                              where sh.CustomerId == shop.CustomerId && (s.Status >= 2)
                              select s).Count();

            var platform = (from ss in db.Payments
                            join sh in db.Shops on ss.ShopId equals sh.Id
                            join c in db.Customers on sh.CustomerId equals c.Id
                            where sh.CustomerId == shop.CustomerId && ss.Status == 0 && ss.CreditType == 0
                            select (Double?)ss.OriginalAmount).Sum() ?? 0;

            var platformorder = (Convert.ToInt32(orderCount) * (db.PlatFormCreditRates.FirstOrDefault().RatePerOrder));
            var varDelivery = (from ss in db.Payments
                               join sh in db.Shops on ss.ShopId equals sh.Id
                               join c in db.Customers on sh.CustomerId equals c.Id
                               where sh.CustomerId == shop.CustomerId && ss.Status == 0 && ss.CreditType == 1
                               select (Double?)ss.OriginalAmount).Sum() ?? 0;

            var varDeliveryCharges = (from ss in db.ShopCharges
                                      join sh in db.Shops on ss.ShopId equals sh.Id
                                      join c in db.Customers on sh.CustomerId equals c.Id
                                      where sh.CustomerId == shop.CustomerId && ss.Status >= 2
                                      select (Double?)ss.GrossDeliveryCharge).Sum() ?? 0;
            var DeliveryCredits = varDelivery - varDeliveryCharges;
            var PlatformCredits = platform - platformorder;

            if ((PlatformCredits < 26 && DeliveryCredits < 67))
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
                order.Status = 2;
                if (model.CustomerId != 0)
                {
                    var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                    order.Customerid = customer.Id;
                    order.CreatedBy = customer.Name;
                    order.UpdatedBy = customer.Name;
                    order.CustomerName = customer.Name;
                    order.CustomerPhonenumber = customer.PhoneNumber;
                }

                order.OrderNumber = Convert.ToInt32(model.OrderNo);
                order.Shopid = shop.Id;
                order.Shopname = shop.Name;
                order.ShopPhonenumber = shop.PhoneNumber ?? shop.ManualPhoneNumber;
                order.ShopOwnerPhonenumber = shop.OwnerPhoneNumber;
                order.TotalPrice = model.ListItems.Sum(i => i.Price);
                order.TotalProduct = model.ListItems.Count();
                order.TotalQuantity = model.ListItems.Sum(i => Convert.ToInt32(i.Quantity));
                order.DateEncoded = DateTime.Now;
                order.DateUpdated = DateTime.Now;
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
                    orderItem.OrderId = order.id;
                    orderItem.OrdeNumber = order.OrderNumber;
                    db.Orders.Add(order);
                    db.SaveChanges();
                }

                if (order != null)
                    return Json(new { message = "Successfully Added to Cart!", Details = order });
                else
                    return Json(new { message = "Failed to Add Cart!" });
            }

        }
        
        //public JsonResult GetUpdateCart(string code, string qty, double amount, string customercode, int isupdate, string deliveryBoyCode = "")
        //{
        //    var cart = db.Carts.FirstOrDefault(i => i.Code == code); // Cart.Get(code);
        //    cart.UpdatedQty = qty;
        //    cart.UpdatedPrice = amount;
        //    cart.isUpdate = isupdate;
        //    if (customercode != null)
        //    {
        //        var customer = db.Customers.FirstOrDefault(i => i.Code == customercode);// Customer.Get(customercode);
        //        cart.UpdatedBy = customer.Name;
        //    }
        //    if (isupdate == 2)
        //    {
        //        cart.CartStatus = 2;
        //    }
        //    if (isupdate == 4)
        //    {
        //        cart.CartStatus = 4;
        //        var delivery = db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryBoyCode); // DeliveryBoy.Get(deliveryBoyCode);
        //        cart.DeliveryBoyCode = delivery.Code;
        //        cart.DeliveryBoyName = delivery.Name;
        //        cart.DeliveryBoyPhoneNumber = delivery.PhoneNumber;

        //    }
        //    cart.DateUpdated = DateTime.Now;
        //    db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
        //    db.SaveChanges();


        //    if (code != null || code != "")
        //    {
        //        return Json(new { message = "Successfully Updated the Order!" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { message = "Failed to Updated the order!" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public JsonResult GetRemoveCart(string code, int isdelete)
        //{
        //    var cart = db.Carts.FirstOrDefault(i => i.Code == code); // Cart.Get(code);
        //    cart.Status = 1;
        //    cart.isDelete = isdelete;
        //    db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
        //    db.SaveChanges();

        //    if (code != null || code != "")
        //    {
        //        return Json(new { message = "Successfully Removed from Cart!" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { message = "Failed to Remove from Cart!" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public JsonResult GetAcceptOrder(int orderNo, int customerId, int status, int priority)
        {

            if (orderNo != 0 && customerId != 0 && status != 0)
            {
                //if (status == 3)
                //{
                //    var topup = db.TopUps.OrderByDescending(q => q.Id).FirstOrDefault(i => i.CustomerCode == customerId && i.CreditType == 0 && i.Status == 0);
                //    if (topup == null)
                //    {

                //    }
                //    else
                //    {
                //        var list = db.PlatFormCreditRates.Where(i => i.Status == 0).ToList();
                //        topup.CreditAmount = topup.CreditAmount - list.FirstOrDefault().RatePerOrder;
                //        topup.DateUpdated = DateTime.Now;
                //        db.Entry(topup).State = System.Data.Entity.EntityState.Modified;
                //        db.SaveChanges();
                //    }
                //}
                var detail = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
                detail.OrderStatus = status;
                detail.DateUpdated = DateTime.Now;
                db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderNo);
                order.Status = status;
                order.UpdatedBy = customer.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var orderList = db.OrderItems.Where(i => i.OrderId == order.id).ToList();
                foreach (var item in orderList)
                {
                    //Product Stock Update
                    var product = db.Products.FirstOrDefault(i => i.Id == item.ProductId);
                    product.HoldOnStok -= Convert.ToInt32(item.Quantity);
                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                //Refund
                var payment = db.Payments.FirstOrDefault(i => i.OrderNo == order.OrderNumber);
                if (payment.PaymentMode == "Online Payment")
                {
                    payment.refundAmount = payment.Amount;
                    payment.refundRemark = "Your order has been cancelled by shop.";
                    payment.UpdatedBy = customer.Name;
                    payment.DateUpdated = DateTime.Now;
                    db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { message = "Successfully Updated the Order!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to Update the Order!" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Have to check later
        //public JsonResult GetShopDeliveredOrders(string shopId, int status, int page = 1, int pageSize = 5)
        //{

        //    var model = new CartAcceptListApiViewModel();
        //    model.List = db.Orders.Where(j => j.Status == status)
        //        .Join(db.Payments, c => c.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
        //        .Join(db.Products, rz => rz.c.ProductCode, pr => pr.Id, (rz, pr) => new { rz, pr })
        //        .Join(db.Shops, py => py.rz.c.ShopCode, s => s.Code, (py, s) => new { py, s })
        //        .Join(db.ShopCharges, pay => pay.py.rz.c.OrderNo, sc => sc.OrderNo, (pay, sc)
        //        => new { pay, sc })
        //        .AsEnumerable()
        //       .Where(i => (i.pay.py.rz.p.PaymentResult == "success" || i.pay.py.rz.p.PaymentMode == "Cash On Hand") && i.pay.py.rz.p.ShopCode == shopCode)
        //       .GroupBy(j => j.pay.py.rz.c.OrderNo).Select(i => new CartAcceptListApiViewModel.CartList
        //       {
        //           Code = i.Any() ? i.FirstOrDefault().pay.py.rz.c.Code : "N/A",
        //           ProductCode = i.Any() ? i.FirstOrDefault().pay.py.rz.c.ProductCode : "N/A",
        //           PaymentMode = i.Any() ? i.FirstOrDefault().pay.py.rz.p.PaymentMode : "N/A",
        //           ShopCode = i.Any() ? i.FirstOrDefault().pay.py.rz.c.ShopCode : "N/A",
        //           ShopName = i.Any() ? i.FirstOrDefault().pay.py.rz.c.ShopName : "N/A",
        //           OrderNo = i.Any() ? i.FirstOrDefault().pay.py.rz.c.OrderNo : "N/A",
        //           CustomerName = i.Any() ? i.FirstOrDefault().pay.py.rz.c.CustomerName : "N/A",
        //          // ProductName = i.Any() ? i.FirstOrDefault().pay.py.pr.MasterProductName : "N/A",
        //           ProductName = i.Any() ? GetMasterProductName(i.FirstOrDefault().pay.py.pr.MasterProductCode) : "N/A",
        //           PhoneNumber = i.Any() ? i.FirstOrDefault().pay.py.rz.c.PhoneNumber : "N/A",
        //           DeliveryAddress = i.Any() ? i.FirstOrDefault().pay.py.rz.c.DeliveryAddress : "N/A",
        //           ShopLatitude = i.Any() ? i.FirstOrDefault().pay.s.Latitude : 0.0,
        //           ShopLongitude = i.Any() ? i.FirstOrDefault().pay.s.Longitude : 0.0,
        //           PackingCharge = i.Any() ? i.FirstOrDefault().sc.Packingcharge : 0.0,
        //           ConvinenientCharge = i.Any() ? i.FirstOrDefault().sc.Convinenientcharge : 0.0,
        //           SinglePrice = i.Any() ? i.FirstOrDefault().pay.py.rz.c.SinglePrice : 0.0,
        //           Price = i.FirstOrDefault().pay.py.rz.c.UpdatedPrice != 0 ? i.FirstOrDefault().pay.py.rz.c.Price : i.FirstOrDefault().pay.py.rz.c.UpdatedPrice,
        //           Amount = GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedAmount == 0 ? GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).Amount : GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedAmount,
        //           OriginalAmount = i.FirstOrDefault().pay.py.rz.p.UpdatedOriginalAmount == 0 ? i.FirstOrDefault().pay.py.rz.p.OriginalAmount : i.FirstOrDefault().pay.py.rz.p.UpdatedOriginalAmount,
        //           GrossDeliveryCharge = i.Any() ? i.FirstOrDefault().sc.GrossDeliveryCharge : 0.0,
        //           ShopDeliveryDiscount = i.Any() ? i.FirstOrDefault().sc.ShopDeliveryDiscount : 0.0,
        //           NetDeliveryCharge = i.Any() ? i.FirstOrDefault().sc.NetDeliveryCharge : 0.0,
        //           Qty = i.FirstOrDefault().pay.py.rz.c.UpdatedQty == "" ? i.FirstOrDefault().pay.py.rz.c.Qty : i.FirstOrDefault().pay.py.rz.c.UpdatedQty,
        //           Date = i.Any() ? i.FirstOrDefault().pay.py.rz.c.DateEncoded.ToString("dd-MMM-yyyy HH:mm") : "N/A",
        //           DateEncoded = i.Any() ? i.FirstOrDefault().pay.py.rz.c.DateEncoded : DateTime.Now,
        //           OrderList = GetOrderPendingList(i.FirstOrDefault().pay.py.rz.c.OrderNo, status), // Cart.GetOrderPendingList(i.FirstOrDefault().pay.py.rz.c.OrderNo, status),
        //           CartStatus = i.Any() ? i.FirstOrDefault().pay.py.rz.c.CartStatus : -5
        //       }).OrderByDescending(i => i.DateEncoded).ToList();

        //    int count = model.List.Count();

        //    int CurrentPage = page;

        //    int PageSize = pageSize;

        //    int TotalCount = count;

        //    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

        //    var items = model.List.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        //    var previous = CurrentPage - 1;
        //    var previousurl = apipath+ "/Api/GetShopDeliveredOrders?shopId=" + shopId + "&status=" + status + "&page=" + previous;

        //    var previousPage = CurrentPage > 1 ? previousurl : "No";

        //    var current = CurrentPage + 1;

        //    var nexturl = apipath+ "/Api/GetShopDeliveredOrders?shopId=" + shopId + "&status=" + status + "&page=" + current;
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

      

        public JsonResult GetDelivaryTodayOrders(string phoneNumber)
        {
            var model = new CartDelivaryListApiViewModel();
            string dt = DateTime.Now.ToString("dd-MMM-yyyy");
            model.ResturantList = db.Orders.Where(j => j.Status == 4 || j.Status == 5)
                .Join(db.Payments, c => c.Shopid, p => p.ShopId, (c, p) => new { c, p })
                .Join(db.Shops, ca => ca.c.Shopid, s => s.Id, (ca, s) => new { ca, s })
                 .Join(db.DeliveryBoys, py => py.ca.c.DeliveryBoyId, d => d.Id, (py, d) => new { py, d })
               .AsEnumerable()
               .Where(i => i.py.ca.c.DeliveryBoyPhoneNumber == phoneNumber && i.py.s.ShopCategoryId == 0 && i.py.ca.c.DateEncoded.ToString("dd-MMM-yyyy") == dt)
               .Select(i => new CartDelivaryListApiViewModel.CartList
               {
                   ShopName = i.py.ca.c.Shopname,
                   CustomerName = i.py.ca.c.CustomerName,
                   OrderNo = i.py.ca.c.OrderNumber,
                   ShopAddress = i.py.s.Address,
                   ShopLatitude = i.py.s.Latitude,
                   ShopLongitude = i.py.s.Longitude,
                   ShopPhoneNumber = i.py.s.PhoneNumber,
                   CartStatus = i.py.ca.c.Status,
                   CustomerPhoneNumber = i.py.ca.c.CustomerPhonenumber,
                   CustomerLatitude = i.py.ca.c.Latitude,
                   CustomerLongitude = i.py.ca.c.Longitude,
                   DeliveryAddress = i.py.ca.c.DeliveryAddress,
                   PaymentMode = GetPayment(i.py.ca.c.OrderNumber).PaymentMode,
                   Amount = GetPayment(i.py.ca.c.OrderNumber).UpdatedAmount == 0 ? GetPayment(i.py.ca.c.OrderNumber).Amount : GetPayment(i.py.ca.c.OrderNumber).UpdatedAmount,
                   OrderList = GetOrderList(i.py.ca.c.id),
                   OnWork = i.d.OnWork ,
                   Date =  i.py.ca.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss"),
                   RefundAmount= ((GetPayment(i.py.ca.c.OrderNumber).refundAmount)?? 0),
                   RefundRemark = ((GetPayment(i.py.ca.c.OrderNumber).refundRemark) ?? "N/A")
               }).ToList();

            model.OtherList = db.Orders.Where(j => j.Status == 4 || j.Status == 5)
               .Join(db.Payments, c => c.Shopid, p => p.ShopId, (c, p) => new { c, p })
               .Join(db.Shops, ca => ca.c.Shopid, s => s.Id, (ca, s) => new { ca, s })
                .Join(db.DeliveryBoys, py => py.ca.c.DeliveryBoyId, d => d.Id, (py, d) => new { py, d })
              .AsEnumerable()
              .Where(i => i.py.ca.c.DeliveryBoyPhoneNumber == phoneNumber && i.py.s.ShopCategoryId != 0)
              .Select(i => new CartDelivaryListApiViewModel.CartList
              {
                  ShopName = i.py.ca.c.Shopname,
                  CustomerName = i.py.ca.c.CustomerName,
                  OrderNo = i.py.ca.c.OrderNumber,
                  ShopAddress = i.py.s.Address,
                  ShopLatitude = i.py.s.Latitude,
                  ShopLongitude = i.py.s.Longitude,
                  ShopPhoneNumber = i.py.s.PhoneNumber,
                  CartStatus = i.py.ca.c.Status,
                  CustomerPhoneNumber = i.py.ca.c.CustomerPhonenumber,
                  CustomerLatitude = i.py.ca.c.Latitude,
                  CustomerLongitude = i.py.ca.c.Longitude,
                  DeliveryAddress = i.py.ca.c.DeliveryAddress,
                  PaymentMode = GetPayment(i.py.ca.c.OrderNumber).PaymentMode,
                  Amount = GetPayment(i.py.ca.c.OrderNumber).UpdatedAmount == 0 ? GetPayment(i.py.ca.c.OrderNumber).Amount : GetPayment(i.py.ca.c.OrderNumber).UpdatedAmount,
                  OrderList = GetOrderList(i.py.ca.c.id),
                  OnWork = i.d.OnWork,
                  Date = i.py.ca.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss"),
                  RefundAmount = ((GetPayment(i.py.ca.c.OrderNumber).refundAmount) ?? 0),
                  RefundRemark = ((GetPayment(i.py.ca.c.OrderNumber).refundRemark) ?? "N/A")
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
                order.DeliverBoyName =  customer.Name;
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

        public  JsonResult GetShopNotification(int shopId)
        {
            var customerTocken = (from c in db.Customers
                                  join s in db.Shops on c.Id equals s.CustomerId
                                  where s.Id == shopId
                                  select c.FcmTocken).ToString();
                       
            if (shopId != 0)
            {
                var shop = db.Orders.OrderByDescending(q => q.id).FirstOrDefault(i => i.Shopid == shopId && i.Status == 2 && i.Status == 0);

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
                var cart = db.Orders.OrderByDescending(q => q.id).FirstOrDefault(i => i.Customerid == customerId && i.Status == 5 && i.Status == 0);// Cart.GetCustomerPickUp(customerCode);

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
                var delivaryBoy = db.Orders.OrderByDescending(q => q.id).FirstOrDefault(i => i.DeliveryBoyPhoneNumber == phoneNo && i.Status  == 4 && i.Status == 0);// Cart.GetDelivaryPhoneNo(phoneNo);

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
            var shop = db.Shops.FirstOrDefault(i => i.Id == order.Shopid);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId);
            var model = new DeliveryBoyViewModel();
            model.ShopLatitude = shop.Latitude;
            model.ShopLongitude = shop.Longitude;
            model.CustomerLatitude = order.Latitude;
            model.CustomerLongitude = order.Longitude;
            if (deliveryBoy != null)
            {
                model.DeliveryBoyName = order.DeliverBoyName;
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

        public JsonResult GetPickUp(int orderNo, int customerId,double Amount,string PaymentMode)
        {
            if (orderNo != 0 && customerId != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderNo);
                order.Status = 5;
                order.UpdatedBy = customer.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var orderList = db.OrderItems.Where(i => i.OrderId == order.id).ToList();
                foreach (var item in orderList)
                {
                    //Product Stock Update
                    var product = db.Products.FirstOrDefault(i => i.Id == item.ProductId);
                    product.HoldOnStok -= Convert.ToInt32(item.Quantity);
                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                var detail = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
                detail.Status = 5;
                detail.DateUpdated = DateTime.Now;
                db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                if (PaymentMode == "Online Payment" && Amount > 1000)
                {
                    
                    var models = new OtpVerification();
                    models.ShopId = order.Shopid;
                    models.CustomerId = customer.Id;
                    models.CustomerName = customer.Name;
                    models.PhoneNumber = order.DeliveryBoyPhoneNumber;
                    models.Otp = _generatedCode;
                    models.ReferenceCode = _referenceCode;
                    models.Verify = false;
                    models.OrderNo = orderNo;
                    models.CreatedBy = customer.Name;
                    models.UpdatedBy = customer.Name;
                    models.DateUpdated = DateTime.Now;
                    db.OtpVerifications.Add(models);
                    db.SaveChanges();
                }

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

                var shopCharge = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
                var shop = db.Shops.FirstOrDefault(i => i.Id == shopCharge.ShopId);
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

            var detail = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
            detail.OrderStatus = 6;
            detail.DateUpdated = DateTime.Now;
            db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
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
                order.DeliverBoyName = deliveryBoy.Name;
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

                var detail = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
                detail.CustomerId = deliveryBoy.CustomerId;
                detail.CustomerName = deliveryBoy.CustomerName;
                detail.DeliveryBoyId = deliveryBoy.Id;
                detail.DeliveryBoyName = deliveryBoy.Name;
                detail.Status = 4;
                detail.UpdatedBy = customer.Name;
                detail.DateUpdated = DateTime.Now;
                db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var fcmToken = (from c in db.Customers
                                where c.Id == deliveryBoyId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                String rtnMessage = Helpers.PushNotification.SendbydeviceId("You have a new Order", "ShopNowChat", "../../assets/b.mp3", fcmToken.ToString());
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
                   Id = i.id,
                   ShopName = i.Shopname,
                   OrderNo = i.OrderNumber,
                   CartStatus = i.Status,
                   Date = i.DateUpdated.ToString("dd-MMM-yyyy"),
                   Price = i.TotalPrice,
                   DateUpdated = i.DateUpdated
               }).OrderByDescending(i => i.DateUpdated).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllOrders(int customerId, int page = 1, int pageSize = 5)
        {
            var model = new CartListApiViewModel();
            model.List = db.Orders
                .Join(db.OrderItems, o => o.id, oi => oi.OrderId, (o, oi) => new { o, oi })
                .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
            .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
             .Join(db.ShopCharges, pay => pay.rz.c.o.OrderNumber, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
              .Join(db.Shops, py => py.pay.rz.c.o.Shopid, s => s.Id, (py, s) => new { py, s })
             .AsEnumerable()
           .Where(i => i.py.pay.rz.c.o.Customerid == customerId && i.py.pay.rz.p.CreditType == 2)
           .Select(i => new CartListApiViewModel.CartList
           {
               Id = i.py.pay.rz.c.o.id,
               ProductId = i.py.pay.rz.c.oi.ProductId,
               ShopPhoneNumber = i.s.PhoneNumber,
               PaymentMode = i.py.pay.rz.p.PaymentMode,
               ShopId = i.py.pay.rz.c.o.Shopid,
               ShopName = i.py.pay.rz.c.o.Shopname,
               CustomerName = i.py.pay.rz.c.o.CustomerName,
               ProductName = GetMasterProductName(i.py.pay.pr.MasterProductId),
               OrderNo = i.py.pay.rz.c.o.OrderNumber,
               Price = i.py.pay.rz.c.o.TotalPrice,
               //OriginalAmount = GetPayment(i.py.pay.rz.c.o.OrderNumber).UpdatedOriginalAmount != 0 ? i.FirstOrDefault().py.pay.rz.p.OriginalAmount : GetPayment(i.FirstOrDefault().py.pay.rz.c.OrderNo).UpdatedOriginalAmount,
               DeliveryBoyId = i.py.pay.rz.c.o.DeliveryBoyId,
               DeliveryBoyName = i.py.pay.rz.c.o.DeliverBoyName,
               DeliveryBoyPhoneNumber = i.py.pay.rz.c.o.DeliveryBoyPhoneNumber,
               PhoneNumber = i.py.pay.rz.c.o.ShopPhonenumber,
               Otp = GetOtp(i.py.pay.rz.c.o.OrderNumber),
               DeliveryAddress = i.py.pay.rz.c.o.DeliveryAddress,
               PackingCharge = i.py.sc.Packingcharge,
               ConvinenientCharge = i.py.sc.Convinenientcharge,
               Amount = GetPayment(i.py.pay.rz.c.o.OrderNumber).Amount,
               GrossDeliveryCharge = i.py.sc.GrossDeliveryCharge,
               ShopDeliveryDiscount = i.py.sc.ShopDeliveryDiscount,
               NetDeliveryCharge = i.py.sc.NetDeliveryCharge,
               Qty = i.py.pay.rz.c.o.TotalQuantity,
               OrderList = GetOrderList(i.py.pay.rz.c.o.OrderNumber),
               Date = i.py.pay.rz.c.o.DateEncoded.ToString("dd/MMM/yyyy HH:mm"),
               DateEncoded = i.py.pay.rz.c.o.DateEncoded,
               CartStatus = i.py.pay.rz.c.o.Status,
               RfAmount = i.py.pay.rz.p.refundAmount,
               RefundRemark = i.py.pay.rz.p.refundRemark

           }).OrderBy(j => j.CartStatus).OrderByDescending(i => i.DateEncoded).ToList();

            int count = model.List.Count();

            int CurrentPage = page;

            int PageSize = pageSize;

            int TotalCount = count;

            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            var items = model.List.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;
            var previousurl = apipath + "/Api/GetAllOrders?customerId=" + customerId + "&page=" + previous;

            var previousPage = CurrentPage > 1 ? previousurl : "No";

            var current = CurrentPage + 1;

            var nexturl = apipath + "/Api/GetAllOrders?customerId=" + customerId + "&page=" + current;
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


        public JsonResult GetShopReAssignOrders(int shopId, int page = 1, int pageSize = 5)
        {
            var model = new CartAcceptListApiViewModel();
            model.List = db.Orders.Where(j => j.Status == 3)
                 .Join(db.OrderItems, o => o.id, oi => oi.OrderId, (o, oi) => new { o, oi })
                .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
                .Join(db.Shops, py => py.rz.c.o.Shopid, s => s.Id, (py, s) => new { py, s })
                .Join(db.ShopCharges, pay => pay.py.rz.c.o.OrderNumber, sc => sc.OrderNo, (pay, sc)
                => new { pay, sc })
                   .AsEnumerable()
               .Where(i => (i.pay.py.rz.p.PaymentResult == "success" || i.pay.py.rz.p.PaymentMode == "Cash On Hand") && i.pay.py.rz.p.ShopId == shopId)
               .Select(i => new CartAcceptListApiViewModel.CartList
               {
                   Id = i.pay.py.rz.c.o.id,
                   ProductId = i.pay.py.rz.c.oi.ProductId,
                   ShopId = i.pay.py.rz.c.o.Shopid,
                   ShopName = i.pay.py.rz.c.o.Shopname,
                   OrderNo = i.pay.py.rz.c.o.OrderNumber,
                   PaymentMode = i.pay.py.rz.p.PaymentMode,
                   CustomerName = i.pay.py.rz.c.o.CustomerName,
                   ProductName = GetMasterProductName(i.pay.py.pr.MasterProductId),
                   PhoneNumber = i.pay.py.rz.c.o.ShopPhonenumber,
                   DeliveryAddress = i.pay.py.rz.c.o.DeliveryAddress,
                   ShopLatitude = i.pay.s.Latitude,
                   ShopLongitude = i.pay.s.Longitude,
                   PackingCharge = i.sc.Packingcharge,
                   ConvinenientCharge = i.sc.Convinenientcharge,
                   Amount = GetPayment(i.pay.py.rz.c.o.OrderNumber).Amount,
                   //OriginalAmount = GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount == 0 ? GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).OriginalAmount : GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount,
                   GrossDeliveryCharge = i.sc.GrossDeliveryCharge,
                   ShopDeliveryDiscount = i.sc.ShopDeliveryDiscount,
                   NetDeliveryCharge = i.sc.NetDeliveryCharge,
                   Qty = i.pay.py.rz.c.o.TotalQuantity,
                   Date = i.pay.py.rz.c.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
                   DateEncoded = i.pay.py.rz.c.o.DateEncoded,
                   OrderList = GetOrderPendingList(i.pay.py.rz.c.o.OrderNumber),
                   CartStatus = i.pay.py.rz.c.o.Status
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
            var model = new CartAcceptListApiViewModel();
            if (mode == 0)
            {
                model.List = db.Orders.Where(j => j.Status == 2)
                 .Join(db.OrderItems, o => o.id, oi => oi.OrderId, (o, oi) => new { o, oi })
                .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
                .Join(db.Shops, py => py.rz.c.o.Shopid, s => s.Id, (py, s) => new { py, s })
                .Join(db.ShopCharges, pay => pay.py.rz.c.o.OrderNumber, sc => sc.OrderNo, (pay, sc)
                => new { pay, sc })
                   .AsEnumerable()
                   .Where(i => (i.pay.py.rz.p.PaymentResult == "success" || i.pay.py.rz.p.PaymentMode == "Cash On Hand" || i.pay.py.rz.p.PaymentMode == "Online Payment" || i.pay.py.rz.p.PaymentMode == "pending") && i.pay.py.rz.p.ShopId == shopId)
                   .Select(i => new CartAcceptListApiViewModel.CartList
                   {
                       Id = i.pay.py.rz.c.o.id,
                       ProductId = i.pay.py.rz.c.oi.ProductId,
                       ShopId = i.pay.py.rz.c.o.Shopid,
                       ShopName = i.pay.py.rz.c.o.Shopname,
                       OrderNo = i.pay.py.rz.c.o.OrderNumber,
                       PaymentMode = i.pay.py.rz.p.PaymentMode,
                       CustomerName = i.pay.py.rz.c.o.CustomerName,
                       ProductName = GetMasterProductName(i.pay.py.pr.MasterProductId),
                       PhoneNumber = i.pay.py.rz.c.o.ShopPhonenumber,
                       DeliveryAddress = i.pay.py.rz.c.o.DeliveryAddress,
                       ShopLatitude = i.pay.s.Latitude,
                       ShopLongitude = i.pay.s.Longitude,
                       PackingCharge = i.sc.Packingcharge,
                       ConvinenientCharge = i.sc.Convinenientcharge,
                       Amount = GetPayment(i.pay.py.rz.c.o.OrderNumber).Amount,
                       //OriginalAmount = GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount == 0 ? GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).OriginalAmount : GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount,
                       GrossDeliveryCharge = i.sc.GrossDeliveryCharge,
                       ShopDeliveryDiscount = i.sc.ShopDeliveryDiscount,
                       NetDeliveryCharge = i.sc.NetDeliveryCharge,
                       Qty = i.pay.py.rz.c.o.TotalQuantity,
                       Date = i.pay.py.rz.c.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
                       DateEncoded = i.pay.py.rz.c.o.DateEncoded,
                       OrderList = GetOrderPendingList(i.pay.py.rz.c.o.OrderNumber),
                       CartStatus = i.pay.py.rz.c.o.Status
                   }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Orders.Where(j => j.Status ==3 || j.Status ==4)
                 .Join(db.OrderItems, o => o.id, oi => oi.OrderId, (o, oi) => new { o, oi })
                .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
                .Join(db.Shops, py => py.rz.c.o.Shopid, s => s.Id, (py, s) => new { py, s })
                .Join(db.ShopCharges, pay => pay.py.rz.c.o.OrderNumber, sc => sc.OrderNo, (pay, sc)
                => new { pay, sc })
                   .AsEnumerable()
                  .Where(i => (i.pay.py.rz.p.PaymentResult == "success" || i.pay.py.rz.p.PaymentMode == "Cash On Hand" || i.pay.py.rz.p.PaymentMode == "Online Payment" || i.pay.py.rz.p.PaymentMode == "pending") && i.pay.py.rz.p.ShopId == shopId)
                  .Select(i => new CartAcceptListApiViewModel.CartList
                  {
                      Id = i.pay.py.rz.c.o.id,
                      ProductId = i.pay.py.rz.c.oi.ProductId,
                      ShopId = i.pay.py.rz.c.o.Shopid,
                      ShopName = i.pay.py.rz.c.o.Shopname,
                      OrderNo = i.pay.py.rz.c.o.OrderNumber,
                      PaymentMode = i.pay.py.rz.p.PaymentMode,
                      CustomerName = i.pay.py.rz.c.o.CustomerName,
                      ProductName = GetMasterProductName(i.pay.py.pr.MasterProductId),
                      PhoneNumber = i.pay.py.rz.c.o.ShopPhonenumber,
                      DeliveryAddress = i.pay.py.rz.c.o.DeliveryAddress,
                      ShopLatitude = i.pay.s.Latitude,
                      ShopLongitude = i.pay.s.Longitude,
                      PackingCharge = i.sc.Packingcharge,
                      ConvinenientCharge = i.sc.Convinenientcharge,
                      Amount = GetPayment(i.pay.py.rz.c.o.OrderNumber).Amount,
                      //OriginalAmount = GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount == 0 ? GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).OriginalAmount : GetPayment(i.FirstOrDefault().pay.py.rz.c.OrderNo).UpdatedOriginalAmount,
                      GrossDeliveryCharge = i.sc.GrossDeliveryCharge,
                      ShopDeliveryDiscount = i.sc.ShopDeliveryDiscount,
                      NetDeliveryCharge = i.sc.NetDeliveryCharge,
                      Qty = i.pay.py.rz.c.o.TotalQuantity,
                      Date = i.pay.py.rz.c.o.DateEncoded.ToString("dd-MMM-yyyy HH:mm"),
                      DateEncoded = i.pay.py.rz.c.o.DateEncoded,
                      OrderList = GetOrderPendingList(i.pay.py.rz.c.o.OrderNumber),
                      CartStatus = i.pay.py.rz.c.o.Status
                  }).OrderByDescending(i => i.DateEncoded).ToList();
            }

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

        public JsonResult GetShopAllOrders(int shopId, int page = 1, int pageSize = 5)
        {
            var model = new CartListApiViewModel();
            model.List = db.Orders.Where(j => (j.Status == 4 || j.Status == 3) && j.Status == 0)
                .Join(db.OrderItems, o => o.id, oi => oi.OrderId, (o, oi) => new { o, oi })
                .Join(db.Payments, c => c.o.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                .Join(db.Products, rz => rz.c.oi.ProductId, pr => pr.Id, (rz, pr) => new { rz, pr })
                .Join(db.Shops, py => py.rz.c.o.Shopid, s => s.Id, (py, s) => new { py, s })
                .Join(db.ShopCharges, pay => pay.py.rz.c.o.OrderNumber, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                    //.Join(db.DeliveryBoys, ca => ca.pay.py.rz.c.DeliveryBoyCode, db => db.Code, (ca, db) => new { ca, db })
                    .AsEnumerable()
               .Where(i => (i.pay.py.rz.p.PaymentResult == "success" || i.pay.py.rz.p.PaymentResult == "pending" || i.pay.py.rz.p.PaymentMode == "Cash On Hand" || i.pay.py.rz.p.PaymentMode == "Online Payment") && i.pay.py.rz.c.o.Shopid == shopId)
               .Select(i => new CartListApiViewModel.CartList
               {
                   Id = i.pay.py.rz.c.o.id,
                   ProductId = i.pay.py.rz.c.oi.ProductId,
                   ShopPhoneNumber = i.pay.py.rz.c.o.ShopPhonenumber,
                   PaymentMode = i.pay.py.rz.p.PaymentMode,
                   ShopId = i.pay.py.rz.c.o.Shopid,
                   ShopName = i.pay.py.rz.c.o.Shopname,
                   CustomerName = i.pay.py.rz.c.o.CustomerName,
                   ProductName = GetMasterProductName(i.pay.py.pr.MasterProductId),
                   OrderNo = i.pay.py.rz.c.o.OrderNumber,
                   Price = i.pay.py.rz.c.o.TotalPrice,
                   //OriginalAmount = GetPayment(i.py.pay.rz.c.o.OrderNumber).UpdatedOriginalAmount != 0 ? i.FirstOrDefault().py.pay.rz.p.OriginalAmount : GetPayment(i.FirstOrDefault().py.pay.rz.c.OrderNo).UpdatedOriginalAmount,
                   DeliveryBoyId = i.pay.py.rz.c.o.DeliveryBoyId,
                   DeliveryBoyName = i.pay.py.rz.c.o.DeliverBoyName,
                   DeliveryBoyPhoneNumber = i.pay.py.rz.c.o.DeliveryBoyPhoneNumber,
                   PhoneNumber = i.pay.py.rz.c.o.ShopPhonenumber,
                   Otp = GetOtp(i.pay.py.rz.c.o.OrderNumber),
                   DeliveryAddress = i.pay.py.rz.c.o.DeliveryAddress,
                   PackingCharge = i.sc.Packingcharge,
                   ConvinenientCharge = i.sc.Convinenientcharge,
                   Amount = GetPayment(i.pay.py.rz.c.o.OrderNumber).Amount,
                   GrossDeliveryCharge = i.sc.GrossDeliveryCharge,
                   ShopDeliveryDiscount = i.sc.ShopDeliveryDiscount,
                   NetDeliveryCharge = i.sc.NetDeliveryCharge,
                   Qty = i.pay.py.rz.c.o.TotalQuantity,
                   OrderList = GetOrderList(i.pay.py.rz.c.o.OrderNumber),
                   Date = i.pay.py.rz.c.o.DateEncoded.ToString("dd/MMM/yyyy HH:mm"),
                   DateEncoded = i.pay.py.rz.c.o.DateEncoded,
                   CartStatus = i.pay.py.rz.c.o.Status,
                   RfAmount = i.pay.py.rz.p.refundAmount,
                   RefundRemark = i.pay.py.rz.p.refundRemark
               }).OrderByDescending(i => i.DateEncoded).ToList();


            int count = model.List.Count();

            int CurrentPage = page;

            int PageSize = pageSize;

            int TotalCount = count;

            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            var items = model.List.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
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
            var shopExist = db.Shops.Where(m => m.OwnerPhoneNumber == model.OwnerPhoneNumber && m.Latitude == model.Latitude && m.Longitude == model.Longitude && (m.Status == 0 || m.Status == 6));
            var shopCustomerExist = db.Shops.FirstOrDefault(i => i.Name == model.Name && i.Status == 1 && i.CustomerId == model.CustomerId);
            if (shopExist == null && shopCustomerExist == null)
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

                Admin admin = new Admin();
                admin.AnonymisedID = shop.Id.ToString();
                //admin.Code = _generateCode("ADM");
                admin.Status = 0;
                admin.DateEncoded = DateTime.Now;
                admin.DateUpdated = DateTime.Now;
                db.Admins.Add(admin);
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
                //otpmodel.Code = _generateCode("SMS");
                otpmodel.Status = 0;
                otpmodel.DateEncoded = DateTime.Now;
                otpmodel.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(otpmodel);
                db.SaveChanges();

                if (shop.Id != 0)
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
            //int errorCode = 0;
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.Id);// Shop.Get(model.Code);
            _mapper.Map(model, shop);
            if (model.AuthorisedBrandName != null)
            {

                var bran = db.Brands.FirstOrDefault(i => i.Name == model.AuthorisedBrandName); // Brand.GetName(model.AuthorisedBrandName);
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
            
            if (shop.Id !=0)
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
            //otpmodel.Code = _generateCode("SMS");
            otpmodel.Status = 0;
            otpmodel.DateEncoded = DateTime.Now;
            otpmodel.DateUpdated = DateTime.Now;
            db.OtpVerifications.Add(otpmodel);
            db.SaveChanges();
            return Json(new { message = "Your Todays OTP is: " + otpmodel.Otp }, JsonRequestBehavior.AllowGet);
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
            //var rate = db.CustomerReviews.Where(j => j.ShopCode == shop.Code).ToList();
            //var reviewCount = db.CustomerReviews.Where(j => j.ShopCode == shop.Code).Count();
            //if (reviewCount > 0)
            //    model.Rating = rate.Sum(l => l.Rating) / reviewCount ?? 0;
            //else
            //    reviewCount = 0;
            //model.CustomerReview = reviewCount;

            model.CategoryLists = db.Database.SqlQuery<ShopDetails.CategoryList>($"select distinct CategoryCode as Code, CategoryName as Name from Products p join Categories c on c.Code = p.CategoryCode where shopid ={id}  and c.Status = 0 and CategoryCode is not null and CategoryName is not null group by CategoryCode,CategoryName order by Name").ToList<ShopDetails.CategoryList>();
            

            if (shop.ShopCategoryId == 0)
            {
                //model.ProductLists = db.Products.Where(i => i.ShopCode == code && i.Status == 0).ToList().Where(i => str != "" ? i.Name.ToLower().StartsWith(str.ToLower()) : true && categoryCode != "" ? i.CategoryCode == categoryCode : true).AsQueryable().ProjectTo<ShopDetails.ProductList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();
                model.ProductLists = (from pl in db.Products
                                      join m in db.MasterProducts on pl.MasterProductId equals m.Id
                                      where pl.ShopId == id && pl.Status == 0  && (categoryId != 0 ? pl.ShopCategoryId == categoryId : true)
                                      select new ShopDetails.ProductList
                                      {
                                          Id = pl.Id,
                                          Name = m.Name,
                                          ShopId = pl.ShopId,
                                          ShopName = pl.ShopName,
                                         // CategoryId = pl.ShopCategoryId,
                                         // CategoryName = pl.ShopCategoryName,
                                         // ColorCode = pl.ColorCode,
                                          Price = pl.Price,
                                          ImagePath = m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23"),
                                          Status = pl.Status

                                      }).Where(i => str != "" ? i.Name.ToLower().Contains(str) : true).ToList();

                
                model.AddOnsLists = db.Products.Where(i=>i.Status ==0)
               .Join(db.ShopDishAddOns, p => p.Id, d => d.ProductId, (p, d) => new { p, d })
                           .AsEnumerable()
                           .Where(i => i.p.ShopId == id && i.p.Status == 0 && i.d.IsActive == true)
                           .Select(i => new ShopDetails.AddOnsList
                           {
                               Id = i.d.Id,
                               Name = i.d.AddOnItemName,
                               ProductId = i.p.Id,
                                // ProductName = i.p.Name,
                                ProductName = GetMasterProductName(i.p.MasterProductId),
                               ShopId = i.p.ShopId,
                               ShopName = i.p.ShopName,
                               AddOnCategoryCode = i.d.AddOnCategoryCode,
                               AddOnCategoryName = i.d.AddOnCategoryName,
                               PortionCode = i.d.PortionCode,
                               PortionName = i.d.PortionName,
                               MinSelectionLimit = i.d.MinSelectionLimit,
                               MaxSelectionLimit = i.d.MaxSelectionLimit,
                               CrustName = i.d.CrustName,
                               AddOnsPrice = i.d.AddOnsPrice,
                               PortionPrice = i.d.PortionPrice,
                               CrustPrice = i.d.CrustPrice,
                               Status = i.p.Status,
                           }).ToList();

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
                                          //CategoryCode = m.NextSubCategoryCode,
                                          CategoryName = m.NextSubCategoryName,
                                          //ColorCode = pl.ColorCode,
                                          Price = pl.Price,
                                          ImagePath = m.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23"),
                                          Status = pl.Status

                                      }).ToList();

                model.AddOnsLists = db.Products
               .Join(db.ShopDishAddOns, p => p.Id, d => d.ProductId, (p, d) => new { p, d })
                           .AsEnumerable()
                           .Where(i => i.p.ShopId == id && i.p.Status == 0 && i.d.Status == 0)
                           .Select(i => new ShopDetails.AddOnsList
                           {
                               Id = i.d.Id,
                               Name = i.d.AddOnItemName,
                               ProductId = i.p.Id,
                               // ProductName = i.p.Name,
                               ProductName =GetMasterProductName(i.p.MasterProductId),
                               ShopId = i.p.ShopId,
                               ShopName = i.p.ShopName,
                               AddOnCategoryCode = i.d.AddOnCategoryCode,
                               AddOnCategoryName = i.d.AddOnCategoryName,
                               PortionCode = i.d.PortionCode,
                               PortionName = i.d.PortionName,
                               MinSelectionLimit = i.d.MinSelectionLimit,
                               MaxSelectionLimit = i.d.MaxSelectionLimit,
                               CrustName = i.d.CrustName,
                               AddOnsPrice = i.d.AddOnsPrice,
                               PortionPrice = i.d.PortionPrice,
                               CrustPrice = i.d.CrustPrice,
                               Status = i.p.Status,
                               AddOnType =i.d.AddOnType
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

        public JsonResult GetProductList(double latitude, double longitude, string str = "", int page = 1, int pageSize = 10)
        {
            var model = new ProductSearchViewModel();
            double? varlongitude = longitude;
            double? varlatitude = latitude;
            int? varpage = page;
            int? varPagesize = pageSize;
           //var s = db.GetProductList(varlongitude, varlatitude, str, varpage, varPagesize).ToList();

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
                ShopOnline=i.isOnline,
                ShopStatus=i.Status
                }).ToList();

            //var productrCount = db.GetProductListCount(varlongitude, varlatitude, str).ToList();
            //int  count =Convert.ToInt32(productrCount[0]);

            int CurrentPage = page;

            int PageSize = pageSize;

            //int TotalCount = count;

            //int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            //var items = s;
            var previous = CurrentPage - 1;
            var previousurl = apipath+ "/Api/GetProductList?latitude=" + latitude + "&longitude=" + longitude + "&str=" + str + "&page=" + previous;

            var previousPage = CurrentPage > 1 ? previousurl : "No";

            var current = CurrentPage + 1;

            var nexturl = apipath+ "/Api/GetProductList?latitude=" + latitude + "&longitude=" + longitude + "&str=" + str + "&page=" + current;
           // var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                //totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                //totalPages = TotalPages,
                previousPage,
                //nextPage
            };

            int count1 = model.ShopList.Count();

            int CurrentPage1 = page;

            int PageSize1 = pageSize;

            int TotalCount1 = count1;

            int TotalPages1 = (int)Math.Ceiling(count1 / (double)PageSize1);

            var items1 = model.ShopList.Skip((CurrentPage1 - 1) * PageSize1).Take(PageSize1).ToList();
            var previous1 = CurrentPage1 - 1;
            var previousurl1 = apipath+ "/Api/GetProductList?latitude=" + latitude + "&longitude=" + longitude + "&str=" + str + "&page=" + previous;

            var previousPage1 = CurrentPage1 > 1 ? previousurl1 : "No";

            var current1 = CurrentPage1 + 1;

            var nexturl1 = "https://admin.shopnowchat.in/Api/GetProductList?latitude=" + latitude + "&longitude=" + longitude + "&str=" + str + "&page=" + current;
            var nextPage1 = CurrentPage1 < TotalPages1 ? nexturl1 : "No";
            var paginationMetadata1 = new
            {
                totalCount = TotalCount1,
                pageSize = PageSize1,
                currentPage = CurrentPage1,
                totalPages = TotalPages1,
                previousPage1,
                nextPage1
            };

            return Json(new { Page = paginationMetadata, /*items,*/ Page1 = paginationMetadata1, items1 }, JsonRequestBehavior.AllowGet);
        }
        public static double GetStockQty(string code)
        {


            using (WebClient myData = new WebClient())
            {

                myData.Headers["X-Auth-Token"] = "62AA1F4C9180EEE6E27B00D2F4F79E5FB89C18D693C2943EA171D54AC7BD4302BE3D88E679706F8C";
                myData.Headers[HttpRequestHeader.Accept] = "application/json";
                myData.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                string getList = myData.DownloadString("http://joyrahq.gofrugal.com/RayMedi_HQ/api/v1/items?q=status==R,outletId==2,itemId=="+code);

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

        public JsonResult GetShopCategoryList(int shopId, string categoryCode, string str = "", int page = 1, int pageSize = 20)
        {
            var shid = db.Shops.Where(s => s.Id == shopId).FirstOrDefault();
            int count = 0;
            //var total = db.GetShopCategoryProductCount(shopCode, categoryCode, str).ToList();
            //if (total.Count > 0)
            //    count = total[0].Value;
            
            var skip = page-1;
            
            //var model = db.GetShopCategoryProducts(shopCode, categoryCode, str, skip, pageSize).ToList();
            

            int CurrentPage = page;

            int PageSize = pageSize;

            int TotalCount = count;

            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

           // var items = model;
            var previous = CurrentPage - 1;
          //var previousurl = "https://admin.shopnowchat.in/Api/GetShopCategoryList?shopCode=" + shopCode + "&categoryCode=" + categoryCode + "&str=" + str + "&page=" + previous;
            

           // var previousPage = CurrentPage > 1 ? previousurl : "No";

            var current = CurrentPage + 1;

            //var nexturl = "https://admin.shopnowchat.in/Api/GetShopCategoryList?shopCode=" + shopCode + "&categoryCode=" + categoryCode + "&str=" + str + "&page=" + current;
            

           // var nextPage = CurrentPage < TotalPages ? nexturl : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                //previousPage,
               // nextPage
            };


            return Json(new { Page = paginationMetadata, /*items*/ }, JsonRequestBehavior.AllowGet);
        }
        string GetMasterProductName(int id)
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
            var shop = db.Shops.FirstOrDefault(i => i.Id == shid.Id); // Shop.Get(code);
            ShopSingleUpdateViewModel model = _mapper.Map<Shop, ShopSingleUpdateViewModel>(shop);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetShopBalanceNotification(int customerId)
        {
            var varShopOwner= db.Customers.Where(s => s.Id == customerId && s.Position ==1).FirstOrDefault();
            if (varShopOwner != null)
            {
                var varCustomer = db.Customers.Where(s => s.Id == customerId && s.Position == 1).FirstOrDefault();
                var customerName = (from c in db.Customers
                                    where c.Id == varCustomer.Id && c.Position ==1
                                    select c.Name).FirstOrDefault();
                
                var orderCount = (from s in db.Orders
                                  join sh in db.Shops on s.Shopid equals sh.Id
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

                var varDeliveryCharges = (from ss in db.ShopCharges
                                          join sh in db.Shops on ss.ShopId equals sh.Id
                                          where sh.CustomerId == customerId && ss.Status >= 2
                                          select (Double?)ss.GrossDeliveryCharge).Sum() ?? 0;

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
            int errorCode = 0;

            var review = _mapper.Map<ShopReviewViewModel, CustomerReview>(model);
            ClassCustomerReview.Add(review, out errorCode);
            if (review.Id != 0)
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
            int errorCode = 0;
            //var review = ClassCustomerReview.Get(model.Code);
            var review = db.CustomerReviews.FirstOrDefault(i => i.Id == model.Id);
            review.CustomerRemark = model.CustomerRemark;
            review.Rating = model.Rating;
            ClassCustomerReview.Edit(review, out errorCode);
            return Json(new { message = "Successfully Updated to Rating!", Details = model });

        }
        public JsonResult GetAllReview(string code, string customerCode, int page = 1, int pageSize = 9)
        {
            var model = new ReviewListViewModel();
            model.CustomerList = db.CustomerReviews
                             .Where(i => i.Status == 0 && i.ShopCode == code && i.CustomerCode == customerCode)
                           .AsEnumerable()
                         .Select(i => new ReviewListViewModel.ReviewlList
                         {
                             Id = i.Id,
                             ShopName = i.ShopName,
                             CustomerName = i.CustomerName,
                             CustomerRemark = i.CustomerRemark,
                             Rating = i.Rating
                         }).ToList();

            model.ReviewlLists = db.CustomerReviews
                             .Where(i => i.Status == 0 && i.ShopCode == code && i.CustomerCode != customerCode)
                           .AsEnumerable()
                         .Select(i => new ReviewListViewModel.ReviewlList
                         {
                             Id = i.Id,
                             ShopName = i.ShopName,
                             CustomerName = i.CustomerName,
                             CustomerRemark = i.CustomerRemark,
                             Rating = i.Rating
                         }).ToList();

            //model.ReviewlLists = CustomerReview.GetList(code).AsQueryable().ProjectTo<ReviewListViewModel.ReviewlList>(_mapperConfiguration).OrderBy(i => i.CustomerName).ToList();

            int count = model.ReviewlLists.Count();

            int CurrentPage = page;

            int PageSize = pageSize;

            int TotalCount = count;

            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            var items = model.ReviewlLists.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previous = CurrentPage - 1;

            var previousurl = "http://192.168.0.111/WebAdmin/Api/GetAllReview?code=" + code + "&page=" + previous;
            var previousPage = CurrentPage > 1 ? previousurl : "No";

            var current = CurrentPage + 1;

            var nexturl = "http://192.168.0.111/WebAdmin/Api/GetAllReview?code=" + code + "&page=" + current;
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
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.Id); // Shop.Get(model.Code);
            _mapper.Map(model, shop);
            shop.UpdatedBy = shop.CustomerName;
            shop.DateUpdated = DateTime.Now;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            //  Shop.Edit(shop, out errorCode);

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
            // int errorCode = 0;
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.Id); // Shop.Get(model.Code);
            shop.ImagePath = model.ImagePath;
            shop.UpdatedBy = shop.CustomerName;
            shop.DateUpdated = DateTime.Now;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            // Shop.Edit(shop, out errorCode);

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
                                isOnline = i.ss.s.isOnline,
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
                shop.isOnline = true;
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
                shop.isOnline = false;
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
            string query = "SELECT top(6) * " +
                               " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and Status = 0 and Latitude != 0 and Longitude != 0" +
                               " order by Rating";
                           model.NearShops = db.Shops.SqlQuery(query,
                            new SqlParameter("Latitude", Latitude),
                            new SqlParameter("Longitude", Longitude)).Select(i => new NearShopImages.shops
                            {
                            id = i.Id,
                            image =i.ImagePath !=null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23"):"",

                            }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
            public JsonResult GetNearPlaces(double Latitude, double Longitude, string a)
        {
            var model = new PlacesListView();
            string query = "SELECT top(6) * " +
                               " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryCode = '0' and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
                               " order by Rating";
            string querySuperMarketList = "SELECT top(6) * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryCode = '2' and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by Rating";
            string queryGroceriesList = "SELECT top(6) * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryCode = '1' and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by Rating";
            string queryHealthList = "SELECT top(6) * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryCode = '3' and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by Rating";
            string queryElectronicsList = "SELECT top(6) * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryCode = '4' and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by Rating";
            string qServicesList = "SELECT top(6) * " +
            " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryCode = '5' and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
            " order by Rating";
            if (a == "-1")
            {

                string queryOtherList = "SELECT top(6) * " +
                " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryCode = '6' and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0" +
                " order by Rating";



   model.ResturantList = db.Shops.SqlQuery(query,
    new SqlParameter("Latitude", Latitude),
    new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
    {
        Id = i.Id,
        Name = i.Name,
        DistrictName = i.StreetName,
      //  Rating = RatingCalculation(i.Id),
        ImagePath = i.ImagePath !=null? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23"):"",
        ShopCategoryId = i.ShopCategoryId,
        ShopCategoryName = i.ShopCategoryName,
        List = GetBannerImageList(i.Id),
        Latitude = i.Latitude,
        Longitude = i.Longitude,
        Status = i.Status,
        isOnline = i.isOnline
    }).ToList();

                model.SuperMarketList = db.Shops.SqlQuery(querySuperMarketList,
    new SqlParameter("Latitude", Latitude),
    new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
    {
        Id = i.Id,
        Name = i.Name,
        DistrictName = i.StreetName,
        ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
        ShopCategoryId = i.ShopCategoryId,
        ShopCategoryName = i.ShopCategoryName,
        List = GetBannerImageList(i.Id),
        Latitude = i.Latitude,
        Longitude = i.Longitude,
        Status = i.Status,
        isOnline = i.isOnline
    }).ToList();

                model.GroceriesList = db.Shops.SqlQuery(queryGroceriesList,
        new SqlParameter("Latitude", Latitude),
        new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
        {
            Id = i.Id,
            Name = i.Name,
            DistrictName = i.StreetName,
            //Rating = RatingCalculation(i.Code),
            ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
            ShopCategoryId = i.ShopCategoryId,
            ShopCategoryName = i.ShopCategoryName,
            List = GetBannerImageList(i.Id),
            Latitude = i.Latitude,
            Longitude = i.Longitude,
            Status = i.Status,
            isOnline = i.isOnline
        }).ToList();

  
                model.HealthList = db.Shops.SqlQuery(queryHealthList,
        new SqlParameter("Latitude", Latitude),
        new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
        {
            Id = i.Id,
            Name = i.Name,
            DistrictName = i.StreetName,
            ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
            ShopCategoryId = i.ShopCategoryId,
            ShopCategoryName = i.ShopCategoryName,
            List = GetBannerImageList(i.Id),
            Latitude = i.Latitude,
            Longitude = i.Longitude,
            Status = i.Status,
            isOnline = i.isOnline
        }).ToList();

                model.ElectronicsList = db.Shops.SqlQuery(queryElectronicsList,
       new SqlParameter("Latitude", Latitude),
       new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
       {
           Id = i.Id,
           Name = i.Name,
           DistrictName = i.StreetName,
           //Rating = RatingCalculation(i.Code),
           ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
           ShopCategoryId = i.ShopCategoryId,
           ShopCategoryName = i.ShopCategoryName,
           List = GetBannerImageList(i.Id),
           Latitude = i.Latitude,
           Longitude = i.Longitude,
           Status = i.Status,
           isOnline = i.isOnline
       }).ToList();


                model.ServicesList = db.Shops.SqlQuery(qServicesList,
    new SqlParameter("Latitude", Latitude),
    new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
    {
        Id = i.Id,
        Name = i.Name,
        DistrictName = i.StreetName,
        //Rating = RatingCalculation(i.Code),
        ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
        ShopCategoryId = i.ShopCategoryId,
        ShopCategoryName = i.ShopCategoryName,
        List = GetBannerImageList(i.Id),
        Latitude = i.Latitude,
        Longitude = i.Longitude,
        Status = i.Status,
        isOnline = i.isOnline
    }).ToList();

    

                model.OtherList = db.Shops.SqlQuery(queryOtherList,
    new SqlParameter("Latitude", Latitude),
    new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
    {
        Id = i.Id,
        Name = i.Name,
        DistrictName = i.StreetName,
        ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
        ShopCategoryId = i.ShopCategoryId,
        ShopCategoryName = i.ShopCategoryName,
        Latitude = i.Latitude,
        Longitude = i.Longitude,
        List = GetBannerImageList(i.Id),
        Status = i.Status,
        isOnline = i.isOnline
    }).ToList();

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else if(a == "0"){
                model.OtherList = db.Shops.SqlQuery(query,
    new SqlParameter("Latitude", Latitude),
    new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
    {
        Id = i.Id,
        Name = i.Name,
        DistrictName = i.StreetName,
       // Rating = RatingCalculation(i.Code),
        ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
        ShopCategoryId = i.ShopCategoryId,
        ShopCategoryName = i.ShopCategoryName,
        List = GetBannerImageList(i.Id),
        Latitude = i.Latitude,
        Longitude = i.Longitude,
        Status = i.Status,
        isOnline = i.isOnline
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
       ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
       ShopCategoryId = i.ShopCategoryId,
       ShopCategoryName = i.ShopCategoryName,
       List = GetBannerImageList(i.Id),// db.Banners.Where(j => j.Status == 0 && j.ShopCode == i.Code).ToList(),
       Latitude = i.Latitude,
       Longitude = i.Longitude,
       Status = i.Status,
       isOnline = i.isOnline
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
          ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
          ShopCategoryId = i.ShopCategoryId,
          ShopCategoryName = i.ShopCategoryName,
          List = GetBannerImageList(i.Id),
          Latitude = i.Latitude,
          Longitude = i.Longitude,
          Status = i.Status,
          isOnline = i.isOnline
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
            ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
            ShopCategoryId = i.ShopCategoryId,
            ShopCategoryName = i.ShopCategoryName,
            List = GetBannerImageList(i.Id),
            Latitude = i.Latitude,
            Longitude = i.Longitude,
            Status = i.Status,
            isOnline = i.isOnline
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
                    ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
                    ShopCategoryId = i.ShopCategoryId,
                ShopCategoryName = i.ShopCategoryName,
                    List = GetBannerImageList(i.Id),
                    Latitude = i.Latitude,
                Longitude = i.Longitude,
                Status = i.Status,
                isOnline = i.isOnline
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
        ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",

        ShopCategoryId = i.ShopCategoryId,
        ShopCategoryName = i.ShopCategoryName,
        List = GetBannerImageList(i.Id),
        Latitude = i.Latitude,
        Longitude = i.Longitude,
        Status = i.Status,
        isOnline = i.isOnline
    }).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string queryOtherList = "SELECT top(6) * " +
              " FROM Shops where(3959 * acos(cos(radians(@Latitude)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@Longitude)) + sin(radians(@Latitude)) * sin(radians(Latitude)))) < 8 and ShopCategoryCode = '6' and (Status = 0 or  Status = 6) and Latitude != 0 and Longitude != 0 " +
              " order by Rating";
    
                model.OtherList = db.Shops.SqlQuery(queryOtherList,
    new SqlParameter("Latitude", Latitude),
    new SqlParameter("Longitude", Longitude)).Select(i => new PlacesListView.Places
    {
        Id = i.Id,
        Name = i.Name,
        DistrictName = i.StreetName,
        //Rating = RatingCalculation(i.Code),
        ImagePath = i.ImagePath != null ? i.ImagePath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23") : "",
        ShopCategoryId = i.ShopCategoryId,
        ShopCategoryName = i.ShopCategoryName,
        List = GetBannerImageList(i.Id),
        Latitude = i.Latitude,
        Longitude = i.Longitude,
        Status = i.Status,
        isOnline=i.isOnline
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
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                         .Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.pay.c.Shopid == shopId && i.pay.c.DateEncoded.ToString("dd-MMM-yyyy") == dt)
                   .Select(i => new ShopOrderAmountApiViewModel.CartList
                   {

                       OrderNo = i.pay.c.OrderNumber,
                       CartStatus = i.pay.c.Status,
                       ShopPaymentStatus = i.pay.c.ShopPaymentStatus,
                       Amount = i.pay.p.OriginalAmount.ToString(),
                       Date = i.pay.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
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
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                         .Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.pay.c.Shopid == shopId && i.pay.c.DateEncoded >= from1 && i.pay.c.DateEncoded <= to1)
                   .Select(i => new ShopOrderAmountApiViewModel.CartList
                   {
                       OrderNo = i.pay.c.OrderNumber,
                       CartStatus = i.pay.c.Status,
                       ShopPaymentStatus = i.pay.c.ShopPaymentStatus,
                       Amount = i.pay.p.OriginalAmount.ToString(),
                       Date = i.pay.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
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
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                         .Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.pay.c.Shopid == shopId && i.pay.c.DateEncoded.ToString("dd-MMM-yyyy") == dt)
                   .Select(i => new ShopOrderAmountApiViewModel.CartList
                   {
                       OrderNo = i.pay.c.OrderNumber,
                       CartStatus = i.pay.c.Status,
                       ShopPaymentStatus = i.pay.c.ShopPaymentStatus,
                       Amount = i.pay.p.OriginalAmount.ToString(),
                       Date = i.pay.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
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
                 .Join(db.Payments, c => c.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                      .Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                  .AsEnumerable()
                .Where(i => i.pay.c.DeliveryBoyPhoneNumber == phoneNumber && i.pay.p.PaymentMode != "Online Payment" && i.pay.c.DeliveryOrderPaymentStatus == 0)
                .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                {
                    Amount = i.pay.p.Amount.ToString(),
                    
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
                         .Join(db.ShopCharges, c => c.OrderNumber, sc => sc.OrderNo, (c, sc) => new { c, sc })
                            .Join(db.Shops, scc => scc.c.Shopid, s => s.Id, (scc, s) => new { scc, s })
                     .AsEnumerable()
                   .Where(i => i.scc.c.DeliveryBoyPhoneNumber == phoneNumber && i.scc.c.DateEncoded.ToString("dd-MMM-yyyy") == dt)
                   .Select(i => new DelivaryBoyReportViewModel.CartList
                   {
                       OrderNo = i.scc.c.OrderNumber,
                       CartStatus = i.scc.c.Status,
                       GrossDeliveryCharge = i.scc.sc.GrossDeliveryCharge,
                       CustomerLatitude =  i.scc.c.Latitude,
                       CustomerLongitude = i.scc.c.Longitude,
                       ShopLatitude = i.s.Latitude,
                       ShopLongitude = i.s.Longitude,
                       DateEncoded = i.scc.c.DateEncoded,
                       Date = i.scc.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
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
                         .Join(db.ShopCharges, c => c.OrderNumber, sc => sc.OrderNo, (c, sc) => new { c, sc })
                            .Join(db.Shops, scc => scc.c.Shopid, s => s.Id, (scc, s) => new { scc, s })
                          .Where(i => ((DbFunctions.TruncateTime(i.scc.c.DateEncoded) >= DbFunctions.TruncateTime(from1)) &&
            (DbFunctions.TruncateTime(i.scc.c.DateEncoded) <= DbFunctions.TruncateTime(to1))))
                          .AsEnumerable()
                   .Select(i => new DelivaryBoyReportViewModel.CartList
                   {

                       OrderNo = i.scc.c.OrderNumber,
                       CartStatus = i.scc.c.Status,
                       GrossDeliveryCharge = i.scc.sc.GrossDeliveryCharge,
                       CustomerLatitude = i.scc.c.Latitude,
                       CustomerLongitude = i.scc.c.Longitude,
                       ShopLatitude = i.s.Latitude,
                       ShopLongitude = i.s.Longitude,
                       DateEncoded = i.scc.c.DateEncoded,
                       Date = i.scc.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
                   }).OrderByDescending(j => j.DateEncoded).ToList();
                if (model.List.Count() != 0)
                {
                    model.EarningOfToday = model.List.Sum(i => i.GrossDeliveryCharge);
                }
            }
            else
            {
                model.List = db.Orders.Where(i => i.Status == 6)
                         .Join(db.ShopCharges, c => c.OrderNumber, sc => sc.OrderNo, (c, sc) => new { c, sc })
                            .Join(db.Shops, scc => scc.c.Shopid, s => s.Id, (scc, s) => new { scc, s })
                     .AsEnumerable()
                   .Where(i => i.scc.c.DeliveryBoyPhoneNumber == phoneNumber && i.scc.c.DateEncoded.ToString("dd-MMM-yyyy") == dt)
                   .Select(i => new DelivaryBoyReportViewModel.CartList
                   {
                       OrderNo = i.scc.c.OrderNumber,
                       CartStatus = i.scc.c.Status,
                       GrossDeliveryCharge = i.scc.sc.GrossDeliveryCharge,
                       CustomerLatitude = i.scc.c.Latitude,
                       CustomerLongitude = i.scc.c.Longitude,
                       ShopLatitude = i.s.Latitude,
                       ShopLongitude = i.s.Longitude,
                       DateEncoded = i.scc.c.DateEncoded,
                       Date = i.scc.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
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
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                         .Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.pay.c.DeliveryBoyPhoneNumber == phoneNumber && i.pay.c.DateEncoded.ToString("dd-MMM-yyyy") == dt && i.pay.p.PaymentMode != "Online Payment" && i.pay.c.DeliveryOrderPaymentStatus == 0)
                   .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                   {
                       OrderNo = i.pay.c.OrderNumber,
                       CartStatus = i.pay.c.Status,
                       GrossDeliveryCharge = i.sc.GrossDeliveryCharge,
                       DeliveryBoyPaymentStatus = i.pay.c.DeliveryBoyPaymentStatus,
                       Amount = i.pay.p.Amount.ToString(),
                       Date = i.pay.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
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
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                         .Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                     .AsEnumerable()
                   .Where(i => i.pay.c.DeliveryBoyPhoneNumber == phoneNumber && i.pay.p.PaymentMode != "Online Payment" && i.pay.c.DateEncoded >= from1 && i.pay.c.DateEncoded <= to1 && i.pay.c.DeliveryOrderPaymentStatus == 0)
                   .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                   {
                       OrderNo = i.pay.c.OrderNumber,
                       CartStatus = i.pay.c.Status,
                       GrossDeliveryCharge = i.sc.GrossDeliveryCharge,
                       DeliveryBoyPaymentStatus = i.pay.c.DeliveryBoyPaymentStatus,
                       Amount = i.pay.p.Amount.ToString(),
                       Date = i.pay.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
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
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNo, (c, p) => new { c, p })
                         .Join(db.ShopCharges, pay => pay.p.OrderNo, sc => sc.OrderNo, (pay, sc) => new { pay, sc })
                    .AsEnumerable()
                   .Where(i => i.pay.c.DeliveryBoyPhoneNumber == phoneNumber && i.pay.c.DateEncoded.ToString("dd-MMM-yyyy") == dt && i.pay.p.PaymentMode != "Online Payment" && i.pay.c.DeliveryOrderPaymentStatus == 0)
                   .Select(i => new DelivaryCreditAmountApiViewModel.CartList
                   {
                       OrderNo = i.pay.c.OrderNumber,
                       CartStatus = i.pay.c.Status,
                       GrossDeliveryCharge = i.sc.GrossDeliveryCharge,
                       DeliveryBoyPaymentStatus = i.pay.c.DeliveryBoyPaymentStatus,
                       Amount = i.pay.p.Amount.ToString(),
                       Date = i.pay.c.DateEncoded.ToString("dd-MMM-yyyy HH:ss")
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
        public void NotificationMessage(string msg, string tag,string token)
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

        List<OrderItem> GetOrderList(int orderId)
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
              
                return  db.Banners.Where(j => j.Status == 0 && j.ShopId == shopId).ToList();
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
            where (s.Status== 0 || s.Status == 6) && s.ShopId == id && (DbFunctions.TruncateTime(s.Fromdate) <= DbFunctions.TruncateTime(DateTime.Now) && DbFunctions.TruncateTime(s.Todate) >= DbFunctions.TruncateTime(DateTime.Now))
            select  new BannerImages { Bannerpath = (s.Bannerpath !=null)?s.Bannerpath.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23"):"", ShopId = s.ShopId, ProductName = s.MasterProductName, ShopName = s.ShopName, ProductId = s.MasterProductId }).ToList();
          
                return teenStudentsName;
            }
            catch(Exception ex)
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
        Double RatingCalculation(string code)
        {
            Double rating = 0;
            var ratingCount = db.CustomerReviews.Where(i => i.ShopCode == code).Count();
           var ratingSum= (from ss in db.CustomerReviews
                           where ss.ShopCode == code 
                          select(Double ?)ss.Rating).Sum() ?? 0;
            if (ratingCount == 0)
                ratingCount = 1;
            rating = (ratingSum * 5) / (ratingCount * 5);
            return Math.Round(rating,1);
        }

        Models.Payment GetPayment(int code)
        {
            try
            {

                return db.Payments.FirstOrDefault(i => i.OrderNo == code);
            }
            catch
            {
                return (Models.Payment)null;
            }
        }
        #endregion


        public JsonResult GetDeliveryBoyPayout(DateTime startDate, DateTime endDate,string phoneNo, int page = 1, int pageSize = 5)
        {
            //DelivaryBoyPayoutReportViewModel
            var model = new DelivaryBoyPayoutReportViewModel();
            model.List = db.ShopCharges
                .Where(i => ((DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(startDate)) &&
            (DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(endDate))) && i.Status == 0)
            .Join(db.Orders.Where(i=>i.Status ==6), sc => sc.OrderNo, c => c.OrderNumber, (sc, c) => new { sc, c })
            .Join(db.DeliveryBoys.Where(i => i.PhoneNumber == phoneNo), c => c.c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
            .GroupBy(i => DbFunctions.TruncateTime(i.c.sc.DateEncoded))
            .AsEnumerable()
            .Select(i => new DelivaryBoyPayoutReportViewModel.PayoutOut
            {
                Date = i.Any() ? i.FirstOrDefault().c.sc.DateEncoded.ToString("dd-MMM-yyyy HH:ss") : "",
                date = i.FirstOrDefault().c.sc.DateEncoded,
                totalamount = i.Sum(a=>a.c.sc.GrossDeliveryCharge),
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
            var list = db.ShopCharges.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(dateEncoded) && i.Status == 0)
           .Join(db.Orders.Where(i => i.Status == 6 && i.DeliveryBoyPaymentStatus == 1), sc => sc.OrderNo, c => c.OrderNumber, (sc, c) => new { sc, c })
           .Join(db.DeliveryBoys.Where(i => i.PhoneNumber == phoneNo), c => c.c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
           .GroupBy(i=> DbFunctions.TruncateTime(dateEncoded))
           .Select(i => new
           {
               amount = i.Any() ? i.Sum(a => a.c.sc.GrossDeliveryCharge) : 0
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
                 .Join(db.Orders.Where(i => i.Status == 6), p => p.p.OrderNo, c => c.OrderNumber, (p, c) => new { p, c })
                 .GroupBy(i => DbFunctions.TruncateTime(i.p.p.DateEncoded))
                 .AsEnumerable()
                 .Select(i=> new ShopApiReportsViewModel.EarningListItem
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
                .Join(db.Orders .Where(i => i.Status == 6), p => p.p.OrderNo, c => c.OrderNumber, (p, c) => new { p, c })
                .Join(db.ShopCharges.Where(i => i.Status == 0), p => p.p.p.OrderNo, sc => sc.OrderNo, (p, sc) => new { p, sc })
                .AsEnumerable()
                .Select(i => new ShopApiReportsViewModel.RefundListItem
                {
                    Date = i.p.p.p.DateEncoded.ToString("dd-MMM-yyyy HH:ss"),
                    DateEncoded = i.p.p.p.DateEncoded,
                    Earning = i.p.p.p.Amount,
                    Refund = i.p.p.p.refundAmount ??0,
                    DeliveryCredits = i.sc.GrossDeliveryCharge,
                    OrderNo = i.p.p.p.OrderNo
                }).OrderByDescending(i => i.DateEncoded).ToList();
                return Json(model.RefundLists, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public double GetShopPaidAmount(DateTime dateEncoded, int shopId)
        {
            var list = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(dateEncoded))
                .Join(db.Shops.Where(i => i.Id == shopId), p => p.ShopId, s => s.Id, (p, s) => new { p, s })
            .Join(db.Orders.Where(i => i.Status == 6 && i.ShopPaymentStatus ==1), p => p.p.OrderNo, c => c.OrderNumber, (p, c) => new { p, c })
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


    }
}