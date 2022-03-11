using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class CustomerGiftCardController : Controller
    {
        private sncEntities db = new sncEntities();

        private const string _prefix = "";
        private static string _referenceCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }

        [AccessPolicy(PageCode = "SNCCGL306")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CustomerGiftCardListViewModel();
            model.ListItems = db.CustomerGiftCards.Where(i => i.Status !=2).Select(i => new CustomerGiftCardListViewModel.ListItem
            {
                Amount = i.Amount,
                CustomerId = i.CustomerId,
                CustomerPhoneNumber = i.CustomerPhoneNumber,
                ExpiryDate = i.ExpiryDate,
                GiftCardCode = i.GiftCardCode,
                Id = i.Id,
                Status = i.Status
            }).ToList();
            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCCGA307")]
        public ActionResult Add(CustomerGiftCardAddViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var giftCard = new CustomerGiftCard
            {
                Amount = model.Amount,
                CreatedBy = user.Name,
                CustomerId = model.CustomerId,
                CustomerPhoneNumber = string.IsNullOrEmpty(model.CustomerPhoneNumber) ? model.NotCustomerPhoneNumber : model.CustomerPhoneNumber,
                ChannelPartnerNumber = model.ChannelPartnerNumber,
                DateEncoded = DateTime.Now,
                GiftCardCode = Helpers.DRC.GenerateGiftCard("SNC-"),
                ReferenceCode = _referenceCode,
                ExpiryDate = model.ExpiryDate,
                Status = 0
            };
            db.CustomerGiftCards.Add(giftCard);
            db.SaveChanges();

            var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == giftCard.CustomerPhoneNumber);
            string msg = "";
            if (!string.IsNullOrEmpty(customer.Name) || customer.Name.ToLower() != "null")
                msg = $"Hi Dear {customer.Name}, your Snowch E-Gift Card details,Reference {giftCard.ReferenceCode}, Gift Card Code: {giftCard.GiftCardCode}, Expiry Date: {giftCard.ExpiryDate.ToString("dd-MMM-yyyy")}, Amount: {giftCard.Amount.ToString()}. T&C apply. http://playstore.snowch.in - Joyra";
            else
                msg = $"Hi Dear, your Snowch E-Gift Card details,Reference {giftCard.ReferenceCode}, Gift Card Code: {giftCard.GiftCardCode}, Expiry Date: {giftCard.ExpiryDate.ToString("dd-MMM-yyyy")}, Amount: {giftCard.Amount.ToString()}. T&C apply. Link <a href='http://playstore.snowch.in'>http://playstore.snowch.in</a> - Joyra";

            //Send exotel Message
            //Customer
            string from = "04448134440";
            SendSMS.execute(from, giftCard.CustomerPhoneNumber, msg);
            //Channel Partner
            if (!string.IsNullOrEmpty(giftCard.ChannelPartnerNumber))
                SendSMS.execute(from, model.ChannelPartnerNumber, msg);

            return RedirectToAction("List");
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCCGE308")]
        public ActionResult Edit(CustomerGiftCardEditViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var customerGiftCard = db.CustomerGiftCards.FirstOrDefault(i=>i.Id == model.EditId);
            if(customerGiftCard != null)
            {
                customerGiftCard.CustomerPhoneNumber = model.EditCustomerPhoneNumber;
                customerGiftCard.CustomerId = model.EditCustomerId;
                customerGiftCard.Amount = model.EditAmount;
                customerGiftCard.ExpiryDate = model.EditExpiryDate;
                db.Entry(customerGiftCard).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SNCCGD309")]
        public JsonResult Delete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var custGiftCard = db.CustomerGiftCards.FirstOrDefault(i => i.Id == id);
            if (custGiftCard != null)
            {
                custGiftCard.Status = 2;
                db.Entry(custGiftCard).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}