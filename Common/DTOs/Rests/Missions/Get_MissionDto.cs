using Common.Models;
using System.Text.Json.Serialization;

namespace Common.DTOs.Rests.Missions
{
    public class Get_MissionDto
    {
        [JsonPropertyOrder(1)] public string orderId { get; set; }
        [JsonPropertyOrder(2)] public string jobId { get; set; }
        [JsonPropertyOrder(3)] public string guid { get; set; }
        [JsonPropertyOrder(4)] public string acsMissionId { get; set; }
        [JsonPropertyOrder(5)] public string carrierId { get; set; }              // 자재 ID (nullable)
        [JsonPropertyOrder(6)] public string name { get; set; }
        [JsonPropertyOrder(7)] public string service { get; set; }
        [JsonPropertyOrder(8)] public string type { get; set; }
        [JsonPropertyOrder(9)] public string subType { get; set; }
        [JsonPropertyOrder(10)] public string linkedFacility { get; set; }
        [JsonPropertyOrder(11)] public int sequence { get; set; }                   //현재 명령의 실행 순서 이 값은 실행 전 재정렬에 따라 변경될 수 있음
        [JsonPropertyOrder(12)] public bool isLocked { get; set; }                   // 취소 불가
        [JsonPropertyOrder(13)] public int sequenceChangeCount { get; set; } = 0;   // 시퀀스가 변경된 누적 횟수 예: 재정렬이 3번 발생했다면 3
        [JsonPropertyOrder(14)] public int retryCount { get; set; } = 0;            // 명령 실패 시 재시도한 횟수 (기본값은 0)
        [JsonPropertyOrder(15)] public string state { get; set; }
        [JsonPropertyOrder(16)] public string specifiedWorkerId { get; set; }            //order 지정된 Worker
        [JsonPropertyOrder(17)] public string assignedWorkerId { get; set; }             //할당된 Worker
        [JsonPropertyOrder(18)] public DateTime createdAt { get; set; }                  // 생성 시각
        [JsonPropertyOrder(19)] public DateTime? updatedAt { get; set; }
        [JsonPropertyOrder(20)] public DateTime? finishedAt { get; set; }
        [JsonPropertyOrder(21)] public DateTime? sequenceUpdatedAt { get; set; }  // 시퀀스가 마지막으로 변경된 시간 재정렬 발생 시 이 값이 갱신됨
        [JsonPropertyOrder(22)] public List<Parameter> parameters { get; set; } = new List<Parameter>();          // 명령 실행 시 필요한 추가 옵션을 JSON 문자열로 저장  예: 속도, 방향, 특수 처리 조건 등

        public override string ToString()
        {
            string parametersStr;
            string preReportsStr;
            string postReportsStr;

            if (parameters != null && parameters.Count > 0)
            {
                // 리스트 안의 Parameter 각각을 { ... } 모양으로 변환
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
                $" orderId = {orderId,-5}" +
                $",jobId = {jobId,-5}" +
                $",acsMissionId = {acsMissionId,-5}" +
                $",guid = {guid,-5}" +
                $",carrierId = {carrierId,-5}" +
                $",name = {name,-5}" +
                $",service = {service,-5}" +
                $",type = {type,-5}" +
                $",subType = {subType,-5}" +
                $",linkedFacility = {linkedFacility,-5}" +
                $",sequence = {sequence,-5}" +
                $",isLocked = {isLocked,-5}" +
                $",sequenceChangeCount = {sequenceChangeCount,-5}" +
                $",retryCount = {retryCount,-5}" +
                $",state = {state,-5}" +
                $",specifiedWorkerId = {specifiedWorkerId,-5}" +
                $",assignedWorkerId = {assignedWorkerId,-5}" +
                $",createdAt = {createdAt,-5}" +
                $",updatedAt = {updatedAt,-5}" +
                $",finishedAt = {finishedAt,-5}" +
                $",sequenceUpdatedAt = {sequenceUpdatedAt,-5}" +
                $",parameters = [{parametersStr,-5}]";
        }
    }
}
