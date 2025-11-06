using System.Text.Json.Serialization;

namespace Common.Models
{
    public class parameter
    {
        [JsonPropertyOrder(1)] public string key { get; set; }
        [JsonPropertyOrder(2)] public string value { get; set; }

        // 사람용 요약 (디버거/로그에서 보기 좋게)
        public override string ToString()
        {
            return
                $"key = {key,-5}" +
                $",value = {value,-5}";
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
    public class PreReport
    {
        [JsonPropertyOrder(1)] public int ceid { get; set; }
        [JsonPropertyOrder(2)] public string eventName { get; set; }
        [JsonPropertyOrder(3)] public int rptid { get; set; }

        public override string ToString()
        {
            return
                $"ceid = {ceid,-5}" +
                $",eventName = {eventName,-5}" +
                $",rptid = {rptid,-5}";
        }
    }
    public class PostReport
    {
        [JsonPropertyOrder(1)] public int ceid { get; set; }
        [JsonPropertyOrder(2)] public string eventName { get; set; }
        [JsonPropertyOrder(3)] public int rptid { get; set; }

        public override string ToString()
        {
            return
                $"ceid = {ceid,-5}" +
                $",eventName = {eventName,-5}" +
                $",rptid = {rptid,-5}";
        }
    }


}
