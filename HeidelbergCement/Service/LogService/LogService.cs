using AirtableApiClient;
using HeidelbergCement.Model;
using HeidelbergCement.Repository;
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
        private readonly ILogger<LogService> logger;

        private readonly IAirtableRepository airtableRepo;
        public LogService(ILogger<LogService> logger, IAirtableRepository airtableRepo)
        {
            this.logger = logger;
            this.airtableRepo = airtableRepo;
        }

        public async Task<ServiceResponse<List<LogMessage>>> ListAllLogs()
        {
            List<LogMessage> returnLogs = new List<LogMessage>();
            string errorMessage = string.Empty;
            var response = await airtableRepo.ListRecordsAsAsync();

            if (response.Success)
            {
                response?.Records?.ToList().ForEach(x =>
                {
                    returnLogs.Add(ExtractFromAirTabelRecord(x));
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
            ServiceResponse<List<LogMessage>> resp = null;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                resp = new ServiceResponse<List<LogMessage>>()
                {
                    Data = null,
                    Sucess = false,
                    Message = errorMessage
                };

            }
            else
            {
                resp = new ServiceResponse<List<LogMessage>>()
                {
                    Data = returnLogs
                };                
            }

            return resp;
        }

        public LogMessage ExtractFromAirTabelRecord(AirtableRecord record)
        {
            LogMessage logMessage = new LogMessage();
            if (record == null)
                return null;
            if (record.Fields.ContainsKey("Summary"))
                logMessage.Title = record.Fields["Summary"] as string;
            if (record.Fields.ContainsKey("Message"))
                logMessage.Text = record.Fields["Message"] as string;
            return logMessage;
        }


        public async Task<ServiceResponse<LogMessage>> WriteLog(LogMessage msg)
        {
            ServiceResponse<LogMessage> resp = null;

            var response = await airtableRepo.CreateRecordAsAsync(msg);

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
                resp = new ServiceResponse<LogMessage>()
                {
                    Data = null,
                    Sucess = false,
                    Message = errorMessage
                };
            }
            else
            {
                var record = response.Record;
                resp = new ServiceResponse<LogMessage>()
                {
                    Data = ExtractFromAirTabelRecord(record)
                };
            }


            return resp;
        }

    }
}
