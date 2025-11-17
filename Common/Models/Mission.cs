using System;
using System.Data;
using System.Text.Json.Serialization;

namespace Common.Models
{
    public enum Service
    {
        NO1,
    }

    public enum MissionType
    {
    }

    //MISSION 상태
    public enum MissionState
    {
        SKIPPED,
        PENDING,                    // WorkerService 대기중
        EXECUTING,                 // Worker 작업중
        CANCELED,                   // Worker 작업취소
        FAILED,                     // Worker에서 실패했을경우
        COMPLETED,                  // Worker에서 성공 완료
        ABORTINITED,                 // SchedulerAbort 접수
        ABORTCOMPLETED,             // SchedulerAbort 완료
        ABORTFAILED,                // SchedulerAbort 실패
        CANCELINITED,                // SchedulerCancel 접수
        CANCELINITCOMPLETED,        // SchedulerCancel 완료
        CNACELFAILED                // SchedulerCancel 실패
    }

    public enum MissionSubType
    {
    }

    public class Mission
    {
        [JsonPropertyOrder(1)] public string orderId { get; set; }
        [JsonPropertyOrder(2)] public string jobId { get; set; }
        [JsonPropertyOrder(3)] public string acsMissionId { get; set; }
        [JsonPropertyOrder(4)] public string guid { get; set; }
        [JsonPropertyOrder(5)] public string carrierId { get; set; }
        [JsonPropertyOrder(6)] public string service { get; set; }
        [JsonPropertyOrder(7)] public string type { get; set; }
        [JsonPropertyOrder(8)] public string subType { get; set; }
        [JsonPropertyOrder(9)] public string linkedFacility { get; set; }
        [JsonPropertyOrder(10)] public int sequence { get; set; }
        [JsonPropertyOrder(11)] public bool isLocked { get; set; }
        [JsonPropertyOrder(12)] public int sequenceChangeCount { get; set; }
        [JsonPropertyOrder(13)] public int retryCount { get; set; }
        [JsonPropertyOrder(14)] public string state { get; set; }
        [JsonPropertyOrder(15)] public string specifiedWorkerId { get; set; }
        [JsonPropertyOrder(16)] public string assignedWorkerId { get; set; }
        [JsonPropertyOrder(17)] public string elevatorId { get; set; }
        [JsonPropertyOrder(18)] public string sourceFloor { get; set; }
        [JsonPropertyOrder(19)] public string destinationFloor { get; set; }
        [JsonPropertyOrder(20)] public string parameterJson { get; set; }
        [JsonPropertyOrder(21)] public DateTime createdAt { get; set; }
        [JsonPropertyOrder(22)] public DateTime? updatedAt { get; set; }
        [JsonPropertyOrder(23)] public DateTime? finishedAt { get; set; }

        // 사람용 요약 (디버거/로그에서 보기 좋게)
        public override string ToString()
        {
            return
                $" orderId = {orderId,-5}" +
                $",jobId = {jobId,-5}" +
                $",acsMissionId = {acsMissionId,-5}" +
                $",guid = {guid,-5}" +
                $",carrierId = {carrierId,-5}" +
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
                $",elevatorId = {elevatorId,-5}" +
                $",sourceFloor = {sourceFloor,-5}" +
                $",destinationFloor = {destinationFloor,-5}" +
                $",parameterJson = {parameterJson,-5}" +
                $",createdAt = {createdAt,-5}" +
                $",updatedAt = {updatedAt,-5}" +
                $",finishedAt = {finishedAt,-5}";
        }

        // 기계용 JSON (전송/저장에만 사용)
        //public string ToJson(bool indented = false)
        //{
        //    return JsonSerializer.Serialize(
        //        this,
        //        new JsonSerializerOptions
        //        {
        //            IncludeFields = true,
        //            WriteIndented = indented
        //        });
        //}
    }
}