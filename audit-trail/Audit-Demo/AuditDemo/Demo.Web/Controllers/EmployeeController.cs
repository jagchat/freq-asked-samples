using Demo.Audit;
using Demo.Data;
using Demo.Data.Models;
using Demo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IMapperSession _session;
        
        public EmployeeController(ILogger<EmployeeController> logger,
            IMapperSession session)
        {
            _logger = logger;
            _session = session;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogTrace("EmployeeController.Index (trace): Started..");
            var emps = await _session.Employees
                                      .ToListAsync();
            _logger.LogTrace("EmployeeController.Index (trace): Completed..");
            return View(emps);
        }

        [Route("~/Employee/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            _logger.LogTrace($"EmployeeController.GetEmployee ({id}): Started..");
            var emp = await _session.Employees.FirstOrDefaultAsync(e => e.Id == int.Parse(id));
            _logger.LogTrace($"EmployeeController.GetEmployee ({id}): Completed..");
            return View(emp);
        }


        [Route("~/Employee/{id}")]
        [HttpPost]
        public async Task<IActionResult> Update(string id, Employee inputModel)
        {
            _logger.LogTrace($"EmployeeController.Update ({id}): Started..");
            try
            {
                var emp = await _session.Employees.FirstOrDefaultAsync(e => e.Id == int.Parse(id));
                emp.Name = inputModel.Name;
                emp.Salary = inputModel.Salary;
                emp.DeptId = inputModel.DeptId;

                _session.BeginTransaction();

                await _session.Save(emp);
                await _session.Commit();

                //var result = new
                //{
                //    EntityType = "Employee",
                //    //((NHibernate.Persister.Entity.SingleTableEntityPersister)(@event).Persister).TableName
                //    TableName = GetTableName(persister.EntityName),
                //    IdCol = persister.EntityMetamodel.IdentifierProperty.Name,
                //    IdValue = @event.Id.ToString(),
                //    UpdateDate = DateTime.Now,
                //    Properties = currentState
                //};
                //var o = JsonSerializer.Serialize(result);
                //_auditService.Publish(o);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ERROR at EmployeeController.Update");
                await _session.Rollback();
            }
            finally
            {
                _session.CloseTransaction();
            }

            _logger.LogTrace($"EmployeeController.Update ({id}): Completed..");
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    
    }
}
