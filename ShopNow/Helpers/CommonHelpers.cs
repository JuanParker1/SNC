﻿using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.Helpers
{
    public class CommonHelpers
    {
        private static ShopnowchatEntities db = new ShopnowchatEntities();

        public static string GetMasterProductName(string code)
        {
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Code == code);
            var name = "N/A";
            if (masterProduct != null)
            {
                name = masterProduct.Name != null ? masterProduct.Name : "N/A";
            }
            return name;
        }
    }
}