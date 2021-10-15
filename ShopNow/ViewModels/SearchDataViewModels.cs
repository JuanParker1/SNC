using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class SearchDataListViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<ListItem> AllListItems { get; set; }
        public List<ListItem> ZeroCountListItems { get; set; }
        public List<ListItem> ListWithLinkedKeywords { get; set; }
        public class ListItem
        {
            public DateTime? Date { get; set; }
            public string Key { get; set; }
            public int? Count { get; set; }
            //public string Color { get; set; }
            public string OldCommonWord { get; set; }
            public string LinkedMasterProduct { get; set; }
        }

        public int AllCount { get; set; }
        public int ZeroCount { get; set; }
        public int LinkedKeywordCount { get; set; }
    }

    public class SearchDataEntryViewModel
    {
        public List<KeywordList> KeywordLists { get; set; }
        public class KeywordList
        {
            public string Keyword { get; set; }
            public string AvailableKeyword { get; set; }
        }
    }
}