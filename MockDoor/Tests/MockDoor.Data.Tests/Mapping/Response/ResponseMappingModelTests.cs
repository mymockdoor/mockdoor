using MockDoor.Data.Models;
using MockDoor.Data.Mappers;
using MockDoor.Shared.Models.Response;
using System.Collections.Generic;
using Xunit;

namespace MockDoor.Data.Tests.Mapping.Response
{
    public class ResponseMappingModelTests
    {
        [Fact]
        public void Should_Return_FullyMapped_ResponseModel_KeepChecksum()
        {
            // Setup
            var mockResponseDto = ResponseMappingTestData.BasicMockResponseDto;
            var defaultValues = new MockResponse();

            // Act
            var model = mockResponseDto.ToEntity(false);

            // Assert
            Assert.NotNull(model);

            Assert.Equal(mockResponseDto.Id, model.ID);
            Assert.Equal(mockResponseDto.Encoding, model.Encoding);
            Assert.Equal(mockResponseDto.ServiceRequestId, model.ServiceRequestId);
            Assert.Equal(mockResponseDto.Checksum, model.Checksum);
            Assert.Equal(mockResponseDto.Body, model.Body);
            Assert.Equal(mockResponseDto.Code, model.Code);
            Assert.Equal(mockResponseDto.ContentType, model.ContentType);
            Assert.Equal(mockResponseDto.Headers.Count, model.Headers.Count);
            Assert.Equal(2, model.Headers.Count);

            Assert.NotEqual(defaultValues.ID, model.ID);
            Assert.NotEqual(defaultValues.Encoding, model.Encoding);
            Assert.NotEqual(defaultValues.ServiceRequestId, model.ServiceRequestId);
            Assert.NotEqual(defaultValues.Checksum, model.Checksum);
            Assert.NotEqual(defaultValues.Body, model.Body);
            Assert.NotEqual(defaultValues.Code, model.Code);
            Assert.NotEqual(defaultValues.ContentType, model.ContentType);
        }

        [Fact]
        public void Should_Return_FullyMapped_ResponseModels_KeepChecksums()
        {
            // Setup
            var mockResponseDtos = new List<MockResponseDto> { ResponseMappingTestData.BasicMockResponseDto, ResponseMappingTestData.BasicMockResponseDto2 };
            var defaultValues = new MockResponse();

            // Act
            var models = mockResponseDtos.ToEntities(false);

            // Assert
            Assert.NotNull(models);
            Assert.Equal(2, models.Count);

            foreach (var model in models)
            {
                Assert.NotEqual(defaultValues.ID, model.ID);
                Assert.NotEqual(defaultValues.Encoding, model.Encoding);
                Assert.NotEqual(defaultValues.ServiceRequestId, model.ServiceRequestId);
                Assert.NotEqual(defaultValues.Checksum, model.Checksum);
                Assert.NotEqual(defaultValues.Body, model.Body);
                Assert.NotEqual(defaultValues.Code, model.Code);
                Assert.NotEqual(defaultValues.ContentType, model.ContentType);
            }

            Assert.Equal(mockResponseDtos[0].Id, models[0].ID);
            Assert.Equal(mockResponseDtos[0].Encoding, models[0].Encoding);
            Assert.Equal(mockResponseDtos[0].ServiceRequestId, models[0].ServiceRequestId);
            Assert.Equal(mockResponseDtos[0].Checksum, models[0].Checksum);
            Assert.Equal(mockResponseDtos[0].Body, models[0].Body);
            Assert.Equal(mockResponseDtos[0].Code, models[0].Code);
            Assert.Equal(mockResponseDtos[0].ContentType, models[0].ContentType);
            Assert.Equal(mockResponseDtos[0].Headers.Count, models[0].Headers.Count);
            Assert.Equal(2, models[0].Headers.Count);

            Assert.Equal(mockResponseDtos[1].Id, models[1].ID);
            Assert.Equal(mockResponseDtos[1].Encoding, models[1].Encoding);
            Assert.Equal(mockResponseDtos[1].ServiceRequestId, models[1].ServiceRequestId);
            Assert.Equal(mockResponseDtos[1].Checksum, models[1].Checksum);
            Assert.Equal(mockResponseDtos[1].Body, models[1].Body);
            Assert.Equal(mockResponseDtos[1].Code, models[1].Code);
            Assert.Equal(mockResponseDtos[1].ContentType, models[1].ContentType);
            Assert.Equal(mockResponseDtos[1].Headers.Count, models[1].Headers.Count);
            Assert.Equal(2, models[1].Headers.Count);
        }

