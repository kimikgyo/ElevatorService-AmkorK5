using Common.DTOs;
using Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ElevatorService.Mappings
{
    public class ElevatorMapping()
    {
        public Device MqttCreateElevator(MqttSubscribeDtoStatusDevice mqttSubscribeDtoStatusDevice)
        {
            var model = new Device
            {
              id = mqttSubscribeDtoStatusDevice.id,
              name = mqttSubscribeDtoStatusDevice.name,
              mode = mqttSubscribeDtoStatusDevice.mode,
              state = mqttSubscribeDtoStatusDevice.state,
              createAt = DateTime.Now,
            };
            return model;
        }

        public void MqttUpdate(Device elevator, MqttSubscribeDtoStatusDevice mqttSubscribeDtoStatusDevice)
        {
            elevator.id = mqttSubscribeDtoStatusDevice.id;
            elevator.name = mqttSubscribeDtoStatusDevice.name;
            elevator.mode = mqttSubscribeDtoStatusDevice.mode;
            elevator.state = mqttSubscribeDtoStatusDevice.state;
            elevator.updateAt = DateTime.Now;
        }
    }
}
