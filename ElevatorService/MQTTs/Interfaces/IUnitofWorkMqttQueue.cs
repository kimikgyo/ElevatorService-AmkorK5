using Common.Models;

namespace ElevatorService.MQTTs.Interfaces
{
    public interface IUnitofWorkMqttQueue
    {
        void MqttPublishMessage(TopicType topicType, TopicSubType topicSubType, object value);

        void HandleReceivedMqttMessage();
    }
}