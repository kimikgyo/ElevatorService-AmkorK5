using Common.DTOs.Rests.Maps;
using Data.Interfaces;

namespace Common.Models
{
    public interface IApi
    {
        Uri BaseAddress { get; }

        Task<List<Response_MapDto>> GetResourceMap();

        Task<ApResponseDto> ElevatorPostCommandQueueAsync(object value);

        Task<ApResponseDto> ElevatorDeleteCommandQueueAsync(string id);

        Task<ApResponseDto> PositionPatchAsync(string id, object value);
    }
}