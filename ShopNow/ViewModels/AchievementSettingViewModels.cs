using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class AchievementSettingListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public string ShopDistrict { get; set; }
            public string Name { get; set; }
            public int CountType { get; set; }
            public int CountValue { get; set; }
            public double Amount { get; set; }

            public string CountTypeText
            {
                get
                {
                    switch (this.CountType)
                    {
                        case 1:
                            return "Category Count";
                        case 2:
                            return "All Shops Count";
                        case 3:
                            return "Selected Shops Count";
                        case 4:
                            return "All Products Count";
                        case 5:
                            return "Selected Products Count";
                        case 6:
                            return "All Order Count";
                        default:
                            return "N/A";
                    }
                }
            }
        }
    }

    public class AchievementSettingCreateViewModel
    {
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
        public int[] ShopIds { get; set; }
        public long[] ProductIds { get; set; }
    }

    public class AchievementSettingEditViewModel : AchievementSettingCreateViewModel
    {
        public int Id { get; set; }
        public string ShopIdstring { get; set; }
        public string ProductIdstring { get; set; }
        public string ShopNames { get; set; }
        public string ProductNames { get; set; }
    }
}