using Common.DTOs.Rests.ElevatorSetting;
using Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;
using System.Threading;

namespace ElevatorService.Mappings
{
    public class ElevatorSettingMapping
    {
        public ElevatorSetting Reponse(Response_Elevator_SettingDto response_Elevator_SettingDto)
        {
            var reponse = new ElevatorSetting
            {
                _id = response_Elevator_SettingDto._id,
                id = response_Elevator_SettingDto.id,
                ip = response_Elevator_SettingDto.ip,
                port = response_Elevator_SettingDto.port,
                mode = response_Elevator_SettingDto.mode,
                timeout = response_Elevator_SettingDto.timeout,
                createBy = response_Elevator_SettingDto.createBy,
                updateBy = response_Elevator_SettingDto.updateBy,
                createdAt = response_Elevator_SettingDto.createdAt,
                updatedAt = response_Elevator_SettingDto.updatedAt
            };
            return reponse;
        }
    }
}