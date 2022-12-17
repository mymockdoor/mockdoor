using MockDoor.Data.Helpers;
using MockDoor.Data.Models;
using MockDoor.Shared.Models.Response;

namespace MockDoor.Data.Mappers;

public static class ResponseMappers
{
    public static MockResponse ToEntity(this MockResponseDto mockResponseDto, bool generateChecksum)
    {
        return mockResponseDto == null
            ? null
            : new MockResponse()
            {
                Description = mockResponseDto.Description,
                Body = mockResponseDto.Body,
                Code = mockResponseDto.Code,
                ContentType = mockResponseDto.ContentType,
                Encoding = mockResponseDto.Encoding,
                Checksum = !generateChecksum
                    ? mockResponseDto.Checksum
                    : ChecksumHelpers.CreateDefaultChecksum(mockResponseDto),
                Headers = mockResponseDto.Headers.ToEntities(),
                ServiceRequestId = mockResponseDto.ServiceRequestId,
                ID = mockResponseDto.Id,
                Enabled = mockResponseDto.Enabled,
                CreatedUtc = mockResponseDto.CreatedUtc,
                Priority = mockResponseDto.Priority,
                Latency = mockResponseDto.Latency
            };
    }

    public static List<MockResponse> ToEntities(this List<MockResponseDto> mockResponseDtos, bool generateChecksum)
    {
        return mockResponseDtos?.Select(rr => rr.ToEntity(generateChecksum)).ToList();
    }

    public static MockResponseDto ToDto(this MockResponse mockResponse, bool generateChecksum)
    {
        return mockResponse == null
            ? null
            : new MockResponseDto()
            {
                Id = mockResponse.ID,
                Description = mockResponse.Description,
                Body = mockResponse.Body,
                Code = mockResponse.Code,
                ContentType = mockResponse.ContentType,
                Encoding = mockResponse.Encoding,
                ServiceRequestId = mockResponse.ServiceRequestId,
                Headers = mockResponse.Headers.ToDtos(),
                Enabled = mockResponse.Enabled,
                CreatedUtc = mockResponse.CreatedUtc,
                Priority = mockResponse.Priority,
                Latency = mockResponse.Latency,
                Checksum = !generateChecksum
                    ? mockResponse.Checksum
                    : ChecksumHelpers.CreateDefaultChecksum(mockResponse)
            };
    }

    public static List<MockResponseDto> ToDtos(this List<MockResponse> mockResponses, bool generateChecksum)
    {
        return mockResponses?.Select(rr => rr.ToDto(generateChecksum)).ToList();
    }

    //Request Response Dto to Update Dto
    public static UpdateMockResponseDto ToUpdateDto(this MockResponse mockResponse)
    {
        return mockResponse == null
            ? null
            : new UpdateMockResponseDto()
            {
                Description = mockResponse.Description,
                Body = mockResponse.Body,
                Code = mockResponse.Code,
                ContentType = mockResponse.ContentType,
                Encoding = mockResponse.Encoding,
                Priority = mockResponse.Priority,
                Enabled = mockResponse.Enabled,
                CreatedUtc = mockResponse.CreatedUtc,
                Headers = mockResponse.Headers?.ToDtos()
            };
    }

    public static List<UpdateMockResponseDto> ToUpdateDtos(this List<MockResponse> mockResponses)
    {
        return mockResponses?.Select(rr => rr.ToUpdateDto()).ToList();
    }

    public static UpdateMockResponseDto ToUpdateDto(this MockResponseDto mockResponseDto)
    {
        return mockResponseDto == null
            ? null
            : new UpdateMockResponseDto()
            {
                Description = mockResponseDto.Description,
                Body = mockResponseDto.Body,
                Code = mockResponseDto.Code,
                ContentType = mockResponseDto.ContentType,
                Encoding = mockResponseDto.Encoding,
                Priority = mockResponseDto.Priority,
                Enabled = mockResponseDto.Enabled,
                CreatedUtc = mockResponseDto.CreatedUtc,
                Headers = mockResponseDto.Headers
            };
    }

    public static List<UpdateMockResponseDto> ToUpdateDtos(this List<MockResponseDto> mockResponseDtos)
    {
        return mockResponseDtos?.Select(rr => rr.ToUpdateDto()).ToList();
    }

    public static MockResponseDto ToDto(this UpdateMockResponseDto mockResponse)
    {
        return mockResponse == null
            ? null
            : new MockResponseDto()
            {
                Description = mockResponse.Description,
                Body = mockResponse.Body,
                Code = mockResponse.Code,
                ContentType = mockResponse.ContentType,
                Encoding = mockResponse.Encoding,
                Priority = mockResponse.Priority,
                Enabled = mockResponse.Enabled,
                CreatedUtc = mockResponse.CreatedUtc ?? DateTime.UtcNow,
                Headers = mockResponse.Headers
            };
    }

    public static List<MockResponseDto> ToDtos(this List<UpdateMockResponseDto> mockResponses)
    {
        return mockResponses?.Select(rr => rr.ToDto()).ToList();
    }

    public static MockResponse UpdateWithDto(this MockResponse baseMockResponse,
        UpdateMockResponseDto updateMockResponse, bool generateChecksum)
    {
        if (baseMockResponse == null)
            throw new Exception("cannot update null request response");

        if (updateMockResponse == null)
            return baseMockResponse;

        baseMockResponse.Description = updateMockResponse.Description;
        baseMockResponse.Body = updateMockResponse.Body;
        baseMockResponse.Code = updateMockResponse.Code;
        baseMockResponse.ContentType = updateMockResponse.ContentType;
        baseMockResponse.Encoding = updateMockResponse.Encoding;
        baseMockResponse.Priority = updateMockResponse.Priority;
        baseMockResponse.Enabled = updateMockResponse.Enabled;
        baseMockResponse.CreatedUtc = updateMockResponse.CreatedUtc ?? DateTime.UtcNow;
        baseMockResponse.Headers = updateMockResponse.Headers?.ToEntities();

        if (generateChecksum)
        {
            baseMockResponse.Checksum = ChecksumHelpers.CreateDefaultChecksum(updateMockResponse);
        }

        return baseMockResponse;
    }
}