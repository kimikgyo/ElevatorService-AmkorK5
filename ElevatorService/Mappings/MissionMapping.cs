using Common.DTOs.MQTTs.Missions;
using Common.DTOs.Rests.Missions;
using Common.Models;
using System.Text.Json;

namespace ElevatorService.Mappings
{
    public class MissionMapping
    {
        public Mission AddRequest(Post_MissionDto apiAddRequestDto, string elevatorId, string sourceFloor, string destFloor)
        {
            var model = new Mission
            {
                orderId = apiAddRequestDto.orderId,
                jobId = apiAddRequestDto.jobId,
                acsMissionId = apiAddRequestDto.guid,
                guid = Guid.NewGuid().ToString(),
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
                state = nameof(MissionState.PENDING),
                specifiedWorkerId = apiAddRequestDto.specifiedWorkerId,
                assignedWorkerId = apiAddRequestDto.assignedWorkerId,
                elevatorId = elevatorId,
                sourceFloor = sourceFloor,
                destinationFloor = destFloor,
                parameterJson = JsonSerializer.Serialize(apiAddRequestDto.parameters),
                createdAt = DateTime.Now,
            };
            return model;
        }

        public Get_MissionDto Response(Mission model)
        {
            var response = new Get_MissionDto()
            {
                orderId = model.orderId,
                jobId = model.jobId,
                guid = model.guid,
                carrierId = model.carrierId,
                service = model.service,
                type = model.type,
                subType = model.subType,
                sequence = model.sequence,
                linkedFacility = model.linkedFacility,
                isLocked = model.isLocked,
                sequenceChangeCount = model.sequenceChangeCount,
                retryCount = model.retryCount,
                state = model.state,
                specifiedWorkerId = model.specifiedWorkerId,
                assignedWorkerId = model.assignedWorkerId,
                createdAt = model.createdAt,
                updatedAt = model.updatedAt,
                finishedAt = model.finishedAt,
            };

            return response;
        }

        public Publish_MissionDto MqttPublish(Mission model)
        {
            var publish = new Publish_MissionDto()
            {
                orderId = model.orderId,
                jobId = model.jobId,
                acsMissionId = model.acsMissionId,
                guid = model.guid,
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
                state = model.state,
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