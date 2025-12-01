using Common.DTOs.Rests.Commands;
using Common.Models;

namespace ElevatorService.Mappings
{
    public class CommandMapping
    {
        public Request_CommandDto Request(Command model)
        {
            var Request = new Request_CommandDto
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

            return Request;
        }
    }
}