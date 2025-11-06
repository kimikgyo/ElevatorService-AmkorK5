using Common.Models;
using System.Text.Json.Serialization;

namespace Common.DTOs
{
    public class MqttSubscribeDtoCommand
    {
        [JsonPropertyOrder(1)] public string commnadId { get; set; }
        [JsonPropertyOrder(2)] public string name { get; set; }
        [JsonPropertyOrder(3)] public string type { get; set; }
        [JsonPropertyOrder(4)] public string subType { get; set; }
        [JsonPropertyOrder(5)] public string state { get; set; }
        [JsonPropertyOrder(6)] public string WorkerId { get; set; }
        [JsonPropertyOrder(7)] public string actionName { get; set; }
        [JsonPropertyOrder(8)] public string parametersjson { get; set; }

        public override string ToString()
        {
            return
               $"commnadId = {commnadId,-5}" +
               $",name = {name,-5}" +
               $",type = {type,-5}" +
               $",subType = {subType,-5}" +
               $",state = {state,-5}" +
               $",WorkerId = {WorkerId,-5}" +
               $",actionName = {actionName,-5}" +
               $",parametersjson = {parametersjson,-5}";
        }
    }

    public class ApiRequestDtoPostCommand
    {
        public string guid { get; set; }
        public string name { get; set; }
        public string service { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public int sequence { get; set; }
        public string state { get; set; }
        public string assignedWorkerId { get; set; }
        public List<parameter> parameters { get; set; } = new List<parameter>();

        public override string ToString()
        {
            string parametersStr;

            if (parameters != null && parameters.Count > 0)
            {
                // 리스트 안의 Parametarr 각각을 { ... } 모양으로 변환
                var items = parameters
                    .Select(p => $"{{ key={p.key}, value={p.value} }}");

                // 여러 개 항목을 ", " 로 이어붙임
                parametersStr = string.Join(", ", items);
            }
            else
            {
                // 값이 없으면 빈 중괄호로 표시
                parametersStr = "{}";
            }

            return
            $" guid = {guid,-5}" +
            $",name = {name,-5}" +
            $",service = {service,-5}" +
            $",type = {type,-5}" +
            $",subType = {subType,-5}" +
            $",sequence = {sequence,-5}" +
            $",state = {state,-5}" +
            $",assignedWorkerId = {assignedWorkerId,-5}" +
            $",parametersStr = {parametersStr,-5}";
        }
    }
}