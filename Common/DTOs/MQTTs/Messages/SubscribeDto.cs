namespace Common.DTOs.MQTTs.Messages
{
    public class SubscribeDto
    {
        public string id { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public string topic { get; set; }
        public string Payload { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
