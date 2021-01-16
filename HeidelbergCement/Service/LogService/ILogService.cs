using HeidelbergCement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeidelbergCement.Service.LogService
{
    public interface ILogService
    {
        Task<bool> WriteLog(LogMessage msg);
        Task<List<LogMessage>> ListAllLogs();
    }
}
