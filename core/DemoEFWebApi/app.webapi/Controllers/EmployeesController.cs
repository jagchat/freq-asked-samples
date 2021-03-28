using app.data;
using app.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace app.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger<EmployeesController> logger;
        private readonly EmployeeRepository employeeRepository;

        public EmployeesController(ILogger<EmployeesController> logger, EmployeeRepository employeeRepository)
        {
            this.logger = logger;
            this.employeeRepository = employeeRepository;
        }

        // GET: api/<EmployeesController>
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            logger.LogTrace("EmployeesController.Get: Started..");
            return employeeRepository.GetAll();
        }

        // GET api/<EmployeesController>/5
        [HttpGet("{id}")]
        public Employee Get(int id)
        {
            logger.LogTrace($"EmployeesController.Get({id}): Started..");
            return employeeRepository.Get(id);
        }

        // POST api/<EmployeesController>
        [HttpPost]
        public IActionResult Post([FromBody] Employee employee)
        {
            logger.LogTrace($"EmployeesController.Post: Started..");
            try
            {
                employeeRepository.Add(employee);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR: Employee cannot be created", ex);
                return BadRequest("Employee cannot be created");
            }                        
        }

        // PUT api/<EmployeesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Employee employee)
        {
            logger.LogTrace($"EmployeesController.Post: Started..");
            try
            {
                employeeRepository.Update(id, employee);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR: Employee cannot be updated", ex);
                return BadRequest("Employee cannot be updated");
            }            
        }

        // DELETE api/<EmployeesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                employeeRepository.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR: Employee cannot be deleted", ex);
                return BadRequest("Employee cannot be deleted");
            }
        }
    }
}
