using Common.Models;
using Data.Interfaces;
using Data.Repositorys;
using ElevatorService.Mappings.Interfaces;
using log4net;
using RestApi.Interfases;
using System.Diagnostics;

namespace ElevatorService.Services
{
    public class Response_Data_Service
    {
        private static readonly ILog ApiLogger = LogManager.GetLogger("ApiEvent");

        public readonly IUnitOfWorkRepository _repository;
        public readonly IUnitOfWorkMapping _mapping;
        public readonly ILog _eventlog;
        public List<MqttTopicSubscribe> mqttTopicSubscribes = new List<MqttTopicSubscribe>();

        public Response_Data_Service(ILog eventLog, IUnitOfWorkRepository repository, IUnitOfWorkMapping mapping)
        {
            _repository = repository;
            _mapping = mapping;
            _eventlog = eventLog;
        }

        public async Task<bool> StartAsyc()
        {
            bool Complete = false;
            bool Resource = false;
            bool Template = false;

            bool GetWorkerData = false;
            while (!Complete)
            {
                try
                {
                    ApiClient();
                    _repository.ElevatorSettings.Delete();
                    foreach (var serviceApi in _repository.ServiceApis.GetAll())
                    {
                        if (serviceApi.type == "Resource")
                        {
                            var Elevators = await serviceApi.Api.Get_Elevators_Async();

                            if (Elevators == null || Elevators.Count == 0)
                            {
                                _eventlog.Info($"{nameof(Elevators)}GetDataFail");
                                break;
                            }
                            else
                            {
                                foreach (var Elevator in Elevators)
                                {
                                    var elevatorSetting = _mapping.ElevatorSettings.Reponse(Elevator);
                                    _repository.ElevatorSettings.Add(elevatorSetting);
                                }
                                Resource = true;
                            }
                        }
                    }
                    if (Resource)
                    {
                        Complete = true;
                        ConfigData.SubscribeTopics = suscreibeTopicsAdd();
                        _eventlog.Info($"GetData{nameof(Complete)}");
                    }
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    LogExceptionMessage(ex);
                    await Task.Delay(500);
                }
            }

            return Complete;
        }

        private List<MqttTopicSubscribe> suscreibeTopicsAdd()
        {
            mqttTopicSubscribes.Clear();
            var elevatorSettings = _repository.ElevatorSettings.GetAll();
            foreach (var elevatorSetting in elevatorSettings)
            {
                var statetopic = new MqttTopicSubscribe
                {
                    topic = $"acs/elevator/{elevatorSetting.id}/status"
                };
                var missiontopic = new MqttTopicSubscribe
                {
                    topic = $"acs/elevator/{elevatorSetting.id}/command"
                };
                mqttTopicSubscribes.Add(statetopic);
                mqttTopicSubscribes.Add(missiontopic);
            }

            return mqttTopicSubscribes;
        }

        public async Task<bool> ReloadAsyc()
        {
            bool Complete = false;
            bool Resource = false;
            bool Template = false;

            bool GetWorkerData = false;
            bool ResourceData = false;
            while (!Complete)
            {
                try
                {
                    _repository.ElevatorSettings.Delete();
                    foreach (var serviceApi in _repository.ServiceApis.GetAll())
                    {
                        if (serviceApi.type == "Resource")
                        {
                            var Elevators = await serviceApi.Api.Get_Elevators_Async();

                            if (Elevators == null || Elevators.Count == 0)
                            {
                                _eventlog.Info($"{nameof(Elevators)}GetDataFail");
                                break;
                            }
                            else
                            {
                                foreach (var Elevator in Elevators)
                                {
                                    var elevatorSetting = _mapping.ElevatorSettings.Reponse(Elevator);

                                    _repository.ElevatorSettings.Add(elevatorSetting);
                                }
                                Resource = true;
                            }
                        }
                    }
                    if (Resource)
                    {
                        Complete = true;
                        ConfigData.SubscribeTopics.Clear();
                        ConfigData.SubscribeTopics = suscreibeTopicsAdd();
                        _eventlog.Info($"GetData{nameof(Complete)}");
                    }
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    LogExceptionMessage(ex);
                    await Task.Delay(500);
                }
            }

            return Complete;
        }

        private void ApiClient()
        {
            //Config 파일을 불러온다
            foreach (var apiInfo in ConfigData.ServiceApis)
            {
                var serviceInfo = _repository.ServiceApis.GetByIpPort(apiInfo.ip, apiInfo.port);
                if (serviceInfo == null)
                {
                    var client = new Api(apiInfo.type, apiInfo.ip, apiInfo.port, double.Parse(apiInfo.timeOut), apiInfo.connectId, apiInfo.connectPassword);
                    apiInfo.Api = client;
                    _repository.ServiceApis.Add(apiInfo);
                }
            }
        }

        public void LogExceptionMessage(Exception ex)
        {
            //string message = ex.InnerException?.Message ?? ex.Message;
            //string message = ex.ToString();
            string message = ex.GetFullMessage() + Environment.NewLine + ex.StackTrace;
            Debug.WriteLine(message);
            _eventlog.Info(message);
        }
    }
}