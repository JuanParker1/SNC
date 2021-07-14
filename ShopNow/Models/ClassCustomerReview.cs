
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.Models
{
    public class ClassCustomerReview
    {
        private const string _prefix = "CRW";

        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }

        public int Id { get; set; }
        public int Id { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerRemark { get; set; }
        public int Rating { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }

        //Default Methods

        private static ShopnowchatEntities _db = new ShopnowchatEntities();

        public static CustomerReview Get(string code)
        {
            try
            {

                return _db.CustomerReviews.FirstOrDefault(i => i.Code == code && i.Status == 0) ?? (CustomerReview)null;
            }
            catch
            {
                return (CustomerReview)null;
            }
        }


        public static bool Validate(CustomerReview model, out int error)
        {
            try
            {
                error = 0;
                return true;
            }
            catch (Exception)
            {
                error = -9;
                return false;
            }
        }

        public static string Add(CustomerReview model, out int error)
        {
            try
            {
                if (Validate(model, out error))
                {
                    model.Code = _generatedCode;
                    model.Status = 0;
                    model.DateEncoded = DateTime.Now;
                    model.DateUpdated = DateTime.Now;
                    _db.CustomerReviews.Add(model);
                    _db.SaveChanges();
                    return model.Code;
                }
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                error = -9;
                return string.Empty;
            }
        }

        public static bool Edit(CustomerReview model, out int error)
        {
            try
            {
                if (Validate(model, out error))
                {
                    model.DateUpdated = DateTime.Now;
                    _db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                error = -9;
                return false;
            }
        }

        public static List<CustomerReview> GetList(string code)
        {
            try
            {
                return _db.CustomerReviews.Where(i => i.Status == 0 && i.ShopCode == code).ToList() ?? new List<CustomerReview>();
            }
            catch
            {
                return new List<CustomerReview>();
            }
        }


    }
}

