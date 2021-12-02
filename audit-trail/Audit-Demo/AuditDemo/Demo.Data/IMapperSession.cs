using Demo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Data
{
    public interface IMapperSession
    {
        void BeginTransaction();
        Task Commit();
        Task Rollback();
        void CloseTransaction();
        Task Save(Employee entity);
        Task Delete(Employee entity);

        IQueryable<Employee> Employees { get; }
    }
}
