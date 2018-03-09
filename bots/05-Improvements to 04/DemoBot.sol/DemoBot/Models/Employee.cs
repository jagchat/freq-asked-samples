using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoBot.Models
{
    public class Employee
    {
        
        public int Empno { get; set; }
        public string Ename { get; set; }
        public double Sal { get; set; }
        public int Deptno { get; set; }

        public string GetFormattedOuptut()
        {
            return $"Employee Info of #{Empno} -> Name: {Ename}, Salary: {Sal}, Department: {Deptno}";
        }        

    }
}