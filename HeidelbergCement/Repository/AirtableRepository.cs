using AirtableApiClient;
using HeidelbergCement.HelperClass;
using HeidelbergCement.Model;
using HeidelbergCement.Service.LogService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeidelbergCement.Repository
{
    public class AirtableRepository : IAirtableRepository
    {

        readonly string baseId = "appD1b1YjWoXkUJwR";
        readonly string appKey;

        private readonly ILogger<LogService> logger;
        private readonly IConfiguration configuration;

        public AirtableRepository(ILogger<LogService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.appKey = configuration.GetSection("appKey").Value;     
        }

        public async Task<AirtableCreateUpdateReplaceRecordResponse> CreateRecordAsAsync(LogMessage msg)
        {
            AirtableCreateUpdateReplaceRecordResponse response;
            using (AirtableBase airtableBase = new AirtableBase(appKey, baseId))
            {
                var fields = new Fields();
                fields.AddField(Constants.ID, msg.Id);
                fields.AddField(Constants.SUMMARY, msg.Title);
                fields.AddField(Constants.MESSAGE, msg.Text);
                fields.AddField(Constants.RECEIVEDAT, msg.ReceivedAt);
                Task<AirtableCreateUpdateReplaceRecordResponse> task = airtableBase.CreateRecord("Messages", fields, true);
                response = await task;               
            }
            
            return response;
        }

        public async Task<AirtableListRecordsResponse> ListRecordsAsAsync()
        {

            AirtableListRecordsResponse response;
            using (AirtableBase airtableBase = new AirtableBase(appKey, baseId))
            {
                Task<AirtableListRecordsResponse> task = airtableBase.ListRecords("Messages");
                response = await task;                
            }

            return response;
        }
    }
}
