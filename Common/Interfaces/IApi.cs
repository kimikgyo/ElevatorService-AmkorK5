using Common.DTOs.Bases;
using Data.Interfaces;

namespace Common.Models
{
    public interface IApi
    {
        Uri BaseAddress { get; }

        Task<List<ApiGetResponseDtoResourceMap>> GetResourceMap();

        Task<ApResponseDto> WorkerPostMissionQueueAsync(object value);

        Task<ApResponseDto> WorkerDeleteMissionQueueAsync(string id);

        Task<ApResponseDto> ElevatorPostCommandQueueAsync(object value);


        Task<ApResponseDto> PositionPatchAsync(string id, object value);
    }
}