using Common.Models;

namespace Common.DTOs.Rests.Commands
{
    public class Request_CommandDto
    {
        public string guid { get; set; }
        public string name { get; set; }
        public string service { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public int sequence { get; set; }
        public string state { get; set; }
        public string assignedWorkerId { get; set; }
        public List<Parameter> parameters { get; set; } = new List<Parameter>();

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
