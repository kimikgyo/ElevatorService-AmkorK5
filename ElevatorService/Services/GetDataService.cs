using Common.DTOs.Bases;
using Common.Models;
using Data.Interfaces;
using Data.Repositorys;
using ElevatorService.Mappings.Interfaces;
using log4net;
using RestApi.Interfases;
using System.Data;
using System.Diagnostics;

namespace ElevatorService.Services
{
    public class GetDataService
    {
        private static readonly ILog ApiLogger = LogManager.GetLogger("ApiEvent");

        public readonly IUnitOfWorkRepository _repository;
        public readonly IUnitOfWorkMapping _mapping;
        public readonly ILog _eventlog;
        public List<MqttTopicSubscribe> mqttTopicSubscribes = new List<MqttTopicSubscribe>();

        public GetDataService(ILog eventLog, IUnitOfWorkRepository repository, IUnitOfWorkMapping mapping)
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
            bool ResourceData = false;
            while (!Complete)
            {
                try
                {
                    ApiClient();
                    Complete = true;
                    //foreach (var serviceApi in _repository.ServiceApis.GetAll())
                    //{
                    //    if (serviceApi.type == "Resource")
                    //    {
                    //        var Maps = await serviceApi.Api.GetResourceMap();
                    //        //var Positions = await serviceApi.Api.GetResourcePosition();

                    //        if (Maps == null)
                    //        {
                    //            _eventlog.Info($"{nameof(Maps)}GetDataFail");
                    //            break;
                    //        }
                    //        //else if (Positions == null)
                    //        //{
                    //        //    _eventlog.Info($"{nameof(Positions)}GetDataFail");
                    //        //    break;
                    //        //}
                    //        else
                    //        {
                    //            foreach (var getmap in Maps)
                    //            {
                    //                var map = _mapping.Maps.ApiGetResourceResponse(getmap);
                    //                _repository.Maps.Add(map);
                    //            }

                    //            //foreach (var getPosition in Positions)
                    //            //{
                    //            //    var position = _mapping.Positions.ApiGetResourceResponse(getPosition);
                    //            //    _repository.Positions.Add(position);
                    //            //}

                    //            Resource = true;
                    //        }
                    //    }
                    //}
                    //if (Resource && Template)
                    //{
                    //    Complete = true;
                    //    ConfigData.SubscribeTopics = mqttTopicSubscribes;
                    //    _eventlog.Info($"GetData{nameof(Complete)}");
                    //}
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
                    ApiClient();
                    foreach (var serviceApi in _repository.ServiceApis.GetAll())
                    {
                        if (serviceApi.type == "Resource")
                        {
                            var getReloadMaps = await serviceApi.Api.GetResourceMap();
                            //var getReloadPositions = await serviceApi.Api.GetResourcePosition();
                            if (getReloadMaps == null)
                            {
                                _eventlog.Info($"{nameof(getReloadMaps)}GetDataFail");
                                break;
                            }
                            //else if (getReloadPositions == null)
                            //{
                            //    _eventlog.Info($"{nameof(getReloadPositions)}GetDataFail");
                            //    break;
                            //}
                            else
                            {
                                ReloadMap(getReloadMaps);
                                //ReloadPosition(getReloadPositions);
                                Resource = true;
                            }
                        }
                    }
                    if (Resource && Template)
                    {
                        Complete = true;
                        ConfigData.SubscribeTopics = mqttTopicSubscribes;
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

        private void ReloadMap(List<ApiGetResponseDtoResourceMap> dtoResourceMaps)
        {
            List<Map> Reload = new List<Map>();
            //update Add
            foreach (var dtoResourceMap in dtoResourceMaps)
            {
                Reload.Add(_mapping.Maps.ApiGetResourceResponse(dtoResourceMap));
            }

            var ReloadId = Reload.Select(x => x.id).ToList();
            var maps = _repository.Maps.GetAll();
            var mapIds = maps.Select(x => x.id);

            //새로운 데이터 기준 으로 기존데이터가 없는것
            var AddMaps = Reload.Where(x => !ReloadId.Contains(x.id)).ToList();

            foreach (var AddMap in AddMaps)
            {
                _repository.Maps.Add(AddMap);
            }

            //기존데이터 기준 새로운 데이터와 같은것 업데이트
            foreach (var map in maps)
            {
                var reloadmap = Reload.FirstOrDefault(x => x.id == map.id);
                if (reloadmap != null)
                {
                    map.mapId = reloadmap.mapId;
                    map.source = reloadmap.source;
                    map.level = reloadmap.level;
                    map.name = reloadmap.name;
                }
            }

            //기존데이터 기준 에서 새로운데이터가 없는것
            var removedateMaps = maps.Where(x => !ReloadId.Contains(x.id)).ToList();
            foreach (var removedateMap in removedateMaps)
            {
                _repository.Maps.Remove(removedateMap);
            }
        }

        //private void ReloadPosition(List<ApiGetResponseDtoResourcePosition> dtoResourcePositions)
        //{
        //    List<Position> Reload = new List<Position>();
        //    //update Add
        //    foreach (var dtoResourcePosition in dtoResourcePositions)
        //    {
        //        Reload.Add(_mapping.Positions.ApiGetResourceResponse(dtoResourcePosition));
        //    }

        //    var ReloadId = Reload.Select(x => x.id);
        //    var positions = _repository.Positions.GetAll();
        //    var positionIds = positions.Select(x => x.id);

        //    //새로운 데이터 기준 으로 기존데이터가 없는것
        //    var AddPositions = Reload.Where(x => !positionIds.Contains(x.id)).ToList();

        //    foreach (var AddPosition in AddPositions)
        //    {
        //        _repository.Positions.Add(AddPosition);
        //    }

        //    //기존데이터 기준 새로운 데이터와 같은것 업데이트
        //    foreach (var position in positions)
        //    {
        //        var Update = Reload.FirstOrDefault(x => x.id == position.id);
        //        if (Update != null)
        //        {
        //            position.source = Update.source;
        //            position.group = Update.group;
        //            position.type = Update.type;
        //            position.subType = Update.subType;
        //            position.mapId = Update.mapId;
        //            position.name = Update.name;
        //            position.x = Update.x;
        //            position.y = Update.y;
        //            position.theth = Update.theth;
        //            position.isDisplayed = Update.isDisplayed;
        //            position.isEnabled = Update.isEnabled;
        //            position.linkedFacility = Update.linkedFacility;
        //            position.linkedRobotId = Update.linkedRobotId;
        //            position.hasCharger = Update.hasCharger;
        //        }
        //    }

        //    //기존데이터 기준 에서 새로운데이터가 없는것
        //    var removes = positions.Where(x => !ReloadId.Contains(x.id)).ToList();
        //    foreach (var remove in removes)
        //    {
        //        _repository.Positions.Remove(remove);
        //    }
        //}

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