//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopNow.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AchievementSetting
    {
        public int Id { get; set; }
        public string ShopDistrict { get; set; }
        public string Name { get; set; }
        public int CountType { get; set; }
        public int CountValue { get; set; }
        public double Amount { get; set; }
        public bool HasAccept { get; set; }
        public int DayLimit { get; set; }
        public int ActivateType { get; set; }
        public int ActivateAfterId { get; set; }
        public int RepeatCount { get; set; }
        public bool IsForBlackListAbusers { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
    }
}
