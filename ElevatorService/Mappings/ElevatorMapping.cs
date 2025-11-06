using Common.DTOs;
using Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ElevatorService.Mappings
{
    public class ElevatorMapping()
    {
        public Elevator MqttCreateElevator(MqttSubscribeDtoStatusElevator mqttSubscribeDtoStatusElevator)
        {
            var model = new Elevator
            {
              id = mqttSubscribeDtoStatusElevator.id,
              name = mqttSubscribeDtoStatusElevator.name,
              mode = mqttSubscribeDtoStatusElevator.mode,
              state = mqttSubscribeDtoStatusElevator.state,
              createAt = DateTime.Now,
            };
            return model;
        }

        public void MqttUpdate(Elevator elevator,MqttSubscribeDtoStatusElevator mqttSubscribeDtoStatusElevator)
        {
            elevator.id = mqttSubscribeDtoStatusElevator.id;
            elevator.name = mqttSubscribeDtoStatusElevator.name;
            elevator.mode = mqttSubscribeDtoStatusElevator.mode;
            elevator.state = mqttSubscribeDtoStatusElevator.state;
            elevator.updateAt = DateTime.Now;
        }
    }
}
