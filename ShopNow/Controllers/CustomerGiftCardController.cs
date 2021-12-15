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
        public ActionResult Add(CustomerGiftCardAddViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var giftCard = new CustomerGiftCard
            {
                Amount = model.Amount,
                CreatedBy = user.Name,
                CustomerId = model.CustomerId,
                CustomerPhoneNumber = model.CustomerPhoneNumber,
                DateEncoded = DateTime.Now,
                GiftCardCode = Helpers.DRC.GenerateGiftCard("SNC-"),
                ExpiryDate = model.ExpiryDate,
                Status = 0
            };
            db.CustomerGiftCards.Add(giftCard);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [HttpPost]
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