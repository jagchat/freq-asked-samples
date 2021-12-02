using Demo.Data.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Data
{
    public class NHibernateMapperSession : IMapperSession
    {
        private readonly ISession _session;
        private ITransaction _transaction;

        public NHibernateMapperSession(ISession session)
        {
            _session = session;
        }

        public IQueryable<Employee> Employees => _session.Query<Employee>();

        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }

        public async Task Commit()
        {
            await _transaction.CommitAsync();
        }

        public async Task Rollback()
        {
            await _transaction.RollbackAsync();
        }

        public void CloseTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task Save(Employee entity)
        {
            await _session.SaveOrUpdateAsync(entity);
        }

        public async Task Delete(Employee entity)
        {
            await _session.DeleteAsync(entity);
        }

    }
}
