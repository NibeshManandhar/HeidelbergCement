using AirtableApiClient;
using HeidelbergCement.Model;
using HeidelbergCement.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeidelbergCement.HelperClass;

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

        /// <summary>
        /// Service to List all messages
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<LogMessage>>> ListAllLogs()
        {
            List<LogMessage> returnLogs = new List<LogMessage>();
            string errorMessage = string.Empty;
            var response = await airtableRepo.ListRecordsAsAsync();

            if (response.Success)
            {
                //List through all Airtable Records object and create LogMessage Object list
                response?.Records?.ToList().ForEach(x =>
                {
                    returnLogs.Add(ExtractLogMessageFromAirTabelRecord(x));
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

        /// <summary>
        /// AirtableRecord to LogMessage Object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public LogMessage ExtractLogMessageFromAirTabelRecord(AirtableRecord record)
        {
            LogMessage logMessage = new LogMessage();
            if (record == null)
                return null;
            if (record.Fields.ContainsKey(Constants.ID))
                logMessage.Id = record.Fields[Constants.ID] as string;
            if (record.Fields.ContainsKey(Constants.SUMMARY))
                logMessage.Title = record.Fields[Constants.SUMMARY] as string;
            if (record.Fields.ContainsKey(Constants.MESSAGE))
                logMessage.Text = record.Fields[Constants.MESSAGE] as string;
            if (record.Fields.ContainsKey(Constants.RECEIVEDAT))
                logMessage.ReceivedAt = record.Fields[Constants.RECEIVEDAT] as DateTime?;            
            return logMessage;
        }


        /// <summary>
        /// Service to Create Message:
        /// </summary>
        /// <param name="msg">Parsed JSON object from  Body with two attributes "title" and “text"</param>
        /// <returns></returns>
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
                    Data = ExtractLogMessageFromAirTabelRecord(record)
                };
            }


            return resp;
        }

    }
}
