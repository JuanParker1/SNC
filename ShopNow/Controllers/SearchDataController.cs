using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class SearchDataController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "SNCSDL246")]
        public ActionResult List(SearchDataListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            //var model = new SearchDataListViewModel();
            model.AllListItems = db.CustomerSearchDatas.Where(i => i.Status == 0 && (model.StartDate != null && model.EndDate != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate)) : false)
                .GroupBy(i => i.SearchKeyword)
                .Select(i => new SearchDataListViewModel.ListItem
                {
                    Count = i.Max(a => a.ResultCount),
                    Date = i.FirstOrDefault().DateEncoded,
                    Key = i.Key,
                }).OrderByDescending(i => i.Date).ToList();

            model.ZeroCountListItems = db.CustomerSearchDatas
                .Where(i => i.Status == 0 && ((model.StartDate != null && model.EndDate != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate)) : true) && string.IsNullOrEmpty(i.LinkedMasterProductIds))
                .AsEnumerable()
                 .GroupBy(i => i.SearchKeyword, StringComparer.InvariantCultureIgnoreCase).Where(i => i.Sum(a => a.ResultCount) == 0)
                 .GroupJoin(db.SearchDatas, k => k.Key?.ToLower(), sd => sd.KeyValue?.ToLower(), (k, sd) => new { k, sd })
                .Select(i => new SearchDataListViewModel.ListItem
                {
                    Id = i.k.FirstOrDefault().Id,
                    Count = i.k.Max(a => a.ResultCount),
                    Date = i.k.FirstOrDefault().DateEncoded,
                    Key = i.k.Key,
                    OldCommonWord = string.Join(",", i.sd.Select(a => a.Source).ToList()).ToString(),
                    IsLinked = i.k.Any(a => a.LinkedMasterProductIds != null)
                }).Where(i => string.IsNullOrEmpty(i.OldCommonWord) && i.IsLinked != true).OrderByDescending(i => i.Date).ToList();

            model.ListWithLinkedKeywords = db.CustomerSearchDatas
              .Where(i => i.Status==0  && ((model.StartDate != null && model.EndDate != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate)) : true))
              .AsEnumerable()
               .GroupBy(i => i.SearchKeyword, StringComparer.InvariantCultureIgnoreCase).Where(i => i.Sum(a => a.ResultCount) == 0)
               .GroupJoin(db.SearchDatas, k => k.Key?.ToLower(), sd => sd.KeyValue?.ToLower(), (k, sd) => new { k, sd })
              .Select(i => new SearchDataListViewModel.ListItem
              {
                  Id = i.k.FirstOrDefault().Id,
                  Count = i.k.Max(a => a.ResultCount),
                  Date = i.k.FirstOrDefault().DateEncoded,
                  Key = i.k.Key,
                  OldCommonWord = string.Join(",", i.sd.Select(a => a.Source).ToList()).ToString(),
                  LinkedMasterProduct = i.k.FirstOrDefault().LinkedMasterProductName
              }).Where(i => !string.IsNullOrEmpty(i.OldCommonWord) || !string.IsNullOrEmpty(i.LinkedMasterProduct)).OrderByDescending(i => i.Date).ToList();

            model.AllCount = model.AllListItems.Count();
            model.ZeroCount = model.ZeroCountListItems.Count();
            model.LinkedKeywordCount = model.ListWithLinkedKeywords.Count();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSDE247")]
        public ActionResult Entry(string str = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new SearchDataEntryViewModel();
            model.KeywordLists = db.KeywordDatas.ToList().Where(i => str != "" ? i.Name.StartsWith(str) : true)
                .GroupJoin(db.SearchDatas, k => k.Name, sd => sd.Source, (k, sd) => new { k, sd })
                .Select(i => new SearchDataEntryViewModel.KeywordList
                {
                    Keyword = i.k.Name,
                    AvailableKeyword = string.Join(",", i.sd.Select(a => a.KeyValue).ToList()).ToString()
                }).OrderBy(i => i.Keyword).ToList();
            ViewBag.Str = str;
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSDE247")]
        public JsonResult Add(string keyword, string keys)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (!string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(keys))
            {
                var keyList = keys.Split(',').ToList();
                foreach (var key in keyList)
                {
                    var isExist = db.SearchDatas.Any(i => i.Source == keyword && i.KeyValue == key);
                    if (!isExist)
                    {
                        var searchData = new SearchData
                        {
                            KeyValue = key.Trim(),
                            Source = keyword
                        };
                        db.SearchDatas.Add(searchData);
                        db.SaveChanges();
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddToSourceAndNickname(int id, string searchWord, string[] searchSource, long[] masterIds)
        {
            if (masterIds == null && searchSource == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (masterIds != null)
            {
                foreach (var masterId in masterIds)
                {
                    var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == masterId);
                    string[] result = masterProduct.NickName.Split(' ');
                    if (!result.Contains(searchWord))
                    {
                        masterProduct.NickName = masterProduct.NickName + " " + searchWord;
                        db.Entry(masterProduct).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    var searchDataList = db.CustomerSearchDatas.Where(i => i.SearchKeyword.ToLower().Replace("  ", " ") == searchWord.ToLower().Replace("  ", " ")).ToList();
                    foreach (var item in searchDataList)
                    {
                        var sd = db.CustomerSearchDatas.FirstOrDefault(i => i.Id == item.Id && i.ResultCount == 0);
                        if (sd != null)
                        {
                            sd.LinkedMasterProductIds = sd.LinkedMasterProductIds != null ? sd.LinkedMasterProductIds + "," + masterProduct.Id : masterProduct.Id.ToString();
                            sd.LinkedMasterProductName = sd.LinkedMasterProductName != null ? sd.LinkedMasterProductName + "," + masterProduct.Name : masterProduct.Name;
                            db.Entry(sd).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //
                    var searchData = db.CustomerSearchDatas.FirstOrDefault(i => i.Id == id);
                    if (searchData != null)
                    {
                        searchData.LinkedMasterProductIds = searchData.LinkedMasterProductIds != null ? searchData.LinkedMasterProductIds + "," + masterProduct.Id : masterProduct.Id.ToString();
                        searchData.LinkedMasterProductName = searchData.LinkedMasterProductName != null ? searchData.LinkedMasterProductName + "," + masterProduct.Name : masterProduct.Name;
                        db.Entry(searchData).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            if (searchSource != null)
            {
                foreach (var source in searchSource)
                {
                    var isExist = db.SearchDatas.Any(i => i.Source == source && i.KeyValue == searchWord);
                    if (!isExist)
                    {
                        var searchData = new SearchData
                        {
                            KeyValue = searchWord.Trim(),
                            Source = source
                        };
                        db.SearchDatas.Add(searchData);
                        db.SaveChanges();
                    }


                    var custSearchData = db.CustomerSearchDatas.FirstOrDefault(i => i.Id == id);
                    if (custSearchData != null)
                    {
                        custSearchData.LinkedMasterProductIds = custSearchData.LinkedMasterProductIds != null ? custSearchData.LinkedMasterProductIds + "," + source : source;
                        custSearchData.LinkedMasterProductName = custSearchData.LinkedMasterProductName != null ? custSearchData.LinkedMasterProductName + "," + source : source;
                        db.Entry(custSearchData).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveCustomerSearch(int id, DateTime? startDate, DateTime? endDate)
        {
            var searchData = db.CustomerSearchDatas.FirstOrDefault(i => i.Id == id);
            searchData.Status = 2;
            db.Entry(searchData).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", new { StartDate = startDate, EndDate = endDate });
        }

        public async Task<JsonResult> GetKeywordSelect2(string q = "")
        {
            var model = await db.KeywordDatas.Where(a => a.Name.StartsWith(q)).Take(50).OrderBy(i => i.Name)
                .Select(i => new
                {
                    id = i.Name,
                    text = i.Name
                }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}