﻿using ShopNow.Models;
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

        public ActionResult List(SearchDataListViewModel model)
        {
            //var model = new SearchDataListViewModel();
            model.AllListItems = db.CustomerSearchDatas.Where(i => (model.StartDate != null && model.EndDate != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate)) : true)
                .GroupBy(i => i.SearchKeyword)
                .Select(i => new SearchDataListViewModel.ListItem
                {
                    Count = i.Max(a => a.ResultCount),
                    Date = i.FirstOrDefault().DateEncoded,
                    Key = i.Key,
                }).OrderByDescending(i => i.Date).ToList();
            model.ZeroCountListItems = db.CustomerSearchDatas.Where(i => i.ResultCount == 0 && ((model.StartDate != null && model.EndDate != null) ? (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate)) : true))
                 .GroupBy(i => i.SearchKeyword)
                 .GroupJoin(db.SearchDatas, k => k.Key, sd => sd.KeyValue, (k, sd) => new { k, sd })
                 .AsEnumerable()
                .Select(i => new SearchDataListViewModel.ListItem
                {
                    Count = i.k.Max(a => a.ResultCount),
                    Date = i.k.FirstOrDefault().DateEncoded,
                    Key = i.k.Key,
                    OldCommonWord = string.Join(",", i.sd.Select(a => a.Source).ToList()).ToString()
                }).OrderByDescending(i => i.Date).ToList();
            model.AllCount = model.AllListItems.Count();
            model.ZeroCount = model.ZeroCountListItems.Count();
            return View(model);
        }

        public ActionResult Entry(string str = "")
        {
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

        public JsonResult Add(string keyword, string keys)
        {
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

        public JsonResult AddToSourceAndNickname(string searchWord, string[] searchSource, long[] masterIds)
        {
            if (masterIds == null && searchSource == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (masterIds != null)
            {
                foreach (var id in masterIds)
                {
                    var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == id);
                    if (!masterProduct.NickName.Contains(searchWord))
                    {
                        masterProduct.NickName = masterProduct.NickName + " " + searchWord;
                        db.Entry(masterProduct).State = EntityState.Modified;
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
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
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