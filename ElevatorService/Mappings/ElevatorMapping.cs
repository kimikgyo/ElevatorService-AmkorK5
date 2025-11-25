using Common.DTOs.MQTTs.Devices;
using Common.Models;

namespace ElevatorService.Mappings
{
    public class ElevatorMapping()
    {
        public Device MqttCreateElevator(Subscribe_DeviceStatusDto mqttSubscribeDtoStatusDevice)
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

        public void MqttUpdate(Device elevator, Subscribe_DeviceStatusDto mqttSubscribeDtoStatusDevice)
        {
            elevator.id = mqttSubscribeDtoStatusDevice.id;
            elevator.name = mqttSubscribeDtoStatusDevice.name;
            elevator.mode = mqttSubscribeDtoStatusDevice.mode;
            elevator.state = mqttSubscribeDtoStatusDevice.state;
            elevator.updateAt = DateTime.Now;
        }
    }
}