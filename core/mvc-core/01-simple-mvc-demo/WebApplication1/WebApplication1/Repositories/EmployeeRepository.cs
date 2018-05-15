using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _emps;
        public EmployeeRepository()
        {
            _emps = new List<Employee>
            {
                new Employee() { Empno = 1001, Ename = "Jag", Sal = 3400, Deptno = 10 },
                new Employee() { Empno = 1002, Ename = "Chat", Sal = 4400, Deptno = 20 },
                new Employee() { Empno = 1003, Ename = "Scott", Sal = 2400, Deptno = 10 },
                new Employee() { Empno = 1004, Ename = "Smith", Sal = 3200, Deptno = 20 },
            };
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _emps;
        }

        public Employee GetEmployeeById(int empno)
        {
            return _emps.FirstOrDefault(o => o.Empno == empno);
        }
    }
}
