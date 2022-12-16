using MockDoor.Data.Models.Headers;
using MockDoor.Data.Mappers;
using MockDoor.Shared.Models.Headers;
using Xunit;
using System.Collections.Generic;

namespace MockDoor.Data.Tests.Mapping.Headers
{
    public class ServiceRequestHeaderMappingTests
    {
        [Fact]
        public void Should_Return_FullyMapped_ServiceRequestHeaderDto()
        {
            // Setup
            var requestHeader = HeaderMappingTestData.BasicServiceRequestHeader;
            var defaultValues = new ServiceRequestHeaderDto();

            var expectedValues = HeaderMappingTestData.BasicServiceRequestHeader.Value;

            // Act
            var dto = requestHeader.ToDto();

            // Assert
            Assert.NotNull(dto);
            Assert.NotNull(dto.Value);

            Assert.Equal(requestHeader.Name, dto.Name);
            Assert.Equal(expectedValues, dto.Value);

            foreach (var item in dto.Value.Split(';'))
            {
                Assert.Contains(item, expectedValues);
            }

            Assert.NotEqual(defaultValues.Name, dto.Name);
        }

        [Fact]
        public void Should_Return_FullyMapped_RequestHeaderModel()
        {
            // Setup
            var requestHeaderDto = HeaderMappingTestData.BasicServiceRequestHeaderDto;
            var defaultValues = new RequestHeader();

            var expectedValuesString = HeaderMappingTestData.BasicServiceRequestHeaderDto.Value;

            // Act
            var serviceHeaderModel = requestHeaderDto.ToEntity();

            // Assert
            Assert.NotNull(serviceHeaderModel);
            Assert.Equal(requestHeaderDto.Name, serviceHeaderModel.Name);
            Assert.Equal(expectedValuesString, serviceHeaderModel.Value);

            Assert.NotEqual(defaultValues.Name, serviceHeaderModel.Name);
            Assert.NotEqual(defaultValues.Value, serviceHeaderModel.Value);
        }


        [Fact]
        public void Should_Return_FullyMapped_List_Of_ServiceRequestHeaderDtos()
        {
            // Setup
            var requestHeaders = new List<RequestHeader> { HeaderMappingTestData.BasicServiceRequestHeader, HeaderMappingTestData.BasicServiceRequestHeader2 };
            var defaultValues = new ServiceRequestHeaderDto();

            var expectedValuesList1 = HeaderMappingTestData.BasicServiceRequestHeader.Value;
            var expectedValuesList2 = HeaderMappingTestData.BasicServiceRequestHeader2.Value;

            // Act
            var dtos = requestHeaders.ToDtos();

            // Assert
            Assert.NotNull(dtos);
            Assert.Equal(2, dtos.Count);

            Assert.NotNull(dtos[0].Value);
            Assert.NotNull(dtos[1].Value);

            Assert.Equal(requestHeaders[0].Name, dtos[0].Name);
            Assert.Equal(expectedValuesList1, dtos[0].Value);

            Assert.Equal(requestHeaders[1].Name, dtos[1].Name);
            Assert.Equal(expectedValuesList2, dtos[1].Value);

            foreach (var item in dtos[0].Value.Split(';'))
            {
                Assert.Contains(item, expectedValuesList1);
            }

            foreach (var item in dtos[1].Value.Split(';'))
            {
                Assert.Contains(item, expectedValuesList2);
            }

            Assert.NotEqual(defaultValues.Name, dtos[0].Name);
            Assert.NotEqual(defaultValues.Name, dtos[1].Name);
        }

        [Fact]
        public void Should_Return_FullyMapped_List_Of_RequestHeaderModels()
        {
            // Setup
            var requestHeaderDtos = new List<ServiceRequestHeaderDto> { HeaderMappingTestData.BasicServiceRequestHeaderDto, HeaderMappingTestData.BasicServiceRequestHeaderDto2 };
            var defaultValues = new RequestHeader();

            var expectedValuesString1 = HeaderMappingTestData.BasicServiceRequestHeaderDto.Value;
            var expectedValuesString2 = HeaderMappingTestData.BasicServiceRequestHeaderDto2.Value;

            // Act
            var serviceHeaderModels = requestHeaderDtos.ToEntities();

            // Assert
            Assert.NotNull(serviceHeaderModels);
            Assert.Equal(2, serviceHeaderModels.Count);

            Assert.Equal(requestHeaderDtos[0].Name, serviceHeaderModels[0].Name);
            Assert.Equal(expectedValuesString1, serviceHeaderModels[0].Value);

            Assert.Equal(requestHeaderDtos[1].Name, serviceHeaderModels[1].Name);
            Assert.Equal(expectedValuesString2, serviceHeaderModels[1].Value);

            Assert.NotEqual(defaultValues.Name, serviceHeaderModels[0].Name);
            Assert.NotEqual(defaultValues.Value, serviceHeaderModels[0].Value);

            Assert.NotEqual(defaultValues.Name, serviceHeaderModels[1].Name);
            Assert.NotEqual(defaultValues.Value, serviceHeaderModels[1].Value);
        }
    }
}
