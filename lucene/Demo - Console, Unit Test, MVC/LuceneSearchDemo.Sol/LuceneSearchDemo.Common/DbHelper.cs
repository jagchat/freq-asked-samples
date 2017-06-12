using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuceneSearchDemo.Common;

namespace LuceneSearchDemo.Common
{
    public class DbHelper
    {
        //TODO: refactor
        public static DataSet GetDataSet(string sqlQuery)
        {
            DataSet ds = new DataSet();
            var sqlCon = new SqlConnection(Utils.GetConnectionString());
            var sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlCon;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlQuery;
            var sqlAdap = new SqlDataAdapter(sqlCmd);
            sqlAdap.Fill(ds);
            return ds;
        }
    }
}
