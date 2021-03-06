using AutoMapper;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
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

        [AccessPolicy(PageCode = "SNCPYR182")]
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
                OrderNumber = i.p.OrderNumber.ToString()
            }).OrderByDescending(i => i.DateEncoded).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SNCPYPR183")]
        public ActionResult PlatformCreditReport(DateTime? StartDate, DateTime? EndDate, int shopId = 0)
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

        [AccessPolicy(PageCode = "SNCPYTPR184")]
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

        [AccessPolicy(PageCode = "SNCPYDR185")]
        public ActionResult DeliveryCreditReport(DateTime? StartDate, DateTime? EndDate, int shopId = 0)
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

        [AccessPolicy(PageCode = "SNCPYSP186")]
        public ActionResult ShopPayment(ShopPaymentListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.EarningDate = model.EarningDate == null ? DateTime.Now : model.EarningDate.Value;

            //model.ListItems = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.EarningDate.Value) && i.OrderNumber !=0)
            //   .Join(db.Shops, p => p.ShopId, s => s.Id, (p, s) => new { p, s })
            //   .Join(db.Orders.Where(i => i.Status == 6), p => p.p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
            //   .GroupJoin(db.PaymentsDatas, p => p.p.p.ReferenceCode, pd => pd.PaymentId, (p, pd) => new { p, pd })
            //   .AsEnumerable()
            //   .Select(i => new ShopPaymentListViewModel.ListItem
            //   {
            //       AccountName = i.p.p.s.AccountName,
            //       AccountNumber = i.p.p.s.AccountNumber,
            //       AccountType = i.p.p.s.AcountType,
            //       FinalAmount = i.p.p.p.Amount - (i.p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.pd.Any() ? (i.pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.pd.Any() ? (i.pd.FirstOrDefault().Tax ?? 0) : 0)),
            //       IfscCode = i.p.p.s.IFSCCode,
            //       PaymentDate = i.p.p.p.DateEncoded,
            //       PaymentId = "JOY" + i.p.p.p.OrderNumber,
            //       ShopName = i.p.p.p.ShopName,
            //       ShopId = i.p.c.ShopId,
            //       ShopOwnerPhoneNumber = i.p.p.s.OwnerPhoneNumber,
            //       TransactionType = i.p.p.p.PaymentMode,
            //       Identifier = (i.p.p.s.AcountType == "CA" && i.p.p.s.BankName == "Axis Bank") ? "I" : "N",
            //       CNR = "JOY" + i.p.p.p.OrderNumber,
            //       DebitAccountNo = "918020043538740",
            //       EmailBody = "",
            //       EmailID = i.p.p.s.Email,
            //       ReceiverIFSC = i.p.p.s.IFSCCode,
            //       Remarks = "",
            //       PhoneNo = i.p.p.s.OwnerPhoneNumber,
            //       CartStatus = i.p.c.Status,
            //       ShopPaymentStatus = i.p.c.ShopPaymentStatus
            //   }).ToList();

            // List 1 without Joyra
            var List1 = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.EarningDate.Value) && i.OrderNumber != 0)
              .Join(db.Shops.Where(i=> i.Id != 123 && i.Id != 203), p => p.ShopId, s => s.Id, (p, s) => new { p, s })
              .Join(db.Orders.Where(i => i.Status == 6 && i.ShopId != 123 && i.ShopId != 203), p => p.p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
              .GroupJoin(db.PaymentsDatas, p => p.p.p.ReferenceCode, pd => pd.PaymentId, (p, pd) => new { p, pd })
              .GroupBy(i => i.p.c.OrderNumber)
              .AsEnumerable()
              .Select(i => new ShopPaymentListViewModel.ListItem
              {
                  AccountName = i.FirstOrDefault().p.p.s.AccountName ?? "Nil",
                  AccountNumber = i.FirstOrDefault().p.p.s.AccountNumber ?? "Nil",
                  AccountType = i.FirstOrDefault().p.p.s.AcountType,
                  //FinalAmount = i.FirstOrDefault().p.p.p.Amount - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)),
                  //FinalAmount = i.FirstOrDefault().p.c.TotalShopPrice !=0 ? i.FirstOrDefault().p.p.p.Amount - Math.Abs(i.FirstOrDefault().p.c.TotalPrice - i.FirstOrDefault().p.c.TotalShopPrice - i.FirstOrDefault().p.c.NetDeliveryCharge) - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)): i.FirstOrDefault().p.p.p.Amount - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)),
                 // FinalAmount = i.FirstOrDefault().p.c.TotalShopPrice != 0 ? (i.FirstOrDefault().p.c.TotalShopPrice + i.FirstOrDefault().p.c.Packingcharge) - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)) : i.FirstOrDefault().p.p.p.Amount - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)),
                  FinalAmount = i.FirstOrDefault().p.c.IsPickupDrop == false ? i.FirstOrDefault().p.c.TotalShopPrice != 0 ? (i.FirstOrDefault().p.c.TotalShopPrice + i.FirstOrDefault().p.c.Packingcharge +i.FirstOrDefault().p.c.WalletAmount) - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)) : i.FirstOrDefault().p.p.p.Amount + i.FirstOrDefault().p.c.WalletAmount - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)): i.FirstOrDefault().p.c.TotalPrice,
                  IfscCode = i.FirstOrDefault().p.p.s.IFSCCode,
                  PaymentDate = i.FirstOrDefault().p.p.p.DateEncoded,
                  PaymentId = "JOY" + i.FirstOrDefault().p.p.p.OrderNumber.ToString(),
                  ShopName = i.FirstOrDefault().p.p.p.ShopName,
                  ShopId = i.FirstOrDefault().p.p.p.ShopId ?? 0,
                  ShopOwnerPhoneNumber = i.FirstOrDefault().p.p.s.OwnerPhoneNumber,
                  TransactionType = i.FirstOrDefault().p.p.p.PaymentMode,
                  Identifier = (i.FirstOrDefault().p.p.s.AcountType == "CA" && i.FirstOrDefault().p.p.s.BankName == "Axis Bank") ? "I" : "N",
                  CNR = "JOY" + i.FirstOrDefault().p.p.p.OrderNumber,
                  DebitAccountNo = "609505027743",
                  EmailBody = "",
                  EmailID = i.FirstOrDefault().p.p.s.Email,
                  ReceiverIFSC = i.FirstOrDefault().p.p.s.IFSCCode,
                  Remarks = "",
                  PhoneNo = i.FirstOrDefault().p.p.s.OwnerPhoneNumber.ToString(),
                  CartStatus = i.FirstOrDefault().p.c.Status,
                  ShopPaymentStatus = i.FirstOrDefault().p.c.ShopPaymentStatus,
                  PaymentMode = i.FirstOrDefault().p.p.s.BankName.ToUpper() == "ICICI BANK" ? "FT" : "NEFT"
              }).ToList();

            // List 2 with Joyra
            var List2 = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.EarningDate.Value) && i.OrderNumber != 0)
              .Join(db.Shops.Where(i => i.Id == 123 || i.Id == 203), p => p.ShopId, s => s.Id, (p, s) => new { p, s })
              .Join(db.Orders.Where(i => i.Status == 6 &&( i.ShopId == 123 || i.ShopId == 203)), p => p.p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
              .GroupJoin(db.ShopBillDetails, p => p.c.OrderNumber, b => b.OrderNumber, (p, b) => new { p, b })
              .GroupJoin(db.PaymentsDatas, p => p.p.p.p.ReferenceCode, pd => pd.PaymentId, (p, pd) => new { p, pd })
              .GroupBy(i => i.p.p.c.OrderNumber)
              .AsEnumerable()
              .Select(i => new ShopPaymentListViewModel.ListItem
              {
                  AccountName = i.FirstOrDefault().p.p.p.s.AccountName ?? "Nil",
                  AccountNumber = i.FirstOrDefault().p.p.p.s.AccountNumber ?? "Nil",
                  AccountType = i.FirstOrDefault().p.p.p.s.AcountType,
                  //FinalAmount = i.FirstOrDefault().p.p.p.Amount - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)),
                  //FinalAmount = i.FirstOrDefault().p.c.TotalShopPrice !=0 ? i.FirstOrDefault().p.p.p.Amount - Math.Abs(i.FirstOrDefault().p.c.TotalPrice - i.FirstOrDefault().p.c.TotalShopPrice - i.FirstOrDefault().p.c.NetDeliveryCharge) - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)): i.FirstOrDefault().p.p.p.Amount - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)),
                  // FinalAmount = i.FirstOrDefault().p.c.TotalShopPrice != 0 ? (i.FirstOrDefault().p.c.TotalShopPrice + i.FirstOrDefault().p.c.Packingcharge) - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)) : i.FirstOrDefault().p.p.p.Amount - (i.FirstOrDefault().p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().pd.Any() ? (i.FirstOrDefault().pd.FirstOrDefault().Tax ?? 0) : 0)),
                  //FinalAmount = i.FirstOrDefault().p.p.c.IsPickupDrop == false ? i.FirstOrDefault().p.p.c.TotalShopPrice != 0 ? (i.FirstOrDefault().p.p.c.TotalShopPrice + i.FirstOrDefault().p.p.c.Packingcharge + i.FirstOrDefault().p.p.c.WalletAmount) - (i.FirstOrDefault().p.p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().p.pd.Any() ? (i.FirstOrDefault().p.pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().p.pd.Any() ? (i.FirstOrDefault().p.pd.FirstOrDefault().Tax ?? 0) : 0)) : i.FirstOrDefault().p.p.p.p.Amount + i.FirstOrDefault().p.p.c.WalletAmount - (i.FirstOrDefault().p.p.p.p.RefundAmount ?? 0) - (Convert.ToDouble(i.FirstOrDefault().p.pd.Any() ? (i.FirstOrDefault().p.pd.FirstOrDefault().Fee ?? 0) : 0)) - (Convert.ToDouble(i.FirstOrDefault().p.pd.Any() ? (i.FirstOrDefault().p.pd.FirstOrDefault().Tax ?? 0) : 0)) : i.FirstOrDefault().p.p.c.TotalPrice,
                  FinalAmount = i.FirstOrDefault().p.b.Any() ? i.FirstOrDefault().p.p.c.IsPickupDrop == false ? i.FirstOrDefault().p.b.FirstOrDefault().BillAmount : i.FirstOrDefault().p.p.c.TotalPrice - (i.FirstOrDefault().p.p.p.p.RefundAmount ?? 0) : 0,
                  IfscCode = i.FirstOrDefault().p.p.p.s.IFSCCode,
                  PaymentDate = i.FirstOrDefault().p.p.p.p.DateEncoded,
                  PaymentId = "JOY" + i.FirstOrDefault().p.p.p.p.OrderNumber.ToString(),
                  ShopName = i.FirstOrDefault().p.p.p.p.ShopName,
                  ShopId = i.FirstOrDefault().p.p.p.p.ShopId ?? 0,
                  ShopOwnerPhoneNumber = i.FirstOrDefault().p.p.p.s.OwnerPhoneNumber,
                  TransactionType = i.FirstOrDefault().p.p.p.p.PaymentMode,
                  Identifier = (i.FirstOrDefault().p.p.p.s.AcountType == "CA" && i.FirstOrDefault().p.p.p.s.BankName == "Axis Bank") ? "I" : "N",
                  CNR = "JOY" + i.FirstOrDefault().p.p.p.p.OrderNumber,
                  DebitAccountNo = "609505027743",
                  EmailBody = "",
                  EmailID = i.FirstOrDefault().p.p.p.s.Email,
                  ReceiverIFSC = i.FirstOrDefault().p.p.p.s.IFSCCode,
                  Remarks = "",
                  PhoneNo = i.FirstOrDefault().p.p.p.s.OwnerPhoneNumber.ToString(),
                  CartStatus = i.FirstOrDefault().p.p.c.Status,
                  ShopPaymentStatus = i.FirstOrDefault().p.p.c.ShopPaymentStatus,
                  PaymentMode = i.FirstOrDefault().p.p.p.s.BankName.ToUpper() == "ICICI BANK" ? "FT" : "NEFT"
              }).ToList();

            model.ListItems = List1.Union(List2).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPYDP187")]
        public ActionResult DeliveryBoyPayment(DeliveryBoyPaymentListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.EarningDate = model.EarningDate == null ? DateTime.Now : model.EarningDate.Value;

            //double deliveryRatePercentage = db.DeliveryRatePercentages.FirstOrDefault(i => i.Status == 0 && (DbFunctions.TruncateTime(model.EarningDate.Value) >= DbFunctions.TruncateTime(i.StartDate) && i.EndDate !=null? DbFunctions.TruncateTime(model.EarningDate.Value) <= DbFunctions.TruncateTime(i.EndDate.Value) : DbFunctions.TruncateTime(model.EarningDate.Value) <= DbFunctions.TruncateTime(DateTime.Now))).Percentage;
            //if (deliveryRatePercentage == 0)
            //{
            model.ListItems = db.Orders.Where(i => i.Status == 6)
               .Join(db.Payments.Where(i => (DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.EarningDate.Value))), c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
               .Join(db.DeliveryBoys, c => c.c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
               .Select(i => new DeliveryBoyPaymentListViewModel.ListItem
               {
                   AccountName = i.d.AccountName.Trim(),
                   AccountNumber = i.d.AccountNumber,
                   AccountType = "SA",
                   //Amount = i.d.WorkType == 1 ? ((i.c.c.Distance <= 5) ? 20 + ((i.c.c.DeliveryRatePercentage / 100) * 20) : 20 + ((i.c.c.DeliveryRatePercentage / 100) * 20) + (i.c.c.IsPickupDrop == false ? ((i.c.c.Distance - 5) * 6 + ((i.c.c.DeliveryRatePercentage / 100) * 6)) : i.c.c.Distance <= 15 ? i.c.c.DeliveryCharge - 50 : (60 + ((i.c.c.Distance - 15) * 8)))) : i.c.c.DeliveryCharge,
                   //Amount = i.d.WorkType == 2 ? i.c.c.DeliveryCharge : i.c.c.IsPickupDrop == false? i.c.c.Distance <=5? (i.c.c.DeliveryChargeUpto5Km - 15) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 15)) : (i.c.c.DeliveryChargeUpto5Km - 15) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 15)) + ((i.c.c.Distance - 5) * 6 + ((i.c.c.DeliveryRatePercentage / 100) * 6)):i.c.c.Distance<=5? (i.c.c.DeliveryChargeUpto5Km - 30) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 30)) : i.c.c.Distance <15? (i.c.c.DeliveryChargeUpto5Km - 30) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 30)) + ((i.c.c.Distance - 5) * 6 /*+ ((i.c.c.DeliveryRatePercentage / 100) * 6)*/): (i.c.c.DeliveryChargeUpto5Km - 30) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 30)) + (60 + ((i.c.c.Distance - 15) * 8)),
                   Amount = i.d.WorkType == 2 ? i.c.c.DeliveryCharge : i.c.c.IsPickupDrop == false? i.c.c.Distance <=5? (i.c.c.DeliveryChargeUpto5Km - 15) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 15)) : (i.c.c.DeliveryChargeUpto5Km - 15) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 15)) + ((i.c.c.Distance - 5) * i.c.c.DeliveryChargePerKm + ((i.c.c.DeliveryRatePercentage / 100) * i.c.c.DeliveryChargePerKm)):i.c.c.Distance<=5? (i.c.c.DeliveryChargeUpto5Km - 30) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 30)) : i.c.c.Distance <15? (i.c.c.DeliveryChargeUpto5Km - 30) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 30)) + ((i.c.c.Distance - 5) * 6 /*+ ((i.c.c.DeliveryRatePercentage / 100) * 6)*/): (i.c.c.DeliveryChargeUpto5Km - 30) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 30)) + (60 + ((i.c.c.Distance - 15) * 8)),
                   IfscCode = i.d.IFSCCode,
                   PaymentDate = i.c.p.DateEncoded,
                   PaymentId = "JOY" + i.c.p.OrderNumber.ToString(),
                   DeliveryBoyId = i.d.Id,
                   DeliveryBoyName = i.d.Name.ToString().Trim(),
                   DeliveryBoyPhoneNumber = i.d.PhoneNumber,
                   TransactionType = i.c.p.PaymentMode,
                   OrderNo = i.c.p.OrderNumber,
                   EmailBody = "",
                   EmailID = i.d.Email,
                   DeliveryBoyPaymentStatus = i.c.c.DeliveryBoyPaymentStatus,
                   PaymentMode = i.d.BankName.ToUpper() == "ICICI BANK" ? "FT" : "NEFT",
                   ShopName = i.c.c.ShopName,
                   COHAmount = (i.c.c.PaymentModeType == 2 && i.c.c.DeliveryBoyPaymentStatus == 0) ? (i.c.c.TotalPrice - i.c.p.RefundAmount ?? 0) : 0,
                   TipsAmount = i.c.c.TipsAmount,
                   //TotalDeliveryBoyAmount = i.d.WorkType == 1 ? ((i.c.c.Distance <= 5) ? 20 + ((i.c.c.DeliveryRatePercentage / 100) * 20) + i.c.c.TipsAmount : 20 + ((i.c.c.DeliveryRatePercentage / 100) * 20) + (i.c.c.IsPickupDrop == false ? ((i.c.c.Distance - 5) * 6 + ((i.c.c.DeliveryRatePercentage / 100) * 6)) : i.c.c.Distance <= 15 ? i.c.c.DeliveryCharge - 50 : (60 + ((i.c.c.Distance - 15) * 8))) + i.c.c.TipsAmount) : i.c.c.DeliveryCharge + i.c.c.TipsAmount
                   TotalDeliveryBoyAmount = i.d.WorkType == 2 ? i.c.c.DeliveryCharge +i.c.c.TipsAmount : i.c.c.IsPickupDrop == false ? i.c.c.Distance <= 5 ? (i.c.c.DeliveryChargeUpto5Km - 15) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 15)) + i.c.c.TipsAmount : (i.c.c.DeliveryChargeUpto5Km - 15) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 15)) + ((i.c.c.Distance - 5) * i.c.c.DeliveryChargePerKm + ((i.c.c.DeliveryRatePercentage / 100) * i.c.c.DeliveryChargePerKm)) + i.c.c.TipsAmount : i.c.c.Distance <= 5 ? (i.c.c.DeliveryChargeUpto5Km - 30) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 30)) + i.c.c.TipsAmount : i.c.c.Distance < 15 ? (i.c.c.DeliveryChargeUpto5Km - 30) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 30)) + ((i.c.c.Distance - 5) * 6 + i.c.c.TipsAmount/*+ ((i.c.c.DeliveryRatePercentage / 100) * 6)*/) : (i.c.c.DeliveryChargeUpto5Km - 30) + ((i.c.c.DeliveryRatePercentage / 100) * (i.c.c.DeliveryChargeUpto5Km - 30)) + (60 + ((i.c.c.Distance - 15) * 8)) + i.c.c.TipsAmount,
               }).OrderBy(i => i.PaymentDate).ToList();
            //}
            //else
            //{
            //    model.ListItems = db.Orders.Where(i => i.Status == 6)
            //       .Join(db.Payments.Where(i => (DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.EarningDate.Value))), c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
            //       .Join(db.DeliveryBoys, c => c.c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
            //       .Select(i => new DeliveryBoyPaymentListViewModel.ListItem
            //       {
            //           AccountName = i.d.AccountName.Trim(),
            //           AccountNumber = i.d.AccountNumber,
            //           AccountType = "SA",
            //           //Amount = i.d.WorkType == 1 ? ((i.c.c.DeliveryCharge == 35 || i.c.c.DeliveryCharge == 50) ? 20 : 20 + (i.c.c.IsPickupDrop == false ? i.c.c.DeliveryCharge - 35 : i.c.c.Distance <= 15 ? i.c.c.DeliveryCharge - 50 : (60 + ((i.c.c.Distance - 15) * 8)))) : i.c.c.DeliveryCharge,
            //           Amount = i.d.WorkType == 1 ? ((i.c.c.Distance <= 5) ? 20 + ((deliveryRatePercentage / 100) * 20) : 20 + ((deliveryRatePercentage / 100) * 20) + (i.c.c.IsPickupDrop == false ? ((i.c.c.Distance - 5) * 6 + ((deliveryRatePercentage / 100) * 6)) : i.c.c.Distance <= 15 ? i.c.c.DeliveryCharge - 50 : (60 + ((i.c.c.Distance - 15) * 8 + ((deliveryRatePercentage / 100) * 8))))) : i.c.c.DeliveryCharge,
            //           IfscCode = i.d.IFSCCode,
            //           PaymentDate = i.c.p.DateEncoded,
            //           PaymentId = "JOY" + i.c.p.OrderNumber.ToString(),
            //           DeliveryBoyId = i.d.Id,
            //           DeliveryBoyName = i.d.Name.ToString().Trim(),
            //           DeliveryBoyPhoneNumber = i.d.PhoneNumber,
            //           TransactionType = i.c.p.PaymentMode,
            //           OrderNo = i.c.p.OrderNumber,
            //           EmailBody = "",
            //           EmailID = i.d.Email,
            //           DeliveryBoyPaymentStatus = i.c.c.DeliveryBoyPaymentStatus,
            //           PaymentMode = i.d.BankName.ToUpper() == "ICICI BANK" ? "FT" : "NEFT",
            //           ShopName = i.c.c.ShopName,
            //           COHAmount = (i.c.c.PaymentModeType == 2 && i.c.c.DeliveryBoyPaymentStatus == 0) ? (i.c.c.TotalPrice - i.c.p.RefundAmount ?? 0) : 0,
            //           TipsAmount = i.c.c.TipsAmount,
            //           // TotalDeliveryBoyAmount = i.d.WorkType == 1 ? ((i.c.c.DeliveryCharge == 35 || i.c.c.DeliveryCharge == 50) ? 20 + i.c.c.TipsAmount : 20 + (i.c.c.IsPickupDrop == false ? i.c.c.DeliveryCharge - 35 : i.c.c.DeliveryCharge - 50) + i.c.c.TipsAmount) : i.c.c.DeliveryCharge + i.c.c.TipsAmount
            //           TotalDeliveryBoyAmount = i.d.WorkType == 1 ? ((i.c.c.Distance <= 5) ? 20 + ((deliveryRatePercentage / 100) * 20) + i.c.c.TipsAmount : 20 + ((deliveryRatePercentage / 100) * 20) + (i.c.c.IsPickupDrop == false ? ((i.c.c.Distance - 5) * 6 + ((deliveryRatePercentage / 100) * 6)) : i.c.c.Distance <= 15 ? i.c.c.DeliveryCharge - 50 : (60 + ((i.c.c.Distance - 15) * 8 + ((deliveryRatePercentage / 100) * 8)))) + i.c.c.TipsAmount) : i.c.c.DeliveryCharge + i.c.c.TipsAmount
            //       }).ToList();
            //}
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPYRP188")]
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

            model.ListItems = db.Payments.Where(i => ((DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate)) &&
                  (DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate))) && (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
                 // .Join(db.Shops, p => p.ShopId, s => s.Id, (p, s) => new { p, s })
                 .Join(db.Orders.Where(i => i.Status == 6), p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                 .GroupJoin(db.Offers, p => p.c.OfferId, o => o.Id, (p, o) => new { p, o })
                 .GroupJoin(db.PaymentsDatas, p => p.p.p.ReferenceCode, pd => pd.PaymentId, (p, pd) => new { p, pd })
                 .AsEnumerable()
                 .Select((i, index) => new RetailerPaymentListViewModel.ListItem
                 {
                     No = index + 1,
                     OrderDate = i.p.p.p.DateEncoded,
                     OrderFirstAmount = i.p.p.p.Amount,
                     OrderNumber = i.p.p.p.OrderNumber.ToString(),
                     PaidAmount = i.p.o.FirstOrDefault()?.OwnerType == 1 ? ((i.p.p.c.TotalShopPrice != 0 ? (i.p.p.c.TotalPrice - Math.Abs(i.p.p.c.TotalShopPrice - i.p.p.c.TotalPrice)) - (i.p.p.p.RefundAmount ?? 0) : i.p.p.p.Amount + i.p.p.c.OfferAmount) - (i.p.p.p.RefundAmount ?? 0)) : i.p.p.c.TotalShopPrice != 0 ? i.p.p.p.Amount - Math.Abs(i.p.p.c.TotalPrice - i.p.p.c.TotalShopPrice) : i.p.p.p.Amount - (i.p.p.p.RefundAmount ?? 0),
                   //PaidAmount = i.p.o.OwnerType == 1 ? ((i.p.p.p.Amount + i.p.p.c.OfferAmount) - (i.p.p.p.RefundAmount ?? 0)) : i.p.p.p.Amount - (i.p.p.p.RefundAmount ?? 0),
                   PaymentAmount = i.p.o.FirstOrDefault()?.OwnerType == 1 ? ((i.p.p.c.TotalShopPrice != 0 ? (i.p.p.p.Amount - Math.Abs(i.p.p.c.TotalPrice - i.p.p.c.TotalShopPrice)) : i.p.p.p.Amount + i.p.p.c.OfferAmount) - (i.p.p.p.RefundAmount ?? 0)) : (i.p.p.c.TotalShopPrice != 0 ? i.p.p.p.Amount - (Math.Abs(i.p.p.c.TotalPrice - i.p.p.c.TotalShopPrice)) : i.p.p.p.Amount) - (i.p.p.p.RefundAmount ?? 0) - Convert.ToDouble((i.pd.Any() ? i.pd.FirstOrDefault().Fee : 0)) - Convert.ToDouble((i.pd.Any() ? i.pd.FirstOrDefault().Tax : 0)),
                   //PaymentAmount = i.p.o.OwnerType == 1 ? ((i.p.p.p.Amount + i.p.p.c.OfferAmount) - (i.p.p.p.RefundAmount ?? 0)) : i.p.p.p.Amount - (i.p.p.p.RefundAmount ?? 0) - Convert.ToDouble((i.pd.Any() ? i.pd.FirstOrDefault().Fee : 0)) - Convert.ToDouble((i.pd.Any() ? i.pd.FirstOrDefault().Tax : 0)),
                   // PaymentAmount = i.p.p.Amount - (i.p.p.RefundAmount ?? 0) - Convert.ToDouble((i.pd.Any()? i.pd.FirstOrDefault().Fee : 0)) - Convert.ToDouble((i.pd.Any() ? i.pd.FirstOrDefault().Tax : 0)),
                   PaymentDate = i.p.p.p.DateEncoded,
                     PaymentId = i.p.p.p.ReferenceCode ?? "N/A",
                     PaymentType = i.p.p.p.PaymentMode,
                     RefundAmount = i.p.p.p.RefundAmount ?? 0,
                     RefundeRemark = i.p.p.p.RefundRemark ?? "N/A",
                     RefundStatus = i.p.p.p.RefundStatus,
                     ShopName = i.p.p.p.ShopName,
                     ShopPaymentStatus = i.p.p.c.ShopPaymentStatus,
                     TransactionFee = i.pd.Any() ? i.pd.FirstOrDefault().Fee : 0,
                     TransactionTax = i.pd.Any() ? i.pd.FirstOrDefault().Tax : 0
                 }).ToList();

            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPYC189")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCPYC189")]
        public ActionResult Create(PaymentCreditsViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.Where(i => i.Id == model.ShopId).FirstOrDefault();
            if (shop != null)
            {
                Payment pay = new Payment();
                pay.Amount = model.Amount + model.GSTAmount;
                pay.OriginalAmount = model.Amount;
                pay.GSTAmount = model.GSTAmount;
                pay.CreditType = model.CreditType;
                pay.ReferenceCode = model.ReferenceCode;
                pay.CustomerId = shop.CustomerId;
                pay.CustomerName = shop.CustomerName;
                pay.ShopId = shop.Id;
                pay.ShopName = shop.Name;
                pay.GSTINNumber = shop.GSTINNumber;
                pay.PaymentMode = "Online Payment";
                pay.Key = "Razor";
                pay.PaymentResult = "success";
                pay.Currency = "INR";
                pay.Credits = model.Amount.ToString();
                pay.PaymentCategoryType = 1;            // 0 - User, 1 - Shop
                pay.CreatedBy = user.Name;
                pay.UpdatedBy = user.Name;
                pay.DateEncoded = DateTime.Now;
                pay.DateUpdated = DateTime.Now;
                if (model.CreditType == 0)               // PlatformCredit
                {
                    if (model.ReferenceCode != null)
                        pay.PlatformCreditType = 1;     // Purchase
                    else
                        pay.PlatformCreditType = 2;     // Free
                }
                else if (model.CreditType == 1)          // DeliveryCredit
                {
                    if (model.ReferenceCode != null)
                        pay.DeliveryCreditType = 1;     // Purchase
                    else
                        pay.DeliveryCreditType = 2;     // Free
                }
                db.Payments.Add(pay);
                db.SaveChanges();

                // ShopCredit
                var isExist = db.ShopCredits.Any(i => i.CustomerId == shop.CustomerId);
                if (isExist)
                {
                    var sc = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
                    sc.DateUpdated = DateTime.Now;
                    if (model.CreditType == 0)
                        sc.PlatformCredit += pay.OriginalAmount;
                    else if (model.CreditType == 1)
                        sc.DeliveryCredit += pay.OriginalAmount;
                    db.Entry(sc).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    ShopCredit shopCredit = new ShopCredit
                    {
                        CustomerId = shop.CustomerId,
                        DateUpdated = DateTime.Now,
                        DeliveryCredit = model.CreditType == 1 ? pay.OriginalAmount : 0,
                        PlatformCredit = model.CreditType == 0 ? pay.OriginalAmount : 0,
                    };
                    db.ShopCredits.Add(shopCredit);
                    db.SaveChanges();
                }

                // Voucher
                model.Id = pay.Id;
                model.GSTINNumber = shop.GSTINNumber;
                model.OriginalAmount = pay.OriginalAmount;
                model.Address = shop.Address;
                model.Email = shop.Email;
                model.PhoneNumber = shop.PhoneNumber;
                model.Amount = pay.Amount;
                model.ReferenceCode = pay.ReferenceCode;
                model.DateEncoded = pay.DateEncoded;
                model.CustomerName = shop.CustomerName;
                return RedirectToAction("CheckOut", new { id = pay.Id });
            }
            return View();
        }

        [AccessPolicy(PageCode = "SNCPYCO190")]
        public ActionResult CheckOut(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var pay = db.Payments.FirstOrDefault(i => i.Id == id);
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == pay.OrderNumber);
            var model = new PaymentCreditsViewModel();
            if (pay != null)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == pay.ShopId);
                model.GSTINNumber = pay.GSTINNumber;
                model.OriginalAmount = pay.OriginalAmount;
                if (order != null)
                {
                    model.Address = order.DeliveryAddress;
                }
                model.Email = shop.Email;
                model.PhoneNumber = shop.PhoneNumber;
                model.Amount = pay.Amount;
                model.ReferenceCode = pay.ReferenceCode;
                model.ShopName = pay.ShopName;
                model.DateEncoded = pay.DateEncoded;
                model.CreditType = pay.CreditType;
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPYCL191")]
        public ActionResult CreditList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PaymentCreditsViewModel();
            model.creditLists = db.Payments.Where(i => (i.CreditType == 0 || i.CreditType == 1) && i.Amount != -20).Select(i => new PaymentCreditsViewModel.CreditList
            {
                Id = i.Id,
                ShopName = i.ShopName,
                CustomerName = i.CustomerName,
                Amount = i.Amount,
                CreditType = i.CreditType,
                ReferenceCode = i.ReferenceCode,
                DateEncoded = i.DateEncoded
            }).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model.creditLists);
        }

        [AccessPolicy(PageCode = "SNCPYOOR329")]
        public ActionResult OrderOfferReport()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new OrderOfferReportViewModel();
            model.ListItems = db.Orders.Where(i => i.Status == 6)
                .Join(db.Offers, o => o.OfferId, of => of.Id, (o, of) => new { o, of })
                .Select(i => new OrderOfferReportViewModel.ListItem
                {
                    OrderNumber = i.o.OrderNumber.ToString(),
                    OfferName = i.of.Name,
                    OfferCode = i.of.OfferCode,
                    PurchasedAmount = i.o.NetTotal,
                    OfferPercentage = i.of.Percentage,
                    OrderDate = i.o.DateEncoded,
                    SNCLossAmount = i.of.OwnerType == 1 ? i.o.OfferAmount : 0,
                    ShopLossAmount = i.of.OwnerType == 2 ? i.o.OfferAmount : 0
                }).ToList();
            return View(model.ListItems);
        }

        [AccessPolicy(PageCode = "SNCPYSBR330")]
        public ActionResult ShopBillDifferenceReport(ShopBillDifferenceReportViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.ShopBillDetails
                .GroupJoin(db.Payments, s => s.OrderNumber, p => p.OrderNumber, (s, p) => new { s, p })
                .GroupJoin(db.Orders, p => p.p.FirstOrDefault().OrderNumber, o => o.OrderNumber, (p, o) => new { p, o })
                .Where(i => i.o.FirstOrDefault().Status == 6 && ((model.StartDate != null && model.EndDate != null) ? DbFunctions.TruncateTime(i.o.FirstOrDefault().DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.o.FirstOrDefault().DateEncoded) <= DbFunctions.TruncateTime(model.EndDate) : true) && (model.ShopId != 0 ? i.o.FirstOrDefault().ShopId == model.ShopId : true))
                .Select(i => new ShopBillDifferenceReportViewModel.ListItem
                {
                    OrderId = i.o.FirstOrDefault().Id,
                    BillAmount = i.p.s.BillAmount,
                    BillNo = i.p.s.BillNo,
                    OrderNumber = i.p.s.OrderNumber.ToString(),
                    ShopName = i.o.FirstOrDefault().ShopName,
                    TotalPrice = Math.Round(i.o.FirstOrDefault().TotalPrice, 2),
                    DifferenceAmount = Math.Round(i.p.s.BillAmount - (i.o.FirstOrDefault().TotalPrice - (i.p.p.FirstOrDefault().RefundAmount ?? 0)), 2),
                    DifferencePercentage = Math.Round(((i.p.s.BillAmount - (i.o.FirstOrDefault().TotalPrice - (i.p.p.FirstOrDefault().RefundAmount ?? 0))) / i.p.s.BillAmount) * 100, 2),
                    OrderDate = i.o.FirstOrDefault().DateEncoded
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPYSBPR331")]
        public ActionResult ShopBillDifferencePendingReport(ShopBillDifferenceReportViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Orders.Where(i => i.Status == 6)
                .GroupJoin(db.Payments, o => o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
                .Where(i => !db.ShopBillDetails.Any(s => s.OrderNumber == i.o.OrderNumber) && ((model.StartDate != null && model.EndDate != null) ? DbFunctions.TruncateTime(i.o.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.o.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate) : true) && (model.ShopId != 0 ? i.o.ShopId == model.ShopId : true))
                .Select(i => new ShopBillDifferenceReportViewModel.ListItem
                {
                    OrderId = i.o.Id,
                    OrderNumber = i.o.OrderNumber.ToString(),
                    ShopName = i.o.ShopName,
                    TotalPrice = Math.Round(i.o.TotalPrice, 2),
                    BillAmount = 0,
                    OrderDate = i.o.DateEncoded
                }).ToList();
            return View(model);
        }

        public ActionResult MarkShopPaymentAsPaidInCart(DateTime date)
        {
            var paymentListByDate = db.Payments.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(date) && i.OrderNumber != 0).Select(i => i.OrderNumber).ToList();
            foreach (var orderno in paymentListByDate)
            {
                var orderList = db.Orders.Where(i => i.OrderNumber == orderno && i.Status == 6).ToList();
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

        public ActionResult ShopPay(int orderNo)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == orderNo && i.Status == 0 && i.ShopPaymentStatus == 0);
            order.ShopPaymentStatus = 1;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("RetailerPayment");
        }

        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status == 1)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetShopOwnerSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status == 1)).Select(i => new
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