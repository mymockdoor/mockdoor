using MockDoor.Data.Models;
using MockDoor.Shared.Models.QueryParameters;

namespace MockDoor.Data.Mappers;

public static class QueryParameterMappers
{
    public static QueryParameter ToEntity(this QueryParameterDto queryParameterDto)
    {
        return queryParameterDto == null
            ? null
            : new QueryParameter()
            {
                Name = queryParameterDto.Name,
                Value = queryParameterDto.Value,
                OrderIndex = queryParameterDto.OrderIndex,
                ServiceRequestId = queryParameterDto.ServiceRequestId
            };
    }

    public static List<QueryParameter> ToEntities(this List<QueryParameterDto> queryParameterDtos)
    {
        return queryParameterDtos?.Select(qp => qp.ToEntity()).ToList();
    }

    public static QueryParameterDto ToDto(this QueryParameter queryParameter)
    {
        return queryParameter == null
            ? null
            : new QueryParameterDto()
            {
                Name = queryParameter.Name,
                Value = queryParameter.Value,
                OrderIndex = queryParameter.OrderIndex,
                ServiceRequestId = queryParameter.ServiceRequestId
            };
    }

    public static List<QueryParameterDto> ToDtos(this List<QueryParameter> queryParameters)
    {
        return queryParameters?.Select(rr => rr.ToDto()).ToList();
    }

    public static QueryParameter UpdateWithDto(this QueryParameter baseQueryParameter,
        QueryParameterDto updateQueryParameter)
    {
        if (baseQueryParameter == null)
            throw new Exception("cannot update null request response");

        if (updateQueryParameter == null)
            return baseQueryParameter;

        baseQueryParameter.Name = updateQueryParameter.Name;
        baseQueryParameter.Value = updateQueryParameter.Value;
        baseQueryParameter.OrderIndex = updateQueryParameter.OrderIndex;

        return baseQueryParameter;
    }
}