using Common.Models;
using Data.Interfaces;

namespace Data.Repositorys
{
    public static class ConfigData
    {
        public static List<ServiceApi> ServiceApis { get; set; }
        public static List<MqttTopicSubscribe> SubscribeTopics { get; set; }
        public static List<MqttTopicPublish> PublishTopics { get; set; }
        public static MQTTSetting mQTTSetting { get; set; }

        public static void Load(IConfiguration configuration)
        {
            ConfigData.ServiceApis = configuration.GetSection("ServiceApiInfo").Get<List<ServiceApi>>();
            ConfigData.mQTTSetting = configuration.GetSection("MQTTSetting").Get<MQTTSetting>();
            ConfigData.SubscribeTopics = configuration.GetSection("MqttTopicSubscribe").Get<List<MqttTopicSubscribe>>();
            ConfigData.PublishTopics = configuration.GetSection("MqttTopicPublish").Get<List<MqttTopicPublish>>();
        }
    }
}
