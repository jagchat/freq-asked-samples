using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LuceneSearchDemo.Common;
using LuceneSearchDemo.Service.Library.Employees;

namespace LuceneSearchDemo.Web.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Details(string empno)
        {
            //TODO: refactor
            var ds = DbHelper.GetDataSet("SELECT * FROM emp WHERE empno = " + empno);
            var vm = new EmployeeSearchResultItem();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var dr = ds.Tables[0].Rows[0];
                vm.Empno = dr["Empno"].ToString();
                vm.Ename = dr["Ename"].ToString();
                vm.Sal = dr["Sal"].ToString();
                vm.Deptno = dr["Deptno"].ToString();
            }
            return PartialView(vm);
        }
    }
}