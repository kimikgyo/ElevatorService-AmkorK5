namespace Common.DTOs.MQTTs.Messages
{
    public class PublishDto
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
