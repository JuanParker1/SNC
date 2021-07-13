﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class AccessPolicyListViewModel
    {
        public string Code { get; set; }
        public string PageCode { get; set; }
        public string PageName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int Position { get; set; }
        public int Status { get; set; }

        public List<AccessPolicy> List { get; set; }
        public class AccessPolicy
        {
            public string Code { get; set; }
            public string PageCode { get; set; }
            public string PageName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string StaffCode { get; set; }
            public string StaffName { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public bool IsAccess { get; set; }
            public int Position { get; set; }
            public int Status { get; set; }
        }

        public List<AccessManage> ManageList { get; set; }
        public class AccessManage
        {
            public string Code { get; set; }
            public string PageCode { get; set; }
            public string PageName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string StaffCode { get; set; }
            public string StaffName { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public bool IsAccess { get; set; }
            public int Position { get; set; }
            public int Status { get; set; }
        }
    }

    public class AccessPolicyViewModel
    {
        public string Code { get; set; }
        public string PageCode { get; set; }
        public string PageName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public bool IsAccess { get; set; }
        public int Position { get; set; }
        public int Status { get; set; }
    }
    public class AccessPolicyMasterViewModel
    {
        public string PageCode { get; set; }
        public string PageName { get; set; }
        public int Position { get; set; }
        public int Status { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
    public class AccessPolicyCreateEditViewModel
    {
        public string Code { get; set; }
        public string PageCode { get; set; }
        public string PageName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public bool IsAccess { get; set; }
        public int Position { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class AccessPolicyItemListViewModel
    {
       
        public List<AccessPolicy> List { get; set; }
        public class AccessPolicy
        {
            public string Code { get; set; }
            public string PageCode { get; set; }
            public string PageName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string StaffCode { get; set; }
            public string StaffName { get; set; }
            public int Position { get; set; }
            public int Status { get; set; }
        }
    }

}