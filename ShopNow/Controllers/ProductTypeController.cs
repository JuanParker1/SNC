using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class ProductTypeController : Controller
    {
        private sncEntities db = new sncEntities();

        public async Task<JsonResult> GetSelect2List(string q = "")
        {
            var model = await db.ProductTypes.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i=>i.id).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}