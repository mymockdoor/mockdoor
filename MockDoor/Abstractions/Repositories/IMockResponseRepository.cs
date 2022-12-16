using MockDoor.Shared.Models.Response;

namespace MockDoor.Abstractions.Repositories
{
    public interface IMockResponseRepository
    {
        Task<(bool success, MockResponseDto result)> CreateAsync(int requestId, MockResponseDto response);
        Task<(bool success, List<MockResponseDto> result)> CreateBulkAsync(int requestId, List<MockResponseDto> responses);
        Task<bool> DeleteAsync(int responseId);
        Task<bool> DeleteBulkAsync(int requestId, List<MockResponseDto> responses);
        Task<MockResponseDto> GetMockResponseAsync(int id);
        Task<UpdateMockResponseDto> GetUpdateMockResponseAsync(int id);

        Task<UpdateMockResponseDto> PatchMockResponseAsync(int mockResponseId, UpdateMockResponseDto updateMockResponse);
        Task<MockResponseDto> UpdateMockResponseAsync(int mockResponseId, MockResponseDto updatedResponse);
    }
}