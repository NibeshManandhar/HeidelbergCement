using AirtableApiClient;
using HeidelbergCement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeidelbergCement.Repository
{
    public interface IAirtableRepository
    {
        public Task<AirtableListRecordsResponse> ListRecordsAsAsync();
        public Task<AirtableCreateUpdateReplaceRecordResponse> CreateRecordAsAsync(LogMessage msg);
    }
}
