using MockDoor.Data.Models;
using MockDoor.Data.Mappers;
using MockDoor.Shared.Models.Response;
using System.Collections.Generic;
using Xunit;

namespace MockDoor.Data.Tests.Mapping.Response
{
    public class ResponseMappingDtoTests
    {
        [Fact]
        public void Should_Return_FullyMapped_ResponseDto_KeepChecksum()
        {
            // Setup
            var mockResponse = ResponseMappingTestData.BasicMockResponse;
            var defaultValues = new MockResponseDto();

            // Act
            var dto = mockResponse.ToDto(false);

            // Assert
            Assert.NotNull(dto);

            Assert.Equal(mockResponse.ID, dto.Id);
            Assert.Equal(mockResponse.Encoding, dto.Encoding);
            Assert.Equal(mockResponse.ServiceRequestId, dto.ServiceRequestId);
            Assert.Equal(mockResponse.Checksum, dto.Checksum);
            Assert.Equal(mockResponse.Body, dto.Body);
            Assert.Equal(mockResponse.Code, dto.Code);
            Assert.Equal(mockResponse.ContentType, dto.ContentType);
            Assert.Equal(mockResponse.Headers.Count, dto.Headers.Count);
            Assert.Equal(2, dto.Headers.Count);

            Assert.NotEqual(defaultValues.Id, dto.Id);
            Assert.NotEqual(defaultValues.Encoding, dto.Encoding);
            Assert.NotEqual(defaultValues.ServiceRequestId, dto.ServiceRequestId);
            Assert.NotEqual(defaultValues.Checksum, dto.Checksum);
            Assert.NotEqual(defaultValues.Body, dto.Body);
            Assert.NotEqual(defaultValues.Code, dto.Code);
            Assert.NotEqual(defaultValues.ContentType, dto.ContentType);
        }

        [Fact]
        public void Should_Return_FullyMapped_ResponseDto__FromUpdateDto()
        {
            // Setup
            var mockResponse = ResponseMappingTestData.BasicUpdateMockResponseDto;
            var defaultValues = new MockResponseDto();

            // Act
            var dto = mockResponse.ToDto();

            // Assert
            Assert.NotNull(dto);

            Assert.Equal(mockResponse.Encoding, dto.Encoding);
            Assert.Equal(mockResponse.Body, dto.Body);
            Assert.Equal(mockResponse.Code, dto.Code);
            Assert.Equal(mockResponse.ContentType, dto.ContentType);

            Assert.NotEqual(defaultValues.Encoding, dto.Encoding);
            Assert.NotEqual(defaultValues.Body, dto.Body);
            Assert.NotEqual(defaultValues.Code, dto.Code);
            Assert.NotEqual(defaultValues.ContentType, dto.ContentType);
        }

        [Fact]
        public void Should_Return_FullyMapped_ResponseDtos_KeepChecksums()
        {
            // Setup
            var mockResponses = new List<MockResponse> { ResponseMappingTestData.BasicMockResponse, ResponseMappingTestData.BasicMockResponse2 };
            var defaultValues = new MockResponseDto();

            // Act
            var dtos = mockResponses.ToDtos(false);

            // Assert
            Assert.NotNull(dtos);
            Assert.Equal(2, dtos.Count);

            foreach (var dto in dtos)
            {
                Assert.NotEqual(defaultValues.Id, dto.Id);
                Assert.NotEqual(defaultValues.Encoding, dto.Encoding);
                Assert.NotEqual(defaultValues.ServiceRequestId, dto.ServiceRequestId);
                Assert.NotEqual(defaultValues.Checksum, dto.Checksum);
                Assert.NotEqual(defaultValues.Body, dto.Body);
                Assert.NotEqual(defaultValues.Code, dto.Code);
                Assert.NotEqual(defaultValues.ContentType, dto.ContentType);
            }

            Assert.Equal(mockResponses[0].ID, dtos[0].Id);
            Assert.Equal(mockResponses[0].Encoding, dtos[0].Encoding);
            Assert.Equal(mockResponses[0].ServiceRequestId, dtos[0].ServiceRequestId);
            Assert.Equal(mockResponses[0].Checksum, dtos[0].Checksum);
            Assert.Equal(mockResponses[0].Body, dtos[0].Body);
            Assert.Equal(mockResponses[0].Code, dtos[0].Code);
            Assert.Equal(mockResponses[0].ContentType, dtos[0].ContentType);
            Assert.Equal(mockResponses[0].Headers.Count, dtos[0].Headers.Count);
            Assert.Equal(2, dtos[0].Headers.Count);

            Assert.Equal(mockResponses[1].ID, dtos[1].Id);
            Assert.Equal(mockResponses[1].Encoding, dtos[1].Encoding);
            Assert.Equal(mockResponses[1].ServiceRequestId, dtos[1].ServiceRequestId);
            Assert.Equal(mockResponses[1].Checksum, dtos[1].Checksum);
            Assert.Equal(mockResponses[1].Body, dtos[1].Body);
            Assert.Equal(mockResponses[1].Code, dtos[1].Code);
            Assert.Equal(mockResponses[1].ContentType, dtos[1].ContentType);
            Assert.Equal(mockResponses[1].Headers.Count, dtos[1].Headers.Count);
            Assert.Equal(2, dtos[1].Headers.Count);
        }

