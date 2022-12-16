using MockDoor.Data.Models.Headers;
using MockDoor.Shared.Models.Headers;

namespace MockDoor.Data.Tests.Mapping.Headers
{
    internal static class HeaderMappingTestData
    {
        #region Service Headers
        public static ServiceHeader BasicServiceHeader { get; } = new ServiceHeader
        {
            ID = 123,
            Enabled = true,
            Name = "TestHeaderName",
            Incoming = true,
            MicroserviceID = 456,
            Outgoing = true
        };
        public static ServiceHeader BasicServiceHeader2 { get; } = new ServiceHeader
        {
            ID = 321,
            Enabled = true,
            Name = "TestHeaderName2",
            Incoming = true,
            MicroserviceID = 654,
            Outgoing = true
        };

        public static ServiceHeaderDto BasicServiceHeaderDto { get; } = new ServiceHeaderDto
        {
            Enabled = true,
            Name = "TestHeaderNameDto",
            Incoming = true,
            Outgoing = true
        };

        public static ServiceHeaderDto BasicServiceHeaderDto2 { get; } = new ServiceHeaderDto
        {
            Enabled = true,
            Name = "TestHeaderNameDto2",
            Incoming = true,
            Outgoing = true
        };
        #endregion

        #region Service Requests
        public static RequestHeader BasicServiceRequestHeader { get; } = new RequestHeader
        {
            ID = 789,
            Name = "testHeaderName",
            Value = "testValue;testValue2",
            ServiceRequestID = 1011
        };
        public static RequestHeader BasicServiceRequestHeader2 { get; } = new RequestHeader
        {
            ID = 987,
            Name = "testHeaderName",
            Value = "testValue3;testValue4",
            ServiceRequestID = 1013
        };

        public static ServiceRequestHeaderDto BasicServiceRequestHeaderDto { get; } = new ServiceRequestHeaderDto
        {
            Name = "testHeaderName",
            Value = "testValue;testValue2"
        };

        public static ServiceRequestHeaderDto BasicServiceRequestHeaderDto2 { get; } = new ServiceRequestHeaderDto
        {
            Name = "testHeaderName2",
            Value = "testValue3;testValue4"
        };
        #endregion

        #region Response Headers
        public static ResponseHeader BasicResponseHeader { get; } = new ResponseHeader
        {
            ID = 1314,
            Name = "testHeaderName",
            MockResponseID = 1516,
            Value = "testValue1;testValue2"
        };

        public static ResponseHeader BasicResponseHeader2 { get; } = new ResponseHeader
        {
            ID = 4131,
            Name = "testHeaderName2",
            MockResponseID = 1517,
            Value = "testValue3;testValue4"
        };

        public static MockResponseHeaderDto BasicMockResponseHeaderDto { get; } = new MockResponseHeaderDto
        {
            Name = "testHeaderName",
            Value = "testValue;testValue2"
        };

        public static MockResponseHeaderDto BasicMockResponseHeaderDto2 { get; } = new MockResponseHeaderDto
        {
            Name = "testHeaderName2",
            Value = "testValue3;testValue4"
        };
        #endregion
    }
}
