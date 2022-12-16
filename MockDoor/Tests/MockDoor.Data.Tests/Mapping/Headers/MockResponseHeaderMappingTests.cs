using MockDoor.Data.Models.Headers;
using MockDoor.Data.Mappers;
using MockDoor.Shared.Models.Headers;
using Xunit;
using System.Collections.Generic;

namespace MockDoor.Data.Tests.Mapping.Headers
{
    public class MockResponseHeaderMappingTests
    {
        [Fact]
        public void Should_Return_FullyMapped_MockResponseHeaderDto()
        {
            // Setup
            var responseHeader = HeaderMappingTestData.BasicResponseHeader;
            var defaultValues = new MockResponseHeaderDto();

            var expectedValuesList = HeaderMappingTestData.BasicResponseHeader.Value;

            // Act
            var dto = responseHeader.ToDto();

            // Assert
            Assert.NotNull(dto);
            Assert.NotNull(dto.Value);

            Assert.Equal(responseHeader.Name, dto.Name);
            Assert.Equal(expectedValuesList, dto.Value);

            foreach (var item in dto.Value.Split(';'))
            {
                Assert.Contains(item, expectedValuesList);
            }

            Assert.NotEqual(defaultValues.Name, dto.Name);
        }

        [Fact]
        public void Should_Return_FullyMapped_RequestHeaderModel()
        {
            // Setup
            var responseHeaderDto = HeaderMappingTestData.BasicMockResponseHeaderDto;
            var defaultValues = new ResponseHeader();

            var expectedValuesString = HeaderMappingTestData.BasicMockResponseHeaderDto.Value;

            // Act
            var serviceHeaderModel = responseHeaderDto.ToEntity();

            // Assert
            Assert.NotNull(serviceHeaderModel);
            Assert.Equal(responseHeaderDto.Name, serviceHeaderModel.Name);
            Assert.Equal(expectedValuesString, serviceHeaderModel.Value);

            Assert.NotEqual(defaultValues.Name, serviceHeaderModel.Name);
            Assert.NotEqual(defaultValues.Value, serviceHeaderModel.Value);
        }


        [Fact]
        public void Should_Return_FullyMapped_List_of_MockResponseHeaderDtos()
        {
            // Setup
            var responseHeaders = new List<ResponseHeader> { HeaderMappingTestData.BasicResponseHeader, HeaderMappingTestData.BasicResponseHeader2 };
            var defaultValues = new MockResponseHeaderDto();

            var expectedValues1 = HeaderMappingTestData.BasicResponseHeader.Value;
            var expectedValues2 = HeaderMappingTestData.BasicResponseHeader2.Value;

            // Act
            var dtos = responseHeaders.ToDtos();

            // Assert
            Assert.NotNull(dtos);
            Assert.Equal(2, dtos.Count);

            Assert.NotNull(dtos[0].Value);
            Assert.NotNull(dtos[1].Value);

            Assert.Equal(responseHeaders[0].Name, dtos[0].Name);
            Assert.Equal(expectedValues1, dtos[0].Value);

            Assert.Equal(responseHeaders[1].Name, dtos[1].Name);
            Assert.Equal(expectedValues2, dtos[1].Value);

            foreach (var item in dtos[0].Value.Split(';'))
            {
                Assert.Contains(item, expectedValues1);
            }

            foreach (var item in dtos[1].Value.Split(';'))
            {
                Assert.Contains(item, expectedValues2);
            }

            Assert.NotEqual(defaultValues.Name, dtos[0].Name);
            Assert.NotEqual(defaultValues.Name, dtos[1].Name);
        }

        [Fact]
        public void Should_Return_FullyMapped_List_of_RequestHeaderModels()
        {
            // Setup
            var responseHeaderDtos = new List<MockResponseHeaderDto> { HeaderMappingTestData.BasicMockResponseHeaderDto, HeaderMappingTestData.BasicMockResponseHeaderDto2 };
            var defaultValues = new ResponseHeader();

            var expectedValuesString1 = HeaderMappingTestData.BasicMockResponseHeaderDto.Value;
            var expectedValuesString2 = HeaderMappingTestData.BasicMockResponseHeaderDto2.Value;

            // Act
            var serviceHeaderModels = responseHeaderDtos.ToEntities();

            // Assert
            Assert.NotNull(serviceHeaderModels);
            Assert.Equal(2, serviceHeaderModels.Count);

            Assert.Equal(responseHeaderDtos[0].Name, serviceHeaderModels[0].Name);
            Assert.Equal(expectedValuesString1, serviceHeaderModels[0].Value);

            Assert.Equal(responseHeaderDtos[1].Name, serviceHeaderModels[1].Name);
            Assert.Equal(expectedValuesString2, serviceHeaderModels[1].Value);

            Assert.NotEqual(defaultValues.Name, serviceHeaderModels[0].Name);
            Assert.NotEqual(defaultValues.Value, serviceHeaderModels[0].Value);

            Assert.NotEqual(defaultValues.Name, serviceHeaderModels[1].Name);
            Assert.NotEqual(defaultValues.Value, serviceHeaderModels[1].Value);
        }
    }
}
