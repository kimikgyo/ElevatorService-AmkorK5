using Common.DTOs;
using Common.Models;
using Data.Interfaces;
using ElevatorService.Mappings.Interfaces;
using ElevatorService.MQTTs.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Data;
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
            var condition = ConditionAddMission(addRequestDtoMission);
            if (condition.elevatorId != null && condition.sourceFloor != null && condition.destFloor != null && condition.massage == null)
            {
                var mission = _mapping.Missions.AddRequest(addRequestDtoMission, condition.elevatorId, condition.sourceFloor, condition.destFloor);
                if (mission != null)
                {
             
                    _repository.Missions.Add(mission);
                    _repository.Commands.Add(CreateCommand_1(mission));     //ElevatorCall
                    //_repository.Commands.Add(CreateCommand_3(mission));     //Elevator진입
                    //_repository.Commands.Add(CreateCommand_4(mission));     //Elevator맵체인지
                    _repository.Commands.Add(CreateCommand_6(mission));     //Elevator목적지선택
                    _repository.Commands.Add(CreateCommand_5(mission));     //ElevaotrDoorClose
                    //_repository.Commands.Add(CreateCommand_8(mission));     //Device 진출위치
                    _repository.Commands.Add(CreateCommand_9(mission));     //Device DoorClose
                }
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        private Command CreateCommand_1(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.guid,
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


        private Command CreateCommand_3(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.guid,
                guid = Guid.NewGuid().ToString(),
                name = "ElevatorEnterMove",
                service = "WORKER",
                type = nameof(Common.Models.CommandType.MOVE),
                subType = nameof(CommandSubType.ELEVATORENTER),
                sequence = 3,
                state = nameof(CommandState.INIT),
                assignedWorkerId = mission.assignedWorkerId,
                createdAt = DateTime.Now,
                updatedAt = null,
                finishedAt = null,
            };
            var parameter = new parameter
            {
                key = "target",
                value = null,
            };
            command.parameters.Add(parameter);
            command.parameterJson = JsonSerializer.Serialize(command.parameters);
            return command;
        }

        private Command CreateCommand_4(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.guid,
                guid = Guid.NewGuid().ToString(),
                name = "WorkerMapChange",
                service = "WORKER",
                type = nameof(Common.Models.CommandType.MAP),
                subType = nameof(CommandSubType.DESTINATIONCHANGE),
                sequence = 4,
                state = nameof(CommandState.INIT),
                assignedWorkerId = mission.assignedWorkerId,
                createdAt = DateTime.Now,
                updatedAt = null,
                finishedAt = null,
            };
            var parameter = new parameter
            {
                key = "mapId",
                value = "Mapid",
            };
            command.parameters.Add(parameter);
            command.parameterJson = JsonSerializer.Serialize(command.parameters);
            return command;
        }

        private Command CreateCommand_5(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.guid,
                guid = Guid.NewGuid().ToString(),
                name = "ElevatorDoorClose",
                service = mission.elevatorId,
                type = nameof(Common.Models.CommandType.ACTION),
                subType = nameof(CommandSubType.DOORCLOSE),
                sequence = 6,
                state = nameof(CommandState.INIT),
                assignedWorkerId = mission.assignedWorkerId,
                createdAt = DateTime.Now,
                updatedAt = null,
                finishedAt = null,
            };
            var parameter = new parameter
            {
                key = "Action",
                value = nameof(Command_ElevatorAction.DOORCLOSE),
            };
            command.parameters.Add(parameter);
            command.parameterJson = JsonSerializer.Serialize(command.parameters);
            return command;
        }

        private Command CreateCommand_6(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.guid,
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


        private Command CreateCommand_8(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.guid,
                guid = Guid.NewGuid().ToString(),
                name = "ElevatorExitMove",
                service = "WORKER",
                type = nameof(Common.Models.CommandType.MOVE),
                subType = nameof(CommandSubType.ELEVATOREXIT),
                sequence = 8,
                state = nameof(CommandState.INIT),
                assignedWorkerId = mission.assignedWorkerId,
                createdAt = DateTime.Now,
                updatedAt = null,
                finishedAt = null,
            };
            var parameter = new parameter
            {
                key = "target",
                value = null,
            };
            command.parameters.Add(parameter);
            command.parameterJson = JsonSerializer.Serialize(command.parameters);
            return command;
        }

        private Command CreateCommand_9(Mission mission)
        {
            var command = new Command
            {
                acsMissionId = mission.guid,
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

        private (string elevatorId, string sourceFloor, string destFloor, string massage) ConditionAddMission(ApiAddRequestDtoMission RequestDto)
        {
            string massage = null;
            string elevatorId = null;
            string sourceFloor = null;
            string destFloor = null;
            elevatorId = RequestDto.parameters.Where(k => k.key.ToUpper() == "ELEVATORID").Select(s => s.value).FirstOrDefault();
            sourceFloor = RequestDto.parameters.Where(k => k.key.ToUpper() == "SOURCEFLOOR").Select(s => s.value).FirstOrDefault();
            destFloor = RequestDto.parameters.Where(k => k.key.ToUpper() == "DESTINATIONFLOOR").Select(s => s.value).FirstOrDefault();

            return (elevatorId, sourceFloor, destFloor, massage);
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