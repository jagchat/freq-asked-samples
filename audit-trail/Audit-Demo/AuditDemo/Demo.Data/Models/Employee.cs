using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Data.Models
{
    public class Employee
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual double Salary { get; set; }
        public virtual int DeptId { get; set; }
    }
}
