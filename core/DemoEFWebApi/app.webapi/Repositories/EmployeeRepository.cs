using app.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace app.webapi.Repositories
{
    public class EmployeeRepository
    {
        private readonly SampleDbContext context;
        private readonly ILogger<EmployeeRepository> logger;

        //method 2 - as demonstrated in Startup
        //NOTE: the following can be used with just "services.AddScoped<EmployeeRepository>()"
        public EmployeeRepository(IConfiguration configuration, ILogger<EmployeeRepository> logger, ILoggerFactory loggerFactory, IServiceProvider provider)
        {
            this.logger = logger;
            logger.LogTrace("EmployeeRepository.constructor: Started..");
            this.context = new SampleDbContext(
                configuration.GetConnectionString("SampleDb"),
                loggerFactory,
                provider.GetService<ILogger<SampleDbContext>>()
                ); //TODO: better way (without refering EntityFramework Assemblies?
        }

        //method 1 - as demonstrated in Startup
        public EmployeeRepository(ILogger<EmployeeRepository> logger, SampleDbContext context)
        {
            this.logger = logger;
            logger.LogTrace("EmployeeRepository.constructor: Started..");
            this.context = context;
            this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public IEnumerable<Employee> GetAll()
        {
            logger.LogTrace("EmployeeRepository.GetAll: Started..");
            return context.Employees.ToList();
        }

        public Employee Get(int id)
        {
            logger.LogTrace($"EmployeeRepository.Get({id}): Started..");
            return context.Employees.FirstOrDefault(o => o.Empno == id);
        }

        public void Add(Employee entity)
        {
            logger.LogTrace($"EmployeeRepository.Add: Started..");
            context.Employees.Add(entity);
            context.SaveChanges();
        }

        public void Update(int empno, Employee entity)
        {
            logger.LogTrace($"EmployeeRepository.Update: Started..");

            logger.LogTrace($"EmployeeRepository.Update: Fetching entity for update..");
            var entityToUpdate = this.Get(empno);

            entityToUpdate.Ename = entity.Ename;
            entityToUpdate.Salary = entity.Salary;
            entityToUpdate.Deptno= entity.Deptno;
            logger.LogTrace($"EmployeeRepository.Update: Update Started...");
            context.SaveChanges();
        }

        public void Delete(int empno)
        {
            logger.LogTrace($"EmployeeRepository.Delete: Started..");
            logger.LogTrace($"EmployeeRepository.Delete: Fetching entity for update..");
            var entityToDelete = this.Get(empno);
            logger.LogTrace($"EmployeeRepository.Delete: Delete Started...");
            context.Employees.Remove(entityToDelete);
            context.SaveChanges();
        }
    }
}
