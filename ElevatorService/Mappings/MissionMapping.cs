using Common.DTOs.MQTTs.Missions;
using Common.DTOs.Rests.Missions;
using Common.Models;
using System.Text.Json;

namespace ElevatorService.Mappings
{
    public class MissionMapping
    {
        public Mission Post(Post_MissionDto apiAddRequestDto, string elevatorId, string sourceFloor, string destFloor, string requestMode)
        {
            var model = new Mission
            {
                guid = Guid.NewGuid().ToString(),
                state = nameof(MissionState.PENDING),
                createdAt = DateTime.Now,

                orderId = apiAddRequestDto.orderId,
                jobId = apiAddRequestDto.jobId,
                acsMissionId = apiAddRequestDto.guid,
                name = apiAddRequestDto.name,
                carrierId = apiAddRequestDto.carrierId,
                service = apiAddRequestDto.service,
                type = apiAddRequestDto.type,
                subType = apiAddRequestDto.subType,
                linkedFacility = apiAddRequestDto.linkedFacility,
                sequence = apiAddRequestDto.sequence,
                isLocked = apiAddRequestDto.isLocked,
                sequenceChangeCount = apiAddRequestDto.sequenceChangeCount,
                retryCount = apiAddRequestDto.retryCount,
                specifiedWorkerId = apiAddRequestDto.specifiedWorkerId,
                assignedWorkerId = apiAddRequestDto.assignedWorkerId,
                elevatorId = elevatorId,
                sourceFloor = sourceFloor,
                destinationFloor = destFloor,
                requestMode = requestMode,
                parameterJson = JsonSerializer.Serialize(apiAddRequestDto.parameters),
            };
            return model;
        }

        public Get_MissionDto Get(Mission model)
        {
            var response = new Get_MissionDto()
            {
                guid = model.guid,
                state = model.state,
                createdAt = model.createdAt,
                updatedAt = model.updatedAt,
                finishedAt = model.finishedAt,

                orderId = model.orderId,
                jobId = model.jobId,
                acsMissionId = model.acsMissionId,
                carrierId = model.carrierId,
                service = model.service,
                type = model.type,
                subType = model.subType,
                sequence = model.sequence,
                linkedFacility = model.linkedFacility,
                isLocked = model.isLocked,
                sequenceChangeCount = model.sequenceChangeCount,
                retryCount = model.retryCount,
                specifiedWorkerId = model.specifiedWorkerId,
                assignedWorkerId = model.assignedWorkerId,
            };

            return response;
        }

        public Publish_MissionDto Publish(Mission model)
        {
            var publish = new Publish_MissionDto()
            {
                guid = model.guid,
                state = model.state,
                createdAt = model.createdAt,
                updatedAt = model.updatedAt,
                finishedAt = model.finishedAt,

                orderId = model.orderId,
                jobId = model.jobId,
                acsMissionId = model.acsMissionId,
                name = model.name,
                carrierId = model.carrierId,
                service = model.service,
                type = model.type,
                subType = model.subType,
                sequence = model.sequence,
                linkedFacility = model.linkedFacility,
                isLocked = model.isLocked,
                sequenceChangeCount = model.sequenceChangeCount,
                retryCount = model.retryCount,
                specifiedWorkerId = model.specifiedWorkerId,
                assignedWorkerId = model.assignedWorkerId,
            };
            return publish;
        }

        public Mission MqttUpdateStatus(Mission model, Subscribe_MissionDto missionData)
        {
            model.state = missionData.state.Replace(" ", "").ToUpper();
            model.updatedAt = DateTime.Now;

            return model;
        }
    }
}