using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace d3sample.Models
{
    public class OutputItem
    {
        public string name { get; set; }
        public string parent { get; set; }
        public List<OutputItem> children { get; set; }
    }
}