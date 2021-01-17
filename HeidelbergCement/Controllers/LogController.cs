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
            var result = await logService.ListAllLogs();
            if (result.Sucess)
                return Ok(result.Data);
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }


        [HttpPost]
        public async Task<IActionResult> WriteLog(LogMessage msg)
        {
            var result = await logService.WriteLog(msg);
            if (result.Sucess)
                return Ok(result.Data);
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
