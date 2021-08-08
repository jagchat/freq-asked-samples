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
    public class DeptRepository
    {
        private readonly SampleDbContext context;
        private readonly ILogger<DeptRepository> logger;

        //method 2 - as demonstrated in Startup
        //NOTE: the following can be used with just "services.AddScoped<DeptRepository>()"
        public DeptRepository(IConfiguration configuration, ILogger<DeptRepository> logger, ILoggerFactory loggerFactory, IServiceProvider provider)
        {
            this.logger = logger;
            logger.LogTrace("DeptRepository.constructor: Started..");
            this.context = new SampleDbContext(
                configuration.GetConnectionString("SampleDb"),
                loggerFactory,
                provider.GetService<ILogger<SampleDbContext>>()
                ); //TODO: better way (without refering EntityFramework Assemblies?
        }

        //method 1 - as demonstrated in Startup
        public DeptRepository(ILogger<DeptRepository> logger, SampleDbContext context)
        {
            this.logger = logger;
            logger.LogTrace("DeptRepository.constructor: Started..");
            this.context = context;
            this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public IEnumerable<Dept> GetAll()
        {
            logger.LogTrace("DeptRepository.GetAll: Started..");
            return context.Depts.ToList();
        }

        public Dept Get(int id)
        {
            logger.LogTrace($"DeptRepository.Get({id}): Started..");
            return context.Depts.FirstOrDefault(o => o.Deptno == id);
        }

        public void Add(Dept entity)
        {
            logger.LogTrace($"DeptRepository.Add: Started..");
            context.Depts.Add(entity);
            context.SaveChanges();
        }

        public void Update(int deptno, Dept entity)
        {
            logger.LogTrace($"DeptRepository.Update: Started..");

            logger.LogTrace($"DeptRepository.Update: Fetching entity for update..");
            var entityToUpdate = this.Get(deptno);

            entityToUpdate.Dname = entity.Dname;
            logger.LogTrace($"DeptRepository.Update: Update Started...");
            context.SaveChanges();
        }

        public void Delete(int deptno)
        {
            logger.LogTrace($"DeptRepository.Delete: Started..");
            logger.LogTrace($"DeptRepository.Delete: Fetching entity for update..");
            var entityToDelete = this.Get(deptno);
            logger.LogTrace($"DeptRepository.Delete: Delete Started...");
            context.Depts.Remove(entityToDelete);
            context.SaveChanges();
        }
    }
}
