using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class FAQListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public int FAQCategoryId { get; set; }
            public string FAQCategoryName { get; set; }
            public string Description { get; set; }
        }
    }
    public class FAQCreateViewModel
    {
        public int FAQCategoryId { get; set; }
        public string Description { get; set; }
    }

    public class FAQEditViewModel : FAQCreateViewModel
    {
        public int Id { get; set; }
        public string FAQCategoryName { get; set; }
    }
}