        [Fact]
        public void Should_Return_FullyMapped_ResponseDto_GenerateChecksum()
        {
            // Setup
            var mockResponse = ResponseMappingTestData.BasicMockResponse;
            var defaultValues = new MockResponseDto();

            // Act
            var dto = mockResponse.ToDto(true);

            // Assert
            Assert.NotNull(dto);

            Assert.Equal(mockResponse.ID, dto.Id);
            Assert.Equal(mockResponse.Encoding, dto.Encoding);
            Assert.Equal(mockResponse.ServiceRequestId, dto.ServiceRequestId);
            Assert.Equal(mockResponse.Body, dto.Body);
            Assert.Equal(mockResponse.Code, dto.Code);
            Assert.Equal(mockResponse.ContentType, dto.ContentType);
            Assert.Equal(mockResponse.Headers.Count, dto.Headers.Count);
            Assert.Equal(2, dto.Headers.Count);

            Assert.NotEqual(mockResponse.Checksum, dto.Checksum);

            Assert.NotEqual(defaultValues.Id, dto.Id);
            Assert.NotEqual(defaultValues.Encoding, dto.Encoding);
            Assert.NotEqual(defaultValues.ServiceRequestId, dto.ServiceRequestId);
            Assert.NotEqual(defaultValues.Checksum, dto.Checksum);
            Assert.NotEqual(defaultValues.Body, dto.Body);
            Assert.NotEqual(defaultValues.Code, dto.Code);
            Assert.NotEqual(defaultValues.ContentType, dto.ContentType);
        }

        [Fact]
        public void Should_Return_FullyMapped_ResponseDtos_GenerateChecksums()
        {
            // Setup
            var mockResponses = new List<MockResponse> { ResponseMappingTestData.BasicMockResponse, ResponseMappingTestData.BasicMockResponse2 };
            var defaultValues = new MockResponseDto();

            // Act
            var dtos = mockResponses.ToDtos(true);

            // Assert
            Assert.NotNull(dtos);
            Assert.Equal(2, dtos.Count);

            foreach (var dto in dtos)
            {
                Assert.NotEqual(defaultValues.Id, dto.Id);
                Assert.NotEqual(defaultValues.Encoding, dto.Encoding);
                Assert.NotEqual(defaultValues.ServiceRequestId, dto.ServiceRequestId);
                Assert.NotEqual(defaultValues.Body, dto.Body);
                Assert.NotEqual(defaultValues.Code, dto.Code);
                Assert.NotEqual(defaultValues.ContentType, dto.ContentType);
            }

            Assert.NotEqual(mockResponses[0].Checksum, dtos[0].Checksum);
            Assert.NotEqual(mockResponses[1].Checksum, dtos[1].Checksum);


            Assert.Equal(mockResponses[0].ID, dtos[0].Id);
            Assert.Equal(mockResponses[0].Encoding, dtos[0].Encoding);
            Assert.Equal(mockResponses[0].ServiceRequestId, dtos[0].ServiceRequestId);
            Assert.Equal(mockResponses[0].Body, dtos[0].Body);
            Assert.Equal(mockResponses[0].Code, dtos[0].Code);
            Assert.Equal(mockResponses[0].ContentType, dtos[0].ContentType);
            Assert.Equal(mockResponses[0].Headers.Count, dtos[0].Headers.Count);
            Assert.Equal(2, dtos[0].Headers.Count);

            Assert.Equal(mockResponses[1].ID, dtos[1].Id);
            Assert.Equal(mockResponses[1].Encoding, dtos[1].Encoding);
            Assert.Equal(mockResponses[1].ServiceRequestId, dtos[1].ServiceRequestId);
            Assert.Equal(mockResponses[1].Body, dtos[1].Body);
            Assert.Equal(mockResponses[1].Code, dtos[1].Code);
            Assert.Equal(mockResponses[1].ContentType, dtos[1].ContentType);
            Assert.Equal(mockResponses[1].Headers.Count, dtos[1].Headers.Count);
            Assert.Equal(2, dtos[1].Headers.Count);
        }

