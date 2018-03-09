using Dapper;
using DemoBot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using NLog;

namespace DemoBot.Repository
{
    //TODO: All, just for demo
    public class DbRepository
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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

        public object GetEmpCount()
        {
            logger.Trace("Invoking Db operation 'GetEmpCount'..");

            try
            {
                using (IDbConnection db = new SqlConnection(GetConnectionString()))
                {
                    return db.ExecuteScalar<int>($"Select COUNT(*) From emp");
                }

            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        public Employee GetEmpById(string empno)
        {
            logger.Trace("Invoking Db operation 'GetEmpById'..");

            using (IDbConnection db = new SqlConnection(GetConnectionString()))
            {
                return db.Query<Employee>($"Select * From Emp Where Empno = {empno}").FirstOrDefault();
            }
        }

        public Department GetDeptById(string deptno)
        {
            logger.Trace("Invoking Db operation 'GetDeptById'..");

            using (IDbConnection db = new SqlConnection(GetConnectionString()))
            {
                return db.Query<Department>($"Select * From Dept Where Deptno = {deptno}").FirstOrDefault();
            }
        }

        public void CreateEmp(Employee emp)
        {
            logger.Trace("Invoking Db operation 'CreateEmp'..");

            using (IDbConnection db = new SqlConnection(GetConnectionString()))
            {
                db.Execute($"INSERT INTO Emp(Empno, Ename, Sal, Deptno) VALUES ({emp.Empno}, '{emp.Ename}', {emp.Sal}, {emp.Deptno})");
            }
        }
    }
}