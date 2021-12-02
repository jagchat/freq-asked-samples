using Demo.Data.Models;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Data.Maps
{
    public class EmployeeMap : ClassMapping<Employee>
    {
        public EmployeeMap()
        {
            Id(x => x.Id, x =>
            {
                x.Type(NHibernateUtil.Int32);
                x.Column("Empno");
            });

            Property(b => b.Name, x =>
            {
                x.Length(50);
                x.Type(NHibernateUtil.String);
                x.NotNullable(true);
                x.Column("Ename");
            });

            Property(b => b.Salary, x =>
            {
                x.Type(NHibernateUtil.Double);
                x.Column("Sal");
            });

            Property(b => b.DeptId, x =>
            {
                x.Type(NHibernateUtil.Int32);
                x.NotNullable(true);
                x.Column("Deptno");
            });

            DynamicUpdate(true);
            Table("Emp");
        }
    }
}
