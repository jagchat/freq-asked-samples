using Dapper;
using DemoBot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DemoBot.Repository
{
    public class DbRepository
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
        }

        public object[] GetAll(string entityName)
        {
            using (IDbConnection db = new SqlConnection(GetConnectionString()))
            {
                return db.Query($"Select * From {entityName}").ToArray(); //just for demo
            }
        }

        public object GetById(string entityName, string id)
        {
            using (IDbConnection db = new SqlConnection(GetConnectionString()))
            {
                if (entityName.ToLower() == "emp")
                {
                    return db.Query($"Select * From {entityName} Where Empno = {id}").FirstOrDefault(); //just for demo
                }
                else if (entityName.ToLower() == "dept")
                {
                    return db.Query($"Select * From {entityName} Where deptno = {id}").FirstOrDefault(); //just for demo
                }
                return db.Query($"Select * From {entityName} Where id = {id}").FirstOrDefault(); //just for demo
            }
        }

        public object ExecuteOperation(ParseResult p) {

            switch (p.Operation.ToUpper()) {
                case "COUNT":
                    using (IDbConnection db = new SqlConnection(GetConnectionString()))
                    {
                        return db.ExecuteScalar<int>($"Select COUNT(*) From { p.TableName}");
                    }
                    break;
            }

            return null;
        }

    }
}