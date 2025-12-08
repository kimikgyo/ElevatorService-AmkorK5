using Common.DTOs.Rests.ElevatorSetting;
using Common.DTOs.Rests.Maps;
using Common.DTOs.Rests.Positions;
using Common.Models;
using Data.Interfaces;
using log4net;
using Newtonsoft.Json;
using static ExceptionFilterUtility;

namespace RestApi.Interfases
{
    public class Api : IApi, IDisposable
    {
        private static readonly ILog ApiLogger = LogManager.GetLogger("ApiEvent");
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _settings;
        private readonly string _type;
        public Uri BaseAddress => _httpClient.BaseAddress;

        public Api(string type, string ip, string port, double timeout, string connectId, string connectPassword, JsonSerializerSettings settings = null)
        {
            _type = type;
            _httpClient = MakeHttpClient(ip, port, timeout, connectId, connectPassword);
            _settings = settings ?? new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
        }

        private HttpClient MakeHttpClient(string ip, string port, double timeout, string connectId, string connectPassword)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
            string uriString = $"http://{ip.Trim()}:{port.TrimEnd('/')}";
            httpClient.BaseAddress = new Uri(uriString);
            return httpClient;
        }

        public async Task<List<Response_Elevator_SettingDto>> Get_Elevators_Async()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Response_Elevator_SettingDto>>("api/elevators");
            }
            //catch (Exception ex) when (True(() => _logger.Error(ex)))
            catch (Exception ex) when (True(() => ApiLogger.Error($"IPAddress = {_httpClient.BaseAddress}" + "\r\n" + ex)))
            {
                return null;
            }
        }

        public async Task<ResponseDto> ElevatorPostCommandQueueAsync(object value)
        {
            if (!AcceptFilterUtility.WriteAccepted) { ApiLogger.Error($"IPAddress = {_httpClient.BaseAddress}" + "\r\n" + $"-- API NOT ALLOWED. [{nameof(ElevatorPostCommandQueueAsync)}] --"); return null; }

            try
            {
                //수정본
                var response = await _httpClient.PostAsJsonAsync($"api/command", value);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var missionQueueResponse = new ResponseDto
                {
                    statusCode = Convert.ToInt32(response.StatusCode),
                    statusText = response.StatusCode.ToString(),
                    message = jsonResponse
                };
                return missionQueueResponse;

                //기존
                //var response = await _httpClient.PostAsJsonAsync("api/Workers/mission_queue", value);
                //var jsonResponse = await response.Content.ReadAsStringAsync();
                //return JsonConvert.DeserializeObject<ApiPostResponseDtoMissionQueue>(jsonResponse);
            }
            //catch (Exception ex) when (True(() => _logger.Error(ex)))
            catch (Exception ex) when (True(() => ApiLogger.Error($"IPAddress = {_httpClient.BaseAddress}" + "\r\n" + ex)))
            {
                return null;
            }
        }

        public async Task<ResponseDto> ElevatorDeleteCommandQueueAsync(string id)
        {
            if (!AcceptFilterUtility.WriteAccepted) { ApiLogger.Error($"IPAddress = {_httpClient.BaseAddress}" + "\r\n" + $"-- API NOT ALLOWED. [{nameof(ElevatorPostCommandQueueAsync)}] --"); return null; }

            try
            {
                //수정본
                var response = await _httpClient.DeleteAsync($"api/command/{id}");
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var missionQueueResponse = new ResponseDto
                {
                    statusCode = Convert.ToInt32(response.StatusCode),
                    statusText = response.StatusCode.ToString(),
                    message = jsonResponse
                };
                return missionQueueResponse;

                //기존
                //var response = await _httpClient.PostAsJsonAsync("api/Workers/mission_queue", value);
                //var jsonResponse = await response.Content.ReadAsStringAsync();
                //return JsonConvert.DeserializeObject<ApiPostResponseDtoMissionQueue>(jsonResponse);
            }
            //catch (Exception ex) when (True(() => _logger.Error(ex)))
            catch (Exception ex) when (True(() => ApiLogger.Error($"IPAddress = {_httpClient.BaseAddress}" + "\r\n" + ex)))
            {
                return null;
            }
        }

        public async Task<ResponseDto> PositionPatchAsync(string Id, object value)
        {
            if (!AcceptFilterUtility.WriteAccepted) { ApiLogger.Error($"IPAddress = {_httpClient.BaseAddress}" + "\r\n" + $"-- API NOT ALLOWED. [{nameof(PositionPatchAsync)}] --"); return null; }

            try
            {
                //수정본
                var response = await _httpClient.PatchAsJsonAsync($"api/positions/{Id}", value);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var missionQueueResponse = new ResponseDto
                {
                    statusCode = Convert.ToInt32(response.StatusCode),
                    statusText = response.StatusCode.ToString(),
                    message = jsonResponse
                };
                return missionQueueResponse;

                //기존
                //var response = await _httpClient.PostAsJsonAsync("api/Workers/mission_queue", value);
                //var jsonResponse = await response.Content.ReadAsStringAsync();
                //return JsonConvert.DeserializeObject<ApiPostResponseDtoMissionQueue>(jsonResponse);
            }
            //catch (Exception ex) when (True(() => _logger.Error(ex)))
            catch (Exception ex) when (True(() => ApiLogger.Error($"IPAddress = {_httpClient.BaseAddress}" + "\r\n" + ex)))
            {
                return null;
            }
        }

        public override string ToString()
        {
            return $"BaseAddress={_httpClient.BaseAddress.AbsoluteUri}";
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}