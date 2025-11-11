using System.Data;

namespace Common.Models
{
    public enum CommandType
    {
        MOVE,
        ACTION,
        MAP
    }

    public enum CommandSubType
    {
        ELEVATORENTER,
        ELEVATOREXIT,
        SOURCEFLOOR,
        DOOROPEN,
        DOORCLOSE,
        DESTINATIONFLOOR,
        DESTINATIONCHANGE
    }

    public enum CommandState
    {
        INIT,
        WAITING,                    // Scheduler 대기중
        SKIPPED,                    //미션을 스킵하는경우
        COMMANDREQUEST,              // Scheduler Post진행시
        COMMANDREQUESTCOMPLETED,     // Scheduler Post진행시
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
    public enum Command_ElevatorAction
    {
        None = 0,
        AGVMODE,
        NOTAGVMODE,
        DOOROPEN,
        DOORCLOSE,
        CALL_B1F,
        CALL_1F,
        CALL_2F,
        CALL_3F,
        CALL_4F,
        CALL_5F,
        CALL_6F,
        GOTO_B1F,
        GOTO_1F,
        GOTO_2F,
        GOTO_3F,
        GOTO_4F,
        GOTO_5F,
        GOTO_6F,
    }

    public class Command
    {
        public string acsMissionId { get; set; }
        public string guid { get; set; }
        public string name { get; set; }
        public string service { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public int sequence { get; set; }
        public string state { get; set; }
        public string assignedWorkerId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? finishedAt { get; set; }
        public string parameterJson { get; set; }
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
            $" acsMissionId = {acsMissionId,-5}" +
            $" guid = {guid,-5}" +
            $",name = {name,-5}" +
            $",service = {service,-5}" +
            $",type = {type,-5}" +
            $",subType = {subType,-5}" +
            $",sequence = {sequence,-5}" +
            $",state = {state,-5}" +
            $",assignedWorkerId = {assignedWorkerId,-5}" +
            $",createdAt = {createdAt,-5}" +
            $",updatedAt = {updatedAt,-5}" +
            $",finishedAt = {finishedAt,-5}" +
            $",parameterJson = {parameterJson,-5}" +
            $",parametersStr = {parametersStr,-5}";
        }
    }
}