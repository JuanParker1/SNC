using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class WalletController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "SNCWAI342")]
        public ActionResult Index()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new WalletIndexViewModel();
            model.ListItems = db.Wallets.Where(i => i.Status == 0)
                .Select(i => new WalletIndexViewModel.ListItem
                {
                    Amount = i.Amount,
                    CustomerGroup = i.CustomerGroup,
                    DateEncoded = i.DateEncoded,
                    Encodedby = i.EncodedBy,
                    ExpiryDate = i.ExpiryDate
                }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult SendWalletAmount(WalletSendAmountViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            switch (model.CustomerGroup)
            {
                case 1:
                    var medicalCustomerlist = GetCustomerList(4, model.DateStart, model.DateEnd, 0, model.Month);
                    //SaveWalletHistory(model.CustomerGroup, medicalCustomerlist, model.Amount, model.ExpiryDate,model.Description,model.ReferenceCode,model.Month);
                    break;
                case 2:
                    var groceryCustomerlist = GetCustomerList(2, model.DateStart, model.DateEnd, 0, model.Month);
                    SaveWalletHistory(model.CustomerGroup, groceryCustomerlist, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 3:
                    var restaurantCustomerlist = GetCustomerList(1, model.DateStart, model.DateEnd, 0, model.Month);
                    SaveWalletHistory(model.CustomerGroup, restaurantCustomerlist, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 4:
                    var supermarketCustomerlist = GetCustomerList(3, model.DateStart, model.DateEnd, 0, model.Month);
                    SaveWalletHistory(model.CustomerGroup, supermarketCustomerlist, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 5: //No Order
                    var customer = db.Customers.Select(i => i.Id).ToList();
                    var order = db.Orders.Where(i => i.Status == 6 && (model.Month != 0 ? i.DateEncoded.Month == model.Month : true) && ((model.DateStart != null && model.DateEnd != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.DateStart.Value) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.DateEnd.Value)) : true)).Select(i => i.CustomerId).ToList();
                    var result = customer.Where(i => !order.Select(a => a).ToArray().Contains(i)).OrderBy(i => i)
                .GroupBy(i => i).Select(i => i.FirstOrDefault()).ToList();
                    SaveWalletHistory(model.CustomerGroup, result, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 6: //No Order Last 10days
                    var allcustomer = db.Customers.Select(i => i.Id).ToList();
                    var orderLast10days = db.Orders.Where(i => (DbFunctions.TruncateTime(DbFunctions.AddDays(DateTime.Now, -10)) <= DbFunctions.TruncateTime(i.DateEncoded)) && i.Status == 6).Select(i => i.CustomerId).ToList();
                    var result1 = allcustomer.Where(i => !orderLast10days.Select(a => a).ToArray().Contains(i)).OrderBy(i => i)
                .GroupBy(i => i).Select(i => i.FirstOrDefault()).ToList();
                    SaveWalletHistory(model.CustomerGroup, result1, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 7:
                    var medicalLast10DaysCustomerlist = GetCustomerList(4, model.DateStart, model.DateEnd, -10, 0);
                    SaveWalletHistory(model.CustomerGroup, medicalLast10DaysCustomerlist, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 8:
                    var groceryLast10DaysCustomerlist = GetCustomerList(2, model.DateStart, model.DateEnd, -10, 0);
                    SaveWalletHistory(model.CustomerGroup, groceryLast10DaysCustomerlist, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 9:
                    var restaurantLast10DaysCustomerlist = GetCustomerList(1, model.DateStart, model.DateEnd, -10, 0);
                    SaveWalletHistory(model.CustomerGroup, restaurantLast10DaysCustomerlist, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 10:
                    var supermarketLast10DaysCustomerlist = GetCustomerList(3, model.DateStart, model.DateEnd, -10, 0);
                    SaveWalletHistory(model.CustomerGroup, supermarketLast10DaysCustomerlist, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 11:
                    var iosCustomer = db.CustomerDeviceInfoes.Where(i => i.Platform == "ios")
                        .Join(db.Customers.Where(i => i.Status == 0 && ((model.DateStart != null && model.DateEnd != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.DateStart.Value) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.DateEnd.Value)) : true)), cd => cd.CustomerId, c => c.Id, (cd, c) => new { c, cd })
                        .Join(db.CustomerAddresses.GroupBy(i => i.CustomerId), cd => cd.c.Id, ca => ca.FirstOrDefault().CustomerId, (cd, ca) => new { cd, ca })
                        .Select(i => i.cd.cd.CustomerId).ToList();
                    SaveWalletHistory(model.CustomerGroup, iosCustomer, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 12:
                    var androidCustomer = db.CustomerDeviceInfoes.Where(i => i.Platform == "android")
                        .Join(db.Customers.Where(i => i.Status == 0 && ((model.DateStart != null && model.DateEnd != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.DateStart.Value) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.DateEnd.Value)) : true)), cd => cd.CustomerId, c => c.Id, (cd, c) => new { c, cd })
                        .Join(db.CustomerAddresses.GroupBy(i => i.CustomerId), cd => cd.c.Id, ca => ca.FirstOrDefault().CustomerId, (cd, ca) => new { cd, ca })
                        .Select(i => i.cd.cd.CustomerId).ToList();
                    SaveWalletHistory(model.CustomerGroup, androidCustomer, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                case 13:
                    var allNewCustomer = db.Customers.Where(i => i.Status == 0 && ((model.DateStart != null && model.DateEnd != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.DateStart.Value) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.DateEnd.Value)) : true))
                       .Join(db.CustomerAddresses.GroupBy(i => i.CustomerId), c => c.Id, ca => ca.FirstOrDefault().CustomerId, (c, ca) => new { c, ca })
                       .Select(i => i.c.Id).ToList();
                    SaveWalletHistory(model.CustomerGroup, allNewCustomer, model.Amount, model.ExpiryDate, model.Description, model.ReferenceCode, model.Month);
                    break;
                default:
                    break;
            }
            return RedirectToAction("Index");
        }

        public List<int> GetCustomerList(int shopCategoryId, DateTime? startDate, DateTime? endDate, int lastdays = 0, int month = 0)
        {
            var allOrders = db.Orders.Where(i => i.Status == 6 && (lastdays != 0 ? (DbFunctions.TruncateTime(DbFunctions.AddDays(DateTime.Now, lastdays)) <= DbFunctions.TruncateTime(i.DateEncoded)) : true) && (month != 0 ? i.DateEncoded.Month == month : true) && ((startDate != null && endDate != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(startDate.Value) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(endDate.Value)) : true))
                      .GroupBy(i => new { i.ShopId, i.CustomerId }).Select(i => new { ShopId = i.FirstOrDefault().ShopId, CustomerId = i.FirstOrDefault().CustomerId })
              .Join(db.Shops.Where(i => i.ShopCategoryId == shopCategoryId).Select(i => new { Id = i.Id, ShopCategoryId = i.ShopCategoryId }), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
              .Select(i => new { CustomerId = i.o.CustomerId, ShopId = i.s.Id, ShopCatId = i.s.ShopCategoryId }).ToList();

            var otherOrders = db.Orders.Where(i => i.Status == 6 && lastdays != 0 ? (DbFunctions.TruncateTime(DbFunctions.AddDays(DateTime.Now, lastdays)) <= DbFunctions.TruncateTime(i.DateEncoded)) : true && (month != 0 ? i.DateEncoded.Month == month : true) && ((startDate != null && endDate != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(startDate.Value) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(endDate.Value)) : true))
               .GroupBy(i => new { i.ShopId, i.CustomerId }).Select(i => new { ShopId = i.FirstOrDefault().ShopId, CustomerId = i.FirstOrDefault().CustomerId })
               .Join(db.Shops.Where(i => i.ShopCategoryId != shopCategoryId).Select(i => new { Id = i.Id, ShopCategoryId = i.ShopCategoryId }), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
               .Select(i => new { CustomerId = i.o.CustomerId, ShopId = i.s.Id, ShopCatId = i.s.ShopCategoryId }).ToList();

            var result = allOrders.Where(i => !otherOrders.Select(a => a.CustomerId).ToArray().Contains(i.CustomerId)).OrderBy(i => i.CustomerId)
                .GroupBy(i => i.CustomerId).Select(i => i.FirstOrDefault().CustomerId)
                .ToList();
            return result;
        }

        public void SaveWalletHistory(int customerGroup, List<int> customerIds, double amount, DateTime? expiryDate, string description, string reference, int month)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;

            var wallet = new Wallet
            {
                Amount = amount,
                CustomerGroup = customerGroup,
                DateEncoded = DateTime.Now,
                EncodedBy = user.Name,
                ExpiryDate = expiryDate,
                Status = 0,
                Description = description,
                Month = month,
                ReferenceCode = reference
            };
            db.Wallets.Add(wallet);
            db.SaveChanges();

            foreach (int id in customerIds)
            {
                var customerWalletHistory = new CustomerWalletHistory
                {
                    Amount = amount,
                    CustomerId = id,
                    DateEncoded = DateTime.Now,
                    Description = wallet.ReferenceCode,
                    ExpiryDate = expiryDate,
                    Type = 1,
                    Status = 0,
                    WalletId = wallet.Id
                };
                db.CustomerWalletHistories.Add(customerWalletHistory);
                db.SaveChanges();
            }

            var customerList = db.Customers.Where(i => customerIds.Contains(i.Id)).OrderBy(i => i.Id).Select(i => new { Id = i.Id, FcmToken = i.FcmTocken, Name = i.Name }).ToList();
            //var fcmTokenList = customerList.Select(i => i.FcmToken).ToArray();
            //var count = Math.Ceiling((double)fcmTokenList.Count() / 1000);
            //var message = "";
            //if (expiryDate != null)
            //    message = $"Hi, Rs.{wallet.Amount} has been added to your wallet. (With Expiry - {expiryDate.Value.ToString("dd-MMM-yyyy")}). Happy Shopping.";
            //else
            //    message = $"Hi, Rs.{wallet.Amount} has been added to your wallet. Happy Shopping.";
            //for (int i = 0; i < count; i++)
            //{
            //    Helpers.PushNotification.SendBulk(message, "Wallet Amount Received", "SpecialOffer", "", fcmTokenList.Skip(i * 1000).Take(1000).ToArray(), "tune2.caf");
            //}
            var message = "";
            foreach (var item in customerList)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == item.Id);
                customer.WalletAmount += wallet.Amount;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();

                if (!string.IsNullOrEmpty(item.Name) && item.Name != "Null")
                    message = $"Hi {item.Name}, Rs.{wallet.Amount} 💵 has been added to your wallet. (Expires 🗓️ {expiryDate.Value.ToString("dd-MMM-yyyy")}). Happy Shopping 😎.";
                else
                    message = $"Hi, Rs.{wallet.Amount} 💵 has been added to your wallet. (Expires 🗓️ {expiryDate.Value.ToString("dd-MMM-yyyy")}). Happy Shopping 😎.";

                if (!string.IsNullOrEmpty(item.FcmToken) && item.FcmToken != "NULL")
                    Helpers.PushNotification.SendbydeviceId(message, $"You have won Rs.{wallet.Amount} 💵 in wallet.", "SpecialOffer", "", item.FcmToken, "tune2.caf", "mywallet");
            }
        }

        [AccessPolicy(PageCode = "SNCWDR343")]
        public ActionResult DispatchReport(WalletDispatchReportViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Customers
                .Join(db.CustomerWalletHistories.Where(i => i.Status == 0 && i.Type == 1 && ((model.StartDate != null && model.EndDate != null) ? DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate) : true)), c => c.Id, cw => cw.CustomerId, (c, cw) => new { c, cw })
                .GroupJoin(db.Wallets, c => c.cw.WalletId, w => w.Id, (c, w) => new { c, w })
                .AsEnumerable()
                .Select(i => new WalletDispatchReportViewModel.ListItem
                {
                    CustomerName = i.c.c.Name,
                    CustomerPhoneNumber = i.c.c.PhoneNumber,
                    DateEncoded = i.c.cw.DateEncoded,
                    Description = i.c.cw.Description,
                    ExpiryDate = i.c.cw.ExpiryDate,
                    Title = i.w.Any() ? i.w.FirstOrDefault().ReferenceCode : "N/A",
                    //TotalWalletBalance = i.c.cw.Amount - GetWalletAmountUsed(i.c.cw.DateEncoded, i.c.cw.ExpiryDate, i.c.c.Id),
                    TotalWalletBalance = i.c.cw.Amount - (GetWalletAmountUsed(i.c.cw.DateEncoded, i.c.cw.ExpiryDate, i.c.c.Id) < i.c.cw.Amount ? GetWalletAmountUsed(i.c.cw.DateEncoded, i.c.cw.ExpiryDate, i.c.c.Id) : i.c.cw.Amount),
                    WalletAmountSent = i.c.cw.Amount,
                    WalletAmountUsed = GetWalletAmountUsed(i.c.cw.DateEncoded, i.c.cw.ExpiryDate, i.c.c.Id) < i.c.cw.Amount ? GetWalletAmountUsed(i.c.cw.DateEncoded, i.c.cw.ExpiryDate, i.c.c.Id) : i.c.cw.Amount
                }).OrderBy(i => i.DateEncoded).ToList();
            int counter = 1;
            model.ListItems.ForEach(x => x.No = counter++);
            return View(model);
        }

        public double GetWalletAmountSent(int id)
        {
            var customerWallet = db.CustomerWalletHistories.FirstOrDefault(i => i.Id == id && i.Type == 1);
            return customerWallet != null ? customerWallet.Amount : 0;
        }

        public double GetWalletAmountUsed(DateTime? startDate, DateTime? endDate, int customerId)
        {
            var orders = db.Orders.Where(i => i.CustomerId == customerId && i.Status == 6 && ((startDate != null && endDate != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(startDate.Value) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(endDate.Value)) : true))
               .Select(i => i.WalletAmount).ToList();
            return orders != null ? orders.Sum(i => i) : 0;
        }
    }
}