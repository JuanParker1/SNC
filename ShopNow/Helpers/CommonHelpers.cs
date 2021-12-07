using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.Helpers
{
    public class CommonHelpers
    {
        private static sncEntities db = new sncEntities();

        //public static string GetMasterProductName(int code)
        //{
        //    var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == code);
        //    var name = "N/A";
        //    if (masterProduct != null)
        //    {
        //        name = masterProduct.Name != null ? masterProduct.Name : "N/A";
        //    }
        //    return name;
        //}

        public static BillingDeliveryChargeViewModel GetDeliveryCharge(int shopId, double totalSize = 0, double totalWeight = 0)
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
            return (model);
        }
    }
}