using System.Collections.Generic;
using System.Net;
using MockDoor.Data.Helpers;
using MockDoor.Data.Models;
using MockDoor.Data.Models.Headers;
using MockDoor.Data.Tests.Mapping.Headers;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Headers;
using MockDoor.Shared.Models.Response;

namespace MockDoor.Data.Tests.Mapping.Response
{
    internal static class ResponseMappingTestData
    {
        public static MockResponse BasicMockResponse { get; } = new MockResponse
        {
            ID = 1,
            Body = "test body",
            Code = HttpStatusCode.Created,
            ContentType = "application/json",
            Encoding = SupportedEncodingType.UNICODE,
            Headers = new List<ResponseHeader> { HeaderMappingTestData.BasicResponseHeader, HeaderMappingTestData.BasicResponseHeader2 },
            ServiceRequest = new ServiceRequest()
            {
                ID = 11,
                FromBody = "test"
            },
            ServiceRequestId = 2,
            Checksum = ChecksumHelpers.CreateChecksum(SupportedEncodingType.UNICODE, "test")
        };

        public static MockResponse BasicMockResponse2 { get; } = new MockResponse
        {
            ID = 2,
            Body = "test body 2",
            Code = HttpStatusCode.NotFound,
            ContentType = "application/json2",
            Encoding = SupportedEncodingType.UNICODE,
            Headers = new List<ResponseHeader> { HeaderMappingTestData.BasicResponseHeader2, HeaderMappingTestData.BasicResponseHeader },
            ServiceRequest = new ServiceRequest()
            {
                ID = 22,
                FromBody = "test"
            },
            ServiceRequestId = 3,
            Checksum = ChecksumHelpers.CreateChecksum(SupportedEncodingType.UNICODE, "test2")
        };

        public static MockResponseDto BasicMockResponseDto { get; } = new MockResponseDto
        {
            Id = 3,
            Body = "test body 3",
            Code = HttpStatusCode.NotFound,
            ContentType = "application/json3",
            Encoding = SupportedEncodingType.UNICODE,
            Headers = new List<MockResponseHeaderDto> { HeaderMappingTestData.BasicMockResponseHeaderDto, HeaderMappingTestData.BasicMockResponseHeaderDto2 },
            ServiceRequestId = 7,
            Checksum = ChecksumHelpers.CreateChecksum(SupportedEncodingType.UNICODE, "test3")
        };

        public static MockResponseDto BasicMockResponseDto2 { get; } = new MockResponseDto
        {
            Id = 4,
            Body = "test body 4",
            Code = HttpStatusCode.NotFound,
            ContentType = "application/json4",
            Encoding = SupportedEncodingType.UNICODE,
            Headers = new List<MockResponseHeaderDto> { HeaderMappingTestData.BasicMockResponseHeaderDto2, HeaderMappingTestData.BasicMockResponseHeaderDto },
            ServiceRequestId = 8,
            Checksum = ChecksumHelpers.CreateChecksum(SupportedEncodingType.UNICODE, "test4")
        };

        public static UpdateMockResponseDto BasicUpdateMockResponseDto { get; } = new UpdateMockResponseDto
        {
            Body = "test body 11",
            Code = HttpStatusCode.NotFound,
            ContentType = "application/json11",
            Encoding = SupportedEncodingType.UNICODE
        };

        public static UpdateMockResponseDto BasicUpdateMockResponseDto2 { get; } = new UpdateMockResponseDto
        {
            Body = "test body 22",
            Code = HttpStatusCode.NotFound,
            ContentType = "application/json22",
            Encoding = SupportedEncodingType.UNICODE
        };
    }
}
