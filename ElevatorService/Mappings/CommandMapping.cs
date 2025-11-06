using Common.DTOs;
using Common.Models;

namespace ElevatorService.Mappings
{
    public class CommandMapping
    {
        public ApiRequestDtoPostCommand ApiRequestDtoPostCommand(Command model)
        {
            var apiRequest = new ApiRequestDtoPostCommand
            {
                guid = model.guid,
                name = model.name,
                service = model.service,
                type = model.type,
                subType = model.subType,
                sequence = model.sequence,
                state = model.state,
                assignedWorkerId = model.assignedWorkerId,
                parameters = model.parameters,
            };

            return apiRequest;
        }
    }
}