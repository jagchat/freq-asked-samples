using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoBot.Models
{
    public class Department
    {
        public string Dname { get; set; }
        public int Deptno { get; set; }

        public string GetFormattedOuptut()
        {
            return $"Dept Info of #{Deptno} -> Name: {Dname}";
        }
    }
}