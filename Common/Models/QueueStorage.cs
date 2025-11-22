using Common.DTOs.MQTTs.Messages;
using System.Collections.Concurrent;

namespace Common.Models
{
    public static class QueueStorage
    {
        #region MQTT

        private static readonly ConcurrentQueue<PublishDto> publishOrder = new ConcurrentQueue<PublishDto>();
        private static readonly ConcurrentQueue<PublishDto> publishJob = new ConcurrentQueue<PublishDto>();
        private static readonly ConcurrentQueue<PublishDto> publishMission = new ConcurrentQueue<PublishDto>();
        private static readonly ConcurrentQueue<PublishDto> publishPosition = new ConcurrentQueue<PublishDto>();

        private static readonly ConcurrentQueue<SubscribeDto> mqttMessagesSubscribe = new ConcurrentQueue<SubscribeDto>();
        private static readonly ConcurrentQueue<SubscribeDto> mqttSubscribeWorker = new ConcurrentQueue<SubscribeDto>();
        private static readonly ConcurrentQueue<SubscribeDto> mqttSubscribeElevator = new ConcurrentQueue<SubscribeDto>();

        public static void MqttEnqueuePublishOrder(PublishDto item)
        {
            //미션 및 Queue 를 실행한부분을 순차적으로 추가시킨다
            publishOrder.Enqueue(item);
        }

        public static bool MqttTryDequeuePublishOrder(out PublishDto item)
        {
            //실행하면 순차적으로 하나씩 Return한다
            return publishOrder.TryDequeue(out item);
        }

        public static void MqttEnqueuePublishJob(PublishDto item)
        {
            //미션 및 Queue 를 실행한부분을 순차적으로 추가시킨다
            publishJob.Enqueue(item);
        }

        public static bool MqttTryDequeuePublishJob(out PublishDto item)
        {
            //실행하면 순차적으로 하나씩 Return한다
            return publishJob.TryDequeue(out item);
        }

        public static void MqttEnqueuePublishMission(PublishDto item)
        {
            //미션 및 Queue 를 실행한부분을 순차적으로 추가시킨다
            publishMission.Enqueue(item);
        }

        public static bool MqttTryDequeuePublishMission(out PublishDto item)
        {
            //실행하면 순차적으로 하나씩 Return한다
            return publishMission.TryDequeue(out item);
        }

        public static void MqttEnqueuePublishPosition(PublishDto item)
        {
            //미션 및 Queue 를 실행한부분을 순차적으로 추가시킨다
            publishPosition.Enqueue(item);
        }

        public static bool MqttTryDequeuePublishPosition(out PublishDto item)
        {
            //실행하면 순차적으로 하나씩 Return한다
            return publishPosition.TryDequeue(out item);
        }

        public static void MqttEnqueueSubscribeWorker(SubscribeDto item)
        {
            //미션 및 Queue 를 실행한부분을 순차적으로 추가시킨다
            mqttSubscribeWorker.Enqueue(item);
        }

        public static bool MqttTryDequeueSubscribeWorker(out SubscribeDto item)
        {
            //실행하면 순차적으로 하나씩 Return한다
            return mqttSubscribeWorker.TryDequeue(out item);
        }

        public static void MqttEnqueueSubscribeElevator(SubscribeDto item)
        {
            //미션 및 Queue 를 실행한부분을 순차적으로 추가시킨다
            mqttSubscribeElevator.Enqueue(item);
        }

        public static bool MqttTryDequeueSubscribeElevator(out SubscribeDto item)
        {
            //실행하면 순차적으로 하나씩 Return한다
            return mqttSubscribeElevator.TryDequeue(out item);
        }

        public static void MqttEnqueueSubscribe(SubscribeDto item)
        {
            //미션 및 Queue 를 실행한부분을 순차적으로 추가시킨다
            mqttMessagesSubscribe.Enqueue(item);
        }

        public static bool MqttTryDequeueSubscribe(out SubscribeDto item)
        {
            //실행하면 순차적으로 하나씩 Return한다
            return mqttMessagesSubscribe.TryDequeue(out item);
        }

        #endregion MQTT
    }
}