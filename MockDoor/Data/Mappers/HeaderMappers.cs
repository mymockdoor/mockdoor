using MockDoor.Data.Models.Headers;
using MockDoor.Shared.Models.Headers;

namespace MockDoor.Data.Mappers
{
    public static class HeaderMappersExtensions
    {
        #region Service Headers
        public static ServiceHeaderDto ToDto(this ServiceHeader serviceHeader)
        {
            return new ServiceHeaderDto()
            {
                Enabled = serviceHeader.Enabled,
                Name = serviceHeader.Name,
                Incoming = serviceHeader.Incoming,
                Outgoing = serviceHeader.Outgoing
            };
        }

        public static List<ServiceHeaderDto> ToDtos(this List<ServiceHeader> serviceHeaders)
        {
            return serviceHeaders?.Select(sh => sh.ToDto()).ToList();
        }

        public static ServiceHeader ToEntity(this ServiceHeaderDto serviceHeaderDto)
        {
            return new ServiceHeader()
            {
                Enabled = serviceHeaderDto.Enabled,
                Name = serviceHeaderDto.Name,
                Incoming = serviceHeaderDto.Incoming,
                Outgoing = serviceHeaderDto.Outgoing
            };
        }

        public static List<ServiceHeader> ToModels(this List<ServiceHeaderDto> serviceHeaderDtos)
        {
            return serviceHeaderDtos?.Select(sh => sh.ToEntity()).ToList();
        }

        #endregion

        #region Service Request Headers
        public static ServiceRequestHeaderDto ToDto(this RequestHeader requestHeader)
        {
            return new ServiceRequestHeaderDto() { Name = requestHeader.Name, Value = requestHeader.Value };
        }

        public static List<ServiceRequestHeaderDto> ToDtos(this List<RequestHeader> requestHeaders)
        {
            return requestHeaders?.Select(rh => rh.ToDto()).ToList();
        }

        public static RequestHeader ToEntity(this ServiceRequestHeaderDto requestHeader)
        {
            return new RequestHeader() { Name = requestHeader.Name, Value = requestHeader.Value };
        }

        public static List<RequestHeader> ToEntities(this List<ServiceRequestHeaderDto> requestHeaderDtos)
        {
            return requestHeaderDtos?.Select(rh => rh.ToEntity()).ToList();
        }
        #endregion

        #region Request Response Headers
        public static MockResponseHeaderDto ToDto(this ResponseHeader responseHeader)
        {
            return new MockResponseHeaderDto() { Id = responseHeader.ID, Name = responseHeader.Name, Value = responseHeader.Value };
        }

        public static List<MockResponseHeaderDto> ToDtos(this List<ResponseHeader> responseHeaders)
        {
            return responseHeaders?.Select(rh => rh.ToDto()).ToList();
        }

        public static ResponseHeader ToEntity(this MockResponseHeaderDto responseHeaderDto)
        {
            return new ResponseHeader() { ID = responseHeaderDto.Id, Name = responseHeaderDto.Name, Value = responseHeaderDto.Value };
        }

        public static List<ResponseHeader> ToEntities(this List<MockResponseHeaderDto> responseHeaderDtos)
        {
            return responseHeaderDtos?.Select(rh => rh.ToEntity()).ToList();
        }
        #endregion
    }
}
