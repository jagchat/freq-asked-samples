using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchDemo.Web.Models
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            Results = new List<SearchResultItem>();
        }

        public List<SearchResultItem> Results { get; set; }
    }
}