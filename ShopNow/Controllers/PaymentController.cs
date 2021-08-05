using AutoMapper;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class PaymentController : Controller
    {

        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public PaymentController()
        {                                                                                                                                               
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Payment, PaymentReportViewModel>();
                config.CreateMap<Payment, PaymentReportViewModel.PaymentReportList>();
                                                                                                             
                                                                        
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNPAYR001")]
        public ActionResult Report()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PaymentReportViewModel();

            model.List = db.Orders
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p }).Where(i => i.c.Status == 6)
            .Select(i => new PaymentReportViewModel.PaymentReportList
            {
                Id = i.c.Id,
                ShopId = i.c.ShopId,
                ShopName = i.c.ShopName,
                Address = i.c.DeliveryAddress,
                DateEncoded = i.c.DateEncoded,
                Amount = i.p.Amount,
                OrderNumber = i.p.OrderNumber
            }).OrderByDescending(i => i.DateEncoded).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPAYPR002")]
        public ActionResult PlatformCreditReport(DateTime? StartDate, DateTime? EndDate, int shopId=0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var rate = db.PlatFormCreditRates.Select(i => i.RatePerOrder).FirstOrDefault();
            var model = new PlatformCreditReportViewModel();

            if (shopId != 0)
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments
                        .Join(db.Orders, p => p.OrderNumber, sc => sc.OrderNumber, (p, sc) => new { p, sc })
                        .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.sc.Status == 6 && i.p.ShopId == shopId)
                        .AsEnumerable()
                        .Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                        {
                            OrderNumber = i.p.OrderNumber,
                            CartStatus = i.sc.Status,
                            RatePerOrder = rate
                        }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.Orders, p => p.OrderNumber, sc => sc.OrderNumber, (p, sc) => new { p, sc })
                        .Where(i => i.sc.Status == 6 && i.p.ShopId == shopId)
                        .AsEnumerable()
                        .Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                        {
                            OrderNumber = i.p.OrderNumber,
                            CartStatus = i.sc.Status,
                            RatePerOrder = rate
                        }).ToList();
                }
            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.Orders, p => p.OrderNumber, sc => sc.OrderNumber, (p, sc) => new { p, sc })
                          .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.sc.Status == 6)
                          .AsEnumerable().Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                          {
                              OrderNumber = i.p.OrderNumber,
                              CartStatus = i.sc.Status,
                              RatePerOrder = rate
                          }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.Orders, p => p.OrderNumber, sc => sc.OrderNumber, (p, sc) => new { p, sc })
                        .Where(i => i.sc.Status == 6)
                        .AsEnumerable().Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                        {
                            OrderNumber = i.p.OrderNumber,
                            CartStatus = i.sc.Status,
                            RatePerOrder = rate
                        }).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPAYTPR003")]
        public ActionResult TodayPlatformCreditReport(int shopId = 0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var rate = db.PlatFormCreditRates.Select(i => i.RatePerOrder).FirstOrDefault();
            var model = new PlatformCreditReportViewModel();
            var date = DateTime.Now;
            if (shopId != 0)
            {
                model.List = db.Payments.Join(db.Orders, p => p.OrderNumber, sc => sc.OrderNumber, (p, sc) => new { p, sc })
                    .Where(i => i.sc.Status == 6 && i.p.ShopId == shopId && i.p.DateUpdated.Year == date.Year && i.p.DateUpdated.Month == date.Month && i.p.DateUpdated.Day == date.Day)
                    .AsEnumerable()
                    .Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                    {
                        OrderNumber = i.p.OrderNumber,
                        CartStatus = i.sc.Status,
                        RatePerOrder = rate
                    }).ToList();
            }
            else
            {
                model.List = db.Payments.Join(db.Orders, p => p.OrderNumber, sc => sc.OrderNumber, (p, sc) => new { p, sc })
                    .Where(i => i.sc.Status == 6 && i.p.DateUpdated.Year == date.Year && i.p.DateUpdated.Month == date.Month && i.p.DateUpdated.Day == date.Day)
                    .AsEnumerable()
                    .Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                    {
                        OrderNumber = i.p.OrderNumber,
                        CartStatus = i.sc.Status,
                        RatePerOrder = rate
                    }).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPAYDR004")]
        public ActionResult DeliveryCreditReport(DateTime? StartDate, DateTime? EndDate, int shopId=0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryCreditReportViewModel();
            if (shopId != 0)
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    //model.List = db.Payments
                    //    .Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                    //    .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                    //    .Where(i => i.j.p.DateEncoded >= startDatetFilter && i.j.p.DateEncoded <= endDateFilter && i.sc.CartStatus == 6 && i.j.t.CreditType == 1 && i.j.p.ShopCode == shopcode)
                    //    .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                    //    {
                    //         OrderNo = i.j.p.OrderNo,
                    //         CartStatus = i.sc.CartStatus,
                    //         DeliveryCharge = i.sc.GrossDeliveryCharge
                    //    }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    //model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                    //    .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                    //    .Where(i => i.sc.CartStatus == 6 && i.j.t.CreditType == 1 && i.j.p.ShopCode == shopcode)
                    //    .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                    //    {
                    //        OrderNo = i.j.p.OrderNo,
                    //        CartStatus = i.sc.CartStatus,
                    //        DeliveryCharge = i.sc.GrossDeliveryCharge
                    //    }).ToList();
                }
            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    //model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                    //    .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                    //    .Where(i => i.j.p.DateEncoded >= startDatetFilter && i.j.p.DateEncoded <= endDateFilter && i.sc.CartStatus == 6 && i.j.t.CreditType == 1)
                    //    .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                    //    {
                    //        OrderNo = i.j.p.OrderNo,
                    //        CartStatus = i.sc.CartStatus,
                    //        DeliveryCharge = i.sc.GrossDeliveryCharge
                    //    }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    //model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                    //    .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                    //    .Where(i => i.sc.CartStatus == 6 && i.j.t.CreditType == 1)
                    //    .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                    //    {
                    //        OrderNo = i.j.p.OrderNo,
                    //        CartStatus = i.sc.CartStatus,
                    //        DeliveryCharge = i.sc.GrossDeliveryCharge
                    //    }).ToList();
                }
            }
            return View(model.List);
        }

        //[AccessPolicy(PageCode = "SHNPAYTDR005")]
        //public ActionResult TodayDeliveryCreditReport(string shopcode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new DeliveryCreditReportViewModel();
        //    var date = DateTime.Now;
        //    if (shopcode != null)
        //    {
        //            model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
        //                .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
        //                .Where(i => i.sc.CartStatus == 6 && i.j.t.CreditType == 1 && i.j.p.ShopCode == shopcode && i.j.p.DateUpdated.Year == date.Year && i.j.p.DateUpdated.Month == date.Month && i.j.p.DateUpdated.Day == date.Day)
        //                .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
        //                {
        //                    OrderNo = i.j.p.OrderNo,
        //                    CartStatus = i.sc.CartStatus,
        //                    DeliveryCharge = i.sc.GrossDeliveryCharge
        //                }).ToList();
        //    }
        //    else
        //    {
        //            model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
        //                .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
        //                .Where(i => i.sc.CartStatus == 6 && i.j.t.CreditType == 1 && i.j.p.DateUpdated.Year == date.Year && i.j.p.DateUpdated.Month == date.Month && i.j.p.DateUpdated.Day == date.Day)
        //                .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
        //                {
        //                    OrderNo = i.j.p.OrderNo,
        //                    CartStatus = i.sc.CartStatus,
        //                    DeliveryCharge = i.sc.GrossDeliveryCharge
        //                }).ToList();
        //    }
        //    return View(model.List);
        //}

        [AccessPolicy(PageCode = "SHNPAYR001")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult ShopPayment(ShopPaymentListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.EarningDate = model.EarningDate == null ? DateTime.Now : model.EarningDate.Value;
            
            model.ListItems = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.EarningDate.Value) && i.OrderNumber !=0)
               .Join(db.Shops, p => p.ShopId, s => s.Id, (p, s) => new { p, s })
               .Join(db.Orders.Where(i => i.Status == 6), p => p.p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
               .GroupJoin(db.PaymentsDatas, p => p.p.p.ReferenceCode, pd => pd.PaymentId, (p, pd) => new { p, pd })
               .AsEnumerable()
               .Select(i => new ShopPaymentListViewModel.ListItem
               {
                   AccountName = i.p.p.s.AccountName,
                   AccountNumber = i.p.p.s.AccountNumber,
                   AccountType = i.p.p.s.AcountType,
                   FinalAmount = i.p.p.p.Amount - (i.p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.pd.Any() ? (i.pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.pd.Any() ? (i.pd.FirstOrDefault().Tax ?? 0) : 0)),
                   IfscCode = i.p.p.s.IFSCCode,
                   PaymentDate = i.p.p.p.DateEncoded,
                   PaymentId = "JOY" + i.p.p.p.OrderNumber,
                   ShopName = i.p.p.p.ShopName,
                   ShopId = i.p.c.ShopId,
                   ShopOwnerPhoneNumber = i.p.p.s.OwnerPhoneNumber,
                   TransactionType = i.p.p.p.PaymentMode,
                   Identifier = (i.p.p.s.AcountType == "CA" && i.p.p.s.BankName == "Axis Bank") ? "I" : "N",
                   CNR = "JOY" + i.p.p.p.OrderNumber,
                   DebitAccountNo = "918020043538740",
                   EmailBody = "",
                   EmailID = i.p.p.s.Email,
                   ReceiverIFSC = i.p.p.s.IFSCCode,
                   Remarks = "",
                   PhoneNo = i.p.p.s.OwnerPhoneNumber,
                   CartStatus = i.p.c.Status,
                   ShopPaymentStatus = i.p.c.ShopPaymentStatus
               }).ToList();
            return View(model);
        }

        public ActionResult MarkShopPaymentAsPaidInCart(DateTime date)
        {
            var paymentListByDate = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(date) && i.OrderNumber != 0).Select(i=>i.OrderNumber).ToList();
            foreach (var orderno in paymentListByDate)
            {
                var orderList = db.Orders.Where(i => i.OrderNumber == orderno && i.Status==6).ToList();
                foreach (var item in orderList)
                {
                    var order = db.Orders.FirstOrDefault(i => i.Id == item.Id);
                    order.ShopPaymentStatus = 1;
                    db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ShopPayment", "Payment");
        }

        public ActionResult DeliveryBoyPayment(DeliveryBoyPaymentListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.EarningDate = model.EarningDate == null ? DateTime.Now : model.EarningDate.Value;

            model.ListItems = db.Orders.Where(i => i.Status == 6)
               .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
               .Join(db.DeliveryBoys, c => c.c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
               .Select(i => new DeliveryBoyPaymentListViewModel.ListItem
               {
                   AccountName = i.d.AccountName,
                   AccountNumber = i.d.AccountNumber,
                   AccountType = "SA",
                   Amount = i.c.c.DeliveryCharge,
                   IfscCode = i.d.IFSCCode,
                   PaymentDate = i.c.p.DateEncoded,
                   PaymentId = "JOY" + i.c.p.OrderNumber,
                   DeliveryBoyId = i.d.Id,
                   DeliveryBoyName = i.d.Name,
                   DeliveryBoyPhoneNumber = i.d.PhoneNumber,
                   TransactionType = i.c.p.PaymentMode,
                   OrderNo = i.c.p.OrderNumber,
                   EmailBody = "",
                   EmailID = i.d.Email,
                   DeliveryBoyPaymentStatus = i.c.c.DeliveryBoyPaymentStatus
               }).Where(i =>(DbFunctions.TruncateTime(i.PaymentDate) == DbFunctions.TruncateTime(model.EarningDate.Value)) /*&& i.OrderNo != null*/).ToList();
            return View(model);
        }

        public ActionResult MarkDeliveryBoyPaymentAsPaidInCart(DateTime date)
        {
            var paymentListByDate = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(date) && i.OrderNumber != 0).Select(i => i.OrderNumber).ToList();
            foreach (var orderno in paymentListByDate)
            {
                var orderList = db.Orders.Where(i => i.OrderNumber == orderno && i.Status == 6).ToList();
                foreach (var item in orderList)
                {
                    var order = db.Orders.FirstOrDefault(i => i.Id == item.Id);
                    order.DeliveryBoyPaymentStatus = 1;
                    db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("DeliveryBoyPayment", "Payment");
        }

        public ActionResult RetailerPayment(RetailerPaymentListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            //Store Procedure Return int
            //model.ListItems = db.ReatillerPaymentReportAdmin(model.StartDate, model.EndDate, model.ShopCode)
            //    .Select((i, index) => new RetailerPaymentListViewModel.ListItem
            //    {
            //        No = index + 1,
            //        OrderDate = i.OrderDate,
            //        OrderFirstAmount = i.OrderFirstAmount,
            //        OrderNo = i.orderNo,
            //        //PaidAmount = i.PaidAmount ?? 0,
            //        PaidAmount = i.OrderFirstAmount - (i.RefundAmount ?? 0),
            //        //PaymentAmount = i.PaymentAmount ?? 0,
            //        PaymentAmount = i.OrderFirstAmount - (i.RefundAmount ?? 0) - Convert.ToDouble((i.TransactionFee ?? 0)) - Convert.ToDouble((i.TransactionTax ?? 0)),
            //        PaymentDate = i.OrderDate,
            //        PaymentId = i.PaymentId ?? "N/A",
            //        PaymentType = i.PaymentType,
            //        RefundAmount = i.RefundAmount ?? 0,
            //        RefundeRemark = i.RefundeRemark ?? "N/A",
            //        RefundStatus = i.RefundStatus,
            //        ShopName = i.ShopName,
            //        ShopPaymentStatus = i.ShopPaymentStatus,
            //        TransactionFee = i.TransactionFee ?? 0,
            //        TransactionTax = i.TransactionTax ?? 0
            //    }).ToList();
          
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult ShopPay(int orderNo)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderNo && i.Status == 0 && i.ShopPaymentStatus == 0);
            order.ShopPaymentStatus = 1;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("RetailerPayment");
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