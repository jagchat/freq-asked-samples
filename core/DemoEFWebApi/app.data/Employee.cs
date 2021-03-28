using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace app.data
{
    [Table("Emp")]
    public class Employee
    {
        [Key]
        public int Empno { get; set; }
        public string Ename { get; set; }
        [Column("Sal")]
        public double Salary { get; set; }
        public int Deptno { get; set; }
    }
}
