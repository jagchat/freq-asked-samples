using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _empRep;

        public EmployeesController(IEmployeeRepository empRep)
        {
            _empRep = empRep;
        }
        public IActionResult Index()
        {
            return View(_empRep.GetAllEmployees());
        }
    }
}