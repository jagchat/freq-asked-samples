using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Data
{
    public class SqlLoggingInterceptor : EmptyInterceptor
    {
        private readonly ILogger<IMapperSession> _logger;

        public SqlLoggingInterceptor(ILogger<IMapperSession> logger)
        {
            _logger = logger;
        }

        //public override void AfterTransactionCompletion(ITransaction tx)
        //{
        //    if (_logger != null)
        //    {
        //        //_logger.LogTrace("----2----");
        //    }
        //    base.AfterTransactionCompletion(tx);
        //}

        //public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        //{
        //    if (_logger != null)
        //    {
        //        for (int i = 0; i < propertyNames.Length; i++)
        //        {
        //            _logger.LogTrace($"--------> {propertyNames[i]} => {previousState[i]} => {currentState[i]} ");
        //        }
        //    }
        //    return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        //}

        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            if (_logger != null)
            {
                //_logger.LogTrace("----1----");
                _logger.LogTrace(sql.ToString());
            }
            return base.OnPrepareStatement(sql);
        }

    }
}
