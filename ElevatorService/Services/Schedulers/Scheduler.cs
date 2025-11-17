using Common.Models;
using log4net;

namespace ElevatorService.Services
{
    public partial class ElevatorService
    {
        public void Schduler()
        {
            commandWaitChange();
            postCommandControl();
        }

        private void commandWaitChange()
        {
            //진행중인 미션이 2작을때 진행한다.
            Mission mission = null;
            var missions = _repository.Missions.GetAll();

            var runmissions = missions.Where(m => m.state == nameof(MissionState.EXECUTING)).ToList();
            if (runmissions.Count >= 2) return;

            var elevator = _repository.Devices.GetAll().FirstOrDefault(r => r.state != nameof(Elevator1_State.DISCONNECT));
            if (elevator == null) return;

            var pandingMissions = missions.Where(r => r.state == nameof(MissionState.PENDING)).OrderBy(r => r.createdAt).ToList();
            if (pandingMissions == null || pandingMissions.Count == 0) return;

            if (runmissions.Count == 0) mission = missions.FirstOrDefault();
            else mission = missionSelect(elevator, pandingMissions);

            if (mission != null)
            {
                var commands = _repository.Commands.GetByMissionId(mission.guid).Where(c => c.state == nameof(CommandState.INIT)).ToList();
                foreach (var command in commands)
                {
                    updateStateCommand(command, nameof(CommandState.WAITING), true);
                }
            }
        }

        private void postCommandControl()
        {
            Command command = null;

            var missions = _repository.Missions.GetAll();

            //var commands = _repository.Commands.GetAll();
            //if (commands == null || commands.Count == 0) return;

            var elevator = _repository.Devices.GetAll().FirstOrDefault(r => r.state != nameof(Elevator1_State.DISCONNECT));
            if (elevator == null) return;

            var pandingMissions = missions.Where(r => (r.state == nameof(MissionState.PENDING)) || (r.state == nameof(MissionState.EXECUTING))).OrderBy(r => r.createdAt).ToList();
            if (pandingMissions == null || pandingMissions.Count == 0) return;

            foreach (var pandingMission in pandingMissions)
            {
                var commands = _repository.Commands.GetByMissionId(pandingMission.guid);
                if (commands == null || commands.Count == 0) continue;

                var runcommand = _repository.Commands.GetByRunCommands(commands).FirstOrDefault();

                foreach (var command1 in commands)
                {
                    TestLogger.Info($"{nameof(postCommandControl)} ,command, MissionId = {command1.acsMissionId} , guid = {command1.guid}, name = {command1.name}, state = {command1.state}");
                }

                if (runcommand != null)
                {
                    TestLogger.Info($"{nameof(postCommandControl)} ,runcommand, MissionId = {runcommand.acsMissionId} , guid = {runcommand.guid}, name = {runcommand.name}, state = {runcommand.state}");
                    continue;
                }
                var commandFailed_Request = commands.FirstOrDefault(m => (m.state == nameof(CommandState.FAILED)) || (m.state == nameof(CommandState.COMMANDREQUEST)));
                if (commandFailed_Request != null)
                {
                    TestLogger.Info($"{nameof(postCommandControl)} ,commandFailed_Request, MissionId = {commandFailed_Request.acsMissionId}, guid = {commandFailed_Request.guid}," +
                                   $", name = {commandFailed_Request.name}, state = {commandFailed_Request.state}");

                    command = commandFailed_Request;
                }
                else
                {
                    command = commands.Where(m => m.state == nameof(CommandState.WAITING)).FirstOrDefault();
                }
                if (command != null)
                {
                    TestLogger.Info($"{nameof(postCommandControl)} ,nomal, MissionId = {command.acsMissionId}, guid = {command.guid}, name = {command.name},state = {command.state}");
                    updateStateCommand(command, nameof(CommandState.COMMANDREQUEST), true);
                    postCommand(command);
                }
            }
        }

        private Mission missionSelect(Device elevator, List<Mission> missions)
        {
            Mission mission = null;

            switch (elevator.state)
            {
                case nameof(Elevator1_State.DOOROPEN_B1F):
                case nameof(Elevator1_State.DOORCLOSE_B1F):
                    mission = missions.FirstOrDefault(m => m.sourceFloor == "B1F");
                    break;

                case nameof(Elevator1_State.DOOROPEN_1F):
                case nameof(Elevator1_State.DOORCLOSE_1F):
                    mission = missions.FirstOrDefault(m => m.sourceFloor == "1F");
                    break;

                case nameof(Elevator1_State.DOOROPEN_2F):
                case nameof(Elevator1_State.DOORCLOSE_2F):
                    mission = missions.FirstOrDefault(m => m.sourceFloor == "2F");
                    break;

                case nameof(Elevator1_State.DOOROPEN_3F):
                case nameof(Elevator1_State.DOORCLOSE_3F):
                    mission = missions.FirstOrDefault(m => m.sourceFloor == "3F");
                    break;

                case nameof(Elevator1_State.DOOROPEN_4F):
                case nameof(Elevator1_State.DOORCLOSE_4F):
                    mission = missions.FirstOrDefault(m => m.sourceFloor == "4F");
                    break;

                case nameof(Elevator1_State.DOOROPEN_5F):
                case nameof(Elevator1_State.DOORCLOSE_5F):
                    mission = missions.FirstOrDefault(m => m.sourceFloor == "5F");
                    break;

                case nameof(Elevator1_State.DOOROPEN_6F):
                case nameof(Elevator1_State.DOORCLOSE_6F):
                    mission = missions.FirstOrDefault(m => m.sourceFloor == "6F");
                    break;

                case nameof(Elevator1_State.UPDRIVING_B1F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "1F")
                                                        || (m.sourceFloor == "2F")
                                                        || (m.sourceFloor == "3F")
                                                        || (m.sourceFloor == "4F")
                                                        || (m.sourceFloor == "5F")
                                                        || (m.sourceFloor == "6F"));
                    break;

                case nameof(Elevator1_State.UPDRIVING_1F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "2F")
                                                        || (m.sourceFloor == "3F")
                                                        || (m.sourceFloor == "4F")
                                                        || (m.sourceFloor == "5F")
                                                        || (m.sourceFloor == "6F"));
                    break;

