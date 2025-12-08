using Common.DTOs.Rests.Device;
using Common.Models;
using Data.Interfaces;
using ElevatorService.Mappings.Interfaces;
using ElevatorService.MQTTs.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElevatorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class deviceController : ControllerBase
    {
        private static readonly ILog logger = LogManager.GetLogger("MissionController"); //Function 실행관련 Log

        private readonly IUnitOfWorkRepository _repository;
        private readonly IUnitOfWorkMapping _mapping;
        private readonly IUnitofWorkMqttQueue _mqttQueue;

        public deviceController(IUnitOfWorkRepository repository, IUnitOfWorkMapping mapping, IUnitofWorkMqttQueue mqttQueue)
        {
            _repository = repository;
            _mapping = mapping;
            _mqttQueue = mqttQueue;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public ActionResult<List<Elevator>> Get()
        {
            var getAll = _repository.Elevators.GetAll();

            return getAll;
        }

        // GET api/<ValuesController>/5
        //[HttpGet("{id}")]
        //public string Response(int id)
        //{
        //    return "value";
        //}

        // POST api/<ValuesController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        // PUT api/<ValuesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        [HttpPatch("{id}")]
        public ActionResult Patch(string id, Patch_DeviceDto value)
        {
            Command command = null;
            if (value != null)
            {
                if (!IsInvalid(value.deviceMode))
                {
                    var GetById = _repository.Elevators.GetById(id);
                    if (GetById != null)
                    {
                        string devicemode = value.deviceMode.ToUpper();
                        command = createModeChangeCommand(id, devicemode);
                    }
                }
            }
            if (command != null) return Ok(command);
            else return NotFound();
        }

        private Command createModeChangeCommand(string id, string devicemode)
        {
            var command = new Command
            {
                acsMissionId = null,
                guid = Guid.NewGuid().ToString(),
                name = "ElevatorCALL",
                service = id,
                type = nameof(Common.Models.CommandType.ACTION),
                subType = devicemode,
                sequence = 1,
                state = nameof(CommandState.INIT),
                assignedWorkerId = null,
                createdAt = DateTime.Now,
                updatedAt = null,
                finishedAt = null,
            };
            var parameter = new Parameter
            {
                key = "Action",
                value = $"{devicemode}",
            };
            command.parameters.Add(parameter);
            command.parameterJson = JsonSerializer.Serialize(command.parameters);
            return command;
        }

        private bool IsInvalid(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                || value.ToUpper() == "NULL"
                || value.ToUpper() == "STRING"
                || value.ToUpper() == "";
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}