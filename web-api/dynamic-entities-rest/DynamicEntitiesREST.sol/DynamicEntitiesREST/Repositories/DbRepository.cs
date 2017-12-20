using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace DynamicEntitiesREST.Repositories
{
    public class DbRepository //TODO: create ApiRepository in similar (generic) fashion
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
        }

        public object[] GetAll(string entityName)
        {

            //TODO: generic to our API Collection Fill and return entities
            //TODO: would be config driven (using existing designer)
            //TODO: can inject hooks here
            //TODO: hard coded for now with direct sql db
            using (IDbConnection db = new SqlConnection(GetConnectionString()))
            {
                return db.Query($"Select * From {entityName}").ToArray(); //just for demo
            }
        }

        public object GetById(string entityName, string id)
        {
            //TODO: generic to our API Collection Filter, Fill and return entity
            //TODO: would be config driven (using existing designer)
            //TODO: can inject hooks here
            //TODO: hard coded for now with direct sql db
            using (IDbConnection db = new SqlConnection(GetConnectionString()))
            {
                //expects primary key to be always "id", if not read from config
                //return db.Query($"Select * From {entityName} Where id = {id}").FirstOrDefault(); //just for demo

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

    }
}