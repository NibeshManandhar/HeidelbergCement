using HeidelbergCement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeidelbergCement.Service.LogService
{
    public interface ILogService
    {
        Task<ServiceResponse<LogMessage>> WriteLog(LogMessage msg);
        Task<ServiceResponse<List<LogMessage>>> ListAllLogs();
    }
}