        [Fact]
        public void Should_Return_FullyMapped_ResponseModel_GenerateChecksum()
        {
            // Setup
            var mockResponseDto = ResponseMappingTestData.BasicMockResponseDto;
            var defaultValues = new MockResponse();

            // Act
            var model = mockResponseDto.ToEntity(true);

            // Assert
            Assert.NotNull(model);

            Assert.Equal(mockResponseDto.Id, model.ID);
            Assert.Equal(mockResponseDto.Encoding, model.Encoding);
            Assert.Equal(mockResponseDto.ServiceRequestId, model.ServiceRequestId);
            Assert.Equal(mockResponseDto.Body, model.Body);
            Assert.Equal(mockResponseDto.Code, model.Code);
            Assert.Equal(mockResponseDto.ContentType, model.ContentType);
            Assert.Equal(mockResponseDto.Headers.Count, model.Headers.Count);
            Assert.Equal(2, model.Headers.Count);

            Assert.NotEqual(mockResponseDto.Checksum, model.Checksum);

            Assert.NotEqual(defaultValues.ID, model.ID);
            Assert.NotEqual(defaultValues.Encoding, model.Encoding);
            Assert.NotEqual(defaultValues.ServiceRequestId, model.ServiceRequestId);
            Assert.NotEqual(defaultValues.Checksum, model.Checksum);
            Assert.NotEqual(defaultValues.Body, model.Body);
            Assert.NotEqual(defaultValues.Code, model.Code);
            Assert.NotEqual(defaultValues.ContentType, model.ContentType);
        }

        [Fact]
        public void Should_Return_FullyMapped_ResponseModels_GenerateChecksums()
        {
            // Setup
            var mockResponseDtos = new List<MockResponseDto> { ResponseMappingTestData.BasicMockResponseDto, ResponseMappingTestData.BasicMockResponseDto2 };
            var defaultValues = new MockResponse();

            // Act
            var models = mockResponseDtos.ToEntities(true);

            // Assert
            Assert.NotNull(models);
            Assert.Equal(2, models.Count);

            foreach (var model in models)
            {
                Assert.NotEqual(defaultValues.ID, model.ID);
                Assert.NotEqual(defaultValues.Encoding, model.Encoding);
                Assert.NotEqual(defaultValues.ServiceRequestId, model.ServiceRequestId);
                Assert.NotEqual(defaultValues.Body, model.Body);
                Assert.NotEqual(defaultValues.Code, model.Code);
                Assert.NotEqual(defaultValues.ContentType, model.ContentType);
            }

            Assert.NotEqual(mockResponseDtos[0].Checksum, models[0].Checksum);
            Assert.NotEqual(mockResponseDtos[1].Checksum, models[1].Checksum);


            Assert.Equal(mockResponseDtos[0].Id, models[0].ID);
            Assert.Equal(mockResponseDtos[0].Encoding, models[0].Encoding);
            Assert.Equal(mockResponseDtos[0].ServiceRequestId, models[0].ServiceRequestId);
            Assert.Equal(mockResponseDtos[0].Body, models[0].Body);
            Assert.Equal(mockResponseDtos[0].Code, models[0].Code);
            Assert.Equal(mockResponseDtos[0].ContentType, models[0].ContentType);
            Assert.Equal(mockResponseDtos[0].Headers.Count, models[0].Headers.Count);
            Assert.Equal(2, models[0].Headers.Count);

            Assert.Equal(mockResponseDtos[1].Id, models[1].ID);
            Assert.Equal(mockResponseDtos[1].Encoding, models[1].Encoding);
            Assert.Equal(mockResponseDtos[1].ServiceRequestId, models[1].ServiceRequestId);
            Assert.Equal(mockResponseDtos[1].Body, models[1].Body);
            Assert.Equal(mockResponseDtos[1].Code, models[1].Code);
            Assert.Equal(mockResponseDtos[1].ContentType, models[1].ContentType);
            Assert.Equal(mockResponseDtos[1].Headers.Count, models[1].Headers.Count);
            Assert.Equal(2, models[1].Headers.Count);
        }

        [Fact]
        public void Should_ReturnMappings_Model_Generate_SameChecksum_WhenSame()
        {
            // Setup
            var mockResponseDto = ResponseMappingTestData.BasicMockResponseDto;
            var mockResponseDto2 = ResponseMappingTestData.BasicMockResponseDto;

            // Act
            var model = mockResponseDto.ToEntity(true);
            var model2 = mockResponseDto2.ToEntity(true);

            // Assert
            Assert.NotNull(model);
            Assert.NotNull(model2);

            Assert.NotEqual(mockResponseDto.Checksum, model.Checksum);
            Assert.Equal(model2.Checksum, model.Checksum);
        }

        [Fact]
        public void Should_ReturnMappings_Model_Generate_DiffChecksum_WhenNotSame()
        {
            // Setup
            var mockResponseDto = ResponseMappingTestData.BasicMockResponseDto;
            var mockResponseDto2 = ResponseMappingTestData.BasicMockResponseDto2;

            // Act
            var model = mockResponseDto.ToEntity(true);
            var model2 = mockResponseDto2.ToEntity(true);

            // Assert
            Assert.NotNull(model);
            Assert.NotNull(model2);

            Assert.NotEqual(mockResponseDto.Checksum, model.Checksum);
            Assert.NotEqual(model2.Checksum, model.Checksum);
        }
    }
}
