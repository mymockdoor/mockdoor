﻿using Microsoft.EntityFrameworkCore;
using MockDoor.Abstractions.Repositories;
using MockDoor.Data.Contexts;
using MockDoor.Data.Helpers;
using MockDoor.Data.Mappers;
using MockDoor.Shared.Models.Response;

namespace MockDoor.Data.Repositories
{
    public class MockResponseRepository : IMockResponseRepository
    {
        private readonly MockDoorMainContext _context;

        public MockResponseRepository(MockDoorMainContext context)
        {
            _context = context;
        }

        public async Task<MockResponseDto> GetMockResponseAsync(int id)
        {
            var response = await _context.MockResponses.Include(mr => mr.Headers).FirstOrDefaultAsync(rr => rr.ID == id);

            if (response == null)
                return null;

            return response.ToDto(false);
        }

        public async Task<UpdateMockResponseDto> GetUpdateMockResponseAsync(int id)
        {
            var response = await GetMockResponseAsync(id);

            return response?.ToUpdateDto();
        }

        public async Task<UpdateMockResponseDto> PatchMockResponseAsync(int mockResponseId, UpdateMockResponseDto updateMockResponse)
        {
            if (updateMockResponse == null)
                throw new Exception("No response provided");

            var existingMockResponse = await _context.MockResponses.FirstOrDefaultAsync(t => t.ID == mockResponseId);

            if (existingMockResponse == null)
                return null;

            existingMockResponse = existingMockResponse.UpdateWithDto(updateMockResponse, generateChecksum: true);

            _context.MockResponses.Update(existingMockResponse);

            await _context.SaveChangesAsync();

            return existingMockResponse.ToUpdateDto();
        }

        public async Task<MockResponseDto> UpdateMockResponseAsync(int mockResponseId, MockResponseDto updatedResponse)
        {
            if (updatedResponse == null)
                throw new Exception("No response provided");

            if (mockResponseId == 0)
            {
                throw new Exception("No request id provided");
            }

            var request = _context.ServiceRequests.Include(sr => sr.MockResponses).FirstOrDefault(sr => sr.ID == mockResponseId);

            if (request == null)
                throw new Exception("Request not found");

            if (updatedResponse.Id > 0)
            {
                var existingMockResponse = await _context.MockResponses.FirstOrDefaultAsync(t => t.ID == updatedResponse.Id);

                if (existingMockResponse == null)
                    throw new Exception("Error response does not exist");

                existingMockResponse.Description = updatedResponse.Description;
                existingMockResponse.Body = updatedResponse.Body;
                existingMockResponse.Encoding = updatedResponse.Encoding;
                existingMockResponse.ContentType = updatedResponse.ContentType;
                existingMockResponse.Code = updatedResponse.Code;
                existingMockResponse.Priority = updatedResponse.Priority;
                existingMockResponse.Checksum = ChecksumHelpers.CreateDefaultChecksum(updatedResponse);
                existingMockResponse.CreatedUtc = updatedResponse.CreatedUtc;

                _context.MockResponses.Update(existingMockResponse);

                await _context.SaveChangesAsync();

                return existingMockResponse.ToDto(false);
            }
            else
            {
                var newModel = updatedResponse.ToEntity(true);
                request.MockResponses.Add(newModel);

                await _context.SaveChangesAsync();

                return newModel.ToDto(false);
            }
        }

        public async Task<(bool success, MockResponseDto result)> CreateAsync(int requestId, MockResponseDto response)
        {
            if (response == null)
            {
                throw new Exception("No response provided");
            }

            var existingRequest = _context.ServiceRequests.Include(r => r.MockResponses).FirstOrDefault(r => r.ID == requestId);

            if (existingRequest == null)
            {
                throw new Exception("Error request does not exist");
            }
            
            var model = response.ToEntity(true);

            existingRequest.MockResponses.Add(model);
            await _context.SaveChangesAsync();
            return (true, model.ToDto(false));
        }


        public async Task<(bool success, List<MockResponseDto> result)> CreateBulkAsync(int requestId, List<MockResponseDto> responses)
        {
            if (responses == null)
            {
                throw new Exception("No responses provided");
            }

            if (responses.Count == 0)
            {
                return (true, new List<MockResponseDto>());
            }

            var existingRequest = _context.ServiceRequests.Include(r => r.MockResponses).FirstOrDefault(r => r.ID == requestId);

            if (existingRequest == null)
            {
                return (false, null);
            }

            responses.ForEach(action => action.Id = 0);

            var models = responses.ToEntities(true).DistinctBy(nr => nr.Checksum).Where(nr => existingRequest.MockResponses.All(rr => rr.Checksum != nr.Checksum)).ToList();

            if (models.Any())
            {
                existingRequest.MockResponses.AddRange(models);
                await _context.SaveChangesAsync();
                return (true, models.ToDtos(false));
            }
            else
            {
                return (false, new List<MockResponseDto>());
            }
        }


        public async Task<bool> DeleteBulkAsync(int requestId, List<MockResponseDto> responses)
        {
            if (responses == null || responses.Count == 0)
            {
                return true;
            }

            var existingRequest = _context.ServiceRequests.Include(r => r.MockResponses).FirstOrDefault(r => r.ID == requestId);

            if (existingRequest == null)
            {
                return false;
            }

            foreach (var responseToDelete in responses)
            {
                existingRequest.MockResponses.RemoveAll(response => response.ID == responseToDelete.Id);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> DeleteAsync(int responseId)
        {
            if (responseId <= 0)
            {
                return false;
            }

            var existingResponse = _context.MockResponses.FirstOrDefault(r => r.ID == responseId);

            if (existingResponse == null)
            {
                return false;
            }

            _context.MockResponses.Remove(existingResponse);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