        [Fact]
        public void Should_Return_FullyMapped_ResponseDtos_From_UpdateDtos()
        {
            // Setup
            var mockResponses = new List<UpdateMockResponseDto> { ResponseMappingTestData.BasicUpdateMockResponseDto, ResponseMappingTestData.BasicUpdateMockResponseDto2 };
            var defaultValues = new MockResponseDto();

            // Act
            var dtos = mockResponses.ToDtos();

            // Assert
            Assert.NotNull(dtos);
            Assert.Equal(2, dtos.Count);

            foreach (var dto in dtos)
            {
                Assert.NotEqual(defaultValues.Encoding, dto.Encoding);
                Assert.NotEqual(defaultValues.Body, dto.Body);
                Assert.NotEqual(defaultValues.Code, dto.Code);
                Assert.NotEqual(defaultValues.ContentType, dto.ContentType);
            }

            Assert.Equal(mockResponses[0].Encoding, dtos[0].Encoding);
            Assert.Equal(mockResponses[0].Body, dtos[0].Body);
            Assert.Equal(mockResponses[0].Code, dtos[0].Code);
            Assert.Equal(mockResponses[0].ContentType, dtos[0].ContentType);

            Assert.Equal(mockResponses[1].Encoding, dtos[1].Encoding);
            Assert.Equal(mockResponses[1].Body, dtos[1].Body);
            Assert.Equal(mockResponses[1].Code, dtos[1].Code);
            Assert.Equal(mockResponses[1].ContentType, dtos[1].ContentType);
        }

        [Fact]
        public void Should_ReturnMappings_Dto_Generate_SameChecksum_WhenSame()
        {
            // Setup
            var mockResponse = ResponseMappingTestData.BasicMockResponse;
            var mockResponse2 = ResponseMappingTestData.BasicMockResponse;

            // Act
            var dto = mockResponse.ToDto(true);
            var dto2 = mockResponse2.ToDto(true);

            // Assert
            Assert.NotNull(dto);
            Assert.NotNull(dto2);

            Assert.NotEqual(mockResponse.Checksum, dto.Checksum);
            Assert.Equal(dto2.Checksum, dto.Checksum);
        }

        [Fact]
        public void Should_ReturnMappings_Dto_Generate_DiffChecksum_WhenNotSame()
        {
            // Setup
            var mockResponse = ResponseMappingTestData.BasicMockResponse;
            var mockResponse2 = ResponseMappingTestData.BasicMockResponse2;

            // Act
            var dto = mockResponse.ToDto(true);
            var dto2 = mockResponse2.ToDto(true);

            // Assert
            Assert.NotNull(dto);
            Assert.NotNull(dto2);

            Assert.NotEqual(mockResponse.Checksum, dto.Checksum);
            Assert.NotEqual(dto2.Checksum, dto.Checksum);
        }
    }
}
