using Common.DTOs;
using Common.Models;
using System.Text.Json;

namespace ElevatorService.Mappings
{
    public class MissionMapping
    {
        public Mission AddRequest(ApiAddRequestDtoMission apiAddRequestDto, string elevatorId, string sourceFloor, string destFloor)
        {
            var model = new Mission
            {
                orderId = apiAddRequestDto.orderId,
                jobId = apiAddRequestDto.jobId,
                acsMissionId = apiAddRequestDto.guid,
                guid = Guid.NewGuid().ToString(),
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

        public ResponseDtoMission Response(Mission model)
        {
            var response = new ResponseDtoMission()
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

        public MqttPublishDtoMission MqttPublish(Mission model)
        {
            var publish = new MqttPublishDtoMission()
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
            };
            return publish;
        }

        public Mission MqttUpdateStatus(Mission model, MqttSubscribeDtoMission missionData)
        {
            model.state = missionData.state.Replace(" ", "").ToUpper();
            model.updatedAt = DateTime.Now;

            return model;
        }
    }
}