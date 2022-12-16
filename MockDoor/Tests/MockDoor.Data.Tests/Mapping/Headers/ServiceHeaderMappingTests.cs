using MockDoor.Data.Models.Headers;
using MockDoor.Data.Mappers;
using MockDoor.Shared.Models.Headers;
using System.Collections.Generic;
using Xunit;

namespace MockDoor.Data.Tests.Mapping.Headers
{
    public class ServiceHeaderMappingTests
    {
        [Fact]
        public void Should_Return_FullyMapped_ServiceHeaderDto()
        {
            // Setup
            var serviceHeader = HeaderMappingTestData.BasicServiceHeader;
            var defaultValues = new ServiceHeader();

            // Act
            var dto = serviceHeader.ToDto();


            // Assert
            Assert.NotNull(dto);
            Assert.Equal(serviceHeader.Name, dto.Name);
            Assert.Equal(serviceHeader.Enabled, dto.Enabled);
            Assert.Equal(serviceHeader.Incoming, dto.Incoming);
            Assert.Equal(serviceHeader.Outgoing, dto.Outgoing);

            Assert.NotEqual(defaultValues.Name, dto.Name);
            Assert.NotEqual(defaultValues.Enabled, dto.Enabled);
            Assert.NotEqual(defaultValues.Incoming, dto.Incoming);
            Assert.NotEqual(defaultValues.Outgoing, dto.Outgoing);
        }

        [Fact]
        public void Should_Return_FullyMapped_ServiceHeaderModel()
        {
            // Setup
            var serviceHeaderDto = HeaderMappingTestData.BasicServiceHeaderDto;
            var defaultValues = new ServiceHeaderDto();

            // Act
            var serviceHeaderModel = serviceHeaderDto.ToEntity();

            // Assert
            Assert.NotNull(serviceHeaderModel);
            Assert.Equal(serviceHeaderDto.Name, serviceHeaderModel.Name);
            Assert.Equal(serviceHeaderDto.Enabled, serviceHeaderModel.Enabled);
            Assert.Equal(serviceHeaderDto.Incoming, serviceHeaderModel.Incoming);
            Assert.Equal(serviceHeaderDto.Outgoing, serviceHeaderModel.Outgoing);

            Assert.NotEqual(defaultValues.Name, serviceHeaderModel.Name);
            Assert.NotEqual(defaultValues.Enabled, serviceHeaderModel.Enabled);
            Assert.NotEqual(defaultValues.Incoming, serviceHeaderModel.Incoming);
            Assert.NotEqual(defaultValues.Outgoing, serviceHeaderModel.Outgoing);
        }


        [Fact]
        public void Should_Return_FullyMapped_List_of_ServiceHeaderDtos()
        {
            // Setup
            var serviceHeaders = new List<ServiceHeader>() { HeaderMappingTestData.BasicServiceHeader, HeaderMappingTestData.BasicServiceHeader2 };

            var defaultValues = new ServiceHeader();

            // Act
            var dtos = serviceHeaders.ToDtos();

            // Assert
            Assert.NotNull(dtos);
            Assert.Equal(2, dtos.Count);

            Assert.Equal(serviceHeaders[0].Name, dtos[0].Name);
            Assert.Equal(serviceHeaders[0].Enabled, dtos[0].Enabled);
            Assert.Equal(serviceHeaders[0].Incoming, dtos[0].Incoming);
            Assert.Equal(serviceHeaders[0].Outgoing, dtos[0].Outgoing);

            Assert.Equal(serviceHeaders[1].Name, dtos[1].Name);
            Assert.Equal(serviceHeaders[1].Enabled, dtos[1].Enabled);
            Assert.Equal(serviceHeaders[1].Incoming, dtos[1].Incoming);
            Assert.Equal(serviceHeaders[1].Outgoing, dtos[1].Outgoing);

            Assert.NotEqual(defaultValues.Name, dtos[0].Name);
            Assert.NotEqual(defaultValues.Enabled, dtos[0].Enabled);
            Assert.NotEqual(defaultValues.Incoming, dtos[0].Incoming);
            Assert.NotEqual(defaultValues.Outgoing, dtos[0].Outgoing);

            Assert.NotEqual(defaultValues.Name, dtos[1].Name);
            Assert.NotEqual(defaultValues.Enabled, dtos[1].Enabled);
            Assert.NotEqual(defaultValues.Incoming, dtos[1].Incoming);
            Assert.NotEqual(defaultValues.Outgoing, dtos[1].Outgoing);
        }

        [Fact]
        public void Should_Return_FullyMapped_List_of_ServiceHeaderModels()
        {
            // Setup
            var serviceHeaderDtos = new List<ServiceHeaderDto> { HeaderMappingTestData.BasicServiceHeaderDto, HeaderMappingTestData.BasicServiceHeaderDto2 };
            var defaultValues = new ServiceHeaderDto();

            // Act
            var serviceHeaderModels = serviceHeaderDtos.ToModels();

            // Assert
            Assert.NotNull(serviceHeaderModels);
            Assert.Equal(2, serviceHeaderModels.Count);

            Assert.Equal(serviceHeaderDtos[0].Name, serviceHeaderModels[0].Name);
            Assert.Equal(serviceHeaderDtos[0].Enabled, serviceHeaderModels[0].Enabled);
            Assert.Equal(serviceHeaderDtos[0].Incoming, serviceHeaderModels[0].Incoming);
            Assert.Equal(serviceHeaderDtos[0].Outgoing, serviceHeaderModels[0].Outgoing);

            Assert.Equal(serviceHeaderDtos[1].Name, serviceHeaderModels[1].Name);
            Assert.Equal(serviceHeaderDtos[1].Enabled, serviceHeaderModels[1].Enabled);
            Assert.Equal(serviceHeaderDtos[1].Incoming, serviceHeaderModels[1].Incoming);
            Assert.Equal(serviceHeaderDtos[1].Outgoing, serviceHeaderModels[1].Outgoing);

            Assert.NotEqual(defaultValues.Name, serviceHeaderModels[0].Name);
            Assert.NotEqual(defaultValues.Enabled, serviceHeaderModels[0].Enabled);
            Assert.NotEqual(defaultValues.Incoming, serviceHeaderModels[0].Incoming);
            Assert.NotEqual(defaultValues.Outgoing, serviceHeaderModels[0].Outgoing);

            Assert.NotEqual(defaultValues.Name, serviceHeaderModels[1].Name);
            Assert.NotEqual(defaultValues.Enabled, serviceHeaderModels[1].Enabled);
            Assert.NotEqual(defaultValues.Incoming, serviceHeaderModels[1].Incoming);
            Assert.NotEqual(defaultValues.Outgoing, serviceHeaderModels[1].Outgoing);
        }
    }
}
