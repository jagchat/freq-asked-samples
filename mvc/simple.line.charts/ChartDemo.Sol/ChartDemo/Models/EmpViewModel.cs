using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace ChartDemo.Models
{
    public class EmpViewModel
    {
        public List<EmployeeItem> EmployeeData { get; set; }

        public void LoadData()
        {
            EmployeeData = new List<EmployeeItem>()
            {
                new EmployeeItem() {Ename = "Jag", Sal = 3400},
                new EmployeeItem() {Ename = "Chat", Sal = 5400},
                new EmployeeItem() {Ename = "Scott", Sal = 6400},
                new EmployeeItem() {Ename = "White", Sal = 2400},
                new EmployeeItem() {Ename = "Smith", Sal = 7400},
                new EmployeeItem() {Ename = "Robert", Sal = 4400}
            };
        }

        //public Chart GetChart()
        //{
        //    return new Chart(600, 400, ChartTheme.Blue)
        //        .AddTitle("Employee Salary Chart")
        //        .AddLegend()
        //        .AddSeries(
        //            name: "EmpSalChart",
        //            chartType: "Pie",
        //            xValue: EmployeeData.Select(o => o.Ename),
        //            yValues: EmployeeData.Select(o => o.Sal));
        //}

        public Chart GetChart()
        {
            return new Chart(600, 400, ChartTheme.Blue)
                .AddTitle("Employee Salary Chart")
                .AddLegend()
                .AddSeries(
                    name: "Salaries",
                    chartType: "Line",
                    xValue: EmployeeData,
                    yValues: EmployeeData,
                    xField: "Ename",
                    yFields:"Sal");
        }
    }
}