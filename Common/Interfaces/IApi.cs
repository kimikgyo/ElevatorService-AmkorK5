using Common.DTOs.Rests.Maps;
using Data.Interfaces;

namespace Common.Models
{
    public interface IApi
    {
        Uri BaseAddress { get; }

        Task<List<Response_MapDto>> GetResourceMap();

        Task<ResponseDto> ElevatorPostCommandQueueAsync(object value);

        Task<ResponseDto> ElevatorDeleteCommandQueueAsync(string id);

        Task<ResponseDto> PositionPatchAsync(string id, object value);
    }
}