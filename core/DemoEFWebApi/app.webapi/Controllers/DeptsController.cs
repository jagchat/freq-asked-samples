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
    public class DeptsController : ControllerBase
    {
        private readonly ILogger<DeptsController> logger;
        private readonly DeptRepository deptRepository;

        public DeptsController(ILogger<DeptsController> logger, DeptRepository deptRepository)
        {
            this.logger = logger;
            this.deptRepository = deptRepository;
        }

        // GET: api/<DeptsController>
        [HttpGet]
        public IEnumerable<Dept> Get()
        {
            logger.LogTrace("DeptsController.Get: Started..");
            return deptRepository.GetAll();
        }

        // GET api/<DeptsController>/5
        [HttpGet("{id}")]
        public Dept Get(int id)
        {
            logger.LogTrace($"DeptsController.Get({id}): Started..");
            return deptRepository.Get(id);
        }

        // POST api/<DeptsController>
        [HttpPost]
        public IActionResult Post([FromBody] Dept dept)
        {
            logger.LogTrace($"DeptsController.Post: Started..");
            try
            {
                deptRepository.Add(dept);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR: Dept cannot be created", ex);
                return BadRequest("Dept cannot be created");
            }                        
        }

        // PUT api/<DeptsController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Dept dept)
        {
            logger.LogTrace($"DeptsController.Post: Started..");
            try
            {
                deptRepository.Update(id, dept);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR: Dept cannot be updated", ex);
                return BadRequest("Dept cannot be updated");
            }            
        }

        // DELETE api/<DeptsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                deptRepository.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR: Dept cannot be deleted", ex);
                return BadRequest("Dept cannot be deleted");
            }
        }
    }
}