                case nameof(Elevator1_State.UPDRIVING_2F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "3F")
                                                     || (m.sourceFloor == "4F")
                                                     || (m.sourceFloor == "5F")
                                                     || (m.sourceFloor == "6F"));
                    break;

                case nameof(Elevator1_State.UPDRIVING_3F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "4F")
                                                   || (m.sourceFloor == "5F")
                                                   || (m.sourceFloor == "6F"));
                    break;

                case nameof(Elevator1_State.UPDRIVING_4F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "5F")
                                                        || (m.sourceFloor == "6F"));
                    break;

                case nameof(Elevator1_State.UPDRIVING_5F):
                case nameof(Elevator1_State.UPDRIVING_6F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "6F"));
                    break;

                case nameof(Elevator1_State.DOWNDRIVING_B1F):
                case nameof(Elevator1_State.DOWNDRIVING_1F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "B1F"));
                    break;

                case nameof(Elevator1_State.DOWNDRIVING_2F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "1F"));
                    break;

                case nameof(Elevator1_State.DOWNDRIVING_3F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "1F")
                                                 || (m.sourceFloor == "2F"));
                    break;

                case nameof(Elevator1_State.DOWNDRIVING_4F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "1F")
                                                 || (m.sourceFloor == "2F")
                                                 || (m.sourceFloor == "3F"));
                    break;

                case nameof(Elevator1_State.DOWNDRIVING_5F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "1F")
                                                 || (m.sourceFloor == "2F")
                                                 || (m.sourceFloor == "3F")
                                                 || (m.sourceFloor == "4F"));
                    break;

                case nameof(Elevator1_State.DOWNDRIVING_6F):
                    mission = missions.FirstOrDefault(m => (m.sourceFloor == "1F")
                                                 || (m.sourceFloor == "2F")
                                                 || (m.sourceFloor == "3F")
                                                 || (m.sourceFloor == "4F")
                                                 || (m.sourceFloor == "5F")
                                                 || (m.sourceFloor == "6F"));
                    break;
            }
            return mission;
        }

        private bool postCommand(Command command)
        {
            bool CommandRequst = false;
            //[조건1] 미션 상태가 COMMANDREQUEST 다르면 COMMANDREQUEST 로 상태변경한다

            switch (command.service)
            {
                case nameof(Service.NO1):
                    //[조건2] Service Api를 조회한다.
                    var elevaotrApi = _repository.ServiceApis.GetAll().FirstOrDefault(r => r.type == nameof(Service.NO1));
                    if (elevaotrApi != null)
                    {
                        //[조건3] API 형식에 맞추어서 Mapping 을 한다.
                        var mapping_Command = _mapping.Commands.ApiRequestDtoPostCommand(command);
                        if (mapping_Command != null)
                        {
                            //[조건4] Service 로 Api Mission 전송을 한다.
                            var postCommand = elevaotrApi.Api.ElevatorPostCommandQueueAsync(mapping_Command).Result;
                            if (postCommand != null)
                            {
                                //[조건5] 상태코드 200~300 까지는 완료 처리
                                if (postCommand.statusCode >= 200 && postCommand.statusCode < 300)
                                {
                                    EventLogger.Info($"PostMission Success = Service = {nameof(Service.NO1)}, Message = {postCommand.statusText}, CommandName = {command.name}, CommandId = {command.guid}, AssignedWorkerId = {command.assignedWorkerId}");
                                    CommandRequst = true;
                                }
                                else EventLogger.Info($"PostMission Failed = Service = {nameof(Service.NO1)}, Message = {postCommand.message}, CommandName = {command.name}, CommandId = {command.guid}, AssignedWorkerId = {command.assignedWorkerId}");
                            }
                        }
                    }

                    break;
            }
            if (CommandRequst)
            {
                updateStateCommand(command, nameof(CommandState.COMMANDREQUESTCOMPLETED), true);
            }
            return CommandRequst;
        }
    }
}