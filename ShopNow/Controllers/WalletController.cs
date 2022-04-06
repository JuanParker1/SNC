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
                    var medicalCustomerlist = GetCustomerList(4);
                    SaveWalletHistory(model.CustomerGroup, medicalCustomerlist, model.Amount, model.ExpiryDate);
                    break;
                case 2:
                    var groceryCustomerlist = GetCustomerList(2);
                    SaveWalletHistory(model.CustomerGroup, groceryCustomerlist, model.Amount, model.ExpiryDate);
                    break;
                case 3:
                    var restaurantCustomerlist = GetCustomerList(1);
                    SaveWalletHistory(model.CustomerGroup, restaurantCustomerlist, model.Amount, model.ExpiryDate);
                    break;
                case 4:
                    var supermarketCustomerlist = GetCustomerList(3);
                    SaveWalletHistory(model.CustomerGroup, supermarketCustomerlist, model.Amount, model.ExpiryDate);
                    break;
                case 5: //No Order
                    var customer = db.Customers.Select(i=>i.Id).ToList();
                    var order = db.Orders.Where(i => i.Status == 6).Select(i=>i.CustomerId).ToList();
                    var result = customer.Where(i => !order.Select(a => a).ToArray().Contains(i)).OrderBy(i => i)
                .GroupBy(i => i).Select(i => i.FirstOrDefault()).ToList();
                    SaveWalletHistory(model.CustomerGroup, result, model.Amount, model.ExpiryDate);
                    break;
                case 6: //No Order Last 10days
                    var allcustomer = db.Customers.Select(i => i.Id).ToList();
                    var orderLast10days = db.Orders.Where(i => (DbFunctions.TruncateTime(DbFunctions.AddDays(DateTime.Now, -10)) <= DbFunctions.TruncateTime(i.DateEncoded)) && i.Status == 6).Select(i => i.CustomerId).ToList();
                    var result1 = allcustomer.Where(i => !orderLast10days.Select(a => a).ToArray().Contains(i)).OrderBy(i => i)
                .GroupBy(i => i).Select(i => i.FirstOrDefault()) .ToList();
                    SaveWalletHistory(model.CustomerGroup, result1, model.Amount, model.ExpiryDate);
                    break;
                case 7:
                    var medicalLast10DaysCustomerlist = GetCustomerList(4,-10);
                    SaveWalletHistory(model.CustomerGroup, medicalLast10DaysCustomerlist, model.Amount, model.ExpiryDate);
                    break;
                case 8:
                    var groceryLast10DaysCustomerlist = GetCustomerList(2,-10);
                    SaveWalletHistory(model.CustomerGroup, groceryLast10DaysCustomerlist, model.Amount, model.ExpiryDate);
                    break;
                case 9:
                    var restaurantLast10DaysCustomerlist = GetCustomerList(1,-10);
                    SaveWalletHistory(model.CustomerGroup, restaurantLast10DaysCustomerlist, model.Amount, model.ExpiryDate);
                    break;
                case 10:
                    var supermarketLast10DaysCustomerlist = GetCustomerList(3,-10);
                    SaveWalletHistory(model.CustomerGroup, supermarketLast10DaysCustomerlist, model.Amount, model.ExpiryDate);
                    break;
                default:
                    break;
            }
            return RedirectToAction("Index");
        }

        public List<int> GetCustomerList(int shopCategoryId,int lastdays=0)
        {
            var allOrders = db.Orders.Where(i => i.Status == 6 && lastdays !=0 ? (DbFunctions.TruncateTime(DbFunctions.AddDays(DateTime.Now, lastdays)) <= DbFunctions.TruncateTime(i.DateEncoded)):true)
                      .GroupBy(i => new { i.ShopId, i.CustomerId }).Select(i => new { ShopId = i.FirstOrDefault().ShopId, CustomerId = i.FirstOrDefault().CustomerId })
              .Join(db.Shops.Where(i => i.ShopCategoryId == shopCategoryId).Select(i => new { Id = i.Id, ShopCategoryId = i.ShopCategoryId }), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
              .Select(i => new { CustomerId = i.o.CustomerId, ShopId = i.s.Id, ShopCatId = i.s.ShopCategoryId }).ToList();

            var otherOrders = db.Orders.Where(i => i.Status == 6 && lastdays != 0 ? (DbFunctions.TruncateTime(DbFunctions.AddDays(DateTime.Now, lastdays)) <= DbFunctions.TruncateTime(i.DateEncoded)) : true)
               .GroupBy(i => new { i.ShopId, i.CustomerId }).Select(i => new { ShopId = i.FirstOrDefault().ShopId, CustomerId = i.FirstOrDefault().CustomerId })
               .Join(db.Shops.Where(i => i.ShopCategoryId != shopCategoryId).Select(i => new { Id = i.Id, ShopCategoryId = i.ShopCategoryId }), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
               .Select(i => new { CustomerId = i.o.CustomerId, ShopId = i.s.Id, ShopCatId = i.s.ShopCategoryId }).ToList();

            var result = allOrders.Where(i => !otherOrders.Select(a => a.CustomerId).ToArray().Contains(i.CustomerId)).OrderBy(i => i.CustomerId)
                .GroupBy(i => i.CustomerId).Select(i => i.FirstOrDefault().CustomerId)
                .ToList();
            return result;
        }

        public void SaveWalletHistory(int customerGroup, List<int> customerIds,double amount,DateTime? expiryDate)
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
                Status = 0
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
                    Description = $"Received from Snowch(#{wallet.Id})",
                    ExpiryDate = expiryDate,
                    Type = 1
                };
                db.CustomerWalletHistories.Add(customerWalletHistory);
                db.SaveChanges();
            }
            
            var fcmTokenList = db.Customers.Where(i=>customerIds.Contains(i.Id)).OrderBy(i => i.Id).Where(i => !string.IsNullOrEmpty(i.FcmTocken) && i.FcmTocken != "NULL").Select(i => i.FcmTocken).ToArray();
            var count = Math.Ceiling((double)fcmTokenList.Count() / 1000);
            var message = "";
            if (expiryDate != null)
                message = $"You have Received Rs.{wallet.Amount} from Snowch. Hurry up it will expire at {expiryDate.Value.ToString("dd-MMM-yyyy")}.";
            else
                message = $"You have Received Rs.{wallet.Amount} from Snowch.";
            for (int i = 0; i < count; i++)
            {
                Helpers.PushNotification.SendBulk(message, "Wallet amount Received", "SpecialOffer", "", fcmTokenList.Skip(i * 1000).Take(1000).ToArray(), "tune2.caf");
            }
        }
    }
}