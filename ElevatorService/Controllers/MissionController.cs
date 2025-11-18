using Common.DTOs;
using Common.Models;
using Data.Interfaces;
using ElevatorService.Mappings.Interfaces;
using ElevatorService.MQTTs.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElevatorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class missionsController : ControllerBase
    {
        private static readonly ILog logger = LogManager.GetLogger("MissionController"); //Function 실행관련 Log

        private readonly IUnitOfWorkRepository _repository;
        private readonly IUnitOfWorkMapping _mapping;
        private readonly IUnitofWorkMqttQueue _mqttQueue;

        public missionsController(IUnitOfWorkRepository repository, IUnitOfWorkMapping mapping, IUnitofWorkMqttQueue mqttQueue)
        {
            _repository = repository;
            _mapping = mapping;
            _mqttQueue = mqttQueue;
        }

        // GET: api/<MissionController>
        //[HttpGet]
        //public ActionResult<Mission> GetAll()
        //{
        //    var mission = _repository.Missions.GetAll().FirstOrDefault();
        //    return Ok(mission);
        //    //List<ResponseDtoMission> _responseDtos = new List<ResponseDtoMission>();

        //    //foreach (var model in _repository.Missions.GetAll())
        //    //{
        //    //    _responseDtos.Add(_mapping.Missions.Response(model));
        //    //    logger.Info($"{this.ControllerLogPath()} Response = {model}");
        //    //}
        //    //return Ok(_responseDtos);
        //}

        //History
        //[HttpGet("history")]
        //public ActionResult<List<ResponseDtoMission>> FindHistory(DateTime startDay, DateTime endDay)
        //{
        //    if (startDay != DateTime.MinValue && endDay != DateTime.MinValue)
        //    {
        //        List<ResponseDtoMission> _responseDtos = new List<ResponseDtoMission>();

        //        if (startDay == endDay) endDay = endDay.AddDays(1);
        //        var histories = _repository.MissionHistorys.FindHistory(startDay, endDay);
        //        foreach (var history in histories)
        //        {
        //            _responseDtos.Add(_mapping.Missions.Response(history));
        //            logger.Info($"{this.ControllerLogPath()} Response = {history}");
        //        }

        //        return Ok(_responseDtos);
        //    }
        //    else
        //    {
        //        return BadRequest("check startDay or endDay");
        //    }
        //}

        //[HttpGet("history/today")]
        //public ActionResult<List<ResponseDtoMission>> GetTodayHistory()
        //{
        //    List<ResponseDtoMission> _responseDtos = new List<ResponseDtoMission>();

        //    DateTime today = DateTime.Today;
        //    DateTime tomorrow = today.AddDays(1);
        //    var histories = _repository.MissionHistorys.FindHistory(today, tomorrow);
        //    foreach (var history in histories)
        //    {
        //        _responseDtos.Add(_mapping.Missions.Response(history));
        //        logger.Info($"{this.ControllerLogPath()} Response = {history}");
        //    }
        //    return Ok(_responseDtos);
        //}

        //finisth
        //[HttpGet("finish/today")]
        //public ActionResult<List<ResponseDtoMission>> GetTodayFinisthHistory()
        //{
        //    List<ResponseDtoMission> _responseDtos = new List<ResponseDtoMission>();

        //    DateTime today = DateTime.Today;
        //    DateTime tomorrow = today.AddDays(1);
        //    var histories = _repository.MissionFinishedHistorys.FindHistory(today, tomorrow);
        //    foreach (var history in histories)
        //    {
        //        _responseDtos.Add(_mapping.Missions.Response(history));
        //        logger.Info($"{this.ControllerLogPath()} Response = {history}");
        //    }
        //    return Ok(_responseDtos);
        //}

        // GET api/<JobController>/5
        //[HttpGet("{id}")]
        //public ActionResult<ResponseDtoMission> GetById(string id)
        //{
        //    ResponseDtoMission responseDto = null;

        //    var mission = _repository.Missions.GetById(id);

        //    if (mission != null)
        //    {
        //        responseDto = _mapping.Missions.Response(mission);
        //    }
        //    logger.Info($"{this.ControllerLogPath(id)} Response = {responseDto}");
        //    return responseDto;
        //}

        // POST api/<MissionController>
        [HttpPost]
        public ActionResult Post([FromBody] ApiAddRequestDtoMission addRequestDtoMission)
        {
            Command command = null;
            Mission mission = null;
            var condition = ConditionAddMission(addRequestDtoMission);
            if (condition.elevatorId != null && condition.massage == null)
            {
                var sourceFloor = addRequestDtoMission.parameters.Where(k => k.key.ToUpper() == "SOURCEFLOOR").Select(s => s.value).FirstOrDefault();
                var destinationFloor = addRequestDtoMission.parameters.Where(k => k.key.ToUpper() == "DESTINATIONFLOOR").Select(s => s.value).FirstOrDefault();
                var doorClose = addRequestDtoMission.parameters.Where(k => k.key.ToUpper() == "ACTION").Select(s => s.value).FirstOrDefault();
                mission = _mapping.Missions.AddRequest(addRequestDtoMission, condition.elevatorId, sourceFloor, destinationFloor);
                if (mission != null)
                {
                    if (sourceFloor != null)
                    {
                        command = createSourceFloorCommand(mission);
                    }
                    else if (destinationFloor != null)
                    {
                        command = createDestinationFloorCommand(mission);
                    }
                    else if (doorClose != null)
                    {
                        command = createDoorClose(mission);
                    }
                }
            }

            if (command != null)
            {
                _repository.Commands.Add(command);     //ElevatorCall
                _repository.Missions.Add(mission);
                _mqttQueue.MqttPublishMessage(TopicType.mission, TopicSubType.status, _mapping.Missions.MqttPublish(mission));
                return Ok(command);
            }
            else
            {
                return NotFound();
            }
        }

        private Command createSourceFloorCommand(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.acsMissionId,
                guid = Guid.NewGuid().ToString(),
                name = "ElevatorCALL",
                service = mission.elevatorId,
                type = nameof(Common.Models.CommandType.ACTION),
                subType = nameof(CommandSubType.SOURCEFLOOR),
                sequence = 1,
                state = nameof(CommandState.INIT),
                assignedWorkerId = mission.assignedWorkerId,
                createdAt = DateTime.Now,
                updatedAt = null,
                finishedAt = null,
            };
            var parameter = new parameter
            {
                key = "Action",
                value = $"CALL_{mission.sourceFloor}",
            };
            command.parameters.Add(parameter);
            command.parameterJson = JsonSerializer.Serialize(command.parameters);
            return command;
        }

        private Command createDestinationFloorCommand(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.acsMissionId,
                guid = Guid.NewGuid().ToString(),
                name = "ElevatorGotoDESTINATION",
                service = mission.elevatorId,
                type = nameof(Common.Models.CommandType.ACTION),
                subType = nameof(CommandSubType.DESTINATIONFLOOR),
                sequence = 5,
                state = nameof(CommandState.INIT),
                assignedWorkerId = mission.assignedWorkerId,
                createdAt = DateTime.Now,
                updatedAt = null,
                finishedAt = null,
            };
            var parameter = new parameter
            {
                key = "Action",
                value = $"GOTO_{mission.destinationFloor}",
            };
            command.parameters.Add(parameter);
            command.parameterJson = JsonSerializer.Serialize(command.parameters);
            return command;
        }

        private Command createDoorClose(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.acsMissionId,
                guid = Guid.NewGuid().ToString(),
                name = "ElevatorDoorClose",
                service = mission.elevatorId,
                type = nameof(Common.Models.CommandType.ACTION),
                subType = nameof(CommandSubType.DOORCLOSE),
                sequence = 9,
                state = nameof(CommandState.INIT),
                assignedWorkerId = mission.assignedWorkerId,
                createdAt = DateTime.Now,
                updatedAt = null,
                finishedAt = null,
            };
            var parameter = new parameter
            {
                key = "Action",
                value = "DOORCLOSE",
            };
            command.parameters.Add(parameter);
            command.parameterJson = JsonSerializer.Serialize(command.parameters);

            return command;
        }

        private (string elevatorId, string massage) ConditionAddMission(ApiAddRequestDtoMission RequestDto)
        {
            string massage = null;
            string elevatorId = null;
            string sourceFloor = null;
            string destFloor = null;
            var missionid = _repository.Missions.GetByAcsId(RequestDto.guid);
            if (missionid != null) massage = "Check missionGuid";

            elevatorId = RequestDto.parameters.Where(k => k.key.ToUpper() == "ELEVATORID").Select(s => s.value).FirstOrDefault();

            return (elevatorId, massage);
        }

        //// PUT api/<MissionController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        private bool IsInvalid(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                || value.ToUpper() == "NULL"
                || value.ToUpper() == "STRING"
                || value.ToUpper() == "";
        }

        //// DELETE api/<MissionController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}