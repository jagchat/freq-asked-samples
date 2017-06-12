using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LuceneSearchDemo.Service.Library.Employees;

namespace SearchDemo.Web.Models
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            Results = new List<EmployeeSearchResultItem>();
        }

        public List<EmployeeSearchResultItem> Results { get; set; }
    }
}