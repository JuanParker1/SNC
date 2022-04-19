using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShopNow.Helpers
{
    public class AchievementHelpers
    {
        public static void UpdateAchievements(int customerId)
        {
            sncEntities db = new sncEntities();
            DateTime achievementStartDateTime = new DateTime(2022, 04, 19);
            var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
            if (customer != null)
            {
                var achievementlist = db.AchievementSettings.Where(i => i.Status == 0).ToList();
                foreach (var item in achievementlist)
                {
                    switch (item.CountType)
                    {
                        case 1:
                            if (item.HasAccept == true)
                            {
                                var customerAcceptedAchievements = db.CustomerAchievements.FirstOrDefault(i => i.Status == 1 && i.CustomerId == customer.Id && i.AchievementId == item.Id);
                                if (customerAcceptedAchievements != null)
                                {
                                    if (item.DayLimit > 0)
                                    {
                                        DateTime achievementExpirydate = customerAcceptedAchievements.DateEncoded.AddDays(item.DayLimit);
                                        var orderListSelectShop = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(customerAcceptedAchievements.DateEncoded) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate))).Select(i => i.ShopId).ToArray();
                                        var shopCategoryCount = db.Shops.Where(i => orderListSelectShop.Contains(i.Id)).GroupBy(i => i.ShopCategoryId).Count();
                                        if (shopCategoryCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }
                                    else
                                    {
                                        var orderListSelectShop = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime)).Select(i => i.ShopId).ToArray();
                                        var shopCategoryCount = db.Shops.Where(i => orderListSelectShop.Contains(i.Id)).GroupBy(i => i.ShopCategoryId).Count();
                                        if (shopCategoryCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (item.DayLimit > 0)
                                {
                                    DateTime achievementExpirydate = achievementStartDateTime.AddDays(item.DayLimit);
                                    var orderListSelectShop = db.Orders.ToList().Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate))).Select(i => i.ShopId);
                                    var shopCategoryCount = db.Shops.Where(i => orderListSelectShop.Contains(i.Id)).GroupBy(i => i.ShopCategoryId).Count();
                                    if (shopCategoryCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                                else
                                {
                                    var orderListSelectShop = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime)).Select(i => i.ShopId);
                                    var shopCategoryCount = db.Shops.Where(i => orderListSelectShop.Contains(i.Id)).GroupBy(i => i.ShopCategoryId).Count();
                                    if (shopCategoryCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                            }
                            break;
                        case 2:
                            if (item.HasAccept == true)
                            {
                                var customerAcceptedAchievements = db.CustomerAchievements.FirstOrDefault(i => i.Status == 1 && i.CustomerId == customer.Id && i.AchievementId == item.Id);
                                if (customerAcceptedAchievements != null)
                                {
                                    if (item.DayLimit > 0)
                                    {
                                        DateTime achievementExpirydate = customerAcceptedAchievements.DateEncoded.AddDays(item.DayLimit);
                                        var orderListShopCount = db.Orders.ToList().Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(customerAcceptedAchievements.DateEncoded) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate))).GroupBy(i => i.ShopId).Count();
                                        if (orderListShopCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }
                                    else
                                    {
                                        var orderListShopCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime)).GroupBy(i => i.ShopId).Count();
                                        if (orderListShopCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (item.DayLimit > 0)
                                {
                                    DateTime achievementExpirydate = achievementStartDateTime.AddDays(item.DayLimit);
                                    var orderListShopCount = db.Orders.ToList().Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate))).GroupBy(i => i.ShopId).Count();
                                    if (orderListShopCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                                else
                                {
                                    var orderListShopCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime)).GroupBy(i => i.ShopId).Count();
                                    if (orderListShopCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                            }
                            break;
                        case 3:
                            if (item.HasAccept == true)
                            {
                                var customerAcceptedAchievements = db.CustomerAchievements.FirstOrDefault(i => i.Status == 1 && i.CustomerId == customer.Id && i.AchievementId == item.Id);
                                if (customerAcceptedAchievements != null)
                                {
                                    var achievementSelectedShopList = db.AchievementShops.Where(i => i.AchievementId == item.Id).Select(i => i.ShopId).ToList();
                                    if (item.DayLimit > 0)
                                    {
                                        DateTime achievementExpirydate = customerAcceptedAchievements.DateEncoded.AddDays(item.DayLimit);
                                       // var orderListShopCount = db.Orders.ToList().Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(customerAcceptedAchievements.DateEncoded) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate)) && achievementSelectedShopList.Contains(i.ShopId)).GroupBy(i => i.ShopId).Count();
                                        var orderListShopCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(customerAcceptedAchievements.DateEncoded) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate)) && achievementSelectedShopList.Contains(i.ShopId)).GroupBy(i => i.ShopId).Count();
                                        if (orderListShopCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }
                                    else
                                    {
                                        var orderListShopCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && achievementSelectedShopList.Contains(i.ShopId)).GroupBy(i => i.ShopId).Count();
                                        if (orderListShopCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                var achievementSelectedShopList = db.AchievementShops.Where(i => i.AchievementId == item.Id).Select(i => i.ShopId).ToList();
                                if (item.DayLimit > 0)
                                {
                                    DateTime achievementExpirydate = achievementStartDateTime.AddDays(item.DayLimit);
                                    //var orderListShopCount = db.Orders.ToList().Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate)) && achievementSelectedShopList.Contains(i.ShopId)).GroupBy(i => i.ShopId).Count();
                                    var orderListShopCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate)) && achievementSelectedShopList.Contains(i.ShopId)).GroupBy(i => i.ShopId).Count();
                                    if (orderListShopCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                                else
                                {
                                    var orderListShopCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && achievementSelectedShopList.Contains(i.ShopId)).GroupBy(i => i.ShopId).Count();
                                    if (orderListShopCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                            }
                            break;
                        case 4:
                            if (item.HasAccept == true)
                            {
                                var customerAcceptedAchievements = db.CustomerAchievements.FirstOrDefault(i => i.Status == 1 && i.CustomerId == customer.Id && i.AchievementId == item.Id);
                                if (customerAcceptedAchievements != null)
                                {
                                    if (item.DayLimit > 0)
                                    {
                                        DateTime achievementExpirydate = customerAcceptedAchievements.DateEncoded.AddDays(item.DayLimit);
                                        var orderListProductCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(customerAcceptedAchievements.DateEncoded) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate)))
                                            .Join(db.OrderItems.GroupBy(i => i.ProductId), o => o.Id, oi => oi.FirstOrDefault().OrderId, (o, oi) => new { o, oi })
                                            .Count();
                                        if (orderListProductCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }
                                    else
                                    {
                                        var orderListProductCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime))
                                             .Join(db.OrderItems.GroupBy(i => i.ProductId), o => o.Id, oi => oi.FirstOrDefault().OrderId, (o, oi) => new { o, oi })
                                            .Count();
                                        if (orderListProductCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (item.DayLimit > 0)
                                {
                                    DateTime achievementExpirydate = achievementStartDateTime.AddDays(item.DayLimit);
                                    var orderListProductCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate)))
                                        .Join(db.OrderItems.GroupBy(i => i.ProductId), o => o.Id, oi => oi.FirstOrDefault().OrderId, (o, oi) => new { o, oi })
                                            .Count();
                                    if (orderListProductCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                                else
                                {
                                    var orderListProductCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime))
                                        .Join(db.OrderItems.GroupBy(i => i.ProductId), o => o.Id, oi => oi.FirstOrDefault().OrderId, (o, oi) => new { o, oi })
                                            .Count();
                                    if (orderListProductCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                            }
                            break;
                        case 5:
                            if (item.HasAccept == true)
                            {
                                var customerAcceptedAchievements = db.CustomerAchievements.FirstOrDefault(i => i.Status == 1 && i.CustomerId == customer.Id && i.AchievementId == item.Id);
                                if (customerAcceptedAchievements != null)
                                {
                                    var achievementSelectedProductList = db.AchievementProducts.Where(i => i.AchievementId == item.Id).Select(i => i.ProductId).ToList();
                                    if (item.DayLimit > 0)
                                    {
                                        DateTime achievementExpirydate = customerAcceptedAchievements.DateEncoded.AddDays(item.DayLimit);
                                        var orderListProductCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(customerAcceptedAchievements.DateEncoded) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate)))
                                            .Join(db.OrderItems.Where(i => achievementSelectedProductList.Contains(i.ProductId)).GroupBy(i => i.ProductId), o => o.Id, oi => oi.FirstOrDefault().OrderId, (o, oi) => new { o, oi })
                                            .Count();
                                        if (orderListProductCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }
                                    else
                                    {
                                        var orderListProductCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime))
                                             .Join(db.OrderItems.Where(i => achievementSelectedProductList.Contains(i.ProductId)).GroupBy(i => i.ProductId), o => o.Id, oi => oi.FirstOrDefault().OrderId, (o, oi) => new { o, oi })
                                            .Count();
                                        if (orderListProductCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                var achievementSelectedProductList = db.AchievementProducts.Where(i => i.AchievementId == item.Id).Select(i => i.ProductId).ToList();
                                if (item.DayLimit > 0)
                                {
                                    DateTime achievementExpirydate = achievementStartDateTime.AddDays(item.DayLimit);
                                    var orderListProductCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate)))
                                        .Join(db.OrderItems.Where(i => achievementSelectedProductList.Contains(i.ProductId)).GroupBy(i => i.ProductId), o => o.Id, oi => oi.FirstOrDefault().OrderId, (o, oi) => new { o, oi })
                                            .Count();
                                    if (orderListProductCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                                else
                                {
                                    var orderListProductCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime))
                                        .Join(db.OrderItems.Where(i => achievementSelectedProductList.Contains(i.ProductId)).GroupBy(i => i.ProductId), o => o.Id, oi => oi.FirstOrDefault().OrderId, (o, oi) => new { o, oi })
                                            .Count();
                                    if (orderListProductCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                            }
                            break;
                        case 6:
                            if (item.HasAccept == true)
                            {
                                var customerAcceptedAchievements = db.CustomerAchievements.FirstOrDefault(i => i.Status == 1 && i.CustomerId == customer.Id && i.AchievementId == item.Id);
                                if (customerAcceptedAchievements != null)
                                {
                                    if (item.DayLimit > 0)
                                    {
                                        DateTime achievementExpirydate = customerAcceptedAchievements.DateEncoded.AddDays(item.DayLimit);
                                        var orderListCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(customerAcceptedAchievements.DateEncoded) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate))).Count();
                                        if (orderListCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }
                                    else
                                    {
                                        var orderListCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime)).Count();
                                        if (orderListCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (item.DayLimit > 0)
                                {
                                    DateTime achievementExpirydate = achievementStartDateTime.AddDays(item.DayLimit);
                                    var orderListCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate))).Count();
                                    if (orderListCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                                else
                                {
                                    var orderListCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime)).Count();
                                    if (orderListCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                            }
                            break;
                        case 7:
                            if (item.HasAccept == true)
                            {
                                var customerAcceptedAchievements = db.CustomerAchievements.FirstOrDefault(i => i.Status == 1 && i.CustomerId == customer.Id && i.AchievementId == item.Id);
                                if (customerAcceptedAchievements != null)
                                {
                                    if (item.DayLimit > 0)
                                    {
                                        DateTime achievementExpirydate = customerAcceptedAchievements.DateEncoded.AddDays(item.DayLimit);
                                        var orderListCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(customerAcceptedAchievements.DateEncoded) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate))).Count();
                                        if (orderListCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }
                                    else
                                    {
                                        var orderListCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime)).Count();
                                        if (orderListCount == item.CountValue)
                                        {
                                            customer.WalletAmount += item.Amount;
                                            db.Entry(customer).State = EntityState.Modified;
                                            db.SaveChanges();

                                            //Wallet History
                                            AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                            //Completed Achievement
                                            AddCustomerCompletedAchievement(customer.Id, item.Id);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (item.DayLimit > 0)
                                {
                                    DateTime achievementExpirydate = achievementStartDateTime.AddDays(item.DayLimit);
                                    var orderListCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(achievementExpirydate))).Count();
                                    if (orderListCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);

                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);
                                    }
                                }
                                else
                                {
                                    var orderListCount = db.Orders.Where(i => i.CustomerId == customer.Id && i.Status == 6 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(achievementStartDateTime)).Count();
                                    if (orderListCount == item.CountValue)
                                    {
                                        customer.WalletAmount += item.Amount;
                                        db.Entry(customer).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Wallet History
                                        AddAchievementCustomerWalletHistory(customer.Id, item.Amount, item.Name);
                                        //Completed Achievement
                                        AddCustomerCompletedAchievement(customer.Id, item.Id);

                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        public static void AddAchievementCustomerWalletHistory(int custId, double amount, string achievementName)
        {
            sncEntities db = new sncEntities();
            var walletHistory = new CustomerWalletHistory();
            walletHistory.Amount = amount;
            walletHistory.CustomerId = custId;
            walletHistory.DateEncoded = DateTime.Now;
            walletHistory.Description = $"Received from Achievement({achievementName})";
            walletHistory.Type = 1;
            walletHistory.ExpiryDate = DateTime.Now.AddDays(10);
            db.CustomerWalletHistories.Add(walletHistory);
            db.SaveChanges();
        }

        public static void AddCustomerCompletedAchievement(int custId,int achievementId)
        {
            sncEntities db = new sncEntities();
            var completedAchievement = new CustomerAchievementCompleted();
            completedAchievement.CustomerId = custId;
            completedAchievement.AchievementId = achievementId;
            completedAchievement.Status = 0;
            completedAchievement.DateEncoded = DateTime.Now;
            db.CustomerAchievementCompleteds.Add(completedAchievement);
            db.SaveChanges();
        }
    }
}