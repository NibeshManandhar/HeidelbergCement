using AirtableApiClient;
using HeidelbergCement.HelperClass;
using HeidelbergCement.Model;
using HeidelbergCement.Repository;
using HeidelbergCement.Service.LogService;
using HidelbergCementTest.TestHelperClasses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HidelbergCementTest
{
    public class LogServiceTest
    {
        private readonly LogService _sut;

        private readonly Mock<ILogger<LogService>> logger = new Mock<ILogger<LogService>>();
        private readonly Mock<IConfiguration> configuration = new Mock<IConfiguration>();
        private readonly Mock<IAirtableRepository> airtableRepo = new Mock<IAirtableRepository>();
        public LogServiceTest()
        {
            //_configuration.Setup(x => x.GetSection("appKey").Value).Returns("DummyAppKey");
            _sut = new LogService(logger.Object, airtableRepo.Object);
        }
      

        [Fact]
        public async Task WriteLog_ShouldReturn_ServiceResponseWithBoolTrueAndLogMessageAsData_WhenSucessful()
        {
            //Arrange
            LogMessage logMessage = new LogMessage() { Title = "Test Title insert", Text = "Test Text insert Mockup" };
            //_configuration.Setup(x => x.GetSection("appKey").Value).Returns("DummyAppKey");

            AirtableRecord airtableRecord = new AirtableRecord();
            airtableRecord.Fields.Add("Summary", logMessage.Title);
            airtableRecord.Fields.Add("Message", logMessage.Text);

            airtableRepo.Setup(x => x.CreateRecordAsAsync(logMessage)).ReturnsAsync(new AirtableCreateUpdateReplaceRecordResponse(airtableRecord));
            //Act
            ServiceResponse<LogMessage> result = await _sut.WriteLog(logMessage);

            //Assert
            Assert.True(result.Sucess);
            Assert.Equal(result.Data, logMessage, new LogMessageCompare());
        }


        [Fact]
        public async Task WriteLog_ShouldReturn_ServiceResponseWithBoolFalseAndWithoutLogMessageAsData_WhenFailed()
        {
            //Arrange
            LogMessage logMessage = new LogMessage() { Title = "Test Title insert", Text = "Test Text insert Mockup" };


            airtableRepo.Setup(x => x.CreateRecordAsAsync(logMessage)).ReturnsAsync(new AirtableCreateUpdateReplaceRecordResponse(new AirtableBadRequestException()));
            //Act
            ServiceResponse<LogMessage> result = await _sut.WriteLog(logMessage);

            //Assert
            Assert.True(!result.Sucess);
            Assert.Null(result.Data);
        }


        [Fact]
        public async Task ListAllLogs_ShouldReturn_ServiceResponseWithBoolTrue_WhenSucessful()
        {
            //Arrange
       
            //AirtableRecord airtableRecord = new AirtableRecord();
            //airtableRecord.Fields.Add("Summary", "Test");
            //airtableRecord.Fields.Add("Message", "Test");


            //List<AirtableRecord> airtableRecords = new List<AirtableRecord>() { airtableRecord };

            //var airtableListRecordsResponseMock = Mock.Of<AirtableListRecordsResponse>(m =>
            //                               m.Success == true &&
            //                               m.Records == airtableRecords);


            AirtableListRecordsResponse airtableListRecordsResponse = new AirtableListRecordsResponse(new AirtableRecordList());

            airtableRepo.Setup(x => x.ListRecordsAsAsync()).ReturnsAsync(airtableListRecordsResponse);
            //Act
            ServiceResponse<List<LogMessage>> result = await _sut.ListAllLogs();

            //Assert
            Assert.True(result.Sucess);
        }



        [Fact]
        public async Task ListAllLogs_ShouldReturn_ServiceResponseWithBoolFalse_WhenFailed()
        {
            //Arrange
            AirtableListRecordsResponse airtableListRecordsResponse = new AirtableListRecordsResponse(new AirtableBadRequestException());
            airtableRepo.Setup(x => x.ListRecordsAsAsync()).ReturnsAsync(airtableListRecordsResponse);
            //Act
            ServiceResponse<List<LogMessage>> result = await _sut.ListAllLogs();

            //Assert
            Assert.False(result.Sucess);
        }


        [Fact]
        public void ExtractFromAirTabelRecord_ShouldReturnLogMessageObject_FromAirTabelRecord()
        {
            //Arrange
            AirtableRecord airtableRecord = new AirtableRecord();
            airtableRecord.Fields.Add(Constants.ID, "Test ID");
            airtableRecord.Fields.Add(Constants.SUMMARY, "Test Title");
            airtableRecord.Fields.Add(Constants.MESSAGE, "This is test Message");
            airtableRecord.Fields.Add(Constants.RECEIVEDAT, DateTime.Now);

            //Act
            LogMessage result = _sut.ExtractLogMessageFromAirTabelRecord(airtableRecord);

            //Assert            
            Assert.Equal(result.Id, airtableRecord.Fields[Constants.ID]);
            Assert.Equal(result.Title, airtableRecord.Fields[Constants.SUMMARY]);
            Assert.Equal(result.Text, airtableRecord.Fields[Constants.MESSAGE]);
            Assert.Equal(result.ReceivedAt, airtableRecord.Fields[Constants.RECEIVEDAT]);
        }
    }
}
