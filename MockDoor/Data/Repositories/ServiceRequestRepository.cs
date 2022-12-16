using Microsoft.EntityFrameworkCore;
using MockDoor.Abstractions.Repositories;
using MockDoor.Data.Contexts;
using MockDoor.Data.Mappers;
using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.ServiceRequest;

namespace MockDoor.Data.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly MockDoorMainContext _context;

        public ServiceRequestRepository(MockDoorMainContext context)
        {
            _context = context;
        }

        public async Task<ServiceRequestDto> GetServiceRequest(int id)
        {
            var sr = await _context.ServiceRequests
                                                        .Include(sr => sr.MockResponses)
                                                        .Include(sr => sr.QueryParameters)
                                                        .Include(sr => sr.RequestHeaders)
                                                        .FirstOrDefaultAsync(rr => rr.ID == id);

            if (sr == null)
                return null;

            return sr.ToDto(createNew: false, createChecksumOnResponses: true);
        }

        public async Task<UpdateServiceRequestDto> GetUpdateServiceRequest(int id)
        {
            var sr = await GetServiceRequest(id);

            return sr?.ToUpdateDto();
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetAllServiceRequestsForMicroserviceAsync(int microserviceId)
        {
            var serviceRequest = await _context.ServiceRequests
                                                                        .Include(sr => sr.QueryParameters)
                                                                        .Include(sr => sr.RequestHeaders)
                                                                        .Include(sr => sr.MockResponses)
                                                                        .ThenInclude(mr => mr.Headers)
                                                                        .Where(sr => sr.MicroserviceID == microserviceId)
                                                                        .ToListAsync();

            return serviceRequest.ToDtos(createNew: false, createChecksumOnResponses: false);
        }

        public async Task<ServiceRequestDto> CreateServiceRequestAsync(int microserviceId, ServiceRequestDto serviceRequestDto)
        {
            if (serviceRequestDto == null)
                throw new Exception("No request provided");

            serviceRequestDto.MicroserviceId = microserviceId;

            var serviceRequest = serviceRequestDto.ToEntity(createNew: true, createChecksumOnResponses: true);

            _context.ServiceRequests.Add(serviceRequest);

            await _context.SaveChangesAsync();

            return serviceRequest.ToDto(createNew: false, createChecksumOnResponses: true);
        }

        public async Task<UpdateServiceRequestDto> UpdateServiceRequest(int serviceRequestId, UpdateServiceRequestDto serviceRequestDto)
        {
            if (serviceRequestDto == null)
                throw new Exception("No request provided");

            var existingServiceRequest = await _context.ServiceRequests
                                                                .Include(sr => sr.MockResponses)
                                                                .Include(sr => sr.QueryParameters)
                                                                .Include(sr => sr.RequestHeaders)
                                                                .FirstOrDefaultAsync(t => t.ID == serviceRequestId);

            if (existingServiceRequest == null)
                return null;

            existingServiceRequest = existingServiceRequest.UpdateWithDto(serviceRequestDto);

            _context.ServiceRequests.Update(existingServiceRequest);

            await _context.SaveChangesAsync();

            return existingServiceRequest.ToUpdateDto(false);
        }

        public async Task<ServiceRequestDto> UpdateMockResponses(int serviceRequestId, List<MockResponseDto> responses)
        {
            if (serviceRequestId <= 0)
                throw new Exception("Invalid response id: " + serviceRequestId);

            if (responses == null)
                throw new Exception("No responses provided");

            var existingServiceRequest = await _context.ServiceRequests
                                                                .Include(sr => sr.MockResponses)
                                                                .Include(sr => sr.QueryParameters)
                                                                .Include(sr => sr.RequestHeaders)
                                                                .FirstOrDefaultAsync(t => t.ID == serviceRequestId);

            if (existingServiceRequest == null)
                return null;

            existingServiceRequest = existingServiceRequest.MergeResponses(responses);

            _context.ServiceRequests.Update(existingServiceRequest);

            await _context.SaveChangesAsync();

            return existingServiceRequest.ToDto(false, false);
        }

        public async Task<bool> DeleteRequest(int serviceRequestId)
        {
            var existedRequest = await _context.ServiceRequests.FirstOrDefaultAsync(rd => rd.ID == serviceRequestId);

            if (existedRequest == null)
                return false;

            _context.ServiceRequests.Remove(existedRequest);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task AddResponseToRequestAsync(int serviceRequestId, MockResponseDto mockResponseDto)
        {
            mockResponseDto.ServiceRequestId = serviceRequestId;

            var newMockResponse = mockResponseDto.ToEntity(true);

            _context.MockResponses.Add(newMockResponse);

            await _context.SaveChangesAsync();
        }

        public async Task<DateTime[]> GetMockResponseTimes(int requestId)
        {
            var dateTimes = _context.MockResponses.Where(rr => rr.ServiceRequestId == requestId).Select(rr => rr.CreatedUtc);

            return await dateTimes.ToArrayAsync();
        }
    }
}
