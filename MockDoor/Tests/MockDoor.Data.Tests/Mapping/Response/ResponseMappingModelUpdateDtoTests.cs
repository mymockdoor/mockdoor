using MockDoor.Data.Models;
using MockDoor.Data.Mappers;
using MockDoor.Shared.Models.Response;
using System.Collections.Generic;
using Xunit;

namespace MockDoor.Data.Tests.Mapping.Response
{
    public class ResponseMappingModelUpdateDtoTests
    {
        [Fact]
        public void Should_Return_FullyMapped_Model_To_UpdateResponseDto()
        {
            // Setup
            var mockResponse = ResponseMappingTestData.BasicMockResponse;
            var defaultValues = new MockResponseDto();

            // Act
            var dto = mockResponse.ToUpdateDto();

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
        public void Should_Return_FullyMapped_Dto_To_UpdateResponseDto()
        {
            // Setup
            var mockResponse = ResponseMappingTestData.BasicMockResponseDto;
            var defaultValues = new MockResponseDto();

            // Act
            var dto = mockResponse.ToUpdateDto();

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
        public void Should_Return_FullyMapped_Model_To_UpdateResponseDtos()
        {
            // Setup
            var mockResponses = new List<MockResponse> { ResponseMappingTestData.BasicMockResponse, ResponseMappingTestData.BasicMockResponse2 };
            var defaultValues = new MockResponseDto();

            // Act
            var dtos = mockResponses.ToUpdateDtos();

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
        public void Should_Return_FullyMapped_Dto_To_UpdateResponseDtos()
        {
            // Setup
            var mockResponses = new List<MockResponseDto> { ResponseMappingTestData.BasicMockResponseDto, ResponseMappingTestData.BasicMockResponseDto2 };
            var defaultValues = new MockResponseDto();

            // Act
            var dtos = mockResponses.ToUpdateDtos();

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
    }
}
