using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeidelbergCement.Model;
using HeidelbergCement.Service.LogService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeidelbergCement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService logService;
        public LogController(ILogService logService)
        {
            this.logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> RetreiveAllLogs()
        {
            return Ok(await logService.ListAllLogs());
        }



        [HttpPost]
        public async Task<IActionResult> WriteLog(LogMessage msg)
        {
            if (await logService.WriteLog(msg))
                return Ok("Create Message sucessfully");
            else
                return BadRequest();
        }
    }
}
