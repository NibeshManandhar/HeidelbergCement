using AirtableApiClient;
using HeidelbergCement.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeidelbergCement.Service.LogService
{
    public class LogService : ILogService
    {
        readonly string baseId = "appD1b1YjWoXkUJwR";
        readonly string appKey;

        private readonly ILogger<LogService> logger;
        private readonly IConfiguration configuration;

        public LogService(ILogger<LogService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;            
            this.appKey = configuration.GetValue<string>("appKey");
        }

        public async Task<List<LogMessage>> ListAllLogs()
        {
            List<LogMessage> returnLog = new List<LogMessage>();
            using (AirtableBase airtableBase = new AirtableBase(appKey, baseId))
            {

                Task<AirtableListRecordsResponse> task = airtableBase.ListRecords("Messages");

                AirtableListRecordsResponse response = await task;
                string errorMessage = string.Empty;
                if (response.Success)
                {
                    response.Records.ToList().ForEach(x =>
                    {
                        returnLog.Add(ExtractFromAirTabelRecord(x));
                    });
                }
                else if (response.AirtableApiError is AirtableApiException)
                {
                    errorMessage = response.AirtableApiError.ErrorMessage;
                }
                else
                {
                    errorMessage = "Unknown error";
                }
                return returnLog;
            }



            // if (!string.IsNullOrEmpty(errorMessage))
            // {
            //     new List<LogMessage>();
            // }
            // else
            // {
            //     return returnLog;
            // }
        }

        private LogMessage ExtractFromAirTabelRecord(AirtableRecord record)
        {
            LogMessage logMessage = new LogMessage();
            if (record.Fields.ContainsKey("Summary"))
                logMessage.Text = record.Fields["Summary"] as string;
            if (record.Fields.ContainsKey("Message"))
                logMessage.Title = record.Fields["Message"] as string;
            return logMessage;

        }


        public async Task<bool> WriteLog(LogMessage msg)
        {
            using (AirtableBase airtableBase = new AirtableBase(appKey, baseId))
            {
                var fields = new Fields();
                fields.AddField("Summary", msg.Title);
                fields.AddField("Message", msg.Text);
                Task<AirtableCreateUpdateReplaceRecordResponse> task = airtableBase.CreateRecord("Messages", fields, true);
                var response = await task;

                if (!response.Success)
                {
                    string errorMessage = null;
                    if (response.AirtableApiError is AirtableApiException)
                    {
                        errorMessage = response.AirtableApiError.ErrorMessage;
                    }
                    else
                    {
                        errorMessage = "Unknown error";
                    }

                    logger.LogError(errorMessage, response.AirtableApiError);
                    return false;
                }
                else
                {
                    var record = response.Record;
                    return true;
                }
            }
        }

    }
}
