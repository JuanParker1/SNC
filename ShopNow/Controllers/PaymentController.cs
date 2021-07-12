using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        private ShopnowchatEntities db = new ShopnowchatEntities();
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

            model.List = db.Carts.Join(db.Payments, c => c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
           .Where(i => i.c.Status == 0 && i.c.CartStatus == 6)
             .GroupBy(i => i.c.OrderNo)
            .Select(i => new PaymentReportViewModel.PaymentReportList
            {
                Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                ShopCode = i.Any() ? i.FirstOrDefault().c.ShopCode : "",
                ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "",
                Address = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
                DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now,
                Amount = i.Any() ? i.FirstOrDefault().p.Amount : 0.0,
                OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : ""
            }).OrderByDescending(i => i.DateEncoded).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPAYPR002")]
        public ActionResult PlatformCreditReport(DateTime? StartDate, DateTime? EndDate, string shopcode="")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var rate = db.PlatFormCreditRates.Select(i => i.RatePerOrder).FirstOrDefault();
            var model = new PlatformCreditReportViewModel();

            if (shopcode != null)
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.ShopCharges, p => p.OrderNo, sc => sc.OrderNo, (p, sc) => new { p, sc })
                        .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.sc.CartStatus == 6 && i.p.ShopCode == shopcode)
                        .AsEnumerable().GroupBy(i => i.sc.OrderNo).Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                        {
                            OrderNo = i.Any() ? i.FirstOrDefault().sc.OrderNo : "N/A",
                            CartStatus = i.Any() ? i.FirstOrDefault().sc.CartStatus : 6,
                            RatePerOrder = rate
                        }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.ShopCharges, p => p.OrderNo, sc => sc.OrderNo, (p, sc) => new { p, sc })
                        .Where(i => i.sc.CartStatus == 6 && i.p.ShopCode == shopcode)
                        .AsEnumerable().GroupBy(i => i.sc.OrderNo).Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                        {
                            OrderNo = i.Any() ? i.FirstOrDefault().sc.OrderNo : "N/A",
                            CartStatus = i.Any() ? i.FirstOrDefault().sc.CartStatus : 6,
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

                    model.List = db.Payments.Join(db.ShopCharges, p => p.OrderNo, sc => sc.OrderNo, (p, sc) => new { p, sc })
                          .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.sc.CartStatus == 6)
                          .AsEnumerable().GroupBy(i => i.sc.OrderNo).Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                          {
                              OrderNo = i.Any() ? i.FirstOrDefault().sc.OrderNo : "N/A",
                              CartStatus = i.Any() ? i.FirstOrDefault().sc.CartStatus : 6,
                              RatePerOrder = rate
                          }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.ShopCharges, p => p.OrderNo, sc => sc.OrderNo, (p, sc) => new { p, sc })
                        .Where(i => i.sc.CartStatus == 6)
                        .AsEnumerable().GroupBy(i => i.sc.OrderNo).Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                        {
                            OrderNo = i.Any() ? i.FirstOrDefault().sc.OrderNo : "N/A",
                            CartStatus = i.Any() ? i.FirstOrDefault().sc.CartStatus : 6,
                            RatePerOrder = rate
                        }).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPAYTPR003")]
        public ActionResult TodayPlatformCreditReport(string shopcode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var rate = db.PlatFormCreditRates.Select(i => i.RatePerOrder).FirstOrDefault();
            var model = new PlatformCreditReportViewModel();
            var date = DateTime.Now;
            if (shopcode != null)
            {
                model.List = db.Payments.Join(db.ShopCharges, p => p.OrderNo, sc => sc.OrderNo, (p, sc) => new { p, sc })
                    .Where(i => i.sc.CartStatus == 6 && i.p.ShopCode == shopcode && i.p.DateUpdated.Year == date.Year && i.p.DateUpdated.Month == date.Month && i.p.DateUpdated.Day == date.Day)
                    .AsEnumerable().GroupBy(i => i.sc.OrderNo).Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                    {
                        OrderNo = i.Any() ? i.FirstOrDefault().sc.OrderNo : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().sc.CartStatus : 6,
                        RatePerOrder = rate
                    }).ToList();
            }
            else
            {
                model.List = db.Payments.Join(db.ShopCharges, p => p.OrderNo, sc => sc.OrderNo, (p, sc) => new { p, sc })
                    .Where(i => i.sc.CartStatus == 6 && i.p.DateUpdated.Year == date.Year && i.p.DateUpdated.Month == date.Month && i.p.DateUpdated.Day == date.Day)
                    .AsEnumerable().GroupBy(i => i.sc.OrderNo).Select(i => new PlatformCreditReportViewModel.PlatformCreditReportList
                    {
                        OrderNo = i.Any() ? i.FirstOrDefault().sc.OrderNo : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().sc.CartStatus : 6,
                        RatePerOrder = rate
                    }).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPAYDR004")]
        public ActionResult DeliveryCreditReport(DateTime? StartDate, DateTime? EndDate, string shopcode="")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryCreditReportViewModel();
            if (shopcode != null)
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                        .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                        .Where(i => i.j.p.DateEncoded >= startDatetFilter && i.j.p.DateEncoded <= endDateFilter && i.sc.CartStatus == 6 && i.j.t.CreditType == 1 && i.j.p.ShopCode == shopcode)
                        .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                        {
                             OrderNo = i.j.p.OrderNo,
                             CartStatus = i.sc.CartStatus,
                             DeliveryCharge = i.sc.GrossDeliveryCharge
                        }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                        .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                        .Where(i => i.sc.CartStatus == 6 && i.j.t.CreditType == 1 && i.j.p.ShopCode == shopcode)
                        .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                        {
                            OrderNo = i.j.p.OrderNo,
                            CartStatus = i.sc.CartStatus,
                            DeliveryCharge = i.sc.GrossDeliveryCharge
                        }).ToList();
                }
            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                        .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                        .Where(i => i.j.p.DateEncoded >= startDatetFilter && i.j.p.DateEncoded <= endDateFilter && i.sc.CartStatus == 6 && i.j.t.CreditType == 1)
                        .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                        {
                            OrderNo = i.j.p.OrderNo,
                            CartStatus = i.sc.CartStatus,
                            DeliveryCharge = i.sc.GrossDeliveryCharge
                        }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                        .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                        .Where(i => i.sc.CartStatus == 6 && i.j.t.CreditType == 1)
                        .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                        {
                            OrderNo = i.j.p.OrderNo,
                            CartStatus = i.sc.CartStatus,
                            DeliveryCharge = i.sc.GrossDeliveryCharge
                        }).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPAYTDR005")]
        public ActionResult TodayDeliveryCreditReport(string shopcode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryCreditReportViewModel();
            var date = DateTime.Now;
            if (shopcode != null)
            {
                    model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                        .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                        .Where(i => i.sc.CartStatus == 6 && i.j.t.CreditType == 1 && i.j.p.ShopCode == shopcode && i.j.p.DateUpdated.Year == date.Year && i.j.p.DateUpdated.Month == date.Month && i.j.p.DateUpdated.Day == date.Day)
                        .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                        {
                            OrderNo = i.j.p.OrderNo,
                            CartStatus = i.sc.CartStatus,
                            DeliveryCharge = i.sc.GrossDeliveryCharge
                        }).ToList();
            }
            else
            {
                    model.List = db.Payments.Join(db.TopUps, p => p.CustomerCode, t => t.CustomerCode, (p, t) => new { p, t })
                        .Join(db.ShopCharges, j => j.p.OrderNo, sc => sc.OrderNo, (j, sc) => new { j, sc })
                        .Where(i => i.sc.CartStatus == 6 && i.j.t.CreditType == 1 && i.j.p.DateUpdated.Year == date.Year && i.j.p.DateUpdated.Month == date.Month && i.j.p.DateUpdated.Day == date.Day)
                        .AsEnumerable().Select(i => new DeliveryCreditReportViewModel.DeliveryCreditReportList
                        {
                            OrderNo = i.j.p.OrderNo,
                            CartStatus = i.sc.CartStatus,
                            DeliveryCharge = i.sc.GrossDeliveryCharge
                        }).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPAYR001")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult ShopPayment(ShopPaymentListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.EarningDate = model.EarningDate == null ? DateTime.Now : model.EarningDate.Value;

            //model.ListItems = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.EarningDate.Value) && !string.IsNullOrEmpty(i.OrderNo))
            //   .Join(db.Shops, p => p.ShopCode, s => s.Code, (p, s) => new { p, s })
            //   .Join(db.Carts.Where(i => i.CartStatus == 6), p => p.p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
            //   .GroupBy(i => i.c.OrderNo)
            //   .AsEnumerable()
            //   .Select(i => new ShopPaymentListViewModel.ListItem
            //   {
            //       AccountName = i.FirstOrDefault().p.s.AccountName,
            //       AccountNumber = i.FirstOrDefault().p.s.AccountNumber,
            //       AccountType = i.FirstOrDefault().p.s.AcountType,
            //       FinalAmount = i.FirstOrDefault().p.p.Amount - (i.FirstOrDefault().p.p.refundAmount ?? 0) - (Convert.ToDouble(getPaymentData(i.FirstOrDefault().p.p.ReferenceCode)?.fee ?? 0)) - (Convert.ToDouble(getPaymentData(i.FirstOrDefault().p.p.ReferenceCode)?.tax ?? 0)),
            //       IfscCode = i.FirstOrDefault().p.s.IFSCCode,
            //       PaymentDate = i.FirstOrDefault().p.p.DateEncoded,
            //       PaymentId = "JOY" + i.FirstOrDefault().p.p.OrderNo,
            //       ShopName = i.FirstOrDefault().p.p.ShopName,
            //       ShopCode = i.FirstOrDefault().p.p.ShopCode,
            //       ShopOwnerPhoneNumber = i.FirstOrDefault().p.s.OwnerPhoneNumber,
            //       TransactionType = i.FirstOrDefault().p.p.PaymentMode,
            //       Identifier = "N",
            //       CNR = "JOY" + i.FirstOrDefault().p.p.OrderNo,
            //       DebitAccountNo = "918020043538740",
            //       EmailBody = "",
            //       EmailID = i.FirstOrDefault().p.s.Email,
            //       ReceiverIFSC = i.FirstOrDefault().p.s.IFSCCode,
            //       Remarks = "",
            //       PhoneNo = i.FirstOrDefault().p.s.OwnerPhoneNumber,
            //       CartStatus = i.FirstOrDefault().c.CartStatus,
            //       ShopPaymentStatus = i.FirstOrDefault().c.ShopPaymentStatus
            //   }).ToList();

            model.ListItems = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.EarningDate.Value) && !string.IsNullOrEmpty(i.OrderNo))
               .Join(db.Shops, p => p.ShopCode, s => s.Code, (p, s) => new { p, s })
               .Join(db.Carts.Where(i => i.CartStatus == 6), p => p.p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
               .GroupJoin(db.paymentsDatas, p => p.p.p.ReferenceCode, pd => pd.paymentId, (p, pd) => new { p, pd })
               .GroupBy(i => i.p.c.OrderNo)
               .AsEnumerable()
               .Select(i => new ShopPaymentListViewModel.ListItem
               {
                   AccountName = i.FirstOrDefault().p.p.s.AccountName,
                   AccountNumber = i.FirstOrDefault().p.p.s.AccountNumber,
                   AccountType = i.FirstOrDefault().p.p.s.AcountType,
                   FinalAmount = i.FirstOrDefault().p.p.p.Amount - (i.FirstOrDefault().p.p.p.refundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().tax ?? 0) : 0)),
                   IfscCode = i.FirstOrDefault().p.p.s.IFSCCode,
                   PaymentDate = i.FirstOrDefault().p.p.p.DateEncoded,
                   PaymentId = "JOY" + i.FirstOrDefault().p.p.p.OrderNo,
                   ShopName = i.FirstOrDefault().p.p.p.ShopName,
                   ShopCode = i.FirstOrDefault().p.p.p.ShopCode,
                   ShopOwnerPhoneNumber = i.FirstOrDefault().p.p.s.OwnerPhoneNumber,
                   TransactionType = i.FirstOrDefault().p.p.p.PaymentMode,
                   Identifier = (i.FirstOrDefault().p.p.s.AcountType == "CA" && i.FirstOrDefault().p.p.s.BankName == "Axis Bank") ? "I" : "N",
                   CNR = "JOY" + i.FirstOrDefault().p.p.p.OrderNo,
                   DebitAccountNo = "918020043538740",
                   EmailBody = "",
                   EmailID = i.FirstOrDefault().p.p.s.Email,
                   ReceiverIFSC = i.FirstOrDefault().p.p.s.IFSCCode,
                   Remarks = "",
                   PhoneNo = i.FirstOrDefault().p.p.s.OwnerPhoneNumber,
                   CartStatus = i.FirstOrDefault().p.c.CartStatus,
                   ShopPaymentStatus = i.FirstOrDefault().p.c.ShopPaymentStatus
               }).ToList();
            return View(model);
        }

        //public paymentsData getPaymentData(string paymentId)
        //{
        //    var payment = db.paymentsDatas.FirstOrDefault(i => i.paymentId == paymentId);
        //    return payment;
        //}

        public ActionResult MarkShopPaymentAsPaidInCart(DateTime date)
        {
            var paymentListByDate = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(date) && i.OrderNo != null).Select(i=>i.OrderNo).ToList();
            foreach (var orderno in paymentListByDate)
            {
                var cartList = db.Carts.Where(i => i.OrderNo == orderno && i.CartStatus==6).ToList();
                foreach (var c in cartList)
                {
                    var cart = db.Carts.FirstOrDefault(i => i.Code == c.Code);
                    cart.ShopPaymentStatus = 1;
                    db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
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

            model.ListItems = db.Carts.Where(i => i.CartStatus == 6)
               .Join(db.ShopCharges, c => c.OrderNo, s => s.OrderNo, (c, s) => new { c, s })
               .Join(db.Payments, c => c.c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
               .Join(db.DeliveryBoys, c => c.c.c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
               .GroupBy(i=>i.c.c.c.OrderNo)
               .Select(i => new DeliveryBoyPaymentListViewModel.ListItem
               {
                   AccountName = i.FirstOrDefault().d.AccountName,
                   AccountNumber = i.FirstOrDefault().d.AccountNumber,
                   AccountType = "SA",
                   Amount = i.FirstOrDefault().c.c.s.GrossDeliveryCharge,
                   IfscCode = i.FirstOrDefault().d.IFSCCode,
                   PaymentDate = i.FirstOrDefault().c.p.DateEncoded,
                   PaymentId = "JOY" + i.FirstOrDefault().c.p.OrderNo,
                   DeliveryBoyCode = i.FirstOrDefault().d.Code,
                   DeliveryBoyName = i.FirstOrDefault().d.Name,
                   DeliveryBoyPhoneNumber = i.FirstOrDefault().d.PhoneNumber,
                   TransactionType = i.FirstOrDefault().c.p.PaymentMode,
                   OrderNo = i.FirstOrDefault().c.p.OrderNo,
                   EmailBody = "",
                   EmailID = i.FirstOrDefault().d.Email,
                   DeliveryBoyPaymentStatus = i.FirstOrDefault().c.c.c.DeliveryBoyPaymentStatus
               }).Where(i =>(DbFunctions.TruncateTime(i.PaymentDate) == DbFunctions.TruncateTime(model.EarningDate.Value)) /*&& i.OrderNo != null*/).ToList();
            return View(model);
        }

        public ActionResult MarkDeliveryBoyPaymentAsPaidInCart(DateTime date)
        {
            var paymentListByDate = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(date) && i.OrderNo != null).Select(i => i.OrderNo).ToList();
            foreach (var orderno in paymentListByDate)
            {
                var cartList = db.Carts.Where(i => i.OrderNo == orderno && i.CartStatus == 6).ToList();
                foreach (var c in cartList)
                {
                    var cart = db.Carts.FirstOrDefault(i => i.Code == c.Code);
                    cart.DeliveryBoyPaymentStatus = 1;
                    db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("DeliveryBoyPayment", "Payment");
        }

        public ActionResult RetailerPayment(RetailerPaymentListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.ReatillerPaymentReportAdmin(model.StartDate, model.EndDate, model.ShopCode)
                .Select((i, index) => new RetailerPaymentListViewModel.ListItem
                {
                    No = index + 1,
                    OrderDate = i.OrderDate,
                    OrderFirstAmount = i.OrderFirstAmount,
                    OrderNo = i.orderNo,
                    //PaidAmount = i.PaidAmount ?? 0,
                    PaidAmount = i.OrderFirstAmount - (i.RefundAmount ?? 0),
                    //PaymentAmount = i.PaymentAmount ?? 0,
                    PaymentAmount = i.OrderFirstAmount - (i.RefundAmount ?? 0) - Convert.ToDouble((i.TransactionFee ?? 0)) - Convert.ToDouble((i.TransactionTax ?? 0)),
                    PaymentDate = i.OrderDate,
                    PaymentId = i.PaymentId ?? "N/A",
                    PaymentType = i.PaymentType,
                    RefundAmount = i.RefundAmount ?? 0,
                    RefundeRemark = i.RefundeRemark ?? "N/A",
                    RefundStatus = i.RefundStatus,
                    ShopName = i.ShopName,
                    ShopPaymentStatus = i.ShopPaymentStatus,
                    TransactionFee = i.TransactionFee ?? 0,
                    TransactionTax = i.TransactionTax ?? 0
                }).ToList();
          
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult ShopPay(string orderNo)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var cartList = db.Carts.Where(i => i.OrderNo == orderNo && i.Status == 0 && i.ShopPaymentStatus == 0).ToList();
            foreach (var c in cartList)
            {
                var cart = GetCart(c.Code);
                cart.ShopPaymentStatus = 1;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("RetailerPayment");
        }

        Cart GetCart(string code)
        {
            try
            {

                return db.Carts.Where(i => i.Code == code).FirstOrDefault();
            }
            catch
            {
                return (Cart)null;
            }
